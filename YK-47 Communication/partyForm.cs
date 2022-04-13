using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleTcp;

namespace YK_47_Communication
{
    public partial class partyForm : Form
    {
        int _Port;
        string _Password, _PartyName = "Bekleniyor", _UserName = "";
        IPAddress _address;
        SimpleTcpClient client;
        public partyForm(IPAddress Address, int Port, string Password)
        {
            InitializeComponent();
            _address = Address;
            _Port = Port;
            _Password = Password;
        }
        private void PartyForm_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.Text = _PartyName + "'s room";
            client = new SimpleTcpClient(_address.ToString()+ ":" + _Port.ToString());
            client.Events.Connected += Events_Connected;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.DataReceived += Events_DataReceived;
            client.Connect();
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            string data = Encoding.UTF8.GetString(e.Data);
            if (data[0] == '/')
            {
                data = data.Substring(1, data.Length - 1);
                string comm = data.Split(' ')[0];
                if (comm == "Kicked")
                {
                    MessageBox.Show("Atıldın ! ;(", "Üzgünüm !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
                else if (comm == "Banned")
                {
                    MessageBox.Show("Banlandın ! ;(", "Üzgünüm !", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.Close();
                }
                else if (comm == "Warn")
                {
                    MessageBox.Show("Uyarıldın ! ;(", "Dikkat !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (comm == "Accept")
                {
                    MessageBox.Show("Kabul Edildin ! *_*", "Wuhow", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Log("İstek kabul edildi $_$", Color.Lime);
                    Log("Belki /Help komutu açıktır ! Bir dene istersen.", Color.Lime);
                    SendMessage("/RoomName?");
                }
                else if (comm.StartsWith("RoomName:"))
                {
                    this.Text = comm.Split(':')[1].ToString() + "'s Room";
                }
                else if (comm.StartsWith("UserName?"))
                {
                    InputDialog dialog = new InputDialog();
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        SendMessage("/UserName:" + dialog.textBox1.Text);
                        _UserName = dialog.textBox1.Text;
                    }
                }
                else if (comm == ("Clear"))
                {
                    richTextBox1.Text = "";
                }
            }
            else
            {
                Log(data, Color.Black);
            }
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            Log(e.IpPort + " adresinden bağlantı kesildi.",Color.Black);
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            Log(e.IpPort + " adresine bağlanıldı istek gönderildi.",Color.Black);
        }

        private void SendMessage(string Message)
        {
            client.Send(Message);
        }

        private void partyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show(client.Statistics.ToString());
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if (textBox1.Text[0] == '/')
                {
                    SendMessage(textBox1.Text);
                }
                else if (textBox1.Text == "/Clear") richTextBox1.Text = "";
                else
                {
                    SendMessage(textBox1.Text);
                }
                textBox1.Text = "";
            }
        }

        private void Log(string Message, Color color)
        {
            richTextBox1.SelectionColor = color;
            richTextBox1.SelectedText = string.Format("{0,7}: {1}", DateTime.Now.ToLongTimeString(), Message) + Environment.NewLine;
        }
    }
}
