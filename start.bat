@echo off
setlocal

REM Chemin du dossier du script
set "BASEDIR=%~dp0"

echo === Lancement du WeatherProducer (ActiveMQ) en Java avec Maven ===
start "WeatherProducer" cmd /k "cd /d %BASEDIR%BackEnd\WeatherProducer && mvn clean compile exec:java"

echo === Lancement du serveur HTTP ===
start "FrontEnd Webserver" cmd /k "cd /d %BASEDIR%FrontEnd && python -m http.server 8080"

echo === Lancement du BackEnd ===
start "BackEnd Server" cmd /k "cd /d %BASEDIR%BackEnd\GPS_Server\bin\Debug && GPS_Server.exe"

echo Tout est lancé !
pause
