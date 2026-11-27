$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

Write-Host "Iniciando o provisionamento da Infraestrutura Base..." -ForegroundColor Cyan

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

# AGUARDAR SWARM INICIALIZAR
Write-Host "Aguardando Docker Swarm ficar disponivel..." -ForegroundColor Yellow

$swarmReady = $false

for ($i = 0; $i -lt 60; $i++)
{
    try
    {
        $state = docker info --format '{{.Swarm.LocalNodeState}}' 2>$null

        if ($state -eq "active")
        {
            $swarmReady = $true
            break
        }
    }
    catch
    {
    }

    Start-Sleep -Seconds 2
}

if (-not $swarmReady)
{
    throw "Docker Swarm nao ficou disponivel."
}

Write-Host "Docker Swarm ativo." -ForegroundColor Green

# CARREGAR .ENV
$envFile = Join-Path $PSScriptRoot ".env"

if (-not (Test-Path $envFile))
{
    throw "Arquivo .env nao encontrado."
}

Get-Content $envFile | ForEach-Object {

    if ($_ -match '^(.*?)=(.*)$')
    {
        $name  = $matches[1].Trim()
        $value = $matches[2].Trim()

        [System.Environment]::SetEnvironmentVariable(
            $name,
            $value,
            "Process"
        )
    }
}

# VALIDAR SECRETS
$requiredSecrets = @(
    "smtp_password_app",
    "sql_sa_user_name",
    "sql_sa_password",
    "jwt_key",
    "admin_user_name",
    "admin_password",
    "rabbitmq_password",
    "rabbitmq_user_name"
)

foreach ($secret in $requiredSecrets)
{
    $exists = docker secret ls --format '{{.Name}}' | Where-Object { $_ -eq $secret }

    if (-not $exists)
    {
        throw "Secret obrigatorio nao encontrado: $secret"
    }
}

# CRIAR REDE
$networkExists = docker network ls --format '{{.Name}}' |
    Where-Object { $_ -eq $env:DOCKER_NETWORK }

if (-not $networkExists)
{
    Write-Host "Criando rede '$($env:DOCKER_NETWORK)'..." -ForegroundColor Yellow

    docker network create `
        --driver overlay `
        --attachable `
        $env:DOCKER_NETWORK | Out-Null

    if ($LASTEXITCODE -ne 0)
    {
        throw "Falha ao criar rede."
    }

    Write-Host "Rede criada." -ForegroundColor Green
}
else
{
    Write-Host "Rede ja existe." -ForegroundColor Yellow
}

# VERIFICAR SQL SERVER
$sqlExists = $true

try
{
    docker service inspect $env:SQL_CONTAINER *> $null
}
catch
{
    $sqlExists = $false
}

if ($LASTEXITCODE -eq 0)
{
    $sqlExists = $true
}

if (-not $sqlExists)
{
    Write-Host "Criando SQL Server..." -ForegroundColor Yellow

    docker service create `
        --name $env:SQL_CONTAINER `
        --restart-condition on-failure `
        --restart-delay 30s `
        --restart-max-attempts 3 `
        --hostname $env:SQL_CONTAINER `
        --network $env:DOCKER_NETWORK `
        --secret sql_sa_user_name `
        --secret sql_sa_password `
        -e ACCEPT_EULA=Y `
        -e MSSQL_SA_PASSWORD_FILE=/run/secrets/sql_sa_password `
        --mount type=volume,source=$env:SQL_VOLUME,target=/var/opt/mssql `
        -p 1433:1433 `
        mcr.microsoft.com/mssql/server:2022-latest | Out-Null

    if ($LASTEXITCODE -ne 0)
    {
        throw "Falha ao criar SQL Server."
    }

    Write-Host "SQL Server criado." -ForegroundColor Green
}
else
{
    Write-Host "SQL Server ja existe." -ForegroundColor Red
}

# VERIFICAR RABBITMQ
$rabbitExists = $false

try
{
    docker service inspect $env:RABBIT_CONTAINER *> $null
}
catch
{
    $rabbitExists = $false
}

if ($LASTEXITCODE -eq 0)
{
    $rabbitExists = $true
}

if (-not $rabbitExists)
{
    Write-Host "Criando RabbitMQ..." -ForegroundColor Yellow

    docker service create `
        --name $env:RABBIT_CONTAINER `
        --restart-condition any `
        --restart-delay 60s `
        --network $env:DOCKER_NETWORK `
        --secret source=rabbitmq_password,target=rabbitmq_password `
        --secret source=rabbitmq_user_name,target=rabbitmq_user_name `
        --mount type=volume,source=$env:RABBIT_VOLUME,target=/var/lib/rabbitmq `
        -p 5672:5672 `
        -p 15672:15672 `
        rabbitmq:3-management | Out-Null

    if ($LASTEXITCODE -ne 0)
    {
        throw "Falha ao criar RabbitMQ."
    }

    Write-Host "RabbitMQ criado." -ForegroundColor Green
}
else
{
    Write-Host "RabbitMQ ja existe." -ForegroundColor Red
}

# AGUARDAR RABBITMQ
Write-Host "Aguardando RabbitMQ..." -ForegroundColor Yellow

$maxRetries = 60
$ready = $false

for ($i = 0; $i -lt $maxRetries; $i++)
{
    $containerId = docker ps `
        --filter "label=com.docker.swarm.service.name=$env:RABBIT_CONTAINER" `
        --format "{{.ID}}" |
        Select-Object -First 1

    if (-not [string]::IsNullOrWhiteSpace($containerId))
    {
        try
        {
            docker exec $containerId rabbitmqctl await_startup --quiet

            if ($LASTEXITCODE -eq 0)
            {
                $ready = $true
                break
            }
        }
        catch
        {
        }
    }

    Start-Sleep -Seconds 2
}

if (-not $ready)
{
    throw "RabbitMQ nao iniciou."
}

# CONFIGURAR USUARIO
$containerId = docker ps `
    --filter "label=com.docker.swarm.service.name=$env:RABBIT_CONTAINER" `
    --format "{{.ID}}" |
    Select-Object -First 1

$rabbitPassword = (docker exec $containerId cat /run/secrets/rabbitmq_password).Trim()
$rabbitUserName = (docker exec $containerId cat /run/secrets/rabbitmq_user_name).Trim()

$userExists = docker exec $containerId rabbitmqctl list_users | Select-String $rabbitUserName

if ($userExists)
{
    Write-Host "Atualizando usuario RabbitMQ..." -ForegroundColor Yellow

    docker exec $containerId rabbitmqctl change_password $rabbitUserName $rabbitPassword | Out-Null
}
else
{
    Write-Host "Criando usuario RabbitMQ..." -ForegroundColor Yellow

    docker exec $containerId rabbitmqctl add_user $rabbitUserName $rabbitPassword | Out-Null
}

docker exec $containerId rabbitmqctl set_user_tags $rabbitUserName administrator | Out-Null

docker exec $containerId rabbitmqctl set_permissions -p / $rabbitUserName ".*" ".*" ".*" | Out-Null

Write-Host "Usuario RabbitMQ configurado." -ForegroundColor Green

Write-Host "Infraestrutura provisionada com sucesso." -ForegroundColor Green