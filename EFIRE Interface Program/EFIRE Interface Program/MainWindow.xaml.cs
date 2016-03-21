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

namespace EFIRE_Interface_Program
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class DataServer : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            // var msg = "{strike: 1, time: 0:00, diode: 1, energy: 500, bin: 0}";
            eStrike nextStrike = new eStrike();
            string msg = "{\"strike\":" + eStrike.strikeNumber.ToString() + ",\"time\":\"" + nextStrike.strikeTime + "\",\"diode\":" +
                nextStrike.diodeStruck + ",\"energy\":" + nextStrike.electronEnergy + ",\"bin\":" + nextStrike.bin + "}";
            Send(msg);
            //Send(e.Data);
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
