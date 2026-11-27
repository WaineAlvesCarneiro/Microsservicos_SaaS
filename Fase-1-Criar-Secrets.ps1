[CmdletBinding()]
param(
    [switch]$RemoveSourceFiles
)

# Cria os Docker Secrets e inicializa o Docker Swarm
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

Write-Host "Iniciando Configuracao de Docker Swarm e Secrets..." -ForegroundColor Cyan

function Invoke-Docker {
    param([Parameter(Mandatory)][string[]]$Arguments)

    & docker @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Falha ao executar: docker $($Arguments -join ' ')"
    }
}

# VALIDAR DOCKER
if (-not (Get-Command docker -ErrorAction SilentlyContinue))
{
    throw "Docker nao encontrado."
}

docker info | Out-Null

if ($LASTEXITCODE -ne 0)
{
    throw "Docker nao esta disponivel."
}

# VALIDAR SWARM
$swarmStatus = docker info --format '{{.Swarm.LocalNodeState}}'

if ($swarmStatus -eq "inactive")
{
    Write-Host "Docker Swarm nao esta ativo. Inicializando..." -ForegroundColor Yellow

    Invoke-Docker -Arguments @("swarm", "init")

    Write-Host "Docker Swarm inicializado com sucesso." -ForegroundColor Green
}
elseif ($swarmStatus -ne "active")
{
    throw "Docker Swarm em estado invalido: $swarmStatus"
}
else
{
    Write-Host "Docker Swarm ja esta ativo." -ForegroundColor Gray
}

# CARREGAR .ENV
$envFile = Join-Path $PSScriptRoot ".env"
$configuration = @{}

if (Test-Path $envFile)
{
    Get-Content $envFile | ForEach-Object {
        if ($_ -match '^(.*?)=(.*)$')
        {
            $configuration[$matches[1].Trim()] = $matches[2].Trim()
        }
    }
}

# DEFINIR SECRETS
$secrets = @{
    "smtp_password_app"  = Join-Path $PSScriptRoot "SecretsTxt\secrets_smtp_password_app.txt"
    "sql_sa_user_name"   = Join-Path $PSScriptRoot "SecretsTxt\secrets_sql_sa_user_name.txt"
    "sql_sa_password"    = Join-Path $PSScriptRoot "SecretsTxt\secrets_sql_sa_password.txt"
    "jwt_key"            = Join-Path $PSScriptRoot "SecretsTxt\secrets_jwt_key.txt"
    "admin_user_name"    = Join-Path $PSScriptRoot "SecretsTxt\secrets_admin_user_name.txt"
    "admin_password"     = Join-Path $PSScriptRoot "SecretsTxt\secrets_admin_password.txt"
    "rabbitmq_user_name" = Join-Path $PSScriptRoot "SecretsTxt\secrets_rabbitmq_user_name.txt"
    "rabbitmq_password"  = Join-Path $PSScriptRoot "SecretsTxt\secrets_rabbitmq_password.txt"
}

# CRIAR SECRETS
foreach ($secret in $secrets.GetEnumerator())
{
    $name = $secret.Key
    $file = $secret.Value
    $existing = @(
        docker secret ls --format '{{.Name}}' | Where-Object { $_ -eq $name }
    )

    if ($existing.Count -gt 0)
    {
        Write-Host "Docker Secret '$name' ja existe; arquivo nao sera lido novamente." -ForegroundColor Yellow
        continue
    }

    if (-not (Test-Path -LiteralPath $file -PathType Leaf))
    {
        throw "Arquivo de secret nao encontrado: $file"
    }

    if ([string]::IsNullOrWhiteSpace((Get-Content -LiteralPath $file -Raw)))
    {
        throw "Arquivo de secret vazio: $file"
    }

    Write-Host "Criando Docker Secret '$name'..." -ForegroundColor Green
    Invoke-Docker -Arguments @("secret", "create", $name, $file)
}

