using MySql.Data.MySqlClient;
using Entity;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Tls.Crypto;
using Org.BouncyCastle.Ocsp;

namespace ConnectDB
{
    public class MySQL
    {
        MySqlConnection conn;


        public string hashMD5(string pass)
        {
            MD5 mh = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(pass);
            byte[] hash = mh.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public MySqlConnection Connect()
        {
            string connect = "server=localhost;port=3306;database=project_chatzelo;uid=root;pwd=;";
            conn = new MySqlConnection(connect);
            conn.Open();
            return conn;
        }

        public Account getAccount(string id)
        {
            Account account = new Account();
            Connect();
            string query = "select * from account where id = '"+id+"'";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                account.Id = reader.GetInt32("id");
                account.Email = reader.GetString("email");
                account.Username = reader.GetString("username");
                account.Password = reader.GetString("password");
                account.Phone = reader.GetString("phone");
            }
            reader.Close();
            return account;
        }

        public List<Account> getListAccount()
        {
            List<Account> list = new List<Account>();
            Connect();
            string query = "select * from account";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Account account = new Account();
                account.Id = reader.GetInt32("id");
                account.Email = reader.GetString("email");
                account.Username = reader.GetString("username");
                account.Password = reader.GetString("password");
                account.Phone = reader.GetString("phone");
                list.Add(account);
            }
            reader.Close();
            return list;
        }

        public Dictionary<string, List<string>> getDsNhom()
        {
            Connect();
            Dictionary<string, List<string>> DSNhom = new Dictionary<string, List<string>>();
            string query = "select * from groupchat";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader readr1 = cmd.ExecuteReader();
            while (readr1.Read())
            {
                List<string> nhom = ThanhVienNhom(readr1["id"].ToString());
                DSNhom.Add(readr1["id"].ToString(), nhom);
            }
            readr1.Close();
            return DSNhom;
        }

        public List<string> ThanhVienNhom(string x)
        {
            List<string> nhom = new List<string>();
            Connect();
            string query1 = "select * from detailgroupchat where id_groupchat = '" + x + "' and status = 1";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            while (readr2.Read())
            {
                nhom.Add(readr2["user"].ToString());
            }
            readr2.Close();
            return nhom;
        }

        public Boolean KiemtraTonTaiNhom(string Group,string user)
        {
            Boolean a = false;
            Connect();
            string query1 = "select * from detailgroupchat where id_groupchat = '" + Group + "'";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            while (readr2.Read())
            {
                if (readr2["user"].ToString().Equals(user))
                {
                    a = true;
                    break;
                }
            }
            readr2.Close();
            return a;
        }

