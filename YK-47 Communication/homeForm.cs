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

namespace YK_47_Communication
{
    public partial class homeForm : Form
    {
        public homeForm()
        {
            InitializeComponent();
        }
        private void Log(string Message)
        {
            richTextBox1.Text += string.Format("{0,7} : {1}\n", DateTime.Now.ToLongTimeString(), Message);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Log("YK-47 Communication Proje başladı.");
            Log("/CreateParty [Parti ismi] [Port] [Şifre] yazarak parti oluşturabilirsiniz.");
            Log("/ConnectParty [Ip] [Port] [Şifre] yazarak var ise partiye bağlanma isteği gönderebilirsiniz.\nKabul edilir iseniz parti için bir pencere açılacaktır.");
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if (textBox1.Text[0] == '/')
                    Command(textBox1.Text.Substring(1, textBox1.Text.Length - 1));
                else
                    Log(string.Format("Komutların başında / olur. /{0} gibi", textBox1.Text));
                textBox1.Text = "";
            }
        }
        private void Command(string Command)
        {
            string command = Command.Split(' ')[0];
            if (command == "CreateParty")
            {
                int port = Convert.ToInt32(Command.Split(' ')[2]);
                string partyName = Command.Split(' ')[1], partyPassword = Command.Split(' ')[3];
                CreateParty(port, partyName, partyPassword);
            }
            else if (command == "ConnectParty")
            {
                IPAddress ip = IPAddress.Parse(Command.Split(' ')[1]);
                int port = Convert.ToInt32(Command.Split(' ')[2]); string pass = Command.Split(' ')[3];
                ConnectParty(ip, port, pass);
            }
            else if (command == "Clear")
                richTextBox1.Text = "";
            else if (Command == "Exit")
                Application.Exit();
            else
            {
                Log(string.Format("\"{0}\" bilinmeyen bir komut", command));
            }
        }
        private void CreateParty(int Port,string partyName,string Password) {
            Log(string.Format("\"{0}\" portuna \"{1}\" şifresi ile \"{2}\" isimli parti oluşturuluyor.",Port,Password,partyName));
            thisPartyForm frm = new thisPartyForm(Port,partyName,Password);
            frm.Show();
        }

        private void ConnectParty(IPAddress address, int Port, string Password) {
            Log(string.Format("\"{0}\" addresinin \"{1}\" portundan yayınlanan odaya  \"{2}\" şifresi ile katılma isteği gönderildi.", address.ToString(),Port.ToString(),Password));
            partyForm frm = new partyForm(address, Port, Password);
            frm.Show();
        }
    }
}
