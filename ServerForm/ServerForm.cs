using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Entity;
using ConnectDB;
using System.Text;
using Message;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.Logging;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Drawing;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.VisualBasic.ApplicationServices;
using Org.BouncyCastle.Asn1.Ocsp;
using static System.Net.Mime.MediaTypeNames;

namespace ServerForm
{
    public partial class ServerForm : Form
    {
        bool active = false;
        IPEndPoint iep;
        Socket server;
        List<Account> listAccount;
        Dictionary<string, List<string>> DSNhom;
        Dictionary<string, Socket> DSClient = new Dictionary<string, Socket>();
        MySQL connect = new MySQL();

        public ServerForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string hostName = Dns.GetHostName();
            foreach (IPAddress ip in Dns.GetHostByName(hostName).AddressList)
            {
                if (ip.ToString().Contains("."))
                {
                    txt_IP.Text = ip.ToString();
                    break;
                }
            }
            txt_Port.Text = "2008";
            active = true;
            iep = new IPEndPoint(IPAddress.Parse(txt_IP.Text), int.Parse(txt_Port.Text));
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(10);
            Thread trd = new Thread(new ThreadStart(this.ThreadTask));
            trd.IsBackground = true;
            trd.Start();
            getListAccount();
            getDsNhom();
        }

        private void getListAccount()
        {
            listAccount = connect.getListAccount();
        }

        

        private void getDsNhom()
        {
            DSNhom = connect.getDsNhom();
        }