        public Boolean KiemtraTTBanbe(string user, string user1)
        {
            Boolean a = false;
            Connect();
            string query1 = "select * from friend where ((id_account_1 = '" + user + "' and id_account_2 = '"+user1+ "') or ((id_account_1 = '" + user1 + "' and id_account_2 = '"+user+"'))) and status = 1;";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            while (readr2.Read())
            {
               a = true;
            }
            readr2.Close();
            return a;
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
        public List<string> getdstinnhan(string user)
        {
            Connect();
            List<string> dstn = new List<string>();
            List<string> list = new List<string>();
            list = DsNhomThamgia(user);
            string query1 = "select * from message where from_user = '" + user + "' or to_user = '" + user + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr = cmd1.ExecuteReader();
            while (readr.Read())
            {
                if (((readr["from_user"].ToString().Equals(user)) && IsNumber(readr["to_user"].ToString()) == true  )  || (readr["from_user"].ToString().Equals(user) && KiemtraTonTaiNhom(readr["to_user"].ToString(), user)) )
                {
                    if (dstn.Count() == 0)
                    {
                        dstn.Add(readr["to_user"].ToString());
                    }
                    else {
                        int dem = 0;
                        for (int i = 0; i < dstn.Count(); i++) {
                            if (dstn[i].Equals(readr["to_user"]))
                            {
                                break;
                            }
                            dem++;
                            if(dem == dstn.Count())
                            {
                                dstn.Add(readr["to_user"].ToString());
                            }
                        }
                    }
                }
                else if (readr["to_user"].Equals(user))
                {
                    if (dstn.Count() == 0)
                    {
                        dstn.Add(readr["from_user"].ToString());
                    }
                    else
                    {
                        int dem = 0;
                        for (int i = 0; i < dstn.Count(); i++)
                        {
                            if (dstn[i].Equals(readr["from_user"]))
                            {
                                break;
                            }
                            dem++;
                            if (dem == dstn.Count())
                            {
                                dstn.Add(readr["from_user"].ToString());
                            }
                        }
                    }
                }
            }
            if (dstn.Count() == 0)
            {
                return list;
            }
            else {
                for (int i = 0; i < list.Count(); i++) {
                    int dem = 0;
                    for (int j = 0; j < dstn.Count(); j++) {
                        if (list[i].Equals(dstn[j]))
                            break;
                        dem++;
                        if (dem == dstn.Count())
                            dstn.Add(list[i]);
                    }
                }
            }
            readr.Close();
            return dstn;
        }

        public List<MESSAGE> LayTinNhan(string x,string y)
        {
            Connect();
            List<MESSAGE> list = new List<MESSAGE>();
            string query1 = "select * from message where (from_user = '" + x + "' and to_user = '" + y + "') or (from_user = '" + y + "' and to_user = '" + x + "');";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr = cmd1.ExecuteReader();
            while (readr.Read())
            {
                MESSAGE message = new MESSAGE();
                message.Id = int.Parse(readr["id"].ToString());
                message.from_user = readr["from_user"].ToString();
                message.to_user = readr["to_user"].ToString();
                message.text = readr["text"].ToString();
                message.date = readr["date"].ToString();
                message.type = int.Parse(readr["type"].ToString());
                message.status = int.Parse(readr["status"].ToString());
                list.Add(message);
            }
            readr.Close();
            return list;
        }

        public List<string> LayTinNhanNhom(string ID_Group)
        {
            Connect();
            List<string> list = new List<string>();
            string query1 = "select * from message where to_user = '" + ID_Group + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr = cmd1.ExecuteReader();
            while (readr.Read())
            {
                string temp = readr["id"].ToString();
                list.Add(temp);
            }
            readr.Read();
            readr.Close();
            return list;
        }

        public MESSAGE GetMessage(string ID_Group)
        {
            Connect();
            MESSAGE list = new MESSAGE();
            string query1 = "select * from message where id = '" + ID_Group + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr = cmd1.ExecuteReader();
            while (readr.Read())
            {
                list.Id = int.Parse(readr["id"].ToString());
                list.from_user = readr["from_user"].ToString();
                list.to_user = readr["to_user"].ToString();
                list.text = readr["text"].ToString();
                list.date = readr["date"].ToString();
                list.type = int.Parse(readr["type"].ToString());
                list.status = int.Parse(readr["status"].ToString());
            }
            readr.Read();
            readr.Close();
            return list;
        }

        public List<string> LayDsNhom(string x)
        {
            Connect();
            List<string> list = new List<string>();
            string query1 = "select * from detailgroupchat where user = "+int.Parse(x)+";";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr = cmd1.ExecuteReader();
            while (readr.Read())
            {
                list.Add(readr["id_groupchat"].ToString());
            }
            readr.Close();
            return list;
        }

        public int sothanhviennhom(string id_group)
        {
            Connect();
            string query1 = "select * from detailgroupchat where id_groupchat = '" + id_group + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr = cmd1.ExecuteReader();
            int dem = 0;
            while (readr.Read())
            {
                dem++;
            }
            readr.Close();
            return dem;
        }

        public Group ThongtinNHOM(string id_group)
        {
            Connect();
            Group gr = new Group();
            string query1 = "select * from groupchat where id = '" + id_group + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr = cmd1.ExecuteReader();
            while (readr.Read())
            {
                gr.Id = id_group.ToString();
                gr.status = int.Parse(readr["status"].ToString());
                gr.namegroup = readr["namegroup"].ToString();
            }
            readr.Close();
            return gr;
        }

        public void sentmess(int sent,string rev,string text)
        {
            Connect();
            string query = "insert into message (from_user,to_user,text,date,type,status) values ('" + sent + "','" + rev + "','" + text + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',1,1)";
            MySqlCommand cmd1 = new MySqlCommand(query, conn);
            cmd1.ExecuteNonQuery();
        }

        public void addAccount(string email, string password, string username, string phone)
        {
            Connect();
            password = hashMD5(password);
            string query = "insert into account (email,password,username,phone) values ('" + email + "','" + password + "','" + username + "','" + phone + "')";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        public void CrearteGroup(string KeyGroup,string Name)
        {
            Connect();
            string query = "insert into groupchat (id,namegroup,status) values ('"+KeyGroup+"','" + Name + "',1);";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        public string KeyGroup() {
            Connect();
            string query1 = "SELECT * FROM groupchat ORDER BY id DESC LIMIT  1;";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            string ketqua = "";
            while (readr2.Read())
            {
                ketqua = readr2["id"].ToString().Substring(2);
                int kt = int.Parse(ketqua);
                kt = kt + 1;
                if (kt < 10)
                {
                    ketqua = "GR000" + kt.ToString();
                }
                else if (kt < 100)
                {
                    ketqua = "GR00" + kt.ToString();
                }
                else if (kt < 1000)
                {
                    ketqua = "GR0" + kt.ToString();
                }
                else
                {
                    ketqua = "GR" + kt.ToString();
                }
            }
            readr2.Close();
            return ketqua;
        }

        public string macuoiGroup()
        {
            Connect();
            string query1 = "SELECT * FROM groupchat ORDER BY id DESC LIMIT  1;";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            string kq = "";
            while (readr2.Read())
            {
                kq = readr2["id"].ToString();
            }
            readr2.Close();
            return kq;
        }
         
        public void addUsertoGR(string id_group, string user, int room_master)
        {
            Connect();
            string query1 = "insert into detailgroupchat (id_groupchat,user,room_master,status) values ('" + id_group + "','" + user + "'," + room_master + ",1);";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            cmd1.ExecuteNonQuery();
        }

        public void deleteThanhvienNhom(string id_group, string user)
        {
            Connect();
            string query1 = "DELETE FROM detailgroupchat WHERE id_groupchat = '"+id_group+"' and user = '"+user+"';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            cmd1.ExecuteNonQuery();
            string query2 = "UPDATE message SET status = 0 WHERE from_user = '" + user + "' and to_user = '"+id_group+"';";
            MySqlCommand cmd2 = new MySqlCommand(query2, conn);
            cmd2.ExecuteNonQuery();
        }

        public List<string> search(string text,string accid)
        {
            Connect();
            List<string> kq = new List<string>();
            string query1 = "SELECT * FROM account WHERE (username LIKE '%" + text+ "%') and id != '"+accid+"';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            while (readr2.Read())
            {
                kq.Add(readr2["id"].ToString());
            }
            readr2.Close();
            return kq;
        }

        public List<string> DsNhomThamgia(string id_user)
        {
            Connect();
            List<string> list = new List<string>();
            string query1 = "SELECT * FROM detailgroupchat where user = '"+id_user+"';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            string kq = "";
            while (readr2.Read())
            {
                list.Add(readr2["id_groupchat"].ToString());  
            }
            readr2.Close();
            return list;
        }
        public List<string> DsThanhVienThamGia(string id_group)
        {
            Connect();
            List<string> list = new List<string>();
            string query1 = "SELECT * FROM detailgroupchat where id_groupchat = '" + id_group + "' and room_master != 1;";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            while (readr2.Read())
            {
                list.Add(readr2["user"].ToString());
            }
            readr2.Close();
            return list;
        }
        public List<string> DsThanhVienThamGiaFull(string id_group,int user)
        {
            Connect();
            List<string> list = new List<string>();
            string query1 = "SELECT * FROM detailgroupchat where id_groupchat = '" + id_group + "' and user != '"+ user + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            while (readr2.Read())
            {
                list.Add(readr2["user"].ToString());
            }
            readr2.Close();
            return list;
        }

        public List<string> DsNhomQuanLy(string id_user)
        {
            Connect();
            List<string> list = new List<string>();
            string query1 = "SELECT * FROM detailgroupchat where user = '" + id_user + "' and room_master = 1;";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            while (readr2.Read())
            {
                list.Add(readr2["id_groupchat"].ToString());
            }
            readr2.Close();
            return list;
        }

        public List<string> KiemtraFriend(string id_user,string id2)
        {
            Connect();
            List<string> list = new List<string>();
            string query1 = "SELECT * FROM friend where ( id_account_1 = '" + id_user + "' and id_account_2 = '"+id2+ "') or ( id_account_1 = '" + id2 + "' and id_account_2 = '"+id_user+"');";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            MySqlDataReader readr2 = cmd1.ExecuteReader();
            while (readr2.Read())
            {
                list.Add(readr2["id_friend"].ToString());
            }
            readr2.Close();
            return list;
        }

        public void RenameGroup(string id_group,string namenew)
        {
            Connect();
            string query1 = "UPDATE groupchat SET namegroup = '"+ namenew + "' WHERE id = '"+ id_group + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            cmd1.ExecuteNonQuery();
        }

        public void DeleteGroup(string id_group)
        {
            Connect();
            string query = "DELETE FROM groupchat WHERE id = '" + id_group + "';";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            delete1(id_group);
            delete2(id_group);
           
        }

        public void SentFriend(string accid,string friendid)
        {
            Connect();
            string query = "insert into friend (id_account_1,id_account_2,status) values ('" + accid + "','" + friendid + "',0)";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        public void delete1(string id_group) {
            Connect();
            string query1 = "DELETE FROM detailgroupchat WHERE id_groupchat = '" + id_group + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            cmd1.ExecuteNonQuery();
        }
        public void delete2(string id_group)
        {
            Connect();
            string query2 = "DELETE FROM message WHERE to_user = '" + id_group + "';";
            MySqlCommand cmd2 = new MySqlCommand(query2, conn);
            cmd2.ExecuteNonQuery();
        }

        public void deleteTinnhan(string id1,string id2)
        {
            Connect();
            string query2 = "DELETE FROM message WHERE (from_user = '" + id1 + "' and to_user ='"+id2+ "') or (from_user = '" + id2 + "' and to_user ='"+id1+"');";
            MySqlCommand cmd2 = new MySqlCommand(query2, conn);
            cmd2.ExecuteNonQuery();
        }

        public List<FriendChat> getListFriend(string email)
        {
            List<FriendChat> list = new List<FriendChat>();
            Connect();
            string query = "select * from account where id IN (select id_account_1 as id from friend, account where account.email = '" + email + "' and account.id = id_account_2 and status = 1 UNION select id_account_2 as id from friend, account where account.email = '" + email + "' and account.id = id_account_1 and status = 1)";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                FriendChat friend = new FriendChat();
                friend.Id = reader.GetInt32("id");
                friend.Email = reader.GetString("email");
                friend.Username = reader.GetString("username");
                friend.Phone = reader.GetString("phone");
                list.Add(friend);
            }
            reader.Close();
            return list;
        }

        public void unFriend(int idaccount, int idfriend)
        {
            Connect();
            string query = "delete from friend where id_account_1 = " + idaccount + " and id_account_2 = " + idfriend + " or id_account_1 = " + idfriend + " and id_account_2 = " + idaccount;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            string query1 = "delete from message where from_user = " + idaccount + " and to_user = " + idfriend + " or from_user = " + idfriend + " and to_user = " + idaccount;
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            cmd1.ExecuteNonQuery();
        }

        public List<string> getListRequestFriend(string email)
        {
            List<string> list = new List<string>();
            Connect();
            string query = "select * from friend where id_account_2 = '" + email+"' and status = 0";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader["id_account_1"].ToString());
            }
            reader.Close();
            return list;
        }
        public void changePassword(int id, string newPassword)
        {
            Connect();
            newPassword = hashMD5(newPassword);
            string query = "update account set password = '" + newPassword + "' where id = " + id;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            reader.Close();
        }
        public void acceptRequest(int idaccount, int idfriend)
        {
            Connect();
            string query = "update friend set status = 1 where id_account_1 = " + idaccount + " and id_account_2 = " + idfriend + " or id_account_1 = " + idfriend + " and id_account_2 = " + idaccount;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            reader.Close();
        }

    }
}