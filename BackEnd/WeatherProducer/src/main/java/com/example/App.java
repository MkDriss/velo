package com.example;

import javax.jms.Connection;
import javax.jms.ConnectionFactory;
import javax.jms.DeliveryMode;
import javax.jms.MessageProducer;
import javax.jms.Queue;
import javax.jms.Session;
import javax.jms.TextMessage;

import org.apache.activemq.ActiveMQConnectionFactory;

public class App {
    public static void main(String[] args) {
        // URL du broker ActiveMQ (exemple : broker local)
        String brokerURL = "tcp://localhost:61616";

        // Nom de la queue
        String queueName = "weather";

        Connection connection = null;

        try {
            // 1. Créer la factory de connexion
            ConnectionFactory factory = new ActiveMQConnectionFactory(brokerURL);

            // 2. Créer la connexion
            connection = factory.createConnection();
            connection.start();

            // 3. Créer une session
            Session session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);

            // 4. Créer la queue
            Queue queue = session.createQueue(queueName);

            // 5. Créer le producteur
            MessageProducer producer = session.createProducer(queue);
            producer.setDeliveryMode(DeliveryMode.NON_PERSISTENT);

            // 6. Créer un message texte
            TextMessage message = session.createTextMessage("Hello Weather!");

            // 7. Envoyer le message
            producer.send(message);
            System.out.println("Message envoyé : " + message.getText());

            // 8. Fermer la session et la connexion
            session.close();
            connection.close();
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            try {
                if (connection != null) connection.close();
            } catch (Exception ignore) {}
        }
    }
}
