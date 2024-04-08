@echo off
cls
echo [INFO] Initializing batch process...
setlocal
call :set_variables
call :find_unity_version
call :check_unity_installed
call :build_project
endlocal
echo [INFO] Batch process completed.
goto :eof

:set_variables
echo [INFO] Setting variables...
set BIN=%~dp0
set ROOT=%BIN%..\..\
set BUILD_DIR=%ROOT%Builds\Client_Win64\
set UNITY_HUB_CLI="C:\Program Files\Unity Hub\Unity Hub.exe" -- --headless
set UNITY_VERSION=2023.2.16f1
set UNITY_DIR=
set UNITY_EXE=Unity.exe
set UNITY_LOG_PATH=%ROOT%Logs\BuildLog-Client-Win64.txt
set UNITY_CMD=-quit -batchmode -nographics -buildTarget Win64 -projectPath "%ROOT%" -executeMethod BuildCommand.PerformBuild -buildOutput "%BUILD_DIR%"
echo [INFO] Variables set.
goto :eof

:find_unity_version
echo [INFO] Locating Unity version %UNITY_VERSION%...
for /f "tokens=*" %%i in ('%UNITY_HUB_CLI% install-path -g') do (
    if exist "%%i\%UNITY_VERSION%\Editor\%UNITY_EXE%" (
        set UNITY_DIR=%%i\%UNITY_VERSION%\Editor
        echo [INFO] Found Unity version %UNITY_VERSION% at %%i\%UNITY_VERSION%\Editor
        goto :check_unity_installed
    )
)
echo [ERROR] Specified Unity version %UNITY_VERSION% not found. Please install the required version and try again.
exit /b 1

:check_unity_installed
if exist "%UNITY_DIR%\%UNITY_EXE%" (
    echo [INFO] Unity is installed.
    set UNITY_PATH=%UNITY_DIR%\%UNITY_EXE%
) else (
    echo [ERROR] Unity is not installed, please install Unity and try again.
    exit /b 1
)
goto :eof

:build_project
echo [INFO] Starting the build process...
echo [INFO] Using Unity path: %UNITY_PATH%
echo [INFO] Build command: "%UNITY_PATH%" %UNITY_CMD%
"%UNITY_PATH%" %UNITY_CMD%
if %ERRORLEVEL% neq 0 (
    echo [ERROR] Build failed with error code %ERRORLEVEL%.
    exit /b %ERRORLEVEL%
) else (
    echo [INFO] Build completed successfully.
)
goto :eof

:eof
echo [INFO] Exiting batch file with code 0.
exit /b 0
