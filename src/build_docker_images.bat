@echo off
setlocal EnableDelayedExpansion

:: Script to build Docker images for all microservices in the IoT-Microservices solution
:: Reads service names from LABEL service.name in Dockerfiles to ensure lowercase naming
:: Builds images locally, visible in Docker Desktop image list
:: Usage: build_docker_images.bat [tag]
:: Example: build_docker_images.bat v1.0.0
:: If no tag is provided, defaults to 'latest'

:: Set default tag
set TAG=%1
if "%TAG%"=="" set TAG=latest

:: Define base image name prefix
set IMAGE_PREFIX=iot-microservices

:: Define the source directory containing microservices
set SRC_DIR=.

:: Check if Docker is installed and running
docker info >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo Error: Docker is not installed or not running. Please start Docker and try again.
    exit /b 1
)

:: Find services with Dockerfiles and extract service names
set SERVICES=
for /d %%i in (%SRC_DIR%\*) do (
    if exist "%%i\Dockerfile" (
        :: Extract service.name from Dockerfile
        set SERVICE_NAME=
        for /F "tokens=2 delims==" %%n in ('findstr /C:"LABEL service.name" "%%i\Dockerfile"') do (
            :: Remove quotes and whitespace
            set SERVICE_NAME=%%n
            set SERVICE_NAME=!SERVICE_NAME:"=!
            set SERVICE_NAME=!SERVICE_NAME: =!
        )
        if "!SERVICE_NAME!"=="" (
            echo Error: No LABEL service.name found in %%i\Dockerfile
            exit /b 1
        )
        set SERVICES=!SERVICES! !SERVICE_NAME!
        set SERVICE_DIR_!SERVICE_NAME!=%%i
    )
)

if "%SERVICES%"=="" (
    echo Error: No services with Dockerfiles found in %SRC_DIR%.
    exit /b 1
)

:: Build images for each service
echo Starting Docker image build process for IoT-Microservices...
for %%s in (%SERVICES%) do (
    set SERVICE=%%s
    set IMAGE_NAME=%IMAGE_PREFIX%/!SERVICE!:%TAG%
    echo Building Docker image for !SERVICE!...
    docker build -t !IMAGE_NAME! -f "!SERVICE_DIR_%%s!"/Dockerfile .
    if %ERRORLEVEL% equ 0 (
        echo Successfully built !IMAGE_NAME!
    ) else (
        echo Failed to build !IMAGE_NAME!
        exit /b 1
    )
)

echo All Docker images built successfully!
echo Images created:
for %%s in (%SERVICES%) do (
    set SERVICE=%%s
    echo - %IMAGE_PREFIX%/!SERVICE!:%TAG%
)

echo.
echo Next steps:
echo 1. Run 'docker images' to verify the built images in Docker Desktop.
echo 2. Use 'docker-compose up -d' to start the services.

endlocal