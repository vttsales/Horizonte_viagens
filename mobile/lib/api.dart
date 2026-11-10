// ALTERAÇÃO PIM: substitui viagens e assinatura fictícias por contrato HTTP autenticado.
import 'dart:async';
import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:http/http.dart' as http;

class ApiError implements Exception {
  final String message;
  final int status;
  ApiError(this.message, [this.status = 0]);
  @override
  String toString() => message;
}

class HorizonteApi extends ChangeNotifier {
  final http.Client client;
  final String baseUrl;
  String? _token;
  String? nome;
  final _storage = const FlutterSecureStorage();
  HorizonteApi({http.Client? client, String? baseUrl})
    : client = client ?? http.Client(),
      baseUrl =
          baseUrl ??
          const String.fromEnvironment(
            'API_URL',
            defaultValue: 'http://10.0.2.2:5218',
          );
  bool get autenticado => _token != null;
  // Em navegador a sessão fica apenas em memória; Android/iOS usam cofre do sistema.
  Future<void> restaurar() async {
    if (!kIsWeb) {
      _token = await _storage.read(key: 'horizonte_token');
    }
    if (_token != null) {
      try {
        final p = await request('GET', '/api/portal/perfil');
        nome = p['nome'];
      } catch (_) {
        await sair();
      }
    }
    notifyListeners();
  }

  Future<dynamic> request(
    String method,
    String path, [
    Map<String, dynamic>? data,
  ]) async {
    final req = http.Request(method, Uri.parse('$baseUrl$path'));
    req.headers['Accept'] = 'application/json';
    if (_token != null) req.headers['Authorization'] = 'Bearer $_token';
    if (data != null) {
      req.headers['Content-Type'] = 'application/json';
      req.body = jsonEncode(data);
    }
    http.Response res;
    try {
      res = await http.Response.fromStream(
        await client.send(req).timeout(const Duration(seconds: 20)),
      ).timeout(const Duration(seconds: 20));
    } on TimeoutException {
      throw ApiError('A conexão demorou. Tente novamente.');
    } on http.ClientException {
      throw ApiError('Não foi possível conectar à API. Confira sua conexão.');
    }
    dynamic body;
    try {
      body = res.body.isEmpty ? null : jsonDecode(res.body);
    } catch (_) {
      throw ApiError(
        'O servidor retornou uma resposta inválida.',
        res.statusCode,
      );
    }
    if (res.statusCode == 401) {
      await sair();
      throw ApiError('Entre novamente para continuar.', 401);
    }
    if (res.statusCode >= 400) {
      var message = body is Map ? body['erro']?.toString() : null;
      if (message == null && body is Map && body['errors'] is Map) {
        message = (body['errors'] as Map).values
            .expand((v) => v as List)
            .join('\n');
      }
      throw ApiError(
        message ?? 'Não foi possível concluir a solicitação.',
        res.statusCode,
      );
    }
    return body;
  }

  Future<void> entrar(String email, String senha) async {
    final data = await request('POST', '/api/autenticacao/login-cliente', {
      'email': email.trim(),
      'senha': senha,
    });
    _token = data['token'];
    nome = data['nome'];
    if (!kIsWeb) await _storage.write(key: 'horizonte_token', value: _token);
    notifyListeners();
  }

  Future<void> sair() async {
    _token = null;
    nome = null;
    if (!kIsWeb) await _storage.delete(key: 'horizonte_token');
    notifyListeners();
  }

  @override
  void dispose() {
    client.close();
    super.dispose();
  }
}

// Modelo explícito evita misturar valor por pessoa e custo do quarto.
class Combo {
  final int id, noites, vagas;
  final String nome, destino, origem, evento, imagem, inclusoes, categoria;
  final double valor, diaria, experiencias;
  final bool confirmado;
  final DateTime? partida;
  Combo.fromJson(Map<String, dynamic> j)
    : id = j['id'],
      noites = j['noites'],
      vagas = j['vagas'],
      nome = j['nome'],
      destino = j['destino'],
      origem = j['origem'],
      evento = j['evento'],
      imagem = j['imagem'],
      inclusoes = j['inclusoes'],
      categoria = j['categoria'],
      valor = (j['valor'] as num).toDouble(),
      diaria = (j['diariaQuarto'] as num).toDouble(),
      experiencias = (j['valorExperiencias'] as num).toDouble(),
      confirmado = j['precoConfirmado'],
      partida = DateTime.tryParse(j['partida'] ?? '');
  double total(int adultos) =>
      (((valor + experiencias) * adultos +
                  diaria * noites * ((adultos + 1) ~/ 2)) *
              100)
          .round() /
      100;
}
