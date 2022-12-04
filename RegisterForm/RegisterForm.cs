using System.Net.Sockets;
using System.Net;
using Validation;
using Message;
using Microsoft.VisualBasic.Logging;
using System.Text.Json;
using System.Windows.Forms;
using System.Text;

namespace RegisterForm
{
    public partial class RegisterForm : Form
    {
        IPEndPoint iep;
        Socket client;
        Thread trd;
        string ipaddress;
        ValidateEmail validEmail = new ValidateEmail();
        ValidatePhone validPhone = new ValidatePhone();

        public RegisterForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txt_Password.UseSystemPasswordChar = true;
            txt_confirmPassword.UseSystemPasswordChar = true;
            string hostName = Dns.GetHostName();
            foreach (IPAddress ip in Dns.GetHostByName(hostName).AddressList)
            {
                if (ip.ToString().Contains("."))
                {
                    ipaddress = ip.ToString();
                    break;
                }
            }
        }

        public void connectServer()
        {
            iep = new IPEndPoint(IPAddress.Parse(ipaddress), 2008);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(iep);
        }

        private void cb_showPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_showPassword.Checked)
            {
                txt_Password.UseSystemPasswordChar = false;
                txt_confirmPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txt_Password.UseSystemPasswordChar = true;
                txt_confirmPassword.UseSystemPasswordChar = true;
            }
        }

        private void btn_Register_Click(object sender, EventArgs e)
        {
            connectServer();
            if (String.IsNullOrEmpty(txt_Email.Text) || String.IsNullOrEmpty(txt_Username.Text) || String.IsNullOrEmpty(txt_Phone.Text) || String.IsNullOrEmpty(txt_Password.Text) || String.IsNullOrEmpty(txt_confirmPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin để đăng ký tài khoản");
            }
            else
            {
                if (validEmail.checkEmail(txt_Email.Text) == false)
                {
                    MessageBox.Show("Email không hợp lệ. Vui lòng nhập lại");
                }
                if (validPhone.checkPhone(txt_Phone.Text) == false)
                {
                    MessageBox.Show("Số điện thoại không hợp lệ. Vui lòng nhập lại");
                }
                else
                {
                    if (txt_Password.Text.Equals(txt_confirmPassword.Text))
                    {
                        trd = new Thread(new ThreadStart(this.ThreadRegister));
                        trd.IsBackground = true;
                        trd.Start();
                    }
                    else
                    {
                        MessageBox.Show("Xác nhận lại mật khẩu không khớp. Vui lòng kiểm tra lại");
                    }

                }
            }
        }

        private void sendJson(object obj)
        {
            byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(obj);
            client.Send(jsonUtf8Bytes, jsonUtf8Bytes.Length, SocketFlags.None);
        }

        private void ThreadRegister()
        {
            byte[] data = new byte[1024];
            Message.Register register = new Message.Register(txt_Email.Text, txt_Password.Text, txt_Username.Text, txt_Phone.Text);
            string jsonString = JsonSerializer.Serialize(register);
            Message.Common common = new Message.Common("Register", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if(comm != null && comm.kind.Equals("Ok"))
                {
                    MessageBox.Show(comm.content);
                }
                else if (comm != null && comm.kind.Equals("Cancel"))
                {
                    MessageBox.Show(comm.content);
                }
                client.Disconnect(true);
                client.Close();
            }
            catch(Exception)
            {
            }
        }
    }
}