using MySqlX.XDevAPI;
using RegisterForm;
using System.Net.Sockets;
using System.Net;
using Validation;
using System.Text.Json;
using Message;
using Microsoft.Win32;
using System.Text;
using ClientForm;
namespace LoginForm
{
    public partial class LoginForm : Form
    {
        IPEndPoint iep;
        Socket client;
        Thread trd;
        string ipaddress;
        string gmail;
        ValidateEmail validEmail = new ValidateEmail();

        public LoginForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txt_Password.UseSystemPasswordChar = true;
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

        private void cb_showPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_showPassword.Checked)
            {
                txt_Password.UseSystemPasswordChar = false;
            }
            else
            {
                txt_Password.UseSystemPasswordChar = true;
            }
        }

        private void Link_Register_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm.RegisterForm form = new RegisterForm.RegisterForm();
            form.Show();
        }

        public void connectServer()
        {
            iep = new IPEndPoint(IPAddress.Parse(ipaddress), 2008);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(iep);
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            connectServer();
            if (String.IsNullOrEmpty(txt_Email.Text) || String.IsNullOrEmpty(txt_Password.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin để đăng nhập");
            }
            else
            {
                if (validEmail.checkEmail(txt_Email.Text) == false)
                {
                    MessageBox.Show("Email không hợp lệ. Vui lòng nhập lại");
                }
                else
                {
                    trd = new Thread(new ThreadStart(this.ThreadLogin));
                    trd.IsBackground = true;
                    trd.Start();
                }   
            }
        }

        private void sendJson(object obj)
        {
            byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(obj);
            client.Send(jsonUtf8Bytes, jsonUtf8Bytes.Length, SocketFlags.None);
        }

        private void ThreadLogin()
        {
            byte[] data = new byte[1024];
            gmail = txt_Email.Text;
            Message.Login login = new Message.Login(txt_Email.Text, txt_Password.Text);
            string jsonString = JsonSerializer.Serialize(login);
            Message.Common common = new Message.Common("Login", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("Ok"))
                {
                    MessageBox.Show("Đăng nhập thành công");
                    //txt_Password.Text = "";
                    ClientForm.ClientForm form = new ClientForm.ClientForm(); 
                    ClientForm.ClientForm.gmailname = txt_Email.Text;
                    ClientForm.ClientForm.ID_Accout = comm.content;
                    form.ShowDialog();
                }
                else if (comm != null && comm.kind.Equals("Cancel"))
                {
                    MessageBox.Show(comm.content);
                    txt_Password.Text = null;
                }
                client.Disconnect(true);
                client.Close();
            }
            catch (Exception)
            {
            }
        }
    }
}