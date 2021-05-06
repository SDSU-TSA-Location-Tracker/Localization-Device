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


        bool replayRunning = false;
        bool pauseReplay = false;
        string data = "";
        DateTime elapsedTime = new DateTime();
        DateTime fileDateTime = new DateTime();



        public MainWindow()
        {
            InitializeComponent();

            blueCircle.Source = new BitmapImage(new Uri(@"/Images/BlueCircle.png", UriKind.Relative));
            yellowCircle.Source = new BitmapImage(new Uri(@"/Images/YellowCircle.png", UriKind.Relative));
            greenCircle.Source = new BitmapImage(new Uri(@"/Images/GreenCircle.png", UriKind.Relative));
            //blankImage.Source = new BitmapImage(new Uri(@"/Images/BlankImage.png", UriKind.Relative));

            BlankReplayGrid();

            Play_Button.Visibility = Visibility.Hidden;
            Pause_Button.Visibility = Visibility.Hidden;

            //    //Create client for MQTT broker.
            //    MqttClient mqttClient = new MqttClient("192.168.0.4");
            //    data = "MQTT Client installed and connected to Holistic node with broker: 192.168.0.4";

            //    //Get published message.
            //    mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            //    //Subscribe to specific topic lines for male and female coordinate locations.
            //    mqttClient.Subscribe(new string[] { "male" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            //    mqttClient.Subscribe(new string[] { "female" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            //    //Create connection.
            //    mqttClient.Connect("CSharpMqttClient", "", "", true, 60);
            //}

            ////We need to automatically update the msg value whenever new data is received, so we cannot simply 
            ////do this main. Because we are modifying form elements not in a function called in main, we have to
            ////use this.Dispatcher. My guess is we have to do this every time we interact with form elements.
            //private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
            //{

            //    var msg = System.Text.Encoding.Default.GetString(e.Message);
            //    string DeviceOne = null;
            //    string DeviceTwo = null;
            //    int i = 0;
            //    //Update the male and female textboxes; for the real program, replace with male/female circles in grid.
            //    //NOTE: we get passed information simultaneously with MQTT.
            //    this.Dispatcher.Invoke((Action)(() =>
            //    {
            //        //Send male/female information to male/female form elements.
            //        string coords = msg;
            //        char[] separators = new char[] { '\t' };
            //        string[] s = coords.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            //        DeviceOne = s[0];
            //        DeviceTwo = s[1];

            //        UpdateLive(DeviceOne, DeviceTwo);
            //    }));


        }

        private void UpdateLive(string d1, string d2)
        {
            BlankLiveGrid();

            string coords = d1 + d2;

            switch(coords)
            {
                case "A0A0":
                    this.Dispatcher.Invoke(() => { A0L.Source = greenCircle.Source; });
                    break;
                case "A1A1":
                    this.Dispatcher.Invoke(() => { A1L.Source = greenCircle.Source; });
                    break;
                case "A2A2":
                    this.Dispatcher.Invoke(() => { A2L.Source = greenCircle.Source; });
                    break;
                case "A3A3":
                    this.Dispatcher.Invoke(() => { A3L.Source = greenCircle.Source; });
                    break;
                case "A4A4":
                    this.Dispatcher.Invoke(() => { A4L.Source = greenCircle.Source; });
                    break;
                case "B0B0":
                    this.Dispatcher.Invoke(() => { B0L.Source = greenCircle.Source; });
                    break;
                case "B1B1":
                    this.Dispatcher.Invoke(() => { B1L.Source = greenCircle.Source; });
                    break;
                case "B2B2":
                    this.Dispatcher.Invoke(() => { B2L.Source = greenCircle.Source; });
                    break;
                case "B3B3":
                    this.Dispatcher.Invoke(() => { B3L.Source = greenCircle.Source; });
                    break;
                case "B4B4":
                    this.Dispatcher.Invoke(() => { B4L.Source = greenCircle.Source; });
                    break;
                case "C0C0":
                    this.Dispatcher.Invoke(() => { C0L.Source = greenCircle.Source; });
                    break;
                case "C1C1":
                    this.Dispatcher.Invoke(() => { C1L.Source = greenCircle.Source; });
                    break;
                case "C2C2":
                    this.Dispatcher.Invoke(() => { C2L.Source = greenCircle.Source; });
                    break;
                case "C3C3":
                    this.Dispatcher.Invoke(() => { C3L.Source = greenCircle.Source; });
                    break;
                case "C4C4":
                    this.Dispatcher.Invoke(() => { C4L.Source = greenCircle.Source; });
                    break;
                case "D0D0":
                    this.Dispatcher.Invoke(() => { D0L.Source = greenCircle.Source; });
                    break;
                case "D1D1":
                    this.Dispatcher.Invoke(() => { D1L.Source = greenCircle.Source; });
                    break;
                case "D2D2":
                    this.Dispatcher.Invoke(() => { D2L.Source = greenCircle.Source; });
                    break;
                case "D3D3":
                    this.Dispatcher.Invoke(() => { D3L.Source = greenCircle.Source; });
                    break;
                case "D4D4":
                    this.Dispatcher.Invoke(() => { D4L.Source = greenCircle.Source; });
                    break;
                case "E0E0":
                    this.Dispatcher.Invoke(() => { E0L.Source = greenCircle.Source; });
                    break;
                case "E1E1":
                    this.Dispatcher.Invoke(() => { E1L.Source = greenCircle.Source; });
                    break;
                case "E2E2":
                    this.Dispatcher.Invoke(() => { E2L.Source = greenCircle.Source; });
                    break;
                case "E3E3":
                    this.Dispatcher.Invoke(() => { E3L.Source = greenCircle.Source; });
                    break;
                case "E4E4":
                    this.Dispatcher.Invoke(() => { E4L.Source = greenCircle.Source; });
                    break;
                default:
                    switch (d1)
                    {
                        case "A0":
                            this.Dispatcher.Invoke(() => { A0L.Source = blueCircle.Source; });
                            break;
                        case "A1":
                            this.Dispatcher.Invoke(() => { A1L.Source = blueCircle.Source; });
                            break;
                        case "A2":
                            this.Dispatcher.Invoke(() => { A2L.Source = blueCircle.Source; });
                            break;
                        case "A3":
                            this.Dispatcher.Invoke(() => { A3L.Source = blueCircle.Source; });
                            break;
                        case "A4":
                            this.Dispatcher.Invoke(() => { A4L.Source = blueCircle.Source; });
                            break;
                        case "B0":
                            this.Dispatcher.Invoke(() => { B0L.Source = blueCircle.Source; });
                            break;
                        case "B1":
                            this.Dispatcher.Invoke(() => { B1L.Source = blueCircle.Source; });
                            break;
                        case "B2":
                            this.Dispatcher.Invoke(() => { B2L.Source = blueCircle.Source; });
                            break;
                        case "B3":
                            this.Dispatcher.Invoke(() => { B3L.Source = blueCircle.Source; });
                            break;
                        case "B4":
                            this.Dispatcher.Invoke(() => { B4L.Source = blueCircle.Source; });
                            break;
                        case "C0":
                            this.Dispatcher.Invoke(() => { C0L.Source = blueCircle.Source; });
                            break;
                        case "C1":
                            this.Dispatcher.Invoke(() => { C1L.Source = blueCircle.Source; });
                            break;
                        case "C2":
                            this.Dispatcher.Invoke(() => { C2L.Source = blueCircle.Source; });
                            break;
                        case "C3":
                            this.Dispatcher.Invoke(() => { C3L.Source = blueCircle.Source; });
                            break;
                        case "C4":
                            this.Dispatcher.Invoke(() => { C4L.Source = blueCircle.Source; });
                            break;
                        case "D0":
                            this.Dispatcher.Invoke(() => { D0L.Source = blueCircle.Source; });
                            break;
                        case "D1":
                            this.Dispatcher.Invoke(() => { D1L.Source = blueCircle.Source; });
                            break;
                        case "D2":
                            this.Dispatcher.Invoke(() => { D2L.Source = blueCircle.Source; });
                            break;
                        case "D3":
                            this.Dispatcher.Invoke(() => { D3L.Source = blueCircle.Source; });
                            break;
                        case "D4":
                            this.Dispatcher.Invoke(() => { D4L.Source = blueCircle.Source; });
                            break;
                        case "E0":
                            this.Dispatcher.Invoke(() => { E0L.Source = blueCircle.Source; });
                            break;
                        case "E1":
                            this.Dispatcher.Invoke(() => { E1L.Source = blueCircle.Source; });
                            break;
                        case "E2":
                            this.Dispatcher.Invoke(() => { E2L.Source = blueCircle.Source; });
                            break;
                        case "E3":
                            this.Dispatcher.Invoke(() => { E3L.Source = blueCircle.Source; });
                            break;
                        case "E4":
                            this.Dispatcher.Invoke(() => { E4L.Source = blueCircle.Source; });
                            break;
                    }

                    switch (d2)
                    {
                        case "A0":
                            this.Dispatcher.Invoke(() => { A0L.Source = yellowCircle.Source; });
                            break;
                        case "A1":
                            this.Dispatcher.Invoke(() => { A1L.Source = yellowCircle.Source; });
                            break;
                        case "A2":
                            this.Dispatcher.Invoke(() => { A2L.Source = yellowCircle.Source; });
                            break;
                        case "A3":
                            this.Dispatcher.Invoke(() => { A3L.Source = yellowCircle.Source; });
                            break;
                        case "A4":
                            this.Dispatcher.Invoke(() => { A4L.Source = yellowCircle.Source; });
                            break;
                        case "B0":
                            this.Dispatcher.Invoke(() => { B0L.Source = yellowCircle.Source; });
                            break;
                        case "B1":
                            this.Dispatcher.Invoke(() => { B1L.Source = yellowCircle.Source; });
                            break;
                        case "B2":
                            this.Dispatcher.Invoke(() => { B2L.Source = yellowCircle.Source; });
                            break;
                        case "B3":
                            this.Dispatcher.Invoke(() => { B3L.Source = yellowCircle.Source; });
                            break;
                        case "B4":
                            this.Dispatcher.Invoke(() => { B4L.Source = yellowCircle.Source; });
                            break;
                        case "C0":
                            this.Dispatcher.Invoke(() => { C0L.Source = yellowCircle.Source; });
                            break;
                        case "C1":
                            this.Dispatcher.Invoke(() => { C1L.Source = yellowCircle.Source; });
                            break;
                        case "C2":
                            this.Dispatcher.Invoke(() => { C2L.Source = yellowCircle.Source; });
                            break;
                        case "C3":
                            this.Dispatcher.Invoke(() => { C3L.Source = yellowCircle.Source; });
                            break;
                        case "C4":
                            this.Dispatcher.Invoke(() => { C4L.Source = yellowCircle.Source; });
                            break;
                        case "D0":
                            this.Dispatcher.Invoke(() => { D0L.Source = yellowCircle.Source; });
                            break;
                        case "D1":
                            this.Dispatcher.Invoke(() => { D1L.Source = yellowCircle.Source; });
                            break;
                        case "D2":
                            this.Dispatcher.Invoke(() => { D2L.Source = yellowCircle.Source; });
                            break;
                        case "D3":
                            this.Dispatcher.Invoke(() => { D3L.Source = yellowCircle.Source; });
                            break;
                        case "D4":
                            this.Dispatcher.Invoke(() => { D4L.Source = yellowCircle.Source; });
                            break;
                        case "E0":
                            this.Dispatcher.Invoke(() => { E0L.Source = yellowCircle.Source; });
                            break;
                        case "E1":
                            this.Dispatcher.Invoke(() => { E1L.Source = yellowCircle.Source; });
                            break;
                        case "E2":
                            this.Dispatcher.Invoke(() => { E2L.Source = yellowCircle.Source; });
                            break;
                        case "E3":
                            this.Dispatcher.Invoke(() => { E3L.Source = yellowCircle.Source; });
                            break;
                        case "E4":
                            this.Dispatcher.Invoke(() => { E4L.Source = yellowCircle.Source; });
                            break;
                    }
                    break;
            }
        }

        private void BlankLiveGrid()
        {
            this.Dispatcher.Invoke(() => { A0L.Source = null; });
            this.Dispatcher.Invoke(() => { A1L.Source = null; });
            this.Dispatcher.Invoke(() => { A2L.Source = null; });
            this.Dispatcher.Invoke(() => { A3L.Source = null; });
            this.Dispatcher.Invoke(() => { A4L.Source = null; });
            this.Dispatcher.Invoke(() => { B0L.Source = null; });
            this.Dispatcher.Invoke(() => { B1L.Source = null; });
            this.Dispatcher.Invoke(() => { B2L.Source = null; });
            this.Dispatcher.Invoke(() => { B3L.Source = null; });
            this.Dispatcher.Invoke(() => { B4L.Source = null; });
            this.Dispatcher.Invoke(() => { C0L.Source = null; });
            this.Dispatcher.Invoke(() => { C1L.Source = null; });
            this.Dispatcher.Invoke(() => { C2L.Source = null; });
            this.Dispatcher.Invoke(() => { C3L.Source = null; });
            this.Dispatcher.Invoke(() => { C4L.Source = null; });
            this.Dispatcher.Invoke(() => { D0L.Source = null; });
            this.Dispatcher.Invoke(() => { D1L.Source = null; });
            this.Dispatcher.Invoke(() => { D2L.Source = null; });
            this.Dispatcher.Invoke(() => { D3L.Source = null; });
            this.Dispatcher.Invoke(() => { D4L.Source = null; });
            this.Dispatcher.Invoke(() => { E0L.Source = null; });
            this.Dispatcher.Invoke(() => { E1L.Source = null; });
            this.Dispatcher.Invoke(() => { E2L.Source = null; });
            this.Dispatcher.Invoke(() => { E3L.Source = null; });
            this.Dispatcher.Invoke(() => { E4L.Source = null; });
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

            if (fileOpenResult == true)
            {
                try
                {
                    string allCoords;
                    loadedReplayFile.LoadedReplayFilePath = loadDialog.FileName;

                    if (File.Exists(loadedReplayFile.LoadedReplayFilePath))
                    {
                        StreamReader loadedReplay = new StreamReader(loadedReplayFile.LoadedReplayFilePath);
                        allCoords = loadedReplay.ReadToEnd();

                        char[] separators = new char[] { '\t', '\r', '\n' };
                        string[] s = allCoords.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        List<string> list1 = new List<string>();
                        List<string> list2 = new List<string>();

                        //Create lists for both male and female device coordinates
                        for (int i = 0; i < s.Length; i += 2)
                        {
                            list1.Add(s[i]);
                            list2.Add(s[i + 1]);
                        }

                        //Load these lists as arrays into replay object
                        loadedReplayFile.DeviceOnePositions = list1.ToArray();
                        loadedReplayFile.DeviceTwoPositions = list2.ToArray();

                        FileName_TextBox.Text = loadDialog.SafeFileName;
                        DisplayDateAndTime(loadDialog.SafeFileName);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading file: " + ex);
                }
            }
        }

        private void DisplayDateAndTime(string fileName)
        {
            string date;
            string time;
            char[] separators = new char[] { '_', '.' };
            string[] dateTime = fileName.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            //date = dateTime[0] + '/' + dateTime[1] + '/' + dateTime[2];
            //time = dateTime[3] + ':' + dateTime[4] + ':' + dateTime[5];

            

            int year = Convert.ToInt32(dateTime[2]);
            int month = Convert.ToInt32(dateTime[1]);
            int day = Convert.ToInt32(dateTime[0]);
            int hour = Convert.ToInt32(dateTime[3]);
            int minute = Convert.ToInt32(dateTime[4]);
            int secs = Convert.ToInt32(dateTime[5]);

            fileDateTime = new DateTime(year, month, day, hour, minute, secs);
            Date_Label.Content = fileDateTime.ToShortDateString();
            StartTimeLabel.Content = fileDateTime.ToLongTimeString();

            //elapsedTime = new DateTime(year, month, day, hour, minute, secs);
            elapsedTime = fileDateTime;
        }

        private AutoResetEvent waitHandle = new AutoResetEvent(false);

        private void StartReplay_Button_Click(object sender, RoutedEventArgs e)
        {

            runThread = new Thread(() => RunReplay(waitHandle));
            runThread.Start();

            Play_Button.Visibility = Visibility.Visible;
            Pause_Button.Visibility = Visibility.Visible;

            //elapsedTime = new DateTime();
            elapsedTime = fileDateTime;
            TimeElapsed_Label.Content = elapsedTime.Hour + elapsedTime.Second;

            PlayPause_Label.Content = "PLAYING";
        }



        private void RunReplay(WaitHandle wh)
        {
            if (replayRunning)
                return;

            replayRunning = true;
            string coords;
            string d1;
            string d2;

             

            for (int i = 0; i < loadedReplayFile.DeviceOnePositions.Length; i++)
            {
                if(i > 0)
                    elapsedTime = elapsedTime.AddSeconds(10);
                
                this.Dispatcher.Invoke(() => {
                    TimeElapsed_Label.Content = elapsedTime.ToLongTimeString();
                });

                d1 = loadedReplayFile.DeviceOnePositions[i];
                d2 = loadedReplayFile.DeviceTwoPositions[i];
                coords = d1 + d2;
                
                switch (coords)
                {
                    case "A0A0":
                        this.Dispatcher.Invoke(() => { A0R.Source = greenCircle.Source; });
                        break;
                    case "A1A1":
                        this.Dispatcher.Invoke(() => { A1R.Source = greenCircle.Source; });
                        break;
                    case "A2A2":
                        this.Dispatcher.Invoke(() => { A2R.Source = greenCircle.Source; });
                        break;
                    case "A3A3":
                        this.Dispatcher.Invoke(() => { A3R.Source = greenCircle.Source; });
                        break;
                    case "A4A4":
                        this.Dispatcher.Invoke(() => { A4R.Source = greenCircle.Source; });
                        break;
                    case "B0B0":
                        this.Dispatcher.Invoke(() => { B0R.Source = greenCircle.Source; });
                        break;
                    case "B1B1":
                        this.Dispatcher.Invoke(() => { B1R.Source = greenCircle.Source; });
                        break;
                    case "B2B2":
                        this.Dispatcher.Invoke(() => { B2R.Source = greenCircle.Source; });
                        break;
                    case "B3B3":
                        this.Dispatcher.Invoke(() => { B3R.Source = greenCircle.Source; });
                        break;
                    case "B4B4":
                        this.Dispatcher.Invoke(() => { B4R.Source = greenCircle.Source; });
                        break;
                    case "C0C0":
                        this.Dispatcher.Invoke(() => { C0R.Source = greenCircle.Source; });
                        break;
                    case "C1C1":
                        this.Dispatcher.Invoke(() => { C1R.Source = greenCircle.Source; });
                        break;
                    case "C2C2":
                        this.Dispatcher.Invoke(() => { C2R.Source = greenCircle.Source; });
                        break;
                    case "C3C3":
                        this.Dispatcher.Invoke(() => { C3R.Source = greenCircle.Source; });
                        break;
                    case "C4C4":
                        this.Dispatcher.Invoke(() => { C4R.Source = greenCircle.Source; });
                        break;
                    case "D0D0":
                        this.Dispatcher.Invoke(() => { D0R.Source = greenCircle.Source; });
                        break;
                    case "D1D1":
                        this.Dispatcher.Invoke(() => { D1R.Source = greenCircle.Source; });
                        break;
                    case "D2D2":
                        this.Dispatcher.Invoke(() => { D2R.Source = greenCircle.Source; });
                        break;
                    case "D3D3":
                        this.Dispatcher.Invoke(() => { D3R.Source = greenCircle.Source; });
                        break;
                    case "D4D4":
                        this.Dispatcher.Invoke(() => { D4R.Source = greenCircle.Source; });
                        break;
                    case "E0E0":
                        this.Dispatcher.Invoke(() => { E0R.Source = greenCircle.Source; });
                        break;
                    case "E1E1":
                        this.Dispatcher.Invoke(() => { E1R.Source = greenCircle.Source; });
                        break;
                    case "E2E2":
                        this.Dispatcher.Invoke(() => { E2R.Source = greenCircle.Source; });
                        break;
                    case "E3E3":
                        this.Dispatcher.Invoke(() => { E3R.Source = greenCircle.Source; });
                        break;
                    case "E4E4":
                        this.Dispatcher.Invoke(() => { E4R.Source = greenCircle.Source; });
                        break;
                    default:
                        switch (d1)
                        {
                            case "A0":
                                this.Dispatcher.Invoke(() => { A0R.Source = blueCircle.Source; });
                                break;
                            case "A1":
                                this.Dispatcher.Invoke(() => { A1R.Source = blueCircle.Source; });
                                break;
                            case "A2":
                                this.Dispatcher.Invoke(() => { A2R.Source = blueCircle.Source; });
                                break;
                            case "A3":
                                this.Dispatcher.Invoke(() => { A3R.Source = blueCircle.Source; });
                                break;
                            case "A4":
                                this.Dispatcher.Invoke(() => { A4R.Source = blueCircle.Source; });
                                break;
                            case "B0":
                                this.Dispatcher.Invoke(() => { B0R.Source = blueCircle.Source; });
                                break;
                            case "B1":
                                this.Dispatcher.Invoke(() => { B1R.Source = blueCircle.Source; });
                                break;
                            case "B2":
                                this.Dispatcher.Invoke(() => { B2R.Source = blueCircle.Source; });
                                break;
                            case "B3":
                                this.Dispatcher.Invoke(() => { B3R.Source = blueCircle.Source; });
                                break;
                            case "B4":
                                this.Dispatcher.Invoke(() => { B4R.Source = blueCircle.Source; });
                                break;
                            case "C0":
                                this.Dispatcher.Invoke(() => { C0R.Source = blueCircle.Source; });
                                break;
                            case "C1":
                                this.Dispatcher.Invoke(() => { C1R.Source = blueCircle.Source; });
                                break;
                            case "C2":
                                this.Dispatcher.Invoke(() => { C2R.Source = blueCircle.Source; });
                                break;
                            case "C3":
                                this.Dispatcher.Invoke(() => { C3R.Source = blueCircle.Source; });
                                break;
                            case "C4":
                                this.Dispatcher.Invoke(() => { C4R.Source = blueCircle.Source; });
                                break;
                            case "D0":
                                this.Dispatcher.Invoke(() => { D0R.Source = blueCircle.Source; });
                                break;
                            case "D1":
                                this.Dispatcher.Invoke(() => { D1R.Source = blueCircle.Source; });
                                break;
                            case "D2":
                                this.Dispatcher.Invoke(() => { D2R.Source = blueCircle.Source; });
                                break;
                            case "D3":
                                this.Dispatcher.Invoke(() => { D3R.Source = blueCircle.Source; });
                                break;
                            case "D4":
                                this.Dispatcher.Invoke(() => { D4R.Source = blueCircle.Source; });
                                break;
                            case "E0":
                                this.Dispatcher.Invoke(() => { E0R.Source = blueCircle.Source; });
                                break;
                            case "E1":
                                this.Dispatcher.Invoke(() => { E1R.Source = blueCircle.Source; });
                                break;
                            case "E2":
                                this.Dispatcher.Invoke(() => { E2R.Source = blueCircle.Source; });
                                break;
                            case "E3":
                                this.Dispatcher.Invoke(() => { E3R.Source = blueCircle.Source; });
                                break;
                            case "E4":
                                this.Dispatcher.Invoke(() => { E4R.Source = blueCircle.Source; });
                                break;
                        }

                        switch (d2)
                        {
                            case "A0":
                                this.Dispatcher.Invoke(() => { A0R.Source = yellowCircle.Source; });
                                break;
                            case "A1":
                                this.Dispatcher.Invoke(() => { A1R.Source = yellowCircle.Source; });
                                break;
                            case "A2":
                                this.Dispatcher.Invoke(() => { A2R.Source = yellowCircle.Source; });
                                break;
                            case "A3":
                                this.Dispatcher.Invoke(() => { A3R.Source = yellowCircle.Source; });
                                break;
                            case "A4":
                                this.Dispatcher.Invoke(() => { A4R.Source = yellowCircle.Source; });
                                break;
                            case "B0":
                                this.Dispatcher.Invoke(() => { B0R.Source = yellowCircle.Source; });
                                break;
                            case "B1":
                                this.Dispatcher.Invoke(() => { B1R.Source = yellowCircle.Source; });
                                break;
                            case "B2":
                                this.Dispatcher.Invoke(() => { B2R.Source = yellowCircle.Source; });
                                break;
                            case "B3":
                                this.Dispatcher.Invoke(() => { B3R.Source = yellowCircle.Source; });
                                break;
                            case "B4":
                                this.Dispatcher.Invoke(() => { B4R.Source = yellowCircle.Source; });
                                break;
                            case "C0":
                                this.Dispatcher.Invoke(() => { C0R.Source = yellowCircle.Source; });
                                break;
                            case "C1":
                                this.Dispatcher.Invoke(() => { C1R.Source = yellowCircle.Source; });
                                break;
                            case "C2":
                                this.Dispatcher.Invoke(() => { C2R.Source = yellowCircle.Source; });
                                break;
                            case "C3":
                                this.Dispatcher.Invoke(() => { C3R.Source = yellowCircle.Source; });
                                break;
                            case "C4":
                                this.Dispatcher.Invoke(() => { C4R.Source = yellowCircle.Source; });
                                break;
                            case "D0":
                                this.Dispatcher.Invoke(() => { D0R.Source = yellowCircle.Source; });
                                break;
                            case "D1":
                                this.Dispatcher.Invoke(() => { D1R.Source = yellowCircle.Source; });
                                break;
                            case "D2":
                                this.Dispatcher.Invoke(() => { D2R.Source = yellowCircle.Source; });
                                break;
                            case "D3":
                                this.Dispatcher.Invoke(() => { D3R.Source = yellowCircle.Source; });
                                break;
                            case "D4":
                                this.Dispatcher.Invoke(() => { D4R.Source = yellowCircle.Source; });
                                break;
                            case "E0":
                                this.Dispatcher.Invoke(() => { E0R.Source = yellowCircle.Source; });
                                break;
                            case "E1":
                                this.Dispatcher.Invoke(() => { E1R.Source = yellowCircle.Source; });
                                break;
                            case "E2":
                                this.Dispatcher.Invoke(() => { E2R.Source = yellowCircle.Source; });
                                break;
                            case "E3":
                                this.Dispatcher.Invoke(() => { E3R.Source = yellowCircle.Source; });
                                break;
                            case "E4":
                                this.Dispatcher.Invoke(() => { E4R.Source = yellowCircle.Source; });
                                break;
                        }
                        break;
                
                }

                //10 secs interval
                System.Threading.Thread.Sleep(10000);
                //busy while loop for when paused
                while (pauseReplay) { }

                BlankReplayGrid();

                
                if (!replayRunning)
                {
                    BlankReplayGrid();
                    break;
                }

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
            replayRunning = false;

            Play_Button.Visibility = Visibility.Hidden;
            Pause_Button.Visibility = Visibility.Hidden;

            TimeElapsed_Label.Content = "";
            PlayPause_Label.Content = "";
        }

        private void Pause_Button_Click(object sender, RoutedEventArgs e)
        {
            pauseReplay = true;
            PlayPause_Label.Content = "PAUSED";
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            pauseReplay = false;
            PlayPause_Label.Content = "PLAYING";
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