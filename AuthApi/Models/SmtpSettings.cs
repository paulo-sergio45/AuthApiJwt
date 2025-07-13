namespace AuthApi.Models
{
    public class SmtpSettings
    { 

        public required string Host { get; set; }
        public required string Port { get; set; }
        public required string ClientId { get; set; }
        public required string SmtpToken { get; set; }
    }
}
