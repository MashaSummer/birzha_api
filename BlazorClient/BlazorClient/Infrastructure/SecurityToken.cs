namespace BlazorClient.Infrastructure
{
    public class SecurityToken
    {
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
