namespace AuthSample.Models
{
    public class Session
    {
        public int Id { get; set; }
        public System.DateTime InsertedAt { get; set; }
        public System.DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string SessionId { get; set; }
    }
}