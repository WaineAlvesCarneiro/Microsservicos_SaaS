# remove serviços, volumes, secrets e rede overlay

$ErrorActionPreference = "Stop"

Write-Host "Iniciando limpeza da infraestrutura..." -ForegroundColor Cyan

# Verifica se o Swarm já está ativo
$swarmState = docker info --format '{{.Swarm.LocalNodeState}}' 2>$null

if ($swarmState -ne "active") {
    Write-Host "Docker Swarm nao esta inicializado. Inicializando..." -ForegroundColor Yellow

    docker swarm init

    if ($LASTEXITCODE -ne 0) {
        throw "Falha ao inicializar o Docker Swarm."
    }

    Write-Host "Docker Swarm inicializado com sucesso." -ForegroundColor Green
}
else {
    Write-Host "Docker Swarm ja esta ativo." -ForegroundColor Green
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

$sqlServiceName = if ($configuration.ContainsKey("SQL_CONTAINER") -and $configuration["SQL_CONTAINER"]) { $configuration["SQL_CONTAINER"] } else { "sqlserver2022" }
$rabbitServiceName = if ($configuration.ContainsKey("RABBIT_CONTAINER") -and $configuration["RABBIT_CONTAINER"]) { $configuration["RABBIT_CONTAINER"] } else { "rabbitmq" }
$sqlVolumeName = if ($configuration.ContainsKey("SQL_VOLUME") -and $configuration["SQL_VOLUME"]) { $configuration["SQL_VOLUME"] } else { "sqlserver_data" }
$rabbitVolumeName = if ($configuration.ContainsKey("RABBIT_VOLUME") -and $configuration["RABBIT_VOLUME"]) { $configuration["RABBIT_VOLUME"] } else { "rabbitmq_data" }
$networkName = if ($configuration.ContainsKey("DOCKER_NETWORK") -and $configuration["DOCKER_NETWORK"]) { $configuration["DOCKER_NETWORK"] } else { "infra_net" }
$api = if ($configuration.ContainsKey("API_NAME") -and $configuration["API_NAME"]) { $configuration["API_NAME"] } else { "identity" }

# 3. Remover serviços (SQL Server e RabbitMQ)
$services = @($sqlServiceName, $rabbitServiceName, $api)

foreach ($svc in $services) {
    $exists = docker service ls --format '{{.Name}}' | Where-Object { $_ -eq $svc }
    if ($exists) {
        Write-Host "Removendo servico '$svc'..." -ForegroundColor Yellow

        docker service rm $svc | Out-Null
        
        Write-Host "Servico '$svc' removido." -ForegroundColor Green
    } else {
        Write-Host "Servico '$svc' nao encontrado. Nenhuma acao necessaria." -ForegroundColor Gray
    }
}

Start-Sleep -Seconds 10

# 4. Remover volumes (SQL Server e RabbitMQ)
$volumes = @($sqlVolumeName, $rabbitVolumeName)

foreach ($vol in $volumes) {
    $exists = docker volume ls --format '{{.Name}}' | Where-Object { $_ -eq $vol }
    if ($exists) {
        Write-Host "Removendo volume '$vol'..." -ForegroundColor Yellow

        docker volume rm $vol | Out-Null

        Write-Host "Volume '$vol' removido." -ForegroundColor Green
    } else {
        Write-Host "Volume '$vol' nao encontrado. Nenhuma acao necessaria." -ForegroundColor Gray
    }
}

# 6. Remover secrets
$secrets = @(
    "smtp_password_app",
    "sql_sa_user_name",
    "sql_sa_password",
    "jwt_key",
    "admin_user_name",
    "admin_password",
    "rabbitmq_user_name",
    "rabbitmq_password"
)

foreach ($secret in $secrets) {
    $exists = docker secret ls --format '{{.Name}}' | Where-Object { $_ -eq $secret }
    if ($exists) {
        Write-Host "Removendo secret '$secret'..." -ForegroundColor Yellow

        docker secret rm $secret | Out-Null

        Write-Host "Secret '$secret' removido." -ForegroundColor Green
    } else {
        Write-Host "Secret '$secret' nao encontrado. Nenhuma acao necessaria." -ForegroundColor Gray
    }
}

# 7. Remover rede overlay
$network = $networkName

$exists = docker network ls --format '{{.Name}}' | Where-Object { $_ -eq $network }

if ($exists) {
    Write-Host "Removendo rede '$network'..." -ForegroundColor Yellow

    docker network rm $network | Out-Null

    Write-Host "Rede '$network' removida." -ForegroundColor Green
}
else {
    Write-Host "Rede '$network' nao encontrada." -ForegroundColor Gray
}

foreach ($directory in $apiBuildDirectories) {
    if (Test-Path -LiteralPath $directory -PathType Container) {
        Write-Host "Removendo artefatos de build '$directory'..." -ForegroundColor Yellow
        Remove-Item -LiteralPath $directory -Recurse -Force
        Write-Host "Artefatos removidos: $directory" -ForegroundColor Green
    }
    else {
        Write-Host "Artefatos '$directory' nao encontrados. Nenhuma acao necessaria." -ForegroundColor Gray
    }
}

Write-Host "Essa Fase foi concluida com sucesso." -ForegroundColor Green
