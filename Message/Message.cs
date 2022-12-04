using Entity;
namespace Message
{
    public class Common
    {
        public Common(string kind, string content)
        {
            this.kind = kind;
            this.content = content;
        }
        public string kind { get; set; }
        public string content { get; set; }

    }



    public class Login
    {
        public Login(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
        public string email {get; set; }
        public string password { get; set; } 
    }

    public class Register
    {
        public Register(string email, string password, string username, string phone)
        {
            this.email = email;
            this.password = password;
            this.username = username;
            this.phone = phone;
        }
        public string email {get; set; }
        public string password { get; set; }
        public string username {get; set; }
        public string phone {get; set; }
    }

    public class Message
    {
        public Message(int usernameSender, string usernameReceiver, string? content)
        {
            this.usernameSender = usernameSender;
            this.usernameReceiver = usernameReceiver;
            this.content = content;
        }
        public int usernameSender { get; set; }
        public string usernameReceiver { get; set; }
        public string? content { get; set; }
    }

    public class SOCKET
    {
        public SOCKET(string id_user)
        {
            this.id_user = id_user;
        }
        public string id_user { get; set; }

    }

    public class RequestFriend
    {
        public RequestFriend(int idaccount, int idfriend)
        {
            this.idaccount = idaccount;
            this.idfriend = idfriend;
        }
        public int idaccount { get; set; }
        public int idfriend { get; set; }
    }

    public class ChangePassword
    {
        public ChangePassword(int id, string oldPass, string newPass)
        {
            this.Id = id;
            this.oldPass = oldPass;
            this.newPass = newPass;
        }

        public int Id { get; set; }
        public string oldPass { get; set; }
        public string newPass { set; get; }
    }

    public class Friend
    {
        public Friend(string gmail, List<string> dsfriend)
        {
            this.gmail = gmail;
            this.dsfriend = dsfriend;
        }
        public string gmail { get; set; }
        public List<string> dsfriend { get; set; }
    }

    public class Request
    {
        public Request(string key, List<string> list)
        {
            this.key = key;
            this.list = list;
        }
        public string key { get; set; }
        public List<string> list { get; set; }
    }

    public class RequestAcc
    {
        public RequestAcc(string key, Account list)
        {
            this.key = key;
            this.list = list;
        }
        public string key { get; set; }
        public Account list { get; set; }
    }
    public class FRIENDCHAT
    {
        public FRIENDCHAT(string key, List<FriendChat> list)
        {
            this.key = key;
            this.list = list;
        }
        public string key { get; set; }
        public List<FriendChat> list { get; set; }
    }
    public class TTGROUP
    {
        public TTGROUP(string key, Group group)
        {
            this.key = key;
            this.group = group;
        }
        public string key { get; set; }
        public Group group { get; set; }
    }

    public class TINNHAN
    {
        public TINNHAN(string key, List<MESSAGE> list)
        {
            this.key = key;
            this.list = list;
        }
        public string key { get; set; }
        public List<MESSAGE> list { get; set; }
    }

    public class GETMESSAGE
    {
        public GETMESSAGE(string key, MESSAGE list)
        {
            this.key = key;
            this.list = list;
        }
        public string key { get; set; }
        public MESSAGE list { get; set; }
    }
}