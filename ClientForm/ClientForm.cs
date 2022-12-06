using System.Net.Sockets;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Forms;
using System;
using Message;
using System.Text.Json;
using System.Text;
using System.Data;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ConnectDB;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualBasic.Logging;
using Entity;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Crypto.Macs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using Org.BouncyCastle.Tls.Crypto;
using System.Security.Principal;
using Org.BouncyCastle.Tls;
using System.Diagnostics.Metrics;
using Microsoft.VisualBasic.ApplicationServices;
using System.ComponentModel;
using File = Entity.File;

namespace ClientForm
{
    public partial class ClientForm : Form
    {
        Thread trd;
        public static string gmailname;
        public static string ID_Accout;
        int userforcus;
        Socket client;
        IPEndPoint iep;
        List<string> dstinnhan = new List<string>();
        List<string> dssearch = new List<string>();
        List<string> dsnhom = new List<string>();
        List<string> DSnhomQuanLy = new List<string>();
        List<string> DSTVnhomQuanLy = new List<string>();
        List<File> fileNames = new List<File>();
        List<FriendChat> listFriend;
        List<string> listRequestFriend;
        string ipaddress;
        int nhomchon = -1;
        public ClientForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            txt_gmail.Text = gmailname;
            string hostName = Dns.GetHostName();
            foreach (IPAddress ip in Dns.GetHostByName(hostName).AddressList)
            {
                if (ip.ToString().Contains("."))
                {
                    ipaddress = ip.ToString();
                    break;
                }
            }
            load();
            dataGridView6.RowHeadersVisible = false;
            dataGridView5.RowHeadersVisible = false;
            dataGridView7.RowHeadersVisible = false;
            trd = new Thread(new ThreadStart(this.ThreadTask));
            trd.IsBackground = true;
            trd.Start();
        }
        private Account getAcc(string id_user)
        {
            Account a = new Account();
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(id_user, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("getAccount", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("getAccount"))
                {
                    Message.RequestAcc? TEMP = JsonSerializer.Deserialize<Message.RequestAcc>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private List<string> getDStinnhan(string id_user)
        {
            List<string> a = new List<string>();
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(id_user, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("dsTinnhan", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("dsTinnhan"))
                {
                    Message.Request? TEMP = JsonSerializer.Deserialize<Message.Request>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private List<string> LayDsNhom(string id_user)
        {
            List<string> a = new List<string>();
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(id_user, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("LayDsNhom", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("LayDsNhom"))
                {
                    Message.Request? TEMP = JsonSerializer.Deserialize<Message.Request>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private List<MESSAGE> LayTinNhan(string id_user, string id2)
        {
            List<MESSAGE> a = new List<MESSAGE>();
            connectServer();
            byte[] data = new byte[1024];
            Message.TINNHAN mes = new Message.TINNHAN(id_user + "|" + id2, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("LayTinNhan", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("LayTinNhan"))
                {
                    Message.TINNHAN? TEMP = JsonSerializer.Deserialize<Message.TINNHAN>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private List<string> LayTinNhanNhom(string idgroup)
        {
            List<string> a = new List<string>();
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(idgroup, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("LayTinNhanNhom", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("LayTinNhanNhom"))
                {
                    Message.Request? TEMP = JsonSerializer.Deserialize<Message.Request>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private MESSAGE GetMessage(string idgroup)
        {
            MESSAGE a = new MESSAGE();
            connectServer();
            byte[] data = new byte[1024];
            Message.GETMESSAGE mes = new Message.GETMESSAGE(idgroup, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("GetMessage", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("GetMessage"))
                {
                    Message.GETMESSAGE? TEMP = JsonSerializer.Deserialize<Message.GETMESSAGE>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }
        private List<string> DsNhomQuanLy(string id_user)
        {
            List<string> a = new List<string>();
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(id_user, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("DsNhomQuanLy", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("DsNhomQuanLy"))
                {
                    Message.Request? TEMP = JsonSerializer.Deserialize<Message.Request>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private List<string> DsThanhVienThamGia(string id_group)
        {
            List<string> a = new List<string>();
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(id_group, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("DsThanhVienThamGia", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("DsThanhVienThamGia"))
                {
                    Message.Request? TEMP = JsonSerializer.Deserialize<Message.Request>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private Group ThongtinGroup(string id_group)
        {
            Group a = new Group();
            connectServer();
            byte[] data = new byte[1024];
            Message.TTGROUP mes = new Message.TTGROUP(id_group, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("ThongTinGroup", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("ThongTinGroup"))
                {
                    Message.TTGROUP? TEMP = JsonSerializer.Deserialize<Message.TTGROUP>(comm.content);
                    return TEMP.group;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private List<FriendChat> ListFriendChat(string gmailname)
        {
            List<FriendChat> a = new List<FriendChat>();
            connectServer();
            byte[] data = new byte[1024];
            Message.FRIENDCHAT mes = new Message.FRIENDCHAT(gmailname, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("ListFriendChat", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("ListFriendChat"))
                {
                    Message.FRIENDCHAT? TEMP = JsonSerializer.Deserialize<Message.FRIENDCHAT>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private List<string> getListRequestFriend(string gmailname)
        {
            List<string> a = new List<string>();
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(gmailname, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("getListRequestFriend", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("getListRequestFriend"))
                {
                    Message.Request? TEMP = JsonSerializer.Deserialize<Message.Request>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private int sothanhviennhom(string id_group)
        {
            int a = 0;
            connectServer();
            byte[] data = new byte[1024];
            Message.Common mes = new Message.Common(id_group, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("sothanhviennhom", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("sothanhviennhom"))
                {
                    Message.Common? TEMP = JsonSerializer.Deserialize<Message.Common>(comm.content);
                    return int.Parse(TEMP.content);
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return 0;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private string KeyGroup()
        {
            string a = "";
            connectServer();
            byte[] data = new byte[1024];
            Message.Common mes = new Message.Common("", null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("KeyGroup", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("KeyGroup"))
                {
                    Message.Common? TEMP = JsonSerializer.Deserialize<Message.Common>(comm.content);
                    return (TEMP.content);
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return "";
                }
            }
            catch (Exception)
            {
            }
            return a;
        }

        private void load()
        {
            loadFile();
            loadtinnhan();
            loadFriend();
            loadRequestFriend();
            loadInfoAccount();
            loadnhom();
            loadnhomquanly();
            textBox2.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            dataGridView5.Rows.Clear();
            dataGridView7.Rows.Clear();
            dataGridView8.Rows.Clear();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            connectServer();
            byte[] data = new byte[1024];
            Message.Common common = new Message.Common("Logout", ID_Accout);
            sendJson(common);
            MessageBox.Show("Đăng xuất thành công");
            this.Close();
        }

        private void clear()
        {
            textBox1.Text = "";
        }

        private void ThreadTask()
        {
            connectServer();
            string temp = "";
            byte[] data = new byte[1024];
            Message.SOCKET mes = new Message.SOCKET(ID_Accout);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("socket", jsonString);
            sendJson(common);
            Boolean thoat = false;
            while (!thoat)
            {
                int recv = client.Receive(data);
                jsonString = Encoding.ASCII.GetString(data, 0, recv);
                Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
                try
                {
                    if (comm != null)
                    {
                        data = new byte[1024];
                        recv = client.Receive(data);
                        jsonString = Encoding.ASCII.GetString(data, 0, recv);
                        comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
                        if (comm != null)
                        {
                            Message.Message? TEMP = JsonSerializer.Deserialize<Message.Message>(comm.content);
                            AppendTextBox(TEMP.usernameSender + ":" + TEMP.content + ":" + TEMP.usernameReceiver);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                try { this.Invoke(new Action<string>(AppendTextBox), new object[] { value }); }
                catch (Exception) { }
                return;
            }
            string[] x = value.Split(":");
            if (x[2].ToCharArray()[0].ToString().Equals("G"))
            {
                if (dstinnhan[userforcus].Equals(x[2]))
                {
                    textBox1.Text += DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt ") + " " + getAcc(x[0]).Username + " : " + x[1] + Environment.NewLine;
                }
            }
            else if (dstinnhan[userforcus].Equals(x[0]))
            {
                textBox1.Text += DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt ") + " " + getAcc(x[0]).Username + " : " + x[1] + Environment.NewLine;
            }
        }



        private void loadInfoAccount()
        {
            Account account = getAcc(ID_Accout);
            textBox4.Text = account.Email;
            textBox3.Text = account.Username;
            textBox5.Text = account.Phone;
        }

        private void loadFriend()
        {
            listFriend = ListFriendChat(gmailname);
            dataGridView2.Rows.Clear();
            for (int i = 0; i < listFriend.Count(); i++)
            {
                FriendChat f = listFriend[i];
                DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[0].Clone();
                row.Cells[0].Value = f.Id;
                row.Cells[1].Value = f.Email;
                row.Cells[2].Value = f.Username;
                row.Cells[3].Value = f.Phone;
                dataGridView2.Rows.Add(row);
            }
            dataGridView2.RowHeadersVisible = false;
        }

        private void loadRequestFriend()
        {
            listRequestFriend = getListRequestFriend(ID_Accout);
            dataGridView4.Rows.Clear();
            for (int i = 0; i < listRequestFriend.Count(); i++)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView4.Rows[0].Clone();
                row.Cells[0].Value = listRequestFriend[i];
                row.Cells[1].Value = getAcc(listRequestFriend[i]).Username;
                dataGridView4.Rows.Add(row);
            }
            dataGridView4.RowHeadersVisible = false;
        }

        private void loadtinnhan()
        {
            dstinnhan = getDStinnhan(ID_Accout);
            dataGridView1.Rows.Clear();
            for (int i = 0; i < dstinnhan.Count(); i++)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                row.Cells[0].Value = dstinnhan[i];
                if (dstinnhan[i].ToCharArray()[0].ToString().Equals("G"))
                {
                    Group gr = new Group();
                    gr = ThongtinGroup(dstinnhan[i]);
                    row.Cells[1].Value = "Nhóm " + gr.namegroup;
                }
                else
                {
                    row.Cells[1].Value = getAcc(dstinnhan[i]).Username;
                }
                dataGridView1.Rows.Add(row);
            }
            dataGridView1.RowHeadersVisible = false;
        }
        private void loadnhom()
        {
            dsnhom = LayDsNhom(ID_Accout);
            dataGridView3.Rows.Clear();
            for (int i = 0; i < dsnhom.Count(); i++)
            {
                Group gr = new Group();
                gr = ThongtinGroup(dsnhom[i].ToString());
                DataGridViewRow row = (DataGridViewRow)dataGridView3.Rows[0].Clone();
                row.Cells[0].Value = gr.Id;
                row.Cells[1].Value = gr.namegroup;
                row.Cells[2].Value = sothanhviennhom(gr.Id.ToString());
                dataGridView3.Rows.Add(row);
            }
            dataGridView3.RowHeadersVisible = false;
        }

        private void loadnhomquanly()
        {
            DSnhomQuanLy = DsNhomQuanLy(ID_Accout);
            dataGridView6.Rows.Clear();
            for (int i = 0; i < DSnhomQuanLy.Count(); i++)
            {
                Group gr = new Group();
                gr = ThongtinGroup(DSnhomQuanLy[i].ToString());
                DataGridViewRow row = (DataGridViewRow)dataGridView6.Rows[0].Clone();
                row.Cells[0].Value = gr.Id;
                row.Cells[1].Value = "Nhóm " + gr.namegroup;
                row.Cells[2].Value = sothanhviennhom(gr.Id.ToString());
                dataGridView6.Rows.Add(row);
            }
            dataGridView6.RowHeadersVisible = false;
        }

        private void sendJson(object obj)
        {
            byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(obj);
            client.Send(jsonUtf8Bytes, jsonUtf8Bytes.Length, SocketFlags.None);
        }
        public void connectServer()
        {
            iep = new IPEndPoint(IPAddress.Parse(ipaddress), 2008);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(iep);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox10.Enabled)
            {
                if (String.IsNullOrEmpty(textBox10.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên nhóm");
                }
                else
                {
                    creatGroup(0, textBox10.Text);
                    textBox10.Text = "";
                    textBox10.Enabled = false;
                    load();
                }
            }
            else
            {
                textBox10.Enabled = true;
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            int rowIndex = e.RowIndex;
            userforcus = rowIndex;
            textBox1.TextAlign = HorizontalAlignment.Center;
            TabControl.SelectedIndex = 0;
            if (dstinnhan[rowIndex].ToCharArray()[0].ToString().Equals("G"))
            {
                Group gr = new Group();
                gr = ThongtinGroup(dstinnhan[rowIndex]);
                textBox1.Text = "Tin nhắn với Nhóm " + gr.namegroup + Environment.NewLine;
                textBox1.TextAlign = HorizontalAlignment.Left;
                List<string> tinnhan = new List<string>();
                tinnhan = LayTinNhanNhom(dstinnhan[rowIndex]);
                for (int i = 0; i < tinnhan.Count(); i++)
                {
                    MESSAGE mess = new MESSAGE();
                    mess = GetMessage(tinnhan[i]);
                    textBox1.Text += mess.date + "   " + getAcc(mess.from_user).Username + " : " + mess.text + Environment.NewLine;
                }
            }
            else
            {
                textBox1.Text = "Tin nhắn với " + getAcc(dstinnhan[rowIndex]).Username + Environment.NewLine;
                textBox1.TextAlign = HorizontalAlignment.Left;
                List<MESSAGE> tinnhan = new List<MESSAGE>();
                string idNhan = getAcc(dstinnhan[rowIndex]).Id.ToString();
                tinnhan = LayTinNhan(idNhan, ID_Accout);
                for (int i = 0; i < tinnhan.Count(); i++)
                {
                    if (tinnhan[i].text.Equals("Bạn đã là bạn với "))
                    {
                        if (tinnhan[i].from_user.Equals(ID_Accout))
                        {
                            textBox1.Text += tinnhan[i].date + "   " + getAcc(tinnhan[i].from_user).Username + " : " + tinnhan[i].text + getAcc(tinnhan[i].to_user).Username + Environment.NewLine;
                        }
                        else
                        {
                            textBox1.Text += tinnhan[i].date + "   " + getAcc(tinnhan[i].to_user).Username + " : " + tinnhan[i].text + getAcc(tinnhan[i].from_user).Username + Environment.NewLine;
                        }
                    }
                    else
                    {
                        textBox1.Text += tinnhan[i].date + "   " + getAcc(tinnhan[i].from_user).Username + " : " + tinnhan[i].text + Environment.NewLine;
                        if (tinnhan[i].to_user.Equals(ID_Accout).ToString().Contains("."))
                        {
                            fileNames.Add(new File()
                            {
                                fileName = tinnhan[i].text
                            });
                        }
                    }
                }
                dataGridView8.Rows.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                MessageBox.Show("Vui lòng chọn đối tượng để nhắn");
            else if (textBox2.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tin nhắn");
            }
            else
            {
                connectServer();
                byte[] data = new byte[1024];
                Message.Message mes = new Message.Message(int.Parse(ID_Accout), dstinnhan[userforcus], textBox2.Text);
                string jsonString = JsonSerializer.Serialize(mes);
                Message.Common common = new Message.Common("sentmess", jsonString);
                sendJson(common);
                int recv = client.Receive(data);
                jsonString = Encoding.ASCII.GetString(data, 0, recv);
                Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
                try
                {
                    if (comm != null && comm.kind.Equals("Ok"))
                    {
                        textBox2.Text = "";
                        loadtinnhan();
                        if (dstinnhan[userforcus].ToCharArray()[0].ToString().Equals("G"))
                        {
                            Group gr = new Group();
                            gr = ThongtinGroup(dstinnhan[userforcus]);
                            textBox1.Text = "Tin nhắn với Nhóm " + gr.namegroup + Environment.NewLine;
                            textBox1.TextAlign = HorizontalAlignment.Left;
                            List<string> tinnhan = new List<string>();
                            tinnhan = LayTinNhanNhom(dstinnhan[userforcus]);
                            for (int i = 0; i < tinnhan.Count(); i++)
                            {
                                MESSAGE mess = new MESSAGE();
                                mess = GetMessage(tinnhan[i]);
                                textBox1.Text += mess.date + "   " + getAcc(mess.from_user).Username + " : " + mess.text + Environment.NewLine;
                            }
                        }
                        else
                        {
                            textBox1.Text = "Tin nhắn với " + getAcc(dstinnhan[userforcus]).Username + Environment.NewLine;
                            textBox1.TextAlign = HorizontalAlignment.Left;
                            List<MESSAGE> tinnhan = new List<MESSAGE>();
                            string idNhan = getAcc(dstinnhan[userforcus]).Id.ToString();
                            tinnhan = LayTinNhan(idNhan, ID_Accout);
                            for (int i = 0; i < tinnhan.Count(); i++)
                            {
                                textBox1.Text += tinnhan[i].date + "   " + getAcc(tinnhan[i].from_user).Username + " : " + tinnhan[i].text + Environment.NewLine;
                            }
                        }
                        MessageBox.Show(comm.content);
                    }
                    else if (comm != null && comm.kind.Equals("No"))
                    {
                        MessageBox.Show(comm.content);
                    }
                }
                catch (Exception)
                {
                }

            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox11.Text = "";
            int rowIndex = e.RowIndex;
            nhomchon = rowIndex;
            Group gr = new Group();
            gr = ThongtinGroup(DSnhomQuanLy[rowIndex]);
            textBox11.Text = gr.namegroup;
            dataGridView7.Rows.Clear();
            DSTVnhomQuanLy = DsThanhVienThamGia(DSnhomQuanLy[rowIndex]);
            for (int i = 0; i < DSTVnhomQuanLy.Count(); i++)
            {
                Account ac = new Account();
                ac = getAcc(DSTVnhomQuanLy[i]);
                DataGridViewRow row = (DataGridViewRow)dataGridView7.Rows[0].Clone();
                row.Cells[0].Value = ac.Id;
                row.Cells[1].Value = ac.Username;
                row.Cells[2].Value = ac.Email;
                dataGridView7.Rows.Add(row);
            }
            dataGridView7.RowHeadersVisible = false;
        }
        public void RenameGroup(string id_group, string namenew)
        {
            connectServer();
            Message.Common mes = new Message.Common(id_group + "|" + namenew, "");
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("RenameGroup", jsonString);
            sendJson(common);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox11.Enabled)
            {
                if (nhomchon == -1)
                {
                    MessageBox.Show("Vui lòng chọn nhóm");
                }
                else if (textBox11.Text.Length == 0)
                {
                    MessageBox.Show("Vui lòng nhập tên nhóm");
                }
                else
                {
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show("Bạn có chắc là muốn đổi tên nhóm thành " + textBox11.Text + " không ?", "Xác nhận!", buttons);
                    if (result == DialogResult.Yes)
                    {
                        RenameGroup(DSnhomQuanLy[nhomchon], textBox11.Text);
                        MessageBox.Show("Đổi tên nhóm thành công.");
                        textBox11.Text = "";
                        textBox11.Enabled = false;
                        load();
                    }
                    else
                    {
                        textBox11.Text = "";
                        textBox11.Enabled = false;
                    }
                }
            }
            else
            {
                textBox11.Enabled = true;
            }
        }
        public void DeleteGroup(string id_group)
        {
            connectServer();
            Message.Common mes = new Message.Common(id_group, "");
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("DeleteGroup", jsonString);
            sendJson(common);
        }
        public void deleteThanhvienNhom(string id_group, string user)
        {
            connectServer();
            Message.Common mes = new Message.Common(id_group, user);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("deleteThanhvienNhom", jsonString);
            sendJson(common);
        }
        private void dataGridView7_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show("Bạn có chắc là muốn xoá " + getAcc(DSTVnhomQuanLy[rowIndex]).Username + " khỏi nhóm không ?", "Xác nhận!", buttons);
            if (result == DialogResult.Yes)
            {
                string temp = getAcc(DSTVnhomQuanLy[rowIndex]).Username;
                deleteThanhvienNhom(ID_Accout + "|" + DSnhomQuanLy[nhomchon] + "|" + temp, DSTVnhomQuanLy[rowIndex]);
                MessageBox.Show("Đã xoá " + getAcc(DSTVnhomQuanLy[rowIndex]).Username + " ra khỏi nhóm thành công.");
                Group gr = new Group();
                load();
                gr = ThongtinGroup(DSnhomQuanLy[rowIndex]);
                textBox11.Text = gr.namegroup;
                dataGridView7.Rows.Clear();
                List<string> list = new List<string>();
                list = DsThanhVienThamGia(DSnhomQuanLy[rowIndex]);
                for (int i = 0; i < list.Count(); i++)
                {
                    Account ac = new Account();
                    ac = getAcc(list[i]);
                    DataGridViewRow row = (DataGridViewRow)dataGridView7.Rows[0].Clone();
                    row.Cells[0].Value = ac.Id;
                    row.Cells[1].Value = ac.Username;
                    row.Cells[2].Value = ac.Email;
                    dataGridView7.Rows.Add(row);
                }
                load();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (nhomchon == -1)
            {
                MessageBox.Show("Vui lòng chọn nhóm.");
            }
            else
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show("Bạn có chắc là muốn nhóm này không ?", "Xác nhận!", buttons);
                if (result == DialogResult.Yes)
                {
                    DeleteGroup(DSnhomQuanLy[nhomchon]);
                    MessageBox.Show("Xoá nhóm " + ThongtinGroup(DSnhomQuanLy[nhomchon]).namegroup + " thành công");
                    load();
                    textBox11.Text = "";
                    dataGridView7.Rows.Clear();
                }
            }
        }

        public bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox12.Enabled)
            {
                Boolean kiemtra = true;
                if (nhomchon != -1)
                {
                    DSTVnhomQuanLy = DsThanhVienThamGia(DSnhomQuanLy[nhomchon]);
                    for (int i = 0; i < DSTVnhomQuanLy.Count(); i++)
                    {
                        if (DSTVnhomQuanLy[i].Equals(textBox12.Text))
                        {
                            MessageBox.Show("Thành viên này đã có trong nhóm");
                            kiemtra = false;
                        }
                    }
                }
                if (nhomchon == -1)
                {
                    MessageBox.Show("Vui lòng chọn nhóm");
                }
                else if (textBox12.Text.Length == 0 || IsNumber(textBox12.Text) == false)
                {
                    MessageBox.Show("Vui lòng nhập ID của thành viên muốn thêm vào nhóm");
                }
                else if (textBox12.Text.Equals(ID_Accout))
                {
                    MessageBox.Show("Bạn không thể thêm chính mình vào nhóm.");
                    textBox12.Text = "";
                }
                else if (kiemtra == true)
                {
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show("Bạn có chắc muốn thêm " + getAcc(textBox12.Text).Username + " vào nhóm không ?", "Xác nhận!", buttons);
                    if (result == DialogResult.Yes)
                    {
                        Group gr = new Group();
                        gr = ThongtinGroup(DSnhomQuanLy[nhomchon].ToString());
                        creatGroup(1, gr.Id + "|" + textBox12.Text);
                        textBox12.Text = "";
                        textBox12.Enabled = false;
                        load();
                    }
                    else
                    {
                        textBox12.Enabled = false;
                    }
                }
                textBox12.Text = "";
            }
            else
            {
                textBox12.Enabled = true;
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                int i = e.RowIndex;
                FriendChat f = listFriend[i];
                DialogResult result = MessageBox.Show("Bạn có muốn xóa bạn với " + f.Username + " không?", "Message", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    connectServer();
                    trd = new Thread(new ThreadStart(() => { this.ThreadUnfriend(Convert.ToInt32(ID_Accout), f.Id); }));
                    trd.IsBackground = true;
                    trd.Start();
                    MessageBox.Show("Hủy kết bạn thành công");

                }
                else
                {

                }
            }
        }

        private void ThreadUnfriend(int idaccount, int idfriend)
        {
            byte[] data = new byte[1024];
            Message.RequestFriend request = new Message.RequestFriend(idaccount, idfriend);
            string jsonString = JsonSerializer.Serialize(request);
            Message.Common common = new Message.Common("Unfriend", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("Ok"))
                {
                    loadtinnhan();
                    loadFriend();
                }
                client.Disconnect(true);
                client.Close();
            }
            catch (Exception)
            {
            }
        }


        private void creatGroup(int type, string namegroup)
        {
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(type + "|" + namegroup + "|" + ID_Accout, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("creatGroup", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            if (comm.kind.Equals("creatGroup"))
            {
                MessageBox.Show(comm.content);
            }
        }

        private void ThreadChangePassword(int idaccount, string oldPass, string newPass)
        {
            byte[] data = new byte[1024];
            Message.ChangePassword change = new Message.ChangePassword(idaccount, oldPass, newPass);
            string jsonString = JsonSerializer.Serialize(change);
            Message.Common common = new Message.Common("Changepass", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("Ok"))
                {
                    MessageBox.Show(comm.content);
                    textBox6.Text = "";
                    textBox7.Text = "";
                    textBox8.Text = "";
                    TabControl.SelectedIndex = 0;
                }
                else if (comm != null && comm.kind.Equals("Cancel"))
                {
                    MessageBox.Show(comm.content);
                }
                client.Disconnect(true);
                client.Close();
            }
            catch (Exception)
            {
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            connectServer();
            if (String.IsNullOrEmpty(textBox6.Text) || String.IsNullOrEmpty(textBox7.Text) || String.IsNullOrEmpty(textBox8.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ password cũ, password mởi, xác nhận lại password");
            }
            else
            {
                if (textBox7.Text.Equals(textBox8.Text))
                {
                    trd = new Thread(new ThreadStart(() => { this.ThreadChangePassword(Convert.ToInt32(ID_Accout), textBox6.Text, textBox7.Text); }));
                    trd.IsBackground = true;
                    trd.Start();
                }
                else
                {
                    MessageBox.Show("Xác nhận lại mật khẩu không khớp. Vui lòng kiểm tra lại");
                }
            }
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                int i = e.RowIndex;
                DialogResult result = MessageBox.Show("Bạn đồng ý kết bạn với " + getAcc(listRequestFriend[i]).Username + " ?", "Message", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    connectServer();
                    byte[] data = new byte[1024];
                    Message.RequestFriend request = new Message.RequestFriend(Convert.ToInt32(ID_Accout), int.Parse(listRequestFriend[i]));
                    string jsonString = JsonSerializer.Serialize(request);
                    Message.Common common = new Message.Common("Acceptfriend", jsonString);
                    sendJson(common);
                    int recv = client.Receive(data);
                    jsonString = Encoding.ASCII.GetString(data, 0, recv);
                    Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
                    if (comm.kind.Equals("Acceptfriend"))
                    {
                        MessageBox.Show(comm.content);
                        loadtinnhan();
                        loadFriend();
                        loadRequestFriend();
                        load();
                    }
                }
            }
            else if (e.ColumnIndex == 3)
            {
                int i = e.RowIndex;
                DialogResult result = MessageBox.Show("Bạn từ chối kết bạn với " + getAcc(listRequestFriend[i]).Username + " ?", "Message", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    connectServer();
                    trd = new Thread(new ThreadStart(() => { this.ThreadUnfriend(Convert.ToInt32(ID_Accout), int.Parse(listRequestFriend[i])); }));
                    trd.IsBackground = true;
                    trd.Start();
                    MessageBox.Show("Từ chối lời mởi kết bạn thành công");
                    load();
                }
                else
                {

                }
            }
            loadtinnhan();
            loadFriend();
            loadRequestFriend();
            load();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox9.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập usernamme bạn cần tìm.");
            }
            else
            {
                dssearch = search(textBox9.Text, ID_Accout);
                if (dssearch.Count() > 0)
                {
                    dataGridView5.Rows.Clear();
                    for (int i = 0; i < dssearch.Count(); i++)
                    {
                        DataGridViewRow row = (DataGridViewRow)dataGridView5.Rows[0].Clone();
                        Account acc = new Account();
                        acc = getAcc(dssearch[i]);
                        row.Cells[0].Value = dssearch[i];
                        row.Cells[1].Value = acc.Email;
                        row.Cells[2].Value = acc.Username;
                        row.Cells[3].Value = acc.Phone;
                        dataGridView5.Rows.Add(row);
                    }
                    dataGridView5.RowHeadersVisible = false;
                }
                else
                {
                    dataGridView5.Rows.Clear();
                    MessageBox.Show("Không có Username nào phù hợp");
                }
            }
        }

        private List<string> search(string text, string ID_User)
        {
            List<string> a = new List<string>();
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(text + "|" + ID_User, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("search", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            try
            {
                if (comm != null && comm.kind.Equals("search"))
                {
                    Message.Request? TEMP = JsonSerializer.Deserialize<Message.Request>(comm.content);
                    return TEMP.list;
                }
                else if (comm != null && comm.kind.Equals("No"))
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return a;
        }


        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int dem = 0;
            for (int i = 0; i < DSnhomQuanLy.Count(); i++)
            {
                if (DSnhomQuanLy[i].Equals(dsnhom[rowIndex]) == false)
                    dem++;
            }
            if (dem == DSnhomQuanLy.Count())
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show("Bạn có chắc là muốn rời nhóm " + ThongtinGroup(dsnhom[rowIndex]).namegroup + " ?", "Xác nhận!", buttons);
                if (result == DialogResult.Yes)
                {
                    creatGroup(2, dsnhom[rowIndex].ToString());
                    load();
                }
            }
            else
            {
                MessageBox.Show("Bạn là chủ nhóm " + ThongtinGroup(dsnhom[rowIndex]).namegroup + " nên không thể rời nhóm.");
            }

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            Account temp = new Account();
            temp = getAcc(dssearch[rowIndex]);
            int dem = 0;

            if (KiemtraFriend(ID_Accout, temp.Id.ToString()) == true)
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show("Bạn có chắc là muốn kết bạn với " + temp.Username + " không ?", "Xác nhận!", buttons);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Bạn đã gửi lời mời kết bạn đến " + temp.Username + ".");
                    SentFriend(ID_Accout, temp.Id.ToString());
                }
            }
            else
            {
                MessageBox.Show("Bạn đã kết bạn với người này rồi.");
            }
        }

        private Boolean KiemtraFriend(string text, string ID_User)
        {
            Boolean kiemtra = false;
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(text + "|" + ID_User, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("KiemtraFriend", jsonString);
            sendJson(common);
            int recv = client.Receive(data);
            jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
            if (comm != null && comm.kind.Equals("KiemtraFriend"))
            {
                Message.Request? TEMP = JsonSerializer.Deserialize<Message.Request>(comm.content);
                if (TEMP.key.Equals("ok"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (comm != null && comm.kind.Equals("No"))
            {
                return false;
            }
            return false;
        }

        private void SentFriend(string text, string ID_User)
        {
            List<string> a = new List<string>();
            connectServer();
            byte[] data = new byte[1024];
            Message.Request mes = new Message.Request(text + "|" + ID_User, null);
            string jsonString = JsonSerializer.Serialize(mes);
            Message.Common common = new Message.Common("SentFriend", jsonString);
            sendJson(common);
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            connectServer();
            byte[] data = new byte[1024];
            Message.Common common = new Message.Common("Logout", ID_Accout);
            sendJson(common);
        }

        private void txt_gmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void loadFile()
        {
            //use binding source to hold dummy data
            BindingSource binding = new BindingSource();
            binding.DataSource = fileNames;

            //bind datagridview to binding source
            dataGridView8.DataSource = binding;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //UploadFile();
            OpenFileDialog dialog = new OpenFileDialog();
            var thread = new Thread(new ParameterizedThreadStart(param => {
                try
                {
                    if (dialog.ShowDialog() == DialogResult.OK) {
                        //initialize destination
                        string InitialDirectory = @"D:\File\";//Environment.CurrentDirectory;
                        //Get filename
                        string filename = Path.GetFileName(dialog.FileName);
                        //get filepath
                        string filepath = InitialDirectory + ID_Accout + @"\" + dstinnhan[userforcus] + @"\" + filename;
                        //File.Copy(dialog.FileName, filepath);
                        fileNames.Add(new File()
                        {
                            fileName = filename
                        });

                        connectServer();
                        byte[] data = new byte[1024];
                        Message.Message mes = new Message.Message(int.Parse(ID_Accout), dstinnhan[userforcus], filename);
                        string jsonString = JsonSerializer.Serialize(mes);
                        Message.Common common = new Message.Common("sentmess", jsonString);
                        sendJson(common);
                        int recv = client.Receive(data);
                        jsonString = Encoding.ASCII.GetString(data, 0, recv);
                        Message.Common? comm = JsonSerializer.Deserialize<Message.Common>(jsonString);
                        MessageBox.Show(filepath);


                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();


        }

        private void dataGridView8_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView8_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView8_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}