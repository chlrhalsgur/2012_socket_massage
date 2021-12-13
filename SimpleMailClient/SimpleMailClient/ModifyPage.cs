using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleMailClient
{
    public partial class ModifyPage : Form
    {
        public ModifyPage()
        {
            InitializeComponent();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            byte[] bDts = new byte[4096];
            bDts = Encoding.UTF8.GetBytes("111:" + txtId.Text + ":" + txtId2.Text + ":" + txtTitle.TextLength + ":" + txtTitle + ":" + txtBody + ":");

            this.Visible = false;
            MyPage myPage = new MyPage();
            myPage.ShowDialog();
        }
    }
}
