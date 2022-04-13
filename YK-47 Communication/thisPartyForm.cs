using SimpleTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YK_47_Communication
{
    public partial class thisPartyForm : Form
    {

        int _Port = 1;
        string _Password = "1921682323211", _PartyName = "Bekleniyor";
        SimpleTcpServer server;
        Dictionary<IPEndPoint, string> clients = new Dictionary<IPEndPoint, string>();
        List<IPAddress> darkList = new List<IPAddress>();
        public thisPartyForm(int Port, string PartyName, string Password)
        {
            InitializeComponent();
            _PartyName = PartyName;
            _Port = Port;
            _Password = Password;
        }

        private void thisPartyForm_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.Text = string.Format("Sizin Parti Odanız {0}", _PartyName);
            Log(string.Format("{0} {1} {2} Ayarları ile parti oluşturuldu.", _PartyName, _Port, _Password), Color.Lime);
            Log(string.Format("{0} komutu ile partiyi başlatın *_* yada {1}", "/Start", "/Help"), Color.Lime);
            server = new SimpleTcpServer(IPAddress.Any.ToString() + ":" + _Port);
            server.Events.ClientConnected += ClientConnected;
            server.Events.ClientDisconnected += ClientDisconnected;
            server.Events.DataReceived += DataReceived;
        }
        void ClientConnected(object sender, ConnectionEventArgs e)
        {
            DialogResult result = MessageBox.Show(e.IpPort + " adresi bağlantı isteğinde bulunuyor.\nBağlanmasına izin verecekmisiniz?", "Dikkat !", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            IPEndPoint newClient = new IPEndPoint(IPAddress.Parse(e.IpPort.ToString().Split(':')[0]), Convert.ToInt32(e.IpPort.ToString().Split(':')[1]));
            if (result == DialogResult.Yes)
            {
                SendMessage("/Accept", newClient);
                SendMessage("/UserName?", newClient);
            }
            else
            {
                SendMessage("/Kicked", newClient);
            }
        }

        void ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Log(e.IpPort + " bağlantıyı kapattı. :" + e.Reason.ToString(), Color.Black);
        }

        void DataReceived(object sender, DataReceivedEventArgs e)
        {
            string data = Encoding.UTF8.GetString(e.Data);
            IPEndPoint newClient = new IPEndPoint(IPAddress.Parse(e.IpPort.ToString().Split(':')[0]), Convert.ToInt32(e.IpPort.ToString().Split(':')[1]));
            if (data[0] == '/')
            {
                CommandRun(data.Substring(1, data.Length - 1), newClient);
            }
            else
            {
                string userName = "";
                foreach (var item in clients)
                    if (item.Key.ToString() == e.IpPort)
                        userName = item.Value;
                SendMessage(userName + " : " + data, null, false);
                Log(userName + " : " + data, Color.Black);
            }
        }
        private void SendMessage(string Message, IPEndPoint IpAndPort = null, bool my = true)
        {
            if (IpAndPort != null)
                server.Send(IpAndPort.Address.ToString() + ":" + IpAndPort.Port.ToString(), Message);
            else
                foreach (var item in clients)
                    if (my == true)
                        server.Send(item.Key.ToString(), "Admin :" + Message);
                    else
                        server.Send(item.Key.ToString(), Message);
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                string Command = textBox1.Text;
                textBox1.Text = "";
                if (Command[0] == '/')
                {
                    CommandRun(Command.Substring(1, Command.Length - 1));
                }
                else
                {
                    SendMessage(Command);
                    Log("Admin(You) : " + Command, Color.Green);
                }
            }
        }

        private void CommandRun(string Command, IPEndPoint who = null)
        {
            string Comm = Command.Split(' ')[0];
            if (Comm == "Start")
            {
                Log("Parti başladı !", Color.Lime);
                PartyStart();
            }
            else if (Comm == "RoomName?")
            {
                SendMessage("/RoomName:" + _PartyName, who);
            }
            else if (Comm.StartsWith("UserName:"))
            {
                clients.Add(who, Comm.Split(':')[1]);
                SendMessage("Kullanıcı adı " + Comm.Split(':')[1] + " olan kullanıcı odaya katıldı.");
            }
            else if (Comm == "Help")
            {
                SendMessage("*/Users : Kullanıcıları listeler.", who);
                SendMessage("*/Report [Kullanıcı Adı]: Kullanıcıyı şikayet eder.", who);
                SendMessage("*/Shake : Admini titretir.", who);

            }
            else if (Comm == "Shale") { }
            else if (Comm == "Report") { }
            else if (Comm == "Users")
            {
                string userNames = "";
                foreach (var item in clients)
                    userNames += item.Value + ",";
                SendMessage(userNames, who);
            }
            else if (Comm == "Clear") {
                SendMessage("/Clear",null,false);
                richTextBox1.Text = "";
            }
        }

        private void Log(string Message, Color color)
        {
            richTextBox1.SelectionColor = color;
            richTextBox1.SelectedText = string.Format("{0,7}: {1}", DateTime.Now.ToLongTimeString(), Message) + Environment.NewLine;
        }
        private void PartyStart()
        {
            server.Start();
        }
    }
}
