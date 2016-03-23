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
            //setTimer();
            while(true)
            {
                eStrike nextStrike = new eStrike();
                string msg = nextStrike.createStrikeJSON();
                Send(msg);
                Thread.Sleep(10);
            }
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenWebSocket_Click(object sender, RoutedEventArgs e)
        {
            //var wssv = new WebSocketServer("ws://localhost:8080");
            var wssv = new WebSocketServer(8080);
            wssv.AddWebSocketService<DataServer>("/DataServer");
            wssv.Start();
            //{strike: n, time:, diode:, energy:, bin:}
        }
    }
}
