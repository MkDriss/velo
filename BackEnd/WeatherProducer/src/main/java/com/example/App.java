package com.example;

import javax.jms.Connection;
import javax.jms.ConnectionFactory;
import javax.jms.DeliveryMode;
import javax.jms.MessageProducer;
import javax.jms.Queue;
import javax.jms.Session;
import javax.jms.TextMessage;

import org.apache.activemq.ActiveMQConnectionFactory;

import java.util.Scanner;

public class App {
    public static void main(String[] args) {
        String brokerURL = "tcp://localhost:61616";
        String queueName = "weatherQueueVelo";

        Connection connection = null;

        try {
            ConnectionFactory factory = new ActiveMQConnectionFactory(brokerURL);
            connection = factory.createConnection();
            connection.start();

            Session session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);
            Queue queue = session.createQueue(queueName);

            MessageProducer producer = session.createProducer(queue);
            producer.setDeliveryMode(DeliveryMode.NON_PERSISTENT);

            Scanner scanner = new Scanner(System.in);
            String userinput = "";

            while (!userinput.equals("0")) {
                System.out.println("Tape 1 pour 'sun', 2 pour 'cloud', '3' pour l'appel de Dieu, 0 pour quitter : ");
                userinput = scanner.nextLine();

                String payload = null;

                switch (userinput) {
                    case "1":
                        payload = "sun";
                        break;
                    case "2":
                        payload = "cloud";
                        break;
                    case "3":
                        payload = "god";
                        break;
                    case "0":
                        System.out.println("Fermeture...");
                        break;
                    default:
                        System.out.println("Choix invalide !");
                        break;
                }

                if (payload != null) {
                    TextMessage msg = session.createTextMessage(payload);
                    producer.send(msg);
                    System.out.println("Message envoyé : " + payload);
                }
            }

            session.close();
            connection.close();

        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            try { if (connection != null) connection.close(); }
            catch (Exception ignore) {}
        }
    }
}