using System.Security.Cryptography;
using System.Text;

namespace WoodenWorkshop.Common.Hashing;

public class HashingUtility : IDisposable
{
    private readonly HashAlgorithm _hashAlgorithm = SHA256.Create();

    public string ComputeHash(string source)
    {
        var hashBytes = _hashAlgorithm.ComputeHash(
            Encoding.UTF8.GetBytes(source)
        );
        return Convert.ToBase64String(hashBytes);
    }

    public void Dispose()
    {
        _hashAlgorithm.Dispose();
    }
}