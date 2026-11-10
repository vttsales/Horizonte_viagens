# ALTERAÇÃO PIM: API_URL aponta para a máquina da API, sem editar Dart.
param([string]$ApiUrl='http://localhost:5218', [string]$Dispositivo='chrome')
$ErrorActionPreference='Stop'
$raizProjeto=Split-Path $PSScriptRoot -Parent
Push-Location (Join-Path $raizProjeto 'mobile')
try {
 flutter pub get
 if ($LASTEXITCODE -ne 0) { throw 'Falha ao restaurar dependências.' }
 if($Dispositivo -eq 'chrome') { flutter run -d chrome --web-port 5300 "--dart-define=API_URL=$ApiUrl" }
 else { flutter run -d $Dispositivo "--dart-define=API_URL=$ApiUrl" }
 if ($LASTEXITCODE -ne 0) { throw 'Flutter encerrou com erro.' }
} finally { Pop-Location }
