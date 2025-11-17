@echo off

echo === Lancement du WeatherProducer (ActiveMQ) en Java avec Maven ===
start "WeatherProducer" cmd /k "cd BackEnd/WeatherProducer && mvn clean compile exec:java"

echo === Lancement du serveur HTTP ===
start "FrontEnd Webserver" cmd /k "cd FrontEnd && python -m http.server 8080"


echo Tout est lancé !
pause
