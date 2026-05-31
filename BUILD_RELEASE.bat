@echo off
echo ================================================
echo   Unlock Steam Cards - Build Release
echo ================================================
echo.

set ROOT=%~dp0
set PUBLISH=%ROOT%publish
set STEAMIDLE_OUT=%ROOT%SteamIdle\bin\x64\Debug\net8.0-windows

:: Nettoyage
echo [1/4] Nettoyage...
if exist "%PUBLISH%" rd /s /q "%PUBLISH%"

:: Build SteamIdle
echo [2/4] Compilation de steam-idle...
dotnet build "%ROOT%SteamIdle\SteamIdle.csproj" -c Debug -r win-x64 /p:Platform=x64
if errorlevel 1 goto error

:: Publish SteamCardFarmer (self-contained)
echo [3/4] Publication de SteamCardFarmer (self-contained)...
dotnet publish "%ROOT%SteamCardFarmer\SteamCardFarmer.csproj" ^
    -c Release -r win-x64 --self-contained true ^
    -o "%PUBLISH%" /p:Platform=x64
if errorlevel 1 goto error

:: Copie steam-idle.exe dans publish
echo [4/4] Copie de steam-idle.exe...
copy /Y "%STEAMIDLE_OUT%\steam-idle.exe" "%PUBLISH%\steam-idle.exe"
copy /Y "%STEAMIDLE_OUT%\steam-idle.dll" "%PUBLISH%\steam-idle.dll"
copy /Y "%STEAMIDLE_OUT%\steam-idle.deps.json" "%PUBLISH%\steam-idle.deps.json"
copy /Y "%STEAMIDLE_OUT%\steam-idle.runtimeconfig.json" "%PUBLISH%\steam-idle.runtimeconfig.json"
copy /Y "%ROOT%SteamCardFarmer\Resources\icon.ico" "%PUBLISH%\Resources\icon.ico" 2>nul

echo.
echo ================================================
echo   BUILD TERMINE !
echo   Dossier : %PUBLISH%
echo ================================================
echo.
echo Prochaine etape : zippe le dossier 'publish' et
echo uploade-le sur GitHub Releases.
pause
goto end

:error
echo.
echo ERREUR lors du build !
pause

:end
