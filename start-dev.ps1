# Set TigerBeetle path and data file with absolute paths
$tigerbeetlePath = "C:\Projects\tigerbeetle.exe"
$dataFile = "C:\Projects\0_0.tigerbeetle"

# Format TigerBeetle data file if it doesn't exist
if (-not (Test-Path $dataFile)) {
    Write-Host "Formatting TigerBeetle data file..." -ForegroundColor Green
    & $tigerbeetlePath format --cluster=0 --replica=0 --replica-count=1 $dataFile
}

# Check if TigerBeetle is running
$tigerbeetleProcess = Get-Process -Name "tigerbeetle" -ErrorAction SilentlyContinue
if (-not $tigerbeetleProcess) {
    Write-Host "Starting TigerBeetle server in a new window..." -ForegroundColor Green
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "& '$tigerbeetlePath' start --addresses=3000 $dataFile"
    
    # Wait for TigerBeetle to be ready
    Write-Host "Waiting for TigerBeetle server to be ready..." -ForegroundColor Yellow
    $maxAttempts = 30
    $attempt = 0
    $ready = $false
    
    while (-not $ready -and $attempt -lt $maxAttempts) {
        try {
            $tcpClient = New-Object System.Net.Sockets.TcpClient
            $result = $tcpClient.BeginConnect("localhost", 3000, $null, $null)
            $success = $result.AsyncWaitHandle.WaitOne(1000)
            if ($success) {
                $tcpClient.EndConnect($result)
                $ready = $true
                Write-Host "TigerBeetle server is ready!" -ForegroundColor Green
            }
            $tcpClient.Close()
        } catch {
            $attempt++
            Write-Host "Waiting for TigerBeetle server... Attempt $attempt of $maxAttempts" -ForegroundColor Yellow
            Start-Sleep -Seconds 1
        }
    }
    
    if (-not $ready) {
        Write-Host "Failed to start TigerBeetle server. Please check the logs." -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "TigerBeetle server is already running" -ForegroundColor Yellow
}

# Check if Temporal is running
$temporalProcess = Get-Process -Name "temporal" -ErrorAction SilentlyContinue
if (-not $temporalProcess) {
    # Start Temporal server
    Write-Host "Starting Temporal server..." -ForegroundColor Green
    Start-Process powershell -ArgumentList "temporal server start-dev"

    # Wait for Temporal server to start
    Write-Host "Waiting for Temporal server to start..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
} else {
    Write-Host "Temporal server is already running" -ForegroundColor Yellow
}


# Build and start DEAT.WebAPI
Write-Host "Building DEAT.WebAPI..." -ForegroundColor Green
$webApiPath = Join-Path $PSScriptRoot "src\Backend\DEAT.WebAPI"
dotnet build $webApiPath
Write-Host "Starting DEAT.WebAPI..." -ForegroundColor Green
Start-Process powershell -ArgumentList "cd '$webApiPath'; dotnet run --launch-profile https"

# Build and start DEAT.AdminUI
Write-Host "Building DEAT.AdminUI..." -ForegroundColor Green
$adminUiPath = Join-Path $PSScriptRoot "src\Frontend\DEAT.AdminUI"
dotnet build $adminUiPath
Write-Host "Starting DEAT.AdminUI..." -ForegroundColor Green
Start-Process powershell -ArgumentList "cd '$adminUiPath'; dotnet run --launch-profile https"

Write-Host "All services started!" -ForegroundColor Green
Write-Host "TigerBeetle: localhost:3000"
Write-Host "Temporal Web UI: http://localhost:8233"
Write-Host "DEAT.WebAPI: https://localhost:7262"
Write-Host "DEAT.AdminUI: https://localhost:7162" 