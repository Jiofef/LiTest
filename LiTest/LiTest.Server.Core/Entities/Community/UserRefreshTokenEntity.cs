namespace LiTest.Server.Services
{
    public class UserRefreshTokenEntity
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public Guid UserId { get; set; }
    }
}
