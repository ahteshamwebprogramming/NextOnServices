using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Helper;

public class Encryption
{
    public static string Encrypt(string input, string key)
    {
        byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
        TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
        tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
        tripleDES.Mode = CipherMode.ECB;
        tripleDES.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = tripleDES.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
        tripleDES.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
    public static string Decrypt(string input, string key)
    {
        byte[] inputArray = Convert.FromBase64String(input);
        TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
        tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
        tripleDES.Mode = CipherMode.ECB;
        tripleDES.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = tripleDES.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
        tripleDES.Clear();
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
    public static string HashSHA3(string URL, string SecretKey)
    {
        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(256);
        byte[] input = Encoding.ASCII.GetBytes(URL + SecretKey);
        hashAlgorithm.BlockUpdate(input, 0, input.Length);
        byte[] result = new byte[32]; // 256 / 8 = 32
        hashAlgorithm.DoFinal(result, 0);
        string hashString = BitConverter.ToString(result);
        hashString = hashString.Replace("-", "").ToLowerInvariant();
        return hashString;
        //var sha3_256 = HashFactory.Crypto.SHA3.CreateKeccak256();
        //var tempHash = sha3_256.ComputeString(URL);
        //return tempHash.ToString().ToLower().Replace("-", "");
    }
    public static string HashSHA1(string URL, string SecretKey)
    {
        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Sha1Digest();
        byte[] input = Encoding.ASCII.GetBytes(URL + SecretKey);
        hashAlgorithm.BlockUpdate(input, 0, input.Length);
        byte[] result = new byte[32]; // 256 / 8 = 32
        hashAlgorithm.DoFinal(result, 0);
        string hashString = BitConverter.ToString(result);
        hashString = hashString.Replace("-", "").ToLowerInvariant();
        return hashString;
        //var sha3_256 = HashFactory.Crypto.SHA3.CreateKeccak256();
        //var tempHash = sha3_256.ComputeString(URL);
        //return tempHash.ToString().ToLower().Replace("-", "");
    }
    public static string HashSHA1_C(string url, string secretKey)
    {
        //string hashedUrl = CalculateSHA1(url + secretKey);

        string input = url + secretKey;

        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha1.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
    public Encryption()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}
