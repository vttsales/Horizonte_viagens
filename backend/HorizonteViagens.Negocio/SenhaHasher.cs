// ALTERAÇÃO PIM: Regras de validação, autenticação, concorrência e inicialização compartilhadas.
using System.Security.Cryptography;

namespace HorizonteViagens.Negocio;

// Transforma a senha num HASH antes de ir para o banco, e confere a senha no login.
//
// Hash é como uma impressão digital: dá para conferir se duas senhas são iguais,
// mas não dá para voltar do hash para a senha. Se o banco vazar, as senhas não vazam.
//
// Usa PBKDF2, que já vem no .NET (System.Security.Cryptography), sem pacote extra.
//   - "salt": 16 bytes aleatórios por senha. Duas pessoas com a mesma senha
//     ficam com hashes diferentes.
//   - "iterações": o cálculo é repetido 100 mil vezes de propósito, para deixar
//     lento quem tentar adivinhar senhas por força bruta.
// O texto guardado tem o formato:  iteracoes.salt.hash  (salt e hash em Base64)
public static class SenhaHasher
{
    private const int TamanhoSalt = 16;
    private const int TamanhoHash = 32;
    private const int Iteracoes = 100_000;
    private static readonly HashAlgorithmName Algoritmo = HashAlgorithmName.SHA256;

    public static string Gerar(string senha)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, Algoritmo, TamanhoHash);

        return $"{Iteracoes}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Conferir(string senha, string hashGuardado)
    {
        var partes = hashGuardado.Split('.');
        if (partes.Length != 3 || !int.TryParse(partes[0], out int iteracoes))
            return false;

        byte[] salt = Convert.FromBase64String(partes[1]);
        byte[] esperado = Convert.FromBase64String(partes[2]);

        // Refaz o cálculo com o MESMO salt e compara o resultado.
        byte[] calculado = Rfc2898DeriveBytes.Pbkdf2(senha, salt, iteracoes, Algoritmo, esperado.Length);

        // FixedTimeEquals compara sempre no mesmo tempo, acertando ou errando,
        // para ninguém descobrir a senha medindo quanto o sistema demora.
        return CryptographicOperations.FixedTimeEquals(calculado, esperado);
    }
}
