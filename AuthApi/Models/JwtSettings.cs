namespace AuthApi.Models
{
    public class JwtSettings
    {

        public required string Jwt { get; set; }
        public required string Key { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required int ExpiresInDays { get; set; }
    }
}
