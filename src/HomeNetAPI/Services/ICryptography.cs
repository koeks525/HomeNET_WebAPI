

namespace HomeNetAPI.Services
{
    public interface ICryptography
    {
        byte[] GenerateSalt();
        string GenerateHashedString(byte[] salt, string password);
    }
}
