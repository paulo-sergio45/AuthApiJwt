using AuthApi.Entities;

namespace AuthApi.Interfaces
{
    public interface ITokenService
    {
        public string GenerateTokenAccess(User user, IList<string> role);
    }
}
