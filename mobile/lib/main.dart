// ALTERAÇÃO PIM: catálogo, autenticação, reservas e carteira conectados ao mesmo backend do site.
import 'dart:math';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:qr_flutter/qr_flutter.dart';
import 'api.dart';

const navy = Color(0xff142c36), gold = Color(0xffd6ab59);
final moeda = NumberFormat.currency(locale: 'pt_BR', symbol: r'R$');
void main() {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(HorizonteApp(api: HorizonteApi()));
}

class HorizonteApp extends StatelessWidget {
  final HorizonteApi api;
  const HorizonteApp({super.key, required this.api});
  @override
  Widget build(BuildContext context) => MaterialApp(
    title: 'Horizonte Viagens',
    debugShowCheckedModeBanner: false,
    theme: ThemeData(
      useMaterial3: true,
      colorScheme: ColorScheme.fromSeed(
        seedColor: navy,
        primary: navy,
        secondary: gold,
      ),
      scaffoldBackgroundColor: const Color(0xfff5f7f8),
      inputDecorationTheme: const InputDecorationTheme(
        border: OutlineInputBorder(),
        filled: true,
        fillColor: Colors.white,
      ),
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          padding: const EdgeInsets.all(18),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
        ),
      ),
    ),
    home: Inicio(api: api),
  );
}

class Inicio extends StatefulWidget {
  final HorizonteApi api;
  const Inicio({super.key, required this.api});
  @override
  State<Inicio> createState() => _InicioState();
}

class _InicioState extends State<Inicio> {
  int tab = 0, adultos = 2;
  String busca = '', categoria = 'Todas';
  bool favoritos = false;
  late Future<dynamic> dados;
  @override
  void initState() {
    super.initState();
    widget.api.addListener(sessao);
    dados = carregar();
    widget.api.restaurar();
  }

  @override
  void dispose() {
    widget.api.removeListener(sessao);
    super.dispose();
  }

  void sessao() {
    if (mounted) setState(() => dados = carregar());
  }

  Future<dynamic> carregar() async {
    if (tab == 0) {
      final lista = await widget.api.request('GET', '/api/portal/catalogo');
      final ids = widget.api.autenticado
          ? await widget.api.request('GET', '/api/portal/favoritos')
          : [];
      return {'lista': lista, 'favoritos': ids};
    }
    if (!widget.api.autenticado) return null;
    if (tab == 3) return widget.api.request('GET', '/api/portal/perfil');
    final reservas = await widget.api.request('GET', '/api/portal/reservas');
    if (tab == 1) return reservas;
    final docs = <dynamic>[];
    for (final r in reservas) {
      if (r['status'] == 'Confirmada') {
        docs.addAll(
          await widget.api.request(
            'GET',
            '/api/portal/reservas/${r['id']}/documentos',
          ),
        );
      }
    }
    return docs;
  }

