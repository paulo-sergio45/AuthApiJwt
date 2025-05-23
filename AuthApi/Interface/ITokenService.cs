using AuthApi.Entities;

namespace AuthApi.Interface
{
    public interface ITokenService
    {
        public string GenerateTokenAcces(ApplicationUser user, IList<string> role);
    }
}
