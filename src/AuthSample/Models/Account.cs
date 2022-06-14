namespace AuthSample.Models
{
    public class Account
    {
        public int Id { get; set; }
        public System.DateTime InsertedAt { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public System.DateTime? LastLogin { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public bool Blocked { get; set; }
    }
}

