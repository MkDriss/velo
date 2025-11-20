# Projet Velo

## Lancer le projet

### Prérequis

- ActiveMQ version 6.1.8 doit être lancé.
- Python 3 doit être installé (pour héberger un serveur web).
- Maven

### Étapes

1. Pour lancer le **Frontend** ainsi que le **Producer ActiveMQ** pour la météo et le **GPS_Server**, exécutez **EN ADMINISTRATEUR** :

    ```
    ./start.bat
    ```

2. Pour accéder au frontend, ouvrez votre navigateur à l'adresse :

    [http://localhost:8080/](http://localhost:8080/)

3. Pour envoyer des données météo dans le activeMq allez sur le CMD nommé **WeatherProducer** et tapez : 

    - *1* Pour envoyer *"sun"* pour simuler un beau temps

    - *2* Pour envoyer *"cloud"* pour simuler un mauvais temps



## En cas de soucis avec le start.bat

### Lancement manuel

NB : Il faut toujours que vous ayez activeMQ 6.1.8 de lancé

1. Lancer le **FrontEnd** :
    ```bash
    cd FrontEnd
    ```

    ```bash
    python -m http.server 8080
    ```

2. Lancer le **GPS_Server**:
    ```bash
    cd BackEnd/GPS_Server/bin/Debug
    ```

    ```bash
    GPS_Server.exe
    ```

3. Lancer le **Weather_Producer**:

    ```bash
    cd BackEnd/WeatherProducer
    ```

    ```bash
    mvn clean compile
    ```
    ```bash
    mvn exec:java
    ```
    
