// ALTERAÇÃO PIM: testa catálogo vindo da API e troca de quantidade, sem conexão externa.
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:horizonte_viagens/api.dart';
import 'package:horizonte_viagens/main.dart';

void main() {
  test('Hospedagem divide quartos e preserva grupo ímpar', () {
    final c = Combo.fromJson({
      'id': 1,
      'nome': 'Praia',
      'destino': 'Ubatuba',
      'origem': 'São Paulo',
      'evento': 'Surf',
      'imagem': 'coast',
      'inclusoes': 'Hotel',
      'categoria': 'Econômico',
      'noites': 6,
      'vagas': 10,
      'valor': 300,
      'diariaQuarto': 500,
      'valorExperiencias': 200,
      'precoConfirmado': false,
      'partida': null,
    });
    expect(c.total(1), 3500);
    expect(c.total(2), 4000);
    expect(c.total(3), 7500);
  });
  test('Erros HTTP preservam a mensagem da API', () async {
    final api = HorizonteApi(
      client: MockClient(
        (_) async => http.Response('{"erro":"Sem vagas"}', 409),
      ),
    );
    await expectLater(
      api.request('GET', '/api/portal/catalogo'),
      throwsA(isA<ApiError>()),
    );
  });
  testWidgets('Vitrine vazia não inventa ofertas', (tester) async {
    final api = HorizonteApi(
      client: MockClient((_) async => http.Response('[]', 200)),
    );
    await tester.pumpWidget(HorizonteApp(api: api));
    await tester.pumpAndSettle();
    expect(find.text('0 destinos para descobrir'), findsOneWidget);
  });
}
