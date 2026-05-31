@echo off
echo ================================================
echo   Unlock Steam Cards - Build Release
echo ================================================
echo.

set ROOT=%~dp0
set PUBLISH=%ROOT%publish
set DEBUG_OUT=%ROOT%SteamCardFarmer\bin\x64\Debug\net8.0-windows\win-x64
set IDLE_DIR=%PUBLISH%\idle

:: Nettoyage
echo [1/5] Nettoyage...
if exist "%PUBLISH%" rd /s /q "%PUBLISH%"
mkdir "%PUBLISH%"
mkdir "%PUBLISH%\Resources"
mkdir "%IDLE_DIR%"

:: Publish SteamCardFarmer (self-contained)
echo [2/5] Publication de SteamCardFarmer...
dotnet publish "%ROOT%SteamCardFarmer\SteamCardFarmer.csproj" -c Release -r win-x64 --self-contained true -o "%PUBLISH%" /p:Platform=x64
if errorlevel 1 goto error

:: Publish SteamIdle dans sous-dossier idle\
echo [3/5] Publication de steam-idle dans idle\...
dotnet publish "%ROOT%SteamIdle\SteamIdle.csproj" -c Release -r win-x64 --self-contained true -o "%IDLE_DIR%"
if errorlevel 1 goto error

:: Copie steam_api64.dll dans idle\
echo [4/5] Copie de steam_api64.dll dans idle\...
if exist "%DEBUG_OUT%\steam_api64.dll" (
    copy /Y "%DEBUG_OUT%\steam_api64.dll" "%IDLE_DIR%\steam_api64.dll"
    echo    OK - steam_api64.dll inclus dans idle\
) else (
    echo    ATTENTION : Lance d'abord l'app en debug F5 pour qu'elle detecte steam_api64.dll
    echo    Puis relance ce script.
    pause
    goto end
)

:: Ressources
echo [5/5] Ressources...
copy /Y "%ROOT%SteamCardFarmer\Resources\icon.ico" "%PUBLISH%\Resources\icon.ico" 2>nul

echo.
echo ================================================
echo   BUILD TERMINE !
echo ================================================
echo Ouvre Inno Setup et compile installer\setup.iss ^(F9^)
pause
goto end

:error
echo ERREUR lors du build !
pause
:end
