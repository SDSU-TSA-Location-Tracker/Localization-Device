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
        string data = "";

        public MainWindow()
        {
            InitializeComponent();
            
            //Create client for MQTT broker.
            MqttClient mqttClient = new MqttClient("192.168.1.232");
            data = "MQTT Client installed and connected to Holistic node with broker: 192.168.1.232";

            //Get published message.
            mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            //Subscribe to specific topic lines for male and female coordinate locations.
            mqttClient.Subscribe(new string[] { "male" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            mqttClient.Subscribe(new string[] { "female" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            //Create connection.
            mqttClient.Connect("CSharpMqttClient", "", "", true, 60);
        }


        //We need to automatically update the msg value whenever new data is received, so we cannot simply 
        //do this main. Because we are modifying form elements not in a function called in main, we have to
        //use this.Dispatcher. My guess is we have to do this every time we interact with form elements.
        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            
            var msg = System.Text.Encoding.Default.GetString(e.Message);
            
            //Update the male and female textboxes; for the real program, replace with male/female circles in grid.
            //NOTE: we get passed information simultaneously with MQTT.
            this.Dispatcher.Invoke((Action)(() =>
            {
                //Send male/female information to male/female form elements.
                if (e.Topic == "male") MaleTextBox.Text = msg;
                else if (e.Topic == "female") FemaleTextBox.Text = msg;
            }));
            
        }
    }
}