Push-Location "$PSScriptRoot\IdentityService\$($configuration['PROJECT_NAME'])"

dotnet user-secrets init

dotnet user-secrets set "SqlServer:Host" $configuration["SQL_HOST"]
dotnet user-secrets set "SqlServer:Database" $configuration["SQL_DATABASE"]

dotnet user-secrets set "RabbitMq:Host" $configuration["RABBIT_HOST"]
dotnet user-secrets set "RabbitMq:Port" $configuration["RABBIT_PORT"]

$smtpPasswordApp = (Get-Content $secrets["smtp_password_app"] -Raw).Trim()
$sqlUserName     = (Get-Content $secrets["sql_sa_user_name"] -Raw).Trim()
$sqlPassword     = (Get-Content $secrets["sql_sa_password"] -Raw).Trim()
$jwtKey          = (Get-Content $secrets["jwt_key"] -Raw).Trim()
$adminPassword   = (Get-Content $secrets["admin_password"] -Raw).Trim()
$adminUserName   = (Get-Content $secrets["admin_user_name"] -Raw).Trim()
$rabbitPassword  = (Get-Content $secrets["rabbitmq_password"] -Raw).Trim()
$rabbitUserName  = (Get-Content $secrets["rabbitmq_user_name"] -Raw).Trim()

dotnet user-secrets set "Smtp:Password"      $smtpPasswordApp
dotnet user-secrets set "SqlServer:UserName" $sqlUserName
dotnet user-secrets set "SqlServer:Password" $sqlPassword

dotnet user-secrets set "Jwt:Key" $jwtKey

dotnet user-secrets set "AdminSettings:UserName" $adminUserName
dotnet user-secrets set "AdminSettings:Password" $adminPassword

dotnet user-secrets set "RabbitMq:UserName" $rabbitUserName
dotnet user-secrets set "RabbitMq:Password" $rabbitPassword

dotnet user-secrets set "OwnerCompany:RazaoSocial" $configuration["OWNER_COMPANY_RAZAO_SOCIAL"]
dotnet user-secrets set "OwnerCompany:Fantasia" $configuration["OWNER_COMPANY_FANTASIA"]
dotnet user-secrets set "OwnerCompany:Cnpj" $configuration["OWNER_COMPANY_CNPJ"]
dotnet user-secrets set "OwnerCompany:Nome" $configuration["OWNER_COMPANY_NOME"]
dotnet user-secrets set "OwnerCompany:Celular" $configuration["OWNER_COMPANY_CELULAR"]
dotnet user-secrets set "OwnerCompany:Email" $configuration["OWNER_COMPANY_EMAIL"]

dotnet user-secrets set "OwnerCompany:SmtpHost" $configuration["OWNER_COMPANY_SMTP_HOST"]
dotnet user-secrets set "OwnerCompany:SmtpPort" $configuration["OWNER_COMPANY_SMTP_PORT"]

dotnet user-secrets set "OwnerCompany:Cep" $configuration["OWNER_COMPANY_CEP"]
dotnet user-secrets set "OwnerCompany:Uf" $configuration["OWNER_COMPANY_UF"]
dotnet user-secrets set "OwnerCompany:Cidade" $configuration["OWNER_COMPANY_CIDADE"]
dotnet user-secrets set "OwnerCompany:Endereco" $configuration["OWNER_COMPANY_ENDERECO"]
dotnet user-secrets set "OwnerCompany:Bairro" $configuration["OWNER_COMPANY_BAIRRO"]

Pop-Location

if ($RemoveSourceFiles) {
    foreach ($file in $secrets.Values) {
        if (Test-Path -LiteralPath $file -PathType Leaf) {
            Remove-Item -LiteralPath $file -Force
            Write-Host "Arquivo removido apos criacao do secret: $file" -ForegroundColor Yellow
        }
    }
}

Write-Host "Docker Secrets disponiveis:" -ForegroundColor Green
& docker secret ls --format "table {{.Name}}\t{{.CreatedAt}}"