  void atualizar() => setState(() => dados = carregar());
  Future<void> login() async {
    await Navigator.push(
      context,
      MaterialPageRoute(builder: (_) => LoginPage(api: widget.api)),
    );
    if (mounted) atualizar();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(
      title: const Text(
        '☀ horizonte',
        style: TextStyle(fontWeight: FontWeight.w800, letterSpacing: -1),
      ),
      actions: [
        if (!widget.api.autenticado)
          TextButton(onPressed: login, child: const Text('Entrar'))
        else
          IconButton(
            onPressed: () => widget.api.sair(),
            icon: const Icon(Icons.logout),
            tooltip: 'Sair',
          ),
      ],
    ),
    body: RefreshIndicator(
      onRefresh: () async {
        atualizar();
        await dados;
      },
      child: FutureBuilder<dynamic>(
        future: dados,
        builder: (context, s) {
          if (s.connectionState != ConnectionState.done) {
            return const Center(child: CircularProgressIndicator());
          }
          if (s.hasError) {
            return estado(
              'Não foi possível carregar',
              s.error.toString(),
              atualizar,
            );
          }
          if (tab > 0 && !widget.api.autenticado) {
            return estado(
              'Seu próximo horizonte',
              'Entre para acessar suas viagens e documentos.',
              login,
            );
          }
          if (tab == 0) return catalogo(s.data);
          if (tab == 1) return reservas(s.data as List);
          if (tab == 2) return carteira(s.data as List);
          return perfil(s.data);
        },
      ),
    ),
    bottomNavigationBar: NavigationBar(
      selectedIndex: tab,
      onDestinationSelected: (v) {
        setState(() {
          tab = v;
          dados = carregar();
        });
      },
      destinations: const [
        NavigationDestination(
          icon: Icon(Icons.travel_explore),
          label: 'Explorar',
        ),
        NavigationDestination(
          icon: Icon(Icons.luggage_outlined),
          label: 'Viagens',
        ),
        NavigationDestination(
          icon: Icon(Icons.wallet_outlined),
          label: 'Carteira',
        ),
        NavigationDestination(
          icon: Icon(Icons.person_outline),
          label: 'Perfil',
        ),
      ],
    ),
  );
  Widget estado(String titulo, String texto, VoidCallback acao) => ListView(
    padding: const EdgeInsets.all(28),
    children: [
      const SizedBox(height: 90),
      const Icon(Icons.explore_outlined, size: 70, color: navy),
      const SizedBox(height: 24),
      Text(
        titulo,
        style: Theme.of(context).textTheme.headlineSmall,
        textAlign: TextAlign.center,
      ),
      const SizedBox(height: 12),
      Text(texto, textAlign: TextAlign.center),
      const SizedBox(height: 25),
      FilledButton(
        onPressed: acao,
        child: Text(
          widget.api.autenticado || tab == 0 ? 'Tentar novamente' : 'Entrar',
        ),
      ),
    ],
  );
  Widget catalogo(dynamic data) {
    final items = (data['lista'] as List)
        .map((j) => Combo.fromJson(Map<String, dynamic>.from(j)))
        .where(
          (c) =>
              ('${c.destino} ${c.evento}').toLowerCase().contains(
                busca.toLowerCase(),
              ) &&
              (categoria == 'Todas' || categoria == c.categoria) &&
              (!favoritos || (data['favoritos'] as List).contains(c.id)),
        )
        .toList();
    return ListView(
      padding: const EdgeInsets.all(20),
      children: [
        Container(
          padding: const EdgeInsets.all(26),
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(22),
            image: const DecorationImage(
              image: AssetImage('assets/hero.jpg'),
              fit: BoxFit.cover,
              colorFilter: ColorFilter.mode(
                Color(0xaa142c36),
                BlendMode.darken,
              ),
            ),
          ),
          child: const Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'NOVOS DESTINOS. GRANDES HISTÓRIAS.',
                style: TextStyle(color: gold, fontSize: 10, letterSpacing: 1.5),
              ),
              SizedBox(height: 15),
              Text(
                'Seu destino é só\no começo.',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 34,
                  fontWeight: FontWeight.bold,
                  height: 1.1,
                ),
              ),
              SizedBox(height: 15),
              Text(
                'Passagem + hospedagem + evento',
                style: TextStyle(color: Colors.white),
              ),
            ],
          ),
        ),
        const SizedBox(height: 22),
        TextField(
          decoration: const InputDecoration(
            labelText: 'Destino ou evento',
            prefixIcon: Icon(Icons.search),
          ),
          onChanged: (v) => setState(() => busca = v),
        ),
        const SizedBox(height: 14),
        DropdownButtonFormField<int>(
          initialValue: adultos,
          decoration: const InputDecoration(labelText: 'Viajantes adultos'),
          items: List.generate(
            6,
            (i) => DropdownMenuItem(
              value: i + 1,
              child: Text('${i + 1} adulto(s)'),
            ),
          ),
          onChanged: (v) => setState(() => adultos = v!),
        ),
        Wrap(
          spacing: 8,
          children: ['Todas', 'Luxo', 'Econômico']
              .map(
                (c) => ChoiceChip(
                  label: Text(c),
                  selected: categoria == c,
                  onSelected: (_) => setState(() => categoria = c),
                ),
              )
              .toList(),
        ),
        FilterChip(
          label: const Text('Meus favoritos'),
          selected: favoritos,
          onSelected: (v) {
            if (!widget.api.autenticado) {
              login();
              return;
            }
            setState(() => favoritos = v);
          },
        ),
        Text('${items.length} destinos para descobrir'),
        const SizedBox(height: 12),
        if (items.isEmpty)
          const Padding(
            padding: EdgeInsets.all(30),
            child: Text('Nenhum pacote encontrado. Tente outra busca.'),
          ),
        ...items.map(
          (c) => Card(
            clipBehavior: Clip.antiAlias,
            margin: const EdgeInsets.only(bottom: 20),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Image.asset(
                  'assets/${c.imagem}.jpg',
                  height: 190,
                  fit: BoxFit.cover,
                  errorBuilder: (_, e, s) =>
                      const SizedBox(height: 190, child: Icon(Icons.landscape)),
                ),
                Padding(
                  padding: const EdgeInsets.all(20),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        '${c.categoria} · ${c.noites + 1} dias / ${c.noites} noites',
                      ),
                      Text(
                        c.destino,
                        style: const TextStyle(
                          fontSize: 28,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(c.evento),
                      const Divider(height: 30),
                      Text(
                        '${c.confirmado ? 'Total' : 'Estimativa'} para $adultos adulto(s)',
                      ),
                      Text(
                        moeda.format(c.total(adultos)),
                        style: const TextStyle(
                          fontSize: 27,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(
                        '${moeda.format(c.total(adultos) / adultos)} por pessoa',
                      ),
                      const SizedBox(height: 16),
                      Row(
                        children: [
                          Expanded(
                            child: FilledButton(
                              onPressed: () async {
                                await Navigator.push(
                                  context,
                                  MaterialPageRoute(
                                    builder: (_) => DetalheCombo(
                                      api: widget.api,
                                      combo: c,
                                      adultos: adultos,
                                    ),
                                  ),
                                );
                                atualizar();
                              },
                              child: const Text('Ver pacote'),
                            ),
                          ),
                          IconButton(
                            tooltip: 'Favoritar',
                            icon: Icon(
                              (data['favoritos'] as List).contains(c.id)
                                  ? Icons.favorite
                                  : Icons.favorite_border,
                            ),
                            onPressed: () async {
                              if (!widget.api.autenticado) {
                                await login();
                                return;
                              }
                              try {
                                await widget.api.request(
                                  (data['favoritos'] as List).contains(c.id)
                                      ? 'DELETE'
                                      : 'PUT',
                                  '/api/portal/favoritos/${c.id}',
                                );
                                atualizar();
                              } catch (e) {
                                mensagem(e.toString());
                              }
                            },
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }

  void mensagem(String s) {
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(s)));
    }
  }

  Widget reservas(List lista) => ListView(
    padding: const EdgeInsets.all(20),
    children: [
      const Text(
        'Minhas viagens',
        style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
      ),
      if (lista.isEmpty)
        const Padding(
          padding: EdgeInsets.symmetric(vertical: 60),
          child: Text(
            'Você ainda não tem reservas. Encontre seu destino na aba Explorar.',
          ),
        ),
      ...lista.map(
        (r) => Card(
          child: ListTile(
            contentPadding: const EdgeInsets.all(20),
            title: Text(r['nomePacote']),
            subtitle: Text(
              '${r['viajantes']} adulto(s) · ${r['status']}\n${moeda.format(r['valor'])}',
            ),
            trailing: const Icon(Icons.chevron_right),
            onTap: () async {
              await Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (_) => ReservaPage(
                    api: widget.api,
                    reserva: Map<String, dynamic>.from(r),
                  ),
                ),
              );
              atualizar();
            },
          ),
        ),
      ),
    ],
  );
  // Preserva a ideia da carteira recebida, usando documentos da API sem segredo de assinatura no app.
  Widget carteira(List docs) => ListView(
    padding: const EdgeInsets.all(20),
    children: [
      const Text(
        'Minha carteira',
        style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
      ),
      const Text('Documentos emitidos para suas reservas confirmadas.'),
      if (docs.isEmpty)
        const Padding(
          padding: EdgeInsets.symmetric(vertical: 60),
          child: Text(
            'Seus documentos aparecerão aqui após a emissão pela agência.',
          ),
        ),
      ...docs.map(
        (d) => Card(
          color: navy,
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(d['tipo'], style: const TextStyle(color: gold)),
                Text(
                  d['titulo'],
                  style: const TextStyle(fontSize: 24, color: Colors.white),
                ),
                Text(
                  'Localizador: ${d['codigo']}',
                  style: const TextStyle(color: Colors.white),
                ),
                const SizedBox(height: 20),
                Container(
                  color: Colors.white,
                  padding: const EdgeInsets.all(10),
                  child: QrImageView(data: d['url'], size: 150),
                ),
                const Text(
                  'QR para abrir o documento do fornecedor.',
                  style: TextStyle(color: Colors.white, fontSize: 12),
                ),
                TextButton(
                  onPressed: () => abrirUrl(d['url']),
                  child: const Text(
                    'Abrir documento',
                    style: TextStyle(color: gold),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    ],
  );
  Widget perfil(dynamic p) => ListView(
    padding: const EdgeInsets.all(24),
    children: [
      const Text(
        'Meu perfil',
        style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
      ),
      Text(p['nome']),
      Text(p['email']),
      Text(p['celular']),
      const SizedBox(height: 24),
      FilledButton(
        onPressed: () async {
          await Navigator.push(
            context,
            MaterialPageRoute(
              builder: (_) => CadastroPage(
                api: widget.api,
                perfil: Map<String, dynamic>.from(p),
              ),
            ),
          );
          atualizar();
        },
        child: const Text('Editar dados'),
      ),
      TextButton(
        onPressed: () => widget.api.sair(),
        child: const Text('Sair da conta'),
      ),
    ],
  );
}

Future<void> abrirUrl(String url) async {
  final u = Uri.tryParse(url);
  if (u == null || u.scheme != 'https') throw ApiError('Endereço inválido.');
  if (!await launchUrl(u, mode: LaunchMode.externalApplication)) {
    throw ApiError('Não foi possível abrir o endereço.');
  }
}

class LoginPage extends StatefulWidget {
  final HorizonteApi api;
  const LoginPage({super.key, required this.api});
  @override
  State<LoginPage> createState() => _LoginState();
}

class _LoginState extends State<LoginPage> {
  final email = TextEditingController(), senha = TextEditingController();
  String? erro;
  bool busy = false;
  @override
  void dispose() {
    email.dispose();
    senha.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(title: const Text('Minha conta')),
    body: ListView(
      padding: const EdgeInsets.all(26),
      children: [
        const SizedBox(height: 30),
        const Text(
          'Seu horizonte espera.',
          style: TextStyle(fontSize: 32, fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 25),
        TextField(
          controller: email,
          keyboardType: TextInputType.emailAddress,
          autofillHints: const [AutofillHints.username],
          decoration: const InputDecoration(labelText: 'E-mail'),
        ),
        const SizedBox(height: 16),
        TextField(
          controller: senha,
          obscureText: true,
          autofillHints: const [AutofillHints.password],
          decoration: const InputDecoration(labelText: 'Senha'),
        ),
        if (erro != null)
          Text(erro!, style: const TextStyle(color: Colors.red)),
        const SizedBox(height: 24),
        FilledButton(
          onPressed: busy
              ? null
              : () async {
                  setState(() => busy = true);
                  try {
                    await widget.api.entrar(email.text, senha.text);
                    if (context.mounted) Navigator.pop(context);
                  } catch (e) {
                    if (mounted) setState(() => erro = e.toString());
                  } finally {
                    if (mounted) setState(() => busy = false);
                  }
                },
          child: Text(busy ? 'Entrando…' : 'Entrar'),
        ),
        TextButton(
          onPressed: () => Navigator.push(
            context,
            MaterialPageRoute(builder: (_) => CadastroPage(api: widget.api)),
          ),
          child: const Text('Ainda não tem conta? Cadastre-se'),
        ),
      ],
    ),
  );
}

class CadastroPage extends StatefulWidget {
  final HorizonteApi api;
  final Map<String, dynamic>? perfil;
  const CadastroPage({super.key, required this.api, this.perfil});
  @override
  State<CadastroPage> createState() => _CadastroState();
}

class _CadastroState extends State<CadastroPage> {
  final campos = <String, TextEditingController>{};
  String? erro;
  bool busy = false;
  final nomes = {
    'nome': 'Nome completo',
    'email': 'E-mail',
    'cpf': 'CPF',
    'celular': 'Celular com DDD',
    'cep': 'CEP',
    'rua': 'Rua',
    'bairro': 'Bairro',
    'cidade': 'Cidade',
    'senha': 'Senha',
  };
  @override
  void initState() {
    super.initState();
    for (final key in nomes.keys) {
      campos[key] = TextEditingController(
        text: widget.perfil?[key]?.toString() ?? '',
      );
    }
  }

  @override
  void dispose() {
    for (final c in campos.values) {
      c.dispose();
    }
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(
      title: Text(widget.perfil == null ? 'Criar conta' : 'Editar perfil'),
    ),
    body: ListView(
      padding: const EdgeInsets.all(24),
      children: [
        ...nomes.entries
            .where((e) => widget.perfil == null || e.key != 'senha')
            .map(
              (e) => Padding(
                padding: const EdgeInsets.only(bottom: 16),
                child: TextField(
                  controller: campos[e.key],
                  obscureText: e.key == 'senha',
                  decoration: InputDecoration(labelText: e.value),
                ),
              ),
            ),
        if (widget.perfil == null)
          const Text(
            'Senha com 8 ou mais caracteres, maiúscula, minúscula, número e símbolo.',
          ),
        if (erro != null)
          Text(erro!, style: const TextStyle(color: Colors.red)),
        FilledButton(
          onPressed: busy
              ? null
              : () async {
                  setState(() => busy = true);
                  try {
                    await widget.api.request(
                      widget.perfil == null ? 'POST' : 'PUT',
                      widget.perfil == null
                          ? '/api/clientes'
                          : '/api/portal/perfil',
                      {
                        for (final e in campos.entries)
                          if (e.key != 'senha' || widget.perfil == null)
                            e.key: e.value.text,
                      },
                    );
                    if (context.mounted) {
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(content: Text('Dados salvos.')),
                      );
                      Navigator.pop(context);
                    }
                  } catch (e) {
                    if (mounted) setState(() => erro = e.toString());
                  } finally {
                    if (mounted) setState(() => busy = false);
                  }
                },
          child: Text(busy ? 'Salvando…' : 'Salvar'),
        ),
      ],
    ),
  );
}

// UUID estável por tentativa: reenvio após falha de rede não duplica a reserva.
String requisicao() {
  final random = Random.secure();
  final b = List.generate(16, (_) => random.nextInt(256));
  b[6] = (b[6] & 15) | 64;
  b[8] = (b[8] & 63) | 128;
  final h = b.map((v) => v.toRadixString(16).padLeft(2, '0')).join();
  return '${h.substring(0, 8)}-${h.substring(8, 12)}-${h.substring(12, 16)}-${h.substring(16, 20)}-${h.substring(20)}';
}

class DetalheCombo extends StatefulWidget {
  final HorizonteApi api;
  final Combo combo;
  final int adultos;
  const DetalheCombo({
    super.key,
    required this.api,
    required this.combo,
    required this.adultos,
  });
  @override
  State<DetalheCombo> createState() => _DetalheState();
}

class _DetalheState extends State<DetalheCombo> {
  late int adultos;
  late Future<dynamic> orcamento;
  String chave = requisicao();
  String? erro;
  bool busy = false;
  @override
  void initState() {
    super.initState();
    adultos = widget.adultos;
    carregar();
  }

  void carregar() {
    orcamento = widget.api.request(
      'GET',
      '/api/portal/orcamento/${widget.combo.id}?viajantes=$adultos',
    );
  }

  @override
  Widget build(BuildContext context) {
    final c = widget.combo;
    return Scaffold(
      appBar: AppBar(title: Text(c.destino)),
      body: ListView(
        padding: const EdgeInsets.all(24),
        children: [
          ClipRRect(
            borderRadius: BorderRadius.circular(20),
            child: Image.asset(
              'assets/${c.imagem}.jpg',
              height: 210,
              fit: BoxFit.cover,
            ),
          ),
          const SizedBox(height: 20),
          Text(
            c.nome,
            style: const TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
          Text(c.evento),
          Text('${c.noites + 1} dias / ${c.noites} noites'),
          Text(c.inclusoes),
          const SizedBox(height: 24),
          DropdownButtonFormField<int>(
            initialValue: adultos,
            decoration: const InputDecoration(labelText: 'Viajantes adultos'),
            items: List.generate(
              6,
              (i) => DropdownMenuItem(
                value: i + 1,
                child: Text('${i + 1} adulto(s)'),
              ),
            ),
            onChanged: busy
                ? null
                : (v) => setState(() {
                    adultos = v!;
                    chave = requisicao();
                    carregar();
                  }),
          ),
          const SizedBox(height: 20),
          FutureBuilder<dynamic>(
            future: orcamento,
            builder: (context, s) {
              if (s.hasError) {
                return Column(
                  children: [
                    Text(s.error.toString()),
                    TextButton(
                      onPressed: () => setState(carregar),
                      child: const Text('Tentar novamente'),
                    ),
                  ],
                );
              }
              if (!s.hasData) {
                return const Center(child: CircularProgressIndicator());
              }
              final o = s.data;
              return Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text(
                    'Transporte: ${moeda.format(o['transporte'])}\n${o['quartos']} quarto(s) / ${o['noites']} noites: ${moeda.format(o['hospedagem'])}\nExperiências: ${moeda.format(o['experiencias'])}',
                  ),
                  Text(
                    'Total ${c.confirmado ? '' : 'estimado'}: ${moeda.format(o['total'])}',
                    style: const TextStyle(
                      fontSize: 26,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  if (erro != null)
                    Text(erro!, style: const TextStyle(color: Colors.red)),
                  const SizedBox(height: 20),
                  if (!c.confirmado ||
                      c.partida == null ||
                      !c.partida!.isAfter(DateTime.now()) ||
                      c.vagas < adultos)
                    const Text(
                      'Saída sob consulta. Aguarde a confirmação de data, preço e vagas pela agência.',
                    )
                  else
                    FilledButton(
                      onPressed: busy
                          ? null
                          : () async {
                              if (!widget.api.autenticado) {
                                await Navigator.push(
                                  context,
                                  MaterialPageRoute(
                                    builder: (_) => LoginPage(api: widget.api),
                                  ),
                                );
                                return;
                              }
                              setState(() => busy = true);
                              try {
                                final reserva = await widget.api
                                    .request('POST', '/api/portal/reservas', {
                                      'comboId': c.id,
                                      'viajantes': adultos,
                                      'totalEsperado': o['total'],
                                      'requisicaoId': chave,
                                    });
                                if (context.mounted) {
                                  await Navigator.pushReplacement(
                                    context,
                                    MaterialPageRoute(
                                      builder: (_) => ReservaPage(
                                        api: widget.api,
                                        reserva: Map<String, dynamic>.from(
                                          reserva,
                                        ),
                                      ),
                                    ),
                                  );
                                }
                              } catch (e) {
                                if (mounted) {
                                  setState(() => erro = e.toString());
                                }
                              } finally {
                                if (mounted) setState(() => busy = false);
                              }
                            },
                      child: Text(busy ? 'Reservando…' : 'Reservar vagas'),
                    ),
                ],
              );
            },
          ),
        ],
      ),
    );
  }
}

class ReservaPage extends StatefulWidget {
  final HorizonteApi api;
  final Map<String, dynamic> reserva;
  const ReservaPage({super.key, required this.api, required this.reserva});
  @override
  State<ReservaPage> createState() => _ReservaState();
}

class _ReservaState extends State<ReservaPage> {
  late Map<String, dynamic> r;
  bool busy = false;
  String? erro;
  @override
  void initState() {
    super.initState();
    r = widget.reserva;
  }

  Future<void> acao(String tipo) async {
    setState(() => busy = true);
    try {
      final d = await widget.api.request(
        'POST',
        '/api/portal/reservas/${r['id']}/$tipo',
      );
      if (tipo == 'pagamento') {
        await abrirUrl(d['url']);
      } else if (tipo == 'verificar') {
        setState(() => r = Map<String, dynamic>.from(d));
      } else {
        setState(() => r = {...r, 'status': 'Cancelada'});
      }
    } catch (e) {
      if (mounted) setState(() => erro = e.toString());
    } finally {
      if (mounted) setState(() => busy = false);
    }
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(title: Text('Reserva HV-${r['id']}')),
    body: ListView(
      padding: const EdgeInsets.all(24),
      children: [
        Text(
          r['nomePacote'],
          style: const TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
        ),
        Text('${r['viajantes']} adulto(s) · ${r['quartos']} quarto(s)'),
        Text(r['status'], style: const TextStyle(fontWeight: FontWeight.bold)),
        Text(moeda.format(r['valor']), style: const TextStyle(fontSize: 28)),
        const SizedBox(height: 30),
        if (r['status'] == 'Pendente') ...[
          const Text(
            'Pagamento de teste. Nenhuma cobrança real. Após finalizar, volte e atualize o status.',
          ),
          FilledButton(
            onPressed: busy ? null : () => acao('pagamento'),
            child: const Text('Abrir pagamento de teste'),
          ),
        ],
        OutlinedButton(
          onPressed: busy ? null : () => acao('verificar'),
          child: const Text('Atualizar status'),
        ),
        if (r['status'] == 'Pendente' && r['pagamentoUrl'] == null)
          TextButton(
            onPressed: busy ? null : () => acao('cancelar'),
            child: const Text('Cancelar reserva pendente'),
          ),
        if (erro != null)
          Text(erro!, style: const TextStyle(color: Colors.red)),
        const SizedBox(height: 25),
        const Text(
          'Os documentos emitidos pela agência ficam na aba Carteira.',
        ),
      ],
    ),
  );
}
