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
using Newtonsoft.Json.Linq;

namespace SimpleMailClient
{
    public partial class SignIn : Form
    {
        Socket socket;

        public SignIn()
        {
            InitializeComponent();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            SignUp signUp = new SignUp();
            signUp.Show();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            btnSignIn.Enabled = false;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var Login = new JObject();

            Program.ID = txtId.Text;

            Login.Add("req", "login");
            Login.Add("id", txtId.Text);
            Login.Add("pw", txtPwd.Text);

            byte[] loginData = Encoding.UTF8.GetBytes(Login.ToString());

            try
            {
                socket.Connect(IPAddress.Parse("127.0.0.1"), 9000);
            }
            catch (Exception ex)
            {
                MsgBoxHelper.Error("연결에 실패했습니다!\n오류 내용:{0}", MessageBoxButtons.OK, ex.Message);
            }

            if (!socket.IsBound)
            {
                MsgBoxHelper.Warn("서버가 실행되고 있지 않습니다!");
                return;
            }


            AsyncObject obj = new AsyncObject(4096);
            obj.workingSocket = socket;
            socket.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);

            socket.Send(loginData);

            //this.Visible = false;
            //MyPage myPage = new MyPage();
            //myPage.Show();
        }
        void DataReceived(IAsyncResult ar)
        {
            AsyncObject obj = (AsyncObject)ar.AsyncState;
            int received = obj.workingSocket.EndReceive(ar);

            if (received <= 0)
            {
                obj.workingSocket.Close();
                return;
            }

            string data = Encoding.UTF8.GetString(obj.Buffer);

            var readJson = JObject.Parse(data);

            if (readJson["res"].ToString() == "true")
            {
                SetVisibility(this, false);
                MyPage myPage = new MyPage();
                myPage.ShowDialog();
            }
            else
            {
                MsgBoxHelper.Show(readJson["result"].ToString(), MessageBoxButtons.OK);
            }

            SetEnabled(btnSignIn, true);
        }
        public void SetVisibility(Control target, bool visible)
        {
            if (target.InvokeRequired)
            {
                target.Invoke(new EventHandler(
                    delegate
                    {
                        target.Visible = visible;
                    }));
            }
            else
            {
                target.Visible = visible;
            }
        }
        public void SetEnabled(Control target, bool enabled)
        {
            if (target.InvokeRequired)
            {
                target.Invoke(new EventHandler(
                    delegate
                    {
                        target.Enabled = enabled;
                    }));
            }
            else
            {
                target.Enabled = enabled;
            }
        }

    }
}
