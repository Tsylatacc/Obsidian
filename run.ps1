[CmdletBinding()]
param(
    [switch]$Foreground
)

$ErrorActionPreference = 'Stop'

$composeScript = Join-Path $PSScriptRoot 'docker-compose.ps1'
if (-not (Test-Path $composeScript)) {
    throw "The Docker Compose wrapper was not found at '$composeScript'."
}

$composeArguments = @('up', '--build')
if (-not $Foreground) {
    $composeArguments += '--detach'
}

& $composeScript @composeArguments
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

if (-not $Foreground) {
    & $composeScript run --rm migration
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}
