using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string BrokerAdress = "192.168.4.65";
            string ClientID = "CSharpMqttClient";
            string Topic = "topic";
            string MessageToPublish;
            //Create MQTT client instance

            MqttClient mqttClient = new MqttClient(BrokerAdress);
            Console.WriteLine("mqttClient installed for broker " + BrokerAdress);

            // register to message received 
            mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            //Subscribe to the indicated topic
            // subscribe to the topic with QoS 2 

            mqttClient.Subscribe(new string[] { Topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            //Connect mqttCient

            mqttClient.Connect(ClientID, "", "", true, 60);

            Console.WriteLine(ClientID + " connected");
            // publish messages on "hello world" topic with QoS 2 

            MessageToPublish = "RECEIVE_START";
            mqttClient.Publish(Topic, Encoding.UTF8.GetBytes(MessageToPublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            System.Console.WriteLine("Published message: " + MessageToPublish);

        }

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received 
            var msg = System.Text.Encoding.Default.GetString(e.Message);
            System.Console.WriteLine("Message Received: " + msg);
        }

    }
}