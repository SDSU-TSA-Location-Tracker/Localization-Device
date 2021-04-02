using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace TestCoorodianatesProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string data = "hello";

        public MainWindow()
        {
            InitializeComponent();

            //We connect to the MQTT broker in the main thread.
            string BrokerAdress = "192.168.4.65";
            string ClientID = "CSharpMqttClient";
            string Topic = "topic";
            string MessageToPublish;
            //Create MQTT client instance

            MqttClient mqttClient = new MqttClient(BrokerAdress);
            data = "mqttClient installed for broker " + BrokerAdress;

            // register to message received 
            mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            //Subscribe to the indicated topic
            // subscribe to the topic with QoS 2 

            mqttClient.Subscribe(new string[] { Topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            //Connect mqttCient

            mqttClient.Connect(ClientID, "", "", true, 60);
        }


        //We need to automatically update the msg value whenever new data is received, so we cannot simply 
        //do this main. Because we are modifying form elements not in a function called in main, we have to
        //use this.Dispatcher. My guess is we have to do this every time we interact with form elements.
        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received 
            var msg = System.Text.Encoding.Default.GetString(e.Message);
            //UpdateTextBox(data);
            this.Dispatcher.Invoke((Action)(() =>
            {
                CoordinatesTextBox.Text = msg;
            }));
            
        }
    }
}