        private List<string> dsTinnhan(string user)
        {
            List<string> list = new List<string>();
            list = connect.getdstinnhan(user);
            return list;
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

        private void sendJson(Socket client, object obj)
        {
          
            byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(obj);
            client.Send(jsonUtf8Bytes, jsonUtf8Bytes.Length, SocketFlags.None);
        }

        private void ThreadTask()
        {
            while (active)
            {
                try
                {
                    Socket client = server.Accept();
                    var t = new Thread(() => ThreadClient(client));
                    t.Start();
                }
                catch (Exception)
                {
                    active = false;
                }

            }
        }

        private void ThreadClient(Socket client)
        {
            byte[] data = new byte[1024];
            int recv = client.Receive(data);
            if (recv == 0)
            {
                return;
            }
            string jsonString = Encoding.ASCII.GetString(data, 0, recv);
            Message.Common? common = JsonSerializer.Deserialize<Message.Common>(jsonString);
            if (common != null)
            {
                if (common.kind.Equals("Register") && common.content != null)
                {
                    Register? register = JsonSerializer.Deserialize<Register>(common.content);
                    if (register != null)
                    {
                        bool check = true;
                        foreach (Account acc in listAccount)
                        {
                            if (acc.Email.Equals(register.email))
                            {
                                check = false;
                            }
                            else if (acc.Phone.Equals(register.phone))
                            {
                                check = false;
                            }
                        }
                        if (check == false)
                        {
                            common = new Common("Cancel", "Email hoặc số điện thoại đã được đăng ký. Vui lòng kiểm tra lại");
                            sendJson(client, common);
                        }
                        else
                        {
                            connect.addAccount(register.email, register.password, register.username, register.phone);
                            common = new Common("Ok", "Đăng ký tài khoản thành công");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("Login") && common.content != null)
                {
                    int id = 0;
                    Login? login = JsonSerializer.Deserialize<Login>(common.content);
                    if (login != null)
                    {
                        bool check = false;
                        login.password = connect.hashMD5(login.password);
                        foreach (Account acc in listAccount)
                        {
                            if (acc.Email.Equals(login.email) && acc.Password.Equals(login.password))
                            {
                                id = acc.Id;
                                check = true;
                                break;
                            }
                            else
                            {
                                check = false;
                            }
                        }
                        if (check == true)
                        {
                            common = new Common("Ok", (string)id.ToString());
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("Cancel", "Sai email hoặc password. Vui lòng kiểm tra lại");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("socket"))
                {
                    Message.SOCKET? mes = JsonSerializer.Deserialize<Message.SOCKET>(common.content);
                    if (mes != null )
                    {
                        try {
                            DSClient.Remove(mes.id_user);
                            DSClient.Add(mes.id_user, client);
                        }
                        catch (Exception ex) { 
                        }
                        common = new Common("Ok","");
                        sendJson(client, common);
                    }
                }
                else if (common.kind.Equals("sentmess"))
                {
                    Message.Message? mes = JsonSerializer.Deserialize<Message.Message>(common.content);
                    if (mes != null )
                    {
                        try
                        {
                            if (mes.usernameReceiver.ToCharArray()[0].ToString().Equals("G"))
                            {
                                List<string> list = connect.DsThanhVienThamGiaFull(mes.usernameReceiver,mes.usernameSender);
                                for (int i = 0; i < list.Count(); i++) {
                                    if (DSClient.Keys.Contains(list[i])) {
                                        Socket friend = DSClient[list[i]];
                                        friend.Send(data, recv, SocketFlags.None);
                                    }
                                }
                                connect.sentmess(mes.usernameSender, mes.usernameReceiver, mes.content);

                            }
                            else if (IsNumber(mes.usernameSender.ToString()) == true)
                            {
                                connect.sentmess(mes.usernameSender, mes.usernameReceiver, mes.content);
                                Socket friend = DSClient[mes.usernameReceiver];
                                friend.Send(data, recv, SocketFlags.None);
                                common = new Common("Ok", "Gửi tin nhắn thất bại");
                                sendJson(client, common);
                            }
                            else
                            {
                                common = new Common("No","Gửi tin nhắn thất bại");
                                sendJson(client, common);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        common = new Common("Ok", "Gửi tin nhắn thành công");
                        sendJson(client, common);
                    }
                }
                else if (common.kind.Equals("Unfriend") && common.content != null)
                {
                    RequestFriend? request = JsonSerializer.Deserialize<RequestFriend>(common.content);
                    if (request != null)
                    {
                        connect.unFriend(request.idaccount, request.idfriend);
                        common = new Common("Ok", "Hủy kết bạn thành công");
                        sendJson(client, common);
                    }
                }
                else if (common.kind.Equals("Logout") )
                {
                    DSClient[common.content].Close();
                    DSClient.Remove(common.content);
                }
                else if (common.kind.Equals("Unfriend") && common.content != null)
                {
                    RequestFriend? request = JsonSerializer.Deserialize<RequestFriend>(common.content);
                    if (request != null)
                    {
                        connect.unFriend(request.idaccount, request.idfriend);
                        common = new Common("Ok", "Hủy kết bạn thành công");
                        sendJson(client, common);
                    }
                }
                else if (common.kind.Equals("creatGroup") && common.content != null)
                {
                    Message.Request? request = JsonSerializer.Deserialize<Message.Request>(common.content);
                    string []x = request.key.Split('|');
                    if (request != null)
                    {
                        if (x[0].Equals("0"))
                        {
                            List<string> strings= new List<string>();
                            strings.Add(x[2]);
                            string key = connect.KeyGroup();
                            DSNhom.Add(key,strings);
                            connect.CrearteGroup(key, x[1]);
                            connect.addUsertoGR(key, x[2], 1);
                            connect.sentmess(int.Parse(x[2]), key, "Đã tạo nhóm.");
                            common = new Common("creatGroup", "Tạo nhóm thành công");
                            sendJson(client, common);
                        }
                        else if (x[0].Equals("1"))
                        {
                            string key = connect.getAccount(x[2]).Username;
                            connect.addUsertoGR(x[1], x[2], 0);
                            connect.sentmess(int.Parse(x[3]), x[1], "Đã thêm " + key + " vào nhóm");
                            common = new Message.Common("creatGroup", "Thêm "+ key + " vào nhóm " + connect.ThongtinNHOM(x[1]).namegroup +" thành công.");
                            sendJson(client, common);
                        }
                        else if (x[0].Equals("2"))
                        {
                            string key = connect.getAccount(x[2]).Username;
                            string key1 = connect.ThongtinNHOM(x[1]).namegroup;
                            connect.deleteThanhvienNhom(x[1], x[2]);
                            connect.sentmess(int.Parse(x[2]), x[1],key + " đã rời nhóm");
                            common = new Message.Common("creatGroup", "Bạn đã rời nhóm " + key1 + " thành công.");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("Acceptfriend") && common.content != null)
                {
                    RequestFriend? request = JsonSerializer.Deserialize<RequestFriend>(common.content);
                    if (request != null)
                    {
                        connect.acceptRequest(request.idaccount, request.idfriend);
                        connect.sentmess(request.idaccount, request.idfriend.ToString(), "Bạn đã là bạn với ");
                        common = new Common("Ok", "Đồng ý kết bạn thành công");
                        sendJson(client, common);
                    }
                }
                else if (common.kind.Equals("Changepass") && common.content != null)
                {
                    ChangePassword? change = JsonSerializer.Deserialize<ChangePassword>(common.content);
                    if (change != null)
                    {
                        bool check = false;
                        Account acc = connect.getAccount(change.Id.ToString());
                        if (connect.hashMD5(change.oldPass).Equals(acc.Password))
                        {
                            connect.changePassword(change.Id, change.newPass);
                            check = true;
                        }
                        else
                        {
                            check = false;
                        }
                        if (check == true)
                        {
                            getListAccount();
                            common = new Common("Ok", "Thay đổi password thành công");
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("Cancel", "Password không chính xác. Vui lòng kiểm tra lại");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("dsTinnhan") && common.content != null)
                {
                    Message.Request? request = JsonSerializer.Deserialize<Message.Request>(common.content);
                    if (request != null)
                    {
                        List<string> a = new List<string>();
                        a = connect.getdstinnhan(request.key);
                        if (a != null)
                        {
                            Message.Request kq = new Message.Request("ok",a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("dsTinnhan", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("LayDsNhom") && common.content != null)
                {
                    Message.Request? request = JsonSerializer.Deserialize<Message.Request>(common.content);
                    if (request != null)
                    {
                        List<string> a = new List<string>();
                        a = connect.LayDsNhom(request.key);
                        if (a != null)
                        {
                            Message.Request kq = new Message.Request("ok", a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("LayDsNhom", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("DsNhomQuanLy") && common.content != null)
                {
                    Message.Request? request = JsonSerializer.Deserialize<Message.Request>(common.content);
                    if (request != null)
                    {
                        List<string> a = new List<string>();
                        a = connect.DsNhomQuanLy(request.key);
                        if (a != null)
                        {
                            Message.Request kq = new Message.Request("ok", a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("DsNhomQuanLy", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("ThongTinGroup") && common.content != null)
                {
                    TTGROUP? request = JsonSerializer.Deserialize<TTGROUP>(common.content);
                    if (request != null)
                    {
                        Group a = new Group();
                        a = connect.ThongtinNHOM(request.key);
                        if (a != null)
                        {
                            Message.TTGROUP kq = new Message.TTGROUP("ok", a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("ThongTinGroup", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("ListFriendChat") && common.content != null)
                {
                    FRIENDCHAT? acc = JsonSerializer.Deserialize<FRIENDCHAT>(common.content);
                    if (acc != null)
                    {
                        List<FriendChat> a = new List<FriendChat>();

                        a = connect.getListFriend(acc.key);
                        if (a != null)
                        {
                            Message.FRIENDCHAT kq = new Message.FRIENDCHAT("ok",a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("ListFriendChat", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("KiemtraFriend") && common.content != null)
                {
                    Message.Request? acc = JsonSerializer.Deserialize<Message.Request>(common.content);
                    if (acc != null)
                    {
                        List<string> a = new List<string>();
                        string []x = acc.key.Split("|");
                        a = connect.KiemtraFriend(x[0], x[1]);
                        if (a.Count()==0)
                        {
                            Message.Request kq = new Message.Request("ok",null);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("KiemtraFriend", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("sothanhviennhom") && common.content != null)
                {
                    Common? acc = JsonSerializer.Deserialize<Common>(common.content);
                    if (acc != null)
                    {
                        int a = 0;
                        a = connect.sothanhviennhom(acc.kind);
                        if (a != null)
                        {
                            Message.Common kq = new Message.Common("ok", a.ToString());
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("sothanhviennhom", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("KeyGroup") && common.content != null)
                {
                    Common? acc = JsonSerializer.Deserialize<Common>(common.content);
                    if (acc != null)
                    {
                        string a = "";
                        a = connect.KeyGroup();
                        if (a != null)
                        {
                            Message.Common kq = new Message.Common("ok", a.ToString());
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("KeyGroup", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("DsThanhVienThamGia") && common.content != null)
                {
                    Message.Request? request = JsonSerializer.Deserialize<Message.Request>(common.content);
                    if (request != null)
                    {
                        List<string> a = new List<string>();
                        a = connect.DsThanhVienThamGia(request.key);
                        if (a != null)
                        {
                            Message.Request kq = new Message.Request("ok", a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("DsThanhVienThamGia", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("search") && common.content != null)
                {
                    Message.Request? request = JsonSerializer.Deserialize<Message.Request>(common.content);
                    if (request != null)
                    {
                        List<string> a = new List<string>();
                        string[] x = request.key.Split("|");
                        a = connect.search(x[0], x[1]);
                        if (a != null)
                        {
                            Message.Request kq = new Message.Request("ok", a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("search", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("SentFriend") && common.content != null)
                {
                    Message.Request? request = JsonSerializer.Deserialize<Message.Request>(common.content);
                    if (request != null)
                    {
                        string[] x = request.key.Split("|");
                        connect.SentFriend(x[0], x[1]);                        
                    }
                }
                else if (common.kind.Equals("getListRequestFriend") && common.content != null)
                {
                    Message.Request? acc = JsonSerializer.Deserialize<Message.Request>(common.content);
                    if (acc != null)
                    {
                        List<string> a = new List<string>();
                        a = connect.getListRequestFriend(acc.key);
                        if (a != null)
                        {
                            Message.Request kq = new Message.Request("ok", a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("getListRequestFriend", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("LayTinNhan") && common.content != null)
                {
                    TINNHAN? acc = JsonSerializer.Deserialize<TINNHAN>(common.content);
                    if (acc != null)
                    {
                        List<MESSAGE> a = new List<MESSAGE>();
                        string []x = acc.key.Split("|");
                        a = connect.LayTinNhan(x[0], x[1]);
                        if (a != null)
                        {
                            Message.TINNHAN kq = new Message.TINNHAN("ok", a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("LayTinNhan", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("RenameGroup") && common.content != null)
                {
                    Common? acc = JsonSerializer.Deserialize<Common>(common.content);
                    if (acc != null)
                    {
                        string[] x = acc.kind.Split("|");
                        connect.RenameGroup(x[0], x[1]);
                    }
                }
                else if (common.kind.Equals("InBox") && common.content != null)
                {
                    Common? acc = JsonSerializer.Deserialize<Common>(common.content);
                    if (acc != null)
                    {
                        string[] x = acc.kind.Split("|");
                        List<string> list = new List<string>();
                        list = connect.DsThanhVienThamGiaFull(x[1], int.Parse(x[0]));
                        if (x[1].ToCharArray()[0].ToString().Equals("G") && connect.KiemtraTonTaiNhom(x[1],x[0]))
                        {
                            if (list.Count() > 0) {
                                for (int i = 0; i < list.Count(); i++)
                                {
                                    if (DSClient.Keys.Contains(list[i]))
                                    {
                                        Socket friend = DSClient[list[i]];
                                        friend.Send(data, recv, SocketFlags.None);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Boolean kt = connect.KiemtraTTBanbe(x[0], x[1]);
                            if (kt == true) {
                                Socket friend = DSClient[x[1]];
                                friend.Send(data, recv, SocketFlags.None);
                                connect.sentmess(int.Parse(x[0]), x[1], acc.content);
                            }
                        }
                    }
                }
                else if (common.kind.Equals("DeleteGroup") && common.content != null)
                {
                    Common? acc = JsonSerializer.Deserialize<Common>(common.content);
                    if (acc != null)
                    {
                        connect.DeleteGroup(acc.kind);
                    }
                }
                else if (common.kind.Equals("deleteThanhvienNhom") && common.content != null)
                {
                    Common? acc = JsonSerializer.Deserialize<Common>(common.content);
                    if (acc != null)
                    {
                        string []x = acc.kind.Split("|");
                        string text = "Đã xoá " + x[2] + " ra khỏi nhóm";
                        connect.deleteThanhvienNhom(x[1],acc.content);
                        connect.sentmess(int.Parse(x[0]), x[1], text);
                    }
                }
                else if (common.kind.Equals("LayTinNhanNhom") && common.content != null)
                {
                    Message.Request? request = JsonSerializer.Deserialize<Message.Request>(common.content);
                    if (request != null)
                    {
                        List<string> a = new List<string>();
                        a = connect.LayTinNhanNhom(request.key);
                        if (a != null)
                        { 
                            Message.Request kq = new Message.Request("ok", a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("LayTinNhanNhom", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("GetMessage") && common.content != null)
                {
                    Message.GETMESSAGE? request = JsonSerializer.Deserialize<Message.GETMESSAGE>(common.content);
                    if (request != null)
                    {
                        MESSAGE a = new MESSAGE();
                        a = connect.GetMessage(request.key);
                        if (a != null)
                        {
                            Message.GETMESSAGE kq = new Message.GETMESSAGE("ok",a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("GetMessage", temp);
                            sendJson(client, common);
                        }
                        else
                        {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }
                else if (common.kind.Equals("getAccount") && common.content != null)
                {
                    RequestAcc? acc = JsonSerializer.Deserialize<RequestAcc>(common.content);
                    if (acc != null)
                    {
                        
                        Account a = new Account();
                        a = connect.getAccount(acc.key);
                        if (a != null)
                        {
                            Message.RequestAcc kq = new Message.RequestAcc("ok",a);
                            string temp = JsonSerializer.Serialize(kq);
                            common = new Message.Common("getAccount", temp);
                            sendJson(client, common);
                        }
                        else {
                            common = new Common("No", "");
                            sendJson(client, common);
                        }
                    }
                }//
            }
        }

    }
}