# Aplicativo Horizonte Viagens

ALTERAÇÃO PIM: aplicativo Flutter integrado à API C#, com catálogo, login, perfil, favoritos, reservas e carteira. Consulte `../docs/COMO-EXECUTAR.md` para configurar API_URL, executar em rede e usar pagamento sandbox.

```sh
flutter pub get
flutter test
flutter run -d chrome --web-port 5300 --dart-define=API_URL=http://localhost:5218
```

Não inclui segredo de pagamento, SQL ou assinatura no cliente. A carteira mostra documentos vinculados pela agência às reservas confirmadas. Dados de teste são restritos às fixtures dos testes.
