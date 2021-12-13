using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace SimpleMailClient
{
    public partial class SendPage : Form
    {
        public string PassId { get; set; }
        Socket mainSock;
        IPAddress thisAddress;


        public SendPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thisAddress = IPAddress.Parse("127.0.0.1");
            int port = 9000;
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSock.Connect(thisAddress, port);


            var mail = new JObject();
            mail.Add("req", "mailSend");
            mail.Add("fromId", txtId.Text);
            mail.Add("title", txtTitle.Text);
            mail.Add("body", txtBody.Text);
            mail.Add("toId", txtId2.Text);

            byte[] mailData = Encoding.UTF8.GetBytes(mail.ToString());

            Program.sendMsg = Program.sendMsg + "from: " + txtId.Text + "to: " + txtId2.Text + "title: " + txtTitle.Text;

            AsyncObject obj = new AsyncObject(4096);
            obj.workingSocket = mainSock;
            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);

            mainSock.Send(mailData);


            this.Visible = false;
            MyPage myPage = new MyPage();
            myPage.ShowDialog();

        }

        private void DataReceived(IAsyncResult ar)
        {
            AsyncObject obj = (AsyncObject)ar.AsyncState;
            try
            {
                int received = obj.workingSocket.EndReceive(ar);

                if (received <= 0)
                {
                    obj.workingSocket.Close();
                    Application.Exit();
                    return;
                }

                string data = Encoding.UTF8.GetString(obj.Buffer);

                obj.clearBuffer();

                obj.workingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
            }
            catch
            {
            }
        }

        private void SendPage_Load(object sender, EventArgs e)
        {
            txtId.Text = Program.ID;
        }
    }
}
