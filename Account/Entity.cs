namespace Entity
{
    public class Account
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public class MESSAGE
    {
        public int Id { get; set; }
        public string from_user { get; set; }
        public string to_user { get; set; }
        public string text { get; set; }
        public string date { get; set; }
        public int type { get; set; }
        public int status { get; set; }
    }

    public class Group
    {
        public string Id { get; set; }
        public string namegroup { get; set; }
        public int status { get; set; }
    }
    public class FriendChat
    {
        public int Id { get; set; }
        public string Email { set; get; }
        public string Username { set; get; }
        public string Phone { set; get; }
        public int Status { set; get; }
    }
}