// ALTERAÇÃO PIM: respeita validade de acessos temporários na autenticação.
using System.Text.RegularExpressions;
using HorizonteViagens.Modelos;
using HorizonteViagens.Repositorio.Interfaces;

namespace HorizonteViagens.Negocio;

// Aqui moram as regras de negócio do cliente.
// Web e Api chamam esta classe; esta classe chama o repositório.
// Nenhuma regra fica no controller nem na view.
public class ClienteNegocio
{
    private readonly IClienteRepositorio _repositorio;

    // INJEÇÃO DE DEPENDÊNCIA: esta classe precisa de um repositório para trabalhar,
    // mas não cria um com "new". Ela PEDE no construtor, e o ASP.NET Core entrega.
    // Quem diz ao ASP.NET qual classe entregar quando alguém pede IClienteRepositorio
    // é a linha AddScoped<IClienteRepositorio, ClienteRepositorio>() do Program.cs.
    // Explicação completa: INJECAO-DE-DEPENDENCIA.md, na pasta da solution.
    public ClienteNegocio(IClienteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public List<Cliente> Listar() => _repositorio.Listar();

    public Cliente? ObterPorId(int id) => _repositorio.ObterPorId(id);

    // Credenciais são conferidas contra o hash armazenado; cliente inativo não entra.
    public Cliente? Autenticar(string email, string senha)
    {
        var cliente = _repositorio.ObterPorEmail(email.Trim().ToLowerInvariant());
        return cliente is { Ativo: true } && (cliente.AcessoAte==null || cliente.AcessoAte>DateTime.UtcNow) && SenhaHasher.Conferir(senha, cliente.SenhaHash) ? cliente : null;
    }

    public List<Cliente> BuscarPorNome(string trecho) =>
        _repositorio.BuscarPorNome(trecho?.Trim() ?? string.Empty);

    // A senha chega separada da entidade: ela é validada aqui e só o hash
    // é guardado no cliente.
    public Cliente Cadastrar(Cliente cliente, string? senha)
    {
        Normalizar(cliente);
        Validar(cliente, ignorarId: null);
        ValidarSenha(senha);

        cliente.SenhaHash = SenhaHasher.Gerar(senha!);
        cliente.Ativo = true;
        _repositorio.Adicionar(cliente);
        return cliente;
    }

    // novaSenha vazia = manter a senha atual.
    public void Atualizar(Cliente cliente, string? novaSenha = null)
    {
        var existente = _repositorio.ObterPorId(cliente.Id)
            ?? throw new RegraDeNegocioException("Cliente não encontrado.");

        Normalizar(cliente);
        Validar(cliente, ignorarId: cliente.Id);

        // Copia só o que pode mudar; DataCadastro continua a original.
        existente.Nome = cliente.Nome;
        existente.Email = cliente.Email;
        existente.Cpf = cliente.Cpf;
        existente.Celular = cliente.Celular;
        existente.Cep = cliente.Cep;
        existente.Rua = cliente.Rua;
        existente.Bairro = cliente.Bairro;
        existente.Cidade = cliente.Cidade;
        existente.Ativo = cliente.Ativo;

        if (!string.IsNullOrWhiteSpace(novaSenha))
        {
            ValidarSenha(novaSenha);
            existente.SenhaHash = SenhaHasher.Gerar(novaSenha);
        }

        _repositorio.Atualizar(existente);
    }

    // Devolve false quando o cliente não existe.
    public bool Remover(int id)
    {
        var cliente = _repositorio.ObterPorId(id);
        if (cliente is null) return false;

        _repositorio.Remover(cliente);
        return true;
    }

    // ---------- regras ----------

    private static void Normalizar(Cliente cliente)
    {
        cliente.Nome = cliente.Nome.Trim();
        cliente.Email = cliente.Email.Trim().ToLowerInvariant();
        // ALTERAÇÃO PIM: recusa letras, dígitos Unicode e formatos incompletos antes de normalizar.
        cliente.Cpf = (cliente.Cpf ?? "").Trim();
        if (!Regex.IsMatch(cliente.Cpf, @"^(?:[0-9]{11}|[0-9]{3}\.[0-9]{3}\.[0-9]{3}-[0-9]{2})$"))
            throw new RegraDeNegocioException("Informe os 11 dígitos do CPF, com ou sem pontos e traço.");
        // Guarda só os dígitos: "123.456.789-09" vira "12345678909".
        cliente.Cpf = new string(cliente.Cpf.Where(char.IsDigit).ToArray());
        // Mesma ideia para o celular: "(16) 9.8899-4455" vira "16988994455".
        // Substitui os quatro Replace() da versão em console de uma vez só.
        cliente.Celular = new string(cliente.Celular.Where(char.IsDigit).ToArray());
    }

    private void Validar(Cliente cliente, int? ignorarId)
    {
        if (cliente.Cpf.Length != 11)
            throw new RegraDeNegocioException("O CPF precisa ter 11 dígitos.");
        // Pendência da Etapa 4: verificar os dois dígitos, rejeitando CPFs inventados.
        if (!CpfValido(cliente.Cpf))
            throw new RegraDeNegocioException("O CPF informado é inválido.");

        if (cliente.Celular.Length != 11)
            throw new RegraDeNegocioException("O celular precisa ter 11 dígitos (DDD + número).");

        if (_repositorio.EmailEmUso(cliente.Email, ignorarId))
            throw new RegraDeNegocioException("Já existe um cliente com este e-mail.");

        if (_repositorio.CpfEmUso(cliente.Cpf, ignorarId))
            throw new RegraDeNegocioException("Já existe um cliente com este CPF.");
    }

    private static bool CpfValido(string cpf)
    {
        if (cpf.Length != 11 || cpf.All(d => d == cpf[0])) return false;
        static int Digito(string valor, int quantidade)
        {
            var soma = 0;
            for (var i = 0; i < quantidade; i++)
                soma += (valor[i] - '0') * (quantidade + 1 - i);
            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }
        return Digito(cpf, 9) == cpf[9] - '0'
            && Digito(cpf, 10) == cpf[10] - '0';
    }

    // As mesmas cinco verificações da versão em console, agora com uma
    // mensagem para cada regra, para o usuário saber exatamente o que falta.
    private static void ValidarSenha(string? senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            throw new RegraDeNegocioException("Crie uma senha.");

        if (senha.Length < 8)
            throw new RegraDeNegocioException("A senha precisa ter no mínimo 8 caracteres.");

        // ALTERAÇÃO PIM: mesmo limite e tratamento de espaços do formulário HTML.
        if (senha.Length > 128)
            throw new RegraDeNegocioException("A senha pode ter no máximo 128 caracteres.");
        if (Regex.IsMatch(senha, @"\s"))
            throw new RegraDeNegocioException("A senha não pode conter espaços.");
        if (!Regex.IsMatch(senha, @"[A-Z]"))
            throw new RegraDeNegocioException("A senha precisa ter uma letra maiúscula.");

        if (!Regex.IsMatch(senha, @"[a-z]"))
            throw new RegraDeNegocioException("A senha precisa ter uma letra minúscula.");

        if (!Regex.IsMatch(senha, @"[0-9]"))
            throw new RegraDeNegocioException("A senha precisa ter um número.");

        if (!Regex.IsMatch(senha, @"[^\p{L}\p{N}\s]"))
            throw new RegraDeNegocioException("A senha precisa ter um caractere especial.");
    }
}
