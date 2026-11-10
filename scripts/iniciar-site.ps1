# ALTERAÇÃO PIM: parâmetros permitem usar o SQL no computador da equipe.
param([string]$Conexao, [string]$Endereco = 'http://localhost:5178')
# ALTERAÇÃO PIM: execução local explícita; não instala serviços nem abre portas no firewall.
$ErrorActionPreference = 'Stop'
$raizProjeto = Split-Path $PSScriptRoot -Parent
$env:ASPNETCORE_ENVIRONMENT = 'Development'
if ($Conexao) { $env:ConnectionStrings__Padrao = $Conexao }
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Instale o SDK .NET 9 antes de continuar.' }
dotnet run --project (Join-Path $raizProjeto 'backend/HorizonteViagens.Web') --no-launch-profile --urls $Endereco
if ($LASTEXITCODE -ne 0) { throw 'O serviço encerrou com erro. Confira a mensagem acima.' }
