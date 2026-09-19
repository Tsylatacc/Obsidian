[CmdletBinding()]
param(
	[Parameter(Mandatory = $true, Position = 0, ValueFromRemainingArguments = $true)]
	[string[]]$ComposeArguments
)

$ErrorActionPreference = 'Stop'

$apiProject = Join-Path $PSScriptRoot 'Obsidian.Api\Obsidian.Api.csproj'
$composeFile = Join-Path $PSScriptRoot 'docker-compose.yml'

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
	throw 'The .NET SDK was not found in PATH.'
}

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
	throw 'Docker was not found in PATH.'
}

if (-not (Test-Path $apiProject)) {
	throw "The Api project was not found at '$apiProject'."
}

if (-not (Test-Path $composeFile)) {
	throw "The docker-compose.yml file was not found at '$composeFile'."
}

$userSecretsOutput = @(& dotnet user-secrets list --project $apiProject 2>&1)
if ($LASTEXITCODE -ne 0) {
	throw "Could not read the Api project's User Secrets.`n$($userSecretsOutput -join [Environment]::NewLine)"
}

$secrets = @{}
foreach ($line in $userSecretsOutput) {
	if ($line -match '^\s*(?<name>[^=]+?)\s*=\s*(?<value>.*)$') {
		$name = $Matches['name'].Trim() -replace ':', '__'
		$secrets[$name] = $Matches['value']
	}
}

$requiredSecrets = @(
	'POSTGRES_DB',
	'POSTGRES_USER',
	'POSTGRES_PASSWORD',
	'RABBITMQ_DEFAULT_USER',
	'RABBITMQ_DEFAULT_PASS',
	'ASPNETCORE_ENVIRONMENT',
	'ASPNETCORE_URLS',
	'ConnectionStrings__db',
	'ConnectionStrings__rabbitmq',
	'EvolutionApi__ApiUrl',
	'EvolutionApi__ApiKey',
	'Jwt__Bearer__Key',
	'Jwt__Bearer__Issuer',
	'Jwt__Bearer__Audience',
	'Jwt__Bearer__ExpiresInMinutes',
	'DOTNET_ENVIRONMENT',
	'EVOLUTION_POSTGRES_DB',
	'EVOLUTION_POSTGRES_USER',
	'EVOLUTION_POSTGRES_PASSWORD',
	'Cors__Origins__0'
)

$missingSecrets = @(
	$requiredSecrets | Where-Object {
		-not $secrets.ContainsKey($_) -or [string]::IsNullOrWhiteSpace($secrets[$_])
	}
)

if ($missingSecrets.Count -gt 0) {
	throw "Missing or empty User Secrets: $($missingSecrets -join ', ')."
}

foreach ($secretName in $requiredSecrets) {
	[Environment]::SetEnvironmentVariable($secretName, $secrets[$secretName], 'Process')
}

docker compose --file $composeFile @ComposeArguments
exit $LASTEXITCODE
