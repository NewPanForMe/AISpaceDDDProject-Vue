using System.Security.Cryptography;
using System.Text;

namespace DDDProject.Domain.Helpers;

/// <summary>
/// 密码帮助类
/// </summary>
public static class PasswordHelper
{
    // AES 加密密钥（必须与前端一致）
    private const string AES_SECRET_KEY = "DDDProject2024SecretKey!";
    private const string AES_IV = "DDDProject2024IV!";

    /// <summary>
    /// 计算密码哈希（SHA256）
    /// </summary>
    /// <param name="password">密码</param>
    /// <returns>哈希后的密码</returns>
    public static string ComputeHash(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    /// <summary>
    /// 解密 AES 加密的密码 (兼容 CryptoJS)
    /// </summary>
    /// <param name="encryptedPassword">加密的密码（Base64 字符串）</param>
    /// <returns>解密后的密码</returns>
    public static string DecryptPassword(string encryptedPassword)
    {
        if (string.IsNullOrEmpty(encryptedPassword))
            return encryptedPassword;

        try
        {
            var encryptedBytes = Convert.FromBase64String(encryptedPassword);

            // CryptoJS 默认使用 OpenSSL 格式: Salted__ + 8字节salt + 密文
            // 检查是否是 OpenSSL 格式
            if (encryptedBytes.Length > 16 &&
                encryptedBytes[0] == 83 &&  // 'S'
                encryptedBytes[1] == 97 &&  // 'a'
                encryptedBytes[2] == 108 && // 'l'
                encryptedBytes[3] == 116 && // 't'
                encryptedBytes[4] == 101 && // 'e'
                encryptedBytes[5] == 100 && // 'd'
                encryptedBytes[6] == 95 &&  // '_'
                encryptedBytes[7] == 95)    // '_'
            {
                // OpenSSL 格式：Salted__ + 8字节salt + 密文
                var salt = new byte[8];
                Array.Copy(encryptedBytes, 8, salt, 0, 8);

                // 使用 OpenSSL 的 EVP_BytesToKey 算法派生密钥和 IV
                var keyAndIV = EvpBytesToKey(AES_SECRET_KEY, salt, 32, 16);
                var key = keyAndIV.Item1;
                var iv = keyAndIV.Item2;

                // 去除 salt (8字节) + header (8字节) 后的密文
                var cipher = new byte[encryptedBytes.Length - 16];
                Array.Copy(encryptedBytes, 16, cipher, 0, encryptedBytes.Length - 16);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                var decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                var decryptedPassword = Encoding.UTF8.GetString(decryptedBytes).TrimEnd('\0');

                // 调试日志
                System.Diagnostics.Debug.WriteLine($"=== OpenSSL 格式解密 ===");
                System.Diagnostics.Debug.WriteLine($"加密密码: {encryptedPassword}");
                System.Diagnostics.Debug.WriteLine($"Salt (HEX): {BitConverter.ToString(salt).Replace("-", "")}");
                System.Diagnostics.Debug.WriteLine($"密钥 (HEX): {BitConverter.ToString(key).Replace("-", "")}");
                System.Diagnostics.Debug.WriteLine($"IV (HEX): {BitConverter.ToString(iv).Replace("-", "")}");
                System.Diagnostics.Debug.WriteLine($"解密密码: {decryptedPassword}");

                return decryptedPassword;
            }
            else
            {
                // 非 OpenSSL 格式，使用与 CryptoJS 相同的方式处理密钥和 IV
                // CryptoJS.enc.Utf8.parse() 会直接将字符串转换为字节数组
                // .NET Aes 需要精确匹配密钥和 IV 长度
                
                // 密钥：直接使用 UTF-8 字节（24 字节）
                var key = Encoding.UTF8.GetBytes(AES_SECRET_KEY);
                
                // IV：截断到 16 字节（CryptoJS 也会使用前 16 字节）
                var ivBytes = Encoding.UTF8.GetBytes(AES_IV);
                var iv = new byte[16];
                Array.Copy(ivBytes, iv, 16);

                // 直接使用密文
                var cipher = encryptedBytes;

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                var decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                var decryptedPassword = Encoding.UTF8.GetString(decryptedBytes).TrimEnd('\0');

                // 调试日志
                System.Diagnostics.Debug.WriteLine($"=== 标准格式解密 ===");
                System.Diagnostics.Debug.WriteLine($"加密密码: {encryptedPassword}");
                System.Diagnostics.Debug.WriteLine($"密钥 (HEX): {BitConverter.ToString(key).Replace("-", "")}");
                System.Diagnostics.Debug.WriteLine($"IV (HEX): {BitConverter.ToString(iv).Replace("-", "")}");
                System.Diagnostics.Debug.WriteLine($"解密密码: {decryptedPassword}");

                return decryptedPassword;
            }
        }
        catch (Exception ex)
        {
            // 解密失败，返回原值（兼容非加密传输）
            // 注意：这里使用 Console.WriteLine 而不是 Debug.WriteLine，以便在测试时能看到
            Console.WriteLine($"=== 解密失败 ===");
            Console.WriteLine($"加密密码: {encryptedPassword}");
            Console.WriteLine($"异常类型: {ex.GetType().FullName}");
            Console.WriteLine($"异常消息: {ex.Message}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            return encryptedPassword;
        }
    }

    /// <summary>
    /// OpenSSL 的 EVP_BytesToKey 算法，用于派生密钥和 IV
    /// </summary>
    /// <param name="password">密码</param>
    /// <param name="salt">盐</param>
    /// <param name="keyLength">密钥长度（字节）</param>
    /// <param name="ivLength">IV 长度（字节）</param>
    /// <returns>密钥和 IV 的元组</returns>
    private static (byte[] key, byte[] iv) EvpBytesToKey(string password, byte[] salt, int keyLength, int ivLength)
    {
        using var md5 = MD5.Create();

        var totalBytes = keyLength + ivLength;
        var key = new byte[keyLength];
        var iv = new byte[ivLength];
        var hashBuffer = new byte[password.Length + salt.Length];
        
        int hashOffset = 0;
        var allHashBytes = new List<byte>();

        // 生成足够的字节：MD5(password + salt) + MD5(MD5(password + salt) + password + salt) + ...
        while (hashOffset < totalBytes)
        {
            // 复制之前的哈希
            if (hashOffset > 0)
            {
                Array.Copy(allHashBytes.ToArray(), hashBuffer, hashOffset);
            }
            
            // 复制密码
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            Array.Copy(passwordBytes, 0, hashBuffer, hashOffset, passwordBytes.Length);
            
            // 复制盐（只取前8字节）
            Array.Copy(salt, 0, hashBuffer, hashOffset + passwordBytes.Length, Math.Min(salt.Length, 8));

            // 计算 MD5
            var hash = md5.ComputeHash(hashBuffer);
            allHashBytes.AddRange(hash);
            
            hashOffset += hash.Length;
        }

        // 提取密钥和 IV
        allHashBytes.CopyTo(0, key, 0, keyLength);
        allHashBytes.CopyTo(keyLength, iv, 0, ivLength);

        return (key, iv);
    }
}
