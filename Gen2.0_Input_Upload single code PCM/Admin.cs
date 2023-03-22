using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    public partial class Admin : Form
    {
        Form1 _frm;
        public Admin(Form1 frm)
        {
            InitializeComponent();
            _frm = frm;
        }

        private void Admin_Load(object sender, EventArgs e)
        {

        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            if (txt_id.Text == "" || txt_mk.Text == "")
            {
                MessageBox.Show("Tên đăng nhập/Mật khẩu trống!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string user = string.Empty, password = string.Empty;
                StreamReader srLgin = new StreamReader(@Application.StartupPath + "\\Account.txt");
                while (srLgin.EndOfStream == false)
                {
                    string str = srLgin.ReadLine();
                    string[] arr = str.Split('=');
                    switch (arr[0])
                    {
                        case "user":
                            user = arr[1];
                            break;
                        case "pass":
                            password = arr[1];
                            break;
                        default:
                            break;
                    }
                }
                srLgin.Close();

                if (user == txt_id.Text && password == txt_mk.Text)
                {
                    _frm.chb_buStk.Checked = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập/Mật khẩu sai!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_login_MouseHover(object sender, EventArgs e)
        {
            btn_login.BackColor = Color.YellowGreen;
        }

        private void btn_login_MouseLeave(object sender, EventArgs e)
        {
            btn_login.BackColor = Color.FromArgb(255, 192, 128);
        }
    }
}
