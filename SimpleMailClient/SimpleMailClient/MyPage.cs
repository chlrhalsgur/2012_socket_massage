using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace SimpleMailClient
{
    public partial class MyPage : Form
    {
        public string PassId { get; set; }
        Socket mainSock;
        IPAddress thisAddress;

        public MyPage()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            SendPage sendPage = new SendPage();
            sendPage.ShowDialog();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            ModifyPage modifyPage = new ModifyPage();
            modifyPage.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            DeletePage deletePage = new DeletePage();
            deletePage.ShowDialog();

        }

        private void MyPage_Load(object sender, EventArgs e)
        {
            txtRec.Text = Program.sendMsg;
            txtSend.Text = Program.sendMsg;
        }

        private void MyPage_Load_1(object sender, EventArgs e)
        {
            refreshSend();
            refreshReceive();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            refreshSend();
            refreshReceive();
        }
        
        public void refreshReceive()
        {
            thisAddress = IPAddress.Parse("127.0.0.1");
            int port = 9000;
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSock.Connect(thisAddress, port);


            var mail = new JObject();
            mail.Add("req", "loadReceive");
            mail.Add("id", Program.ID);

            byte[] mailData = Encoding.UTF8.GetBytes(mail.ToString());

            AsyncObject obj = new AsyncObject(4096);
            obj.workingSocket = mainSock;
            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);

            mainSock.Send(mailData);
            Program.receiveMsg = "";
            txtRec.Text = Program.receiveMsg;
            txtSend.Text = Program.sendMsg;
        }
        public void refreshSend()
        {
            thisAddress = IPAddress.Parse("127.0.0.1");
            int port = 9000;
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSock.Connect(thisAddress, port);


            var mail = new JObject();
            mail.Add("req", "loadSend");
            mail.Add("id", Program.ID);

            byte[] mailData = Encoding.UTF8.GetBytes(mail.ToString());

            AsyncObject obj = new AsyncObject(4096);
            obj.workingSocket = mainSock;
            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived2, obj);

            mainSock.Send(mailData);
            Program.sendMsg = "";
            txtSend.Text = Program.sendMsg;
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
                Program.receiveMsg = "";
                Program.receiveMsg = Program.receiveMsg + data;
                txtRec.Text = Program.receiveMsg;
                txtSend.Text = Program.sendMsg;

                obj.clearBuffer();

                obj.workingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
            }
            catch
            {
            }
        }
        private void DataReceived2(IAsyncResult ar)
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
                Program.sendMsg = Program.sendMsg + data;
                txtSend.Text = Program.sendMsg;

                obj.clearBuffer();

                obj.workingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
            }
            catch
            {
            }
        }

    }
}
