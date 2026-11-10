# ALTERAÇÃO PIM: cria/atualiza apenas o banco apontado explicitamente, via migrations.
param([Parameter(Mandatory=$true)][string]$Conexao, [switch]$AcessosTeste)
# ALTERAÇÃO PIM: execução local explícita; não instala serviços nem abre portas no firewall.
$ErrorActionPreference = 'Stop'
$raizProjeto = Split-Path $PSScriptRoot -Parent
$env:ASPNETCORE_ENVIRONMENT = 'Development'
if ($Conexao) { $env:ConnectionStrings__Padrao = $Conexao }
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Instale o SDK .NET 9 antes de continuar.' }
$argumentos = @('run','--project',(Join-Path $raizProjeto 'backend/HorizonteViagens.Api'),'--no-launch-profile','--','--preparar-banco')
if ($AcessosTeste) { $argumentos += '--acessos-teste' }
dotnet @argumentos
if ($LASTEXITCODE -ne 0) { throw 'Não foi possível preparar o banco. Confira conexão e permissões.' }
