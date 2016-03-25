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
using WebSocketSharp;
using WebSocketSharp.Server;
using Team_E_FIRE_Data_Processing_Program_BBB;
using System.Timers;
using System.Threading;

namespace EFIRE_Interface_Program
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class DataServer : WebSocketBehavior
    {
        //Nate TODO timer is better than sleep
        //private static Timer strikeTimer;

        //private void setTimer() //static?
        //{
        //    strikeTimer = new Timer(100);
        //    strikeTimer.Elapsed += onTimedEvent;
        //    strikeTimer.AutoReset = true;
        //    strikeTimer.Enabled = true;
        //}

        //private void onTimedEvent(Object source, ElapsedEventArgs e) //static?
        //{
        //    eStrike nextStrike = new eStrike();
        //    string msg = nextStrike.createStrikeJSON();
        //    Send(msg); //problem when the above are static
        //}

        protected override void OnMessage(MessageEventArgs e)
        {
            SPI_Reader BBB_reader = new SPI_Reader();
            string testFileDestination = MainWindow.fileDestination;
            CSV_Writer CSV_writer = new CSV_Writer(testFileDestination);
            
            //setTimer();
            while (MainWindow.runTest == true)
            {
                eStrike nextStrike = BBB_reader.readBbbDatapoint();
                string msg = nextStrike.createStrikeJSON();
                Send(msg);
                //TODO need to calculate energy somewhere
                CSV_writer.WriteStrikeToCSV(eStrike.strikeNumber, nextStrike.diodeStruck, 0, nextStrike.strikeTime);
                if (eStrike.strikeNumber > 1000)
                {
                    MainWindow.runTest = false;
                    CSV_writer.close();
                }
                Thread.Sleep(10);
                //MainWindow.lblJSON.Content = msg; //TODO GRRRR
            }
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static public string fileDestination;
        static public bool runTest;

        private void btnOpenWebSocket_Click(object sender, RoutedEventArgs e)
        {
            txtCsvTargetFile.Text = @"C:\testTargetButton.csv";
            fileDestination = txtCsvTargetFile.Text;
            runTest = true;
            //var wssv = new WebSocketServer("ws://localhost:8080");
            var wssv = new WebSocketServer("ws://172.16.181.150:80");
            wssv.AddWebSocketService<DataServer>("/DataServer");
            wssv.Start();
            btnOpenWebSocket.IsEnabled = false;
            lblWsOpen.Content = "WebSocket is Opened";
            
            //{strike: n, time:, diode:, energy:, bin:}
        }
    }
}
