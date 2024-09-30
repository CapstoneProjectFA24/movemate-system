using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MoveMate.Service.Helper.ZaloPayHelper.Crypto
{
    public class RSAHelper
    {
        public static string Encrypt(string data, string publicKey)
        {
            byte[] publicKeyBytes = Convert.FromBase64String(publicKey);
            AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(publicKeyBytes);
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;

            RSAParameters rsaParameters = new RSAParameters
            {
                Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned(),
                Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned()
            };

            //You can then easily import the key parameters into RSACryptoServiceProvider:
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParameters);
            
            //Finally, do your encryption:
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(data);
            // Sign data with Pkcs1
            byte[] encryptedData = rsa.Encrypt(dataToEncrypt, false);
            // Convert Bytes to Hash
            var hash = Convert.ToBase64String(encryptedData);

            return hash;
        }
        public static string EncryptV1(string data, string publicKey)
        {
            string hash = "";
            try
            {
                byte[] keys = Convert.FromBase64String(publicKey);
                X509Certificate2 cert = new X509Certificate2(keys);
                hash = Encrypt(data, cert);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return hash;
        }
        public static string Encrypt(string plainText, X509Certificate2 cert)
        {
#pragma warning disable SYSLIB0027 // Type or member is obsolete
            RSACryptoServiceProvider publicKey = (RSACryptoServiceProvider)cert.PublicKey.Key;
#pragma warning restore SYSLIB0027 // Type or member is obsolete
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = publicKey.Encrypt(plainBytes, false);
            string encryptedText = Convert.ToBase64String(encryptedBytes);
            return encryptedText;
        }

        public static string Decrypt(string encryptedText, X509Certificate2 cert)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable SYSLIB0028 // Type or member is obsolete
            RSACryptoServiceProvider privateKey = (RSACryptoServiceProvider)cert.PrivateKey;
#pragma warning restore SYSLIB0028 // Type or member is obsolete
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            byte[] decryptedBytes = privateKey.Decrypt(encryptedBytes, false);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            return decryptedText;
        }
    }
}