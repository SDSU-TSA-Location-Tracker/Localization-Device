using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
//using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace TSA_Tracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Replay loadedReplayFile = new Replay();
        Image blueCircle = new Image();
        Image yellowCircle = new Image();
        Image greenCircle = new Image();
        Image blankImage = new Image();
        Thread runThread;


        //string loadedReplayFile;
        string liveDeviceOnePath;
        string liveDeviceTwoPath;
        bool replayRunning = false;
        string data = "";



        public MainWindow()
        {
            InitializeComponent();

            blueCircle.Source = new BitmapImage(new Uri(@"/Images/BlueCircle.png", UriKind.Relative));
            yellowCircle.Source = new BitmapImage(new Uri(@"/Images/YellowCircle.png", UriKind.Relative));
            greenCircle.Source = new BitmapImage(new Uri(@"/Images/GreenCircle.png", UriKind.Relative));
            //blankImage.Source = new BitmapImage(new Uri(@"/Images/BlankImage.png", UriKind.Relative));

            BlankReplayGrid();

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
            string DeviceOne = null;
            string DeviceTwo = null;

            //Update the male and female textboxes; for the real program, replace with male/female circles in grid.
            //NOTE: we get passed information simultaneously with MQTT.
            this.Dispatcher.Invoke((Action)(() =>
            {
                //Send male/female information to male/female form elements.
                if (e.Topic == "male") DeviceOne = msg;
                else if (e.Topic == "female") DeviceTwo = msg;

                UpdateLive(DeviceOne, DeviceTwo);
            }));


        }

        private void UpdateLive(string d1, string d2)
        {
            if (d1 == "A0" && d2 == "A0") this.Dispatcher.Invoke(() => { A0L.Source = greenCircle.Source; });
            else if (d1 == "A1" && d2 == "A1") this.Dispatcher.Invoke(() => { A1L.Source = greenCircle.Source; });
            else if (d1 == "A2" && d2 == "A2") this.Dispatcher.Invoke(() => { A2L.Source = greenCircle.Source; });
            else if (d1 == "A3" && d2 == "A3") this.Dispatcher.Invoke(() => { A3L.Source = greenCircle.Source; });
            else if (d1 == "A4" && d2 == "A4") this.Dispatcher.Invoke(() => { A4L.Source = greenCircle.Source; });
            else if (d1 == "B0" && d2 == "B0") this.Dispatcher.Invoke(() => { B0L.Source = greenCircle.Source; });
            else if (d1 == "B1" && d2 == "B1") this.Dispatcher.Invoke(() => { B1L.Source = greenCircle.Source; });
            else if (d1 == "B2" && d2 == "B2") this.Dispatcher.Invoke(() => { B2L.Source = greenCircle.Source; });
            else if (d1 == "B3" && d2 == "B3") this.Dispatcher.Invoke(() => { B3L.Source = greenCircle.Source; });
            else if (d1 == "B4" && d2 == "B4") this.Dispatcher.Invoke(() => { B4L.Source = greenCircle.Source; });
            else if (d1 == "C0" && d2 == "C0") this.Dispatcher.Invoke(() => { C0L.Source = greenCircle.Source; });
            else if (d1 == "C1" && d2 == "C1") this.Dispatcher.Invoke(() => { C1L.Source = greenCircle.Source; });
            else if (d1 == "C2" && d2 == "C2") this.Dispatcher.Invoke(() => { C2L.Source = greenCircle.Source; });
            else if (d1 == "C3" && d2 == "C3") this.Dispatcher.Invoke(() => { C3L.Source = greenCircle.Source; });
            else if (d1 == "C4" && d2 == "C4") this.Dispatcher.Invoke(() => { C4L.Source = greenCircle.Source; });
            else if (d1 == "D0" && d2 == "D0") this.Dispatcher.Invoke(() => { D0L.Source = greenCircle.Source; });
            else if (d1 == "D1" && d2 == "D1") this.Dispatcher.Invoke(() => { D1L.Source = greenCircle.Source; });
            else if (d1 == "D2" && d2 == "D2") this.Dispatcher.Invoke(() => { D2L.Source = greenCircle.Source; });
            else if (d1 == "D3" && d2 == "D3") this.Dispatcher.Invoke(() => { D3L.Source = greenCircle.Source; });
            else if (d1 == "D4" && d2 == "D4") this.Dispatcher.Invoke(() => { D4L.Source = greenCircle.Source; });
            else if (d1 == "E0" && d2 == "E0") this.Dispatcher.Invoke(() => { E0L.Source = greenCircle.Source; });
            else if (d1 == "E1" && d2 == "E1") this.Dispatcher.Invoke(() => { E1L.Source = greenCircle.Source; });
            else if (d1 == "E2" && d2 == "E2") this.Dispatcher.Invoke(() => { E2L.Source = greenCircle.Source; });
            else if (d1 == "E3" && d2 == "E3") this.Dispatcher.Invoke(() => { E3L.Source = greenCircle.Source; });
            else if (d1 == "E4" && d2 == "E4") this.Dispatcher.Invoke(() => { E4L.Source = greenCircle.Source; });
            else
            {
                if (d1 == "A0") this.Dispatcher.Invoke(() => { A0L.Source = blueCircle.Source; });
                else if (d1 == "A1") this.Dispatcher.Invoke(() => { A1L.Source = blueCircle.Source; });
                else if (d1 == "A2") this.Dispatcher.Invoke(() => { A2L.Source = blueCircle.Source; });
                else if (d1 == "A3") this.Dispatcher.Invoke(() => { A3L.Source = blueCircle.Source; });
                else if (d1 == "A4") this.Dispatcher.Invoke(() => { A4L.Source = blueCircle.Source; });
                else if (d1 == "B0") this.Dispatcher.Invoke(() => { B0L.Source = blueCircle.Source; });
                else if (d1 == "B1") this.Dispatcher.Invoke(() => { B1L.Source = blueCircle.Source; });
                else if (d1 == "B2") this.Dispatcher.Invoke(() => { B2L.Source = blueCircle.Source; });
                else if (d1 == "B3") this.Dispatcher.Invoke(() => { B3L.Source = blueCircle.Source; });
                else if (d1 == "B4") this.Dispatcher.Invoke(() => { B4L.Source = blueCircle.Source; });
                else if (d1 == "C0") this.Dispatcher.Invoke(() => { C0L.Source = blueCircle.Source; });
                else if (d1 == "C1") this.Dispatcher.Invoke(() => { C1L.Source = blueCircle.Source; });
                else if (d1 == "C2") this.Dispatcher.Invoke(() => { C2L.Source = blueCircle.Source; });
                else if (d1 == "C3") this.Dispatcher.Invoke(() => { C3L.Source = blueCircle.Source; });
                else if (d1 == "C4") this.Dispatcher.Invoke(() => { C4L.Source = blueCircle.Source; });
                else if (d1 == "D0") this.Dispatcher.Invoke(() => { D0L.Source = blueCircle.Source; });
                else if (d1 == "D1") this.Dispatcher.Invoke(() => { D1L.Source = blueCircle.Source; });
                else if (d1 == "D2") this.Dispatcher.Invoke(() => { D2L.Source = blueCircle.Source; });
                else if (d1 == "D3") this.Dispatcher.Invoke(() => { D3L.Source = blueCircle.Source; });
                else if (d1 == "D4") this.Dispatcher.Invoke(() => { D4L.Source = blueCircle.Source; });
                else if (d1 == "E0") this.Dispatcher.Invoke(() => { E0L.Source = blueCircle.Source; });
                else if (d1 == "E1") this.Dispatcher.Invoke(() => { E1L.Source = blueCircle.Source; });
                else if (d1 == "E2") this.Dispatcher.Invoke(() => { E2L.Source = blueCircle.Source; });
                else if (d1 == "E3") this.Dispatcher.Invoke(() => { E3L.Source = blueCircle.Source; });
                else if (d1 == "E4") this.Dispatcher.Invoke(() => { E4L.Source = blueCircle.Source; });

                if (d2 == "A0") this.Dispatcher.Invoke(() => { A0L.Source = yellowCircle.Source; });
                else if (d2 == "A1") this.Dispatcher.Invoke(() => { A1L.Source = yellowCircle.Source; });
                else if (d2 == "A2") this.Dispatcher.Invoke(() => { A2L.Source = yellowCircle.Source; });
                else if (d2 == "A3") this.Dispatcher.Invoke(() => { A3L.Source = yellowCircle.Source; });
                else if (d2 == "A4") this.Dispatcher.Invoke(() => { A4L.Source = yellowCircle.Source; });
                else if (d2 == "B0") this.Dispatcher.Invoke(() => { B0L.Source = yellowCircle.Source; });
                else if (d2 == "B1") this.Dispatcher.Invoke(() => { B1L.Source = yellowCircle.Source; });
                else if (d2 == "B2") this.Dispatcher.Invoke(() => { B2L.Source = yellowCircle.Source; });
                else if (d2 == "B3") this.Dispatcher.Invoke(() => { B3L.Source = yellowCircle.Source; });
                else if (d2 == "B4") this.Dispatcher.Invoke(() => { B4L.Source = yellowCircle.Source; });
                else if (d2 == "C0") this.Dispatcher.Invoke(() => { C0L.Source = yellowCircle.Source; });
                else if (d2 == "C1") this.Dispatcher.Invoke(() => { C1L.Source = yellowCircle.Source; });
                else if (d2 == "C2") this.Dispatcher.Invoke(() => { C2L.Source = yellowCircle.Source; });
                else if (d2 == "C3") this.Dispatcher.Invoke(() => { C3L.Source = yellowCircle.Source; });
                else if (d2 == "C4") this.Dispatcher.Invoke(() => { C4L.Source = yellowCircle.Source; });
                else if (d2 == "D0") this.Dispatcher.Invoke(() => { D0L.Source = yellowCircle.Source; });
                else if (d2 == "D1") this.Dispatcher.Invoke(() => { D1L.Source = yellowCircle.Source; });
                else if (d2 == "D2") this.Dispatcher.Invoke(() => { D2L.Source = yellowCircle.Source; });
                else if (d2 == "D3") this.Dispatcher.Invoke(() => { D3L.Source = yellowCircle.Source; });
                else if (d2 == "D4") this.Dispatcher.Invoke(() => { D4L.Source = yellowCircle.Source; });
                else if (d2 == "E0") this.Dispatcher.Invoke(() => { E0L.Source = yellowCircle.Source; });
                else if (d2 == "E1") this.Dispatcher.Invoke(() => { E1L.Source = yellowCircle.Source; });
                else if (d2 == "E2") this.Dispatcher.Invoke(() => { E2L.Source = yellowCircle.Source; });
                else if (d2 == "E3") this.Dispatcher.Invoke(() => { E3L.Source = yellowCircle.Source; });
                else if (d2 == "E4") this.Dispatcher.Invoke(() => { E4L.Source = yellowCircle.Source; });
            }
        }

        private void LoadReplayFileButton_Click(object sender, RoutedEventArgs e)
        {
            string directory = Environment.CurrentDirectory + "\\ReplayFiles";

            OpenFileDialog loadDialog = new OpenFileDialog();
            loadDialog.Title = ("Select Replay File");
            loadDialog.InitialDirectory = (directory);
            loadDialog.Filter = ("Text File|*.txt");
            loadDialog.RestoreDirectory = true;

            bool? fileOpenResult = loadDialog.ShowDialog();

            if(fileOpenResult == true)
            {
                try
                {
                    string allCoords;
                    loadedReplayFile.LoadedReplayFilePath = loadDialog.FileName;

                    if(File.Exists(loadedReplayFile.LoadedReplayFilePath))
                    {
                        StreamReader loadedReplay = new StreamReader(loadedReplayFile.LoadedReplayFilePath);
                        allCoords = loadedReplay.ReadToEnd();

                        char[] separators = new char[] { '\t', '\r', '\n' };
                        string[] s = allCoords.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        List<string> list1 = new List<string>();
                        List<string> list2 = new List<string>();

                        //Create lists for both male and female device coordinates
                        for(int i = 0; i < s.Length; i += 2)
                        {
                            list1.Add(s[i]);
                            list2.Add(s[i+1]);
                        }

                        //Load these lists as arrays into replay object
                        loadedReplayFile.DeviceOnePositions = list1.ToArray();
                        loadedReplayFile.DeviceTwoPositions = list2.ToArray();

                        FileName_TextBox.Text = loadDialog.SafeFileName;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error loading file: " + ex);
                }
            }
        }

        private AutoResetEvent waitHandle = new AutoResetEvent(false);

        private void StartReplay_Button_Click(object sender, RoutedEventArgs e)
        {
            
            runThread = new Thread(() => RunReplay(waitHandle));
            runThread.Start();

        }

        

        private void RunReplay(WaitHandle wh)
        {
            if (replayRunning)
                return;

            replayRunning = true;

            for (int i = 0; i < loadedReplayFile.DeviceOnePositions.Length; i++)
            {

                if (loadedReplayFile.DeviceOnePositions[i] == "A0" && loadedReplayFile.DeviceTwoPositions[i] == "A0") this.Dispatcher.Invoke(() => { A0R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "A1" && loadedReplayFile.DeviceTwoPositions[i] == "A1") this.Dispatcher.Invoke(() => { A1R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "A2" && loadedReplayFile.DeviceTwoPositions[i] == "A2") this.Dispatcher.Invoke(() => { A2R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "A3" && loadedReplayFile.DeviceTwoPositions[i] == "A3") this.Dispatcher.Invoke(() => { A3R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "A4" && loadedReplayFile.DeviceTwoPositions[i] == "A4") this.Dispatcher.Invoke(() => { A4R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "B0" && loadedReplayFile.DeviceTwoPositions[i] == "B0") this.Dispatcher.Invoke(() => { B0R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "B1" && loadedReplayFile.DeviceTwoPositions[i] == "B1") this.Dispatcher.Invoke(() => { B1R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "B2" && loadedReplayFile.DeviceTwoPositions[i] == "B2") this.Dispatcher.Invoke(() => { B2R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "B3" && loadedReplayFile.DeviceTwoPositions[i] == "B3") this.Dispatcher.Invoke(() => { B3R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "B4" && loadedReplayFile.DeviceTwoPositions[i] == "B4") this.Dispatcher.Invoke(() => { B4R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "C0" && loadedReplayFile.DeviceTwoPositions[i] == "C0") this.Dispatcher.Invoke(() => { C0R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "C1" && loadedReplayFile.DeviceTwoPositions[i] == "C1") this.Dispatcher.Invoke(() => { C1R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "C2" && loadedReplayFile.DeviceTwoPositions[i] == "C2") this.Dispatcher.Invoke(() => { C2R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "C3" && loadedReplayFile.DeviceTwoPositions[i] == "C3") this.Dispatcher.Invoke(() => { C3R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "C4" && loadedReplayFile.DeviceTwoPositions[i] == "C4") this.Dispatcher.Invoke(() => { C4R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "D0" && loadedReplayFile.DeviceTwoPositions[i] == "D0") this.Dispatcher.Invoke(() => { D0R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "D1" && loadedReplayFile.DeviceTwoPositions[i] == "D1") this.Dispatcher.Invoke(() => { D1R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "D2" && loadedReplayFile.DeviceTwoPositions[i] == "D2") this.Dispatcher.Invoke(() => { D2R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "D3" && loadedReplayFile.DeviceTwoPositions[i] == "D3") this.Dispatcher.Invoke(() => { D3R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "D4" && loadedReplayFile.DeviceTwoPositions[i] == "D4") this.Dispatcher.Invoke(() => { D4R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "E0" && loadedReplayFile.DeviceTwoPositions[i] == "E0") this.Dispatcher.Invoke(() => { E0R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "E1" && loadedReplayFile.DeviceTwoPositions[i] == "E1") this.Dispatcher.Invoke(() => { E1R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "E2" && loadedReplayFile.DeviceTwoPositions[i] == "E2") this.Dispatcher.Invoke(() => { E2R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "E3" && loadedReplayFile.DeviceTwoPositions[i] == "E3") this.Dispatcher.Invoke(() => { E3R.Source = greenCircle.Source; });
                else if (loadedReplayFile.DeviceOnePositions[i] == "E4" && loadedReplayFile.DeviceTwoPositions[i] == "E4") this.Dispatcher.Invoke(() => { E4R.Source = greenCircle.Source; });
                else
                {
                    if (loadedReplayFile.DeviceOnePositions[i] == "A0") this.Dispatcher.Invoke(() => { A0R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "A1") this.Dispatcher.Invoke(() => { A1R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "A2") this.Dispatcher.Invoke(() => { A2R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "A3") this.Dispatcher.Invoke(() => { A3R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "A4") this.Dispatcher.Invoke(() => { A4R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "B0") this.Dispatcher.Invoke(() => { B0R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "B1") this.Dispatcher.Invoke(() => { B1R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "B2") this.Dispatcher.Invoke(() => { B2R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "B3") this.Dispatcher.Invoke(() => { B3R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "B4") this.Dispatcher.Invoke(() => { B4R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "C0") this.Dispatcher.Invoke(() => { C0R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "C1") this.Dispatcher.Invoke(() => { C1R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "C2") this.Dispatcher.Invoke(() => { C2R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "C3") this.Dispatcher.Invoke(() => { C3R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "C4") this.Dispatcher.Invoke(() => { C4R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "D0") this.Dispatcher.Invoke(() => { D0R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "D1") this.Dispatcher.Invoke(() => { D1R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "D2") this.Dispatcher.Invoke(() => { D2R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "D3") this.Dispatcher.Invoke(() => { D3R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "D4") this.Dispatcher.Invoke(() => { D4R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "E0") this.Dispatcher.Invoke(() => { E0R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "E1") this.Dispatcher.Invoke(() => { E1R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "E2") this.Dispatcher.Invoke(() => { E2R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "E3") this.Dispatcher.Invoke(() => { E3R.Source = blueCircle.Source; });
                    else if (loadedReplayFile.DeviceOnePositions[i] == "E4") this.Dispatcher.Invoke(() => { E4R.Source = blueCircle.Source; });

                    if (loadedReplayFile.DeviceTwoPositions[i] == "A0") this.Dispatcher.Invoke(() => { A0R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "A1") this.Dispatcher.Invoke(() => { A1R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "A2") this.Dispatcher.Invoke(() => { A2R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "A3") this.Dispatcher.Invoke(() => { A3R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "A4") this.Dispatcher.Invoke(() => { A4R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "B0") this.Dispatcher.Invoke(() => { B0R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "B1") this.Dispatcher.Invoke(() => { B1R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "B2") this.Dispatcher.Invoke(() => { B2R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "B3") this.Dispatcher.Invoke(() => { B3R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "B4") this.Dispatcher.Invoke(() => { B4R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "C0") this.Dispatcher.Invoke(() => { C0R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "C1") this.Dispatcher.Invoke(() => { C1R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "C2") this.Dispatcher.Invoke(() => { C2R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "C3") this.Dispatcher.Invoke(() => { C3R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "C4") this.Dispatcher.Invoke(() => { C4R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "D0") this.Dispatcher.Invoke(() => { D0R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "D1") this.Dispatcher.Invoke(() => { D1R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "D2") this.Dispatcher.Invoke(() => { D2R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "D3") this.Dispatcher.Invoke(() => { D3R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "D4") this.Dispatcher.Invoke(() => { D4R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "E0") this.Dispatcher.Invoke(() => { E0R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "E1") this.Dispatcher.Invoke(() => { E1R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "E2") this.Dispatcher.Invoke(() => { E2R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "E3") this.Dispatcher.Invoke(() => { E3R.Source = yellowCircle.Source; });
                    else if (loadedReplayFile.DeviceTwoPositions[i] == "E4") this.Dispatcher.Invoke(() => { E4R.Source = yellowCircle.Source; });
                }

                System.Threading.Thread.Sleep(10000);
                BlankReplayGrid();

            }

            replayRunning = false;
        }

        private void BlankReplayGrid()
        {
            this.Dispatcher.Invoke(() => { A0R.Source = null; });
            this.Dispatcher.Invoke(() => { A1R.Source = null; });
            this.Dispatcher.Invoke(() => { A2R.Source = null; });
            this.Dispatcher.Invoke(() => { A3R.Source = null; });
            this.Dispatcher.Invoke(() => { A4R.Source = null; });
            this.Dispatcher.Invoke(() => { B0R.Source = null; });
            this.Dispatcher.Invoke(() => { B1R.Source = null; });
            this.Dispatcher.Invoke(() => { B2R.Source = null; });
            this.Dispatcher.Invoke(() => { B3R.Source = null; });
            this.Dispatcher.Invoke(() => { B4R.Source = null; });
            this.Dispatcher.Invoke(() => { C0R.Source = null; });
            this.Dispatcher.Invoke(() => { C1R.Source = null; });
            this.Dispatcher.Invoke(() => { C2R.Source = null; });
            this.Dispatcher.Invoke(() => { C3R.Source = null; });
            this.Dispatcher.Invoke(() => { C4R.Source = null; });
            this.Dispatcher.Invoke(() => { D0R.Source = null; });
            this.Dispatcher.Invoke(() => { D1R.Source = null; });
            this.Dispatcher.Invoke(() => { D2R.Source = null; });
            this.Dispatcher.Invoke(() => { D3R.Source = null; });
            this.Dispatcher.Invoke(() => { D4R.Source = null; });
            this.Dispatcher.Invoke(() => { E0R.Source = null; });
            this.Dispatcher.Invoke(() => { E1R.Source = null; });
            this.Dispatcher.Invoke(() => { E2R.Source = null; });
            this.Dispatcher.Invoke(() => { E3R.Source = null; });
            this.Dispatcher.Invoke(() => { E4R.Source = null; });
        }

        private void StopReplay_Button_Click(object sender, RoutedEventArgs e)
        {
           

                
        }
    }

    public class Replay
    {
        public string LoadedReplayFilePath { get; set; }
        public string[] DeviceOnePositions { get; set; } 
        public string[] DeviceTwoPositions { get; set; }
        public DateTime StartTime { get; set; }

    }
}
