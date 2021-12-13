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
    public partial class SignUp : Form
    {
        Socket socket;
        public SignUp()
        {
            InitializeComponent();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            SignIn signIn = new SignIn();
            signIn.Show();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            btnSignUp.Enabled = false;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var Register = new JObject();

            Register.Add("req", "register");
            Register.Add("id", txtId.Text);
            Register.Add("pw", txtPwd.Text);

            byte[] registerData = Encoding.UTF8.GetBytes(Register.ToString());

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

            socket.Send(registerData);



            this.Visible = false;
            SignIn signIn = new SignIn();
            signIn.Show();
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
                MsgBoxHelper.Show("회원가입 완료!", MessageBoxButtons.OK);

                SetVisibility(this, false);
                SignIn signIn = new SignIn();
                signIn.Show();
            }
            else
            {
                MsgBoxHelper.Show(readJson["result"].ToString(), MessageBoxButtons.OK);
            }
            SetEnabled(btnSignUp, true);
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
