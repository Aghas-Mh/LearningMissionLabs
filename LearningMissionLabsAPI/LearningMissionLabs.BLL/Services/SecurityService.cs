using LearningMissionLabs.BLL.Interfaces;
using LearningMissionLabs.DAL;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using LearningMissionLabs.BLL.Models;

namespace LearningMissionLabs.BLL.Services
{
    public class SecurityService : ISecurityService
    {
        public static string publicRsaKey = null;
        private static string privateRsaKey = null;
        private readonly ILearningMissionContext _dbContext;
        public static List<ClientInfo> _clientsInfo = new List<ClientInfo>();

        public SecurityService(ILearningMissionContext learningMissionContext)
        {
            _dbContext = learningMissionContext;
        }

        public async Task SetConnection(string id, string ip, int port, string publickey)
        {
            var user = _clientsInfo.Where(user => user.Id == id).FirstOrDefault();
            if (user == null)
            {
                _clientsInfo.Add(new ClientInfo { 
                    Id = id, 
                    IP = ip, 
                    Port = port, 
                    publicKey = publickey
                });
                return;
            }
            user.publicKey = publickey;
        }

        public async Task<ClientInfo> getClient(string id, string ip, int port)
        {
            return _clientsInfo.Where(user => user.Id == id && user.IP == ip && user.Port == port).FirstOrDefault()!;
        }

        public async Task<string> getClientKey(string id, string ip, int port)
        {
            var user = _clientsInfo.Where(user => user.Id == id && user.IP == ip && user.Port == port).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            return user.publicKey;
        }

        public async Task<string> GetPublicKey()
        {
            if (publicRsaKey != null) return publicRsaKey;
            privateRsaKey = await _dbContext.GetRSAKey();
            if (privateRsaKey == null)
            {
                await CreateRsaKey();
                await _dbContext.CreateRSA(privateRsaKey);
            }
            else
            {
                await ReadRsaKey();
            }
            return publicRsaKey;
        }

        private async Task CreateRsaKey()
        {
            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();
            UpdateKeys(keyPair);
        }

        private async Task ReadRsaKey()
        {
            StringReader privateKeyStringReader = new StringReader(privateRsaKey);
            PemReader pemReader = new PemReader(privateKeyStringReader);
            AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
            UpdateKeys(keyPair);
        }

        private void UpdateKeys(AsymmetricCipherKeyPair keyPair)
        {
            using (StringWriter keyStringWriter = new StringWriter())
            {
                PemWriter pemWriter = new PemWriter(keyStringWriter);
                pemWriter.WriteObject(keyPair.Private);
                privateRsaKey = keyStringWriter.ToString();
            }
            using (StringWriter keyStringWriter = new StringWriter())
            {
                PemWriter pemWriter = new PemWriter(keyStringWriter);
                pemWriter.WriteObject(keyPair.Public);
                publicRsaKey = keyStringWriter.ToString();
            }
        }

        public async Task<string> Encrypt(string message)
        {
            if (publicRsaKey == null) await GetPublicKey();
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicRsaKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] encryptedBytes = rsa.Encrypt(messageBytes, RSAEncryptionPadding.OaepSHA512);
            string encryptedText = Convert.ToBase64String(encryptedBytes);
            return encryptedText;
        }

        public async Task<string> Encrypt(string message, string publicKey)
        {
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] encryptedBytes = rsa.Encrypt(messageBytes, RSAEncryptionPadding.OaepSHA512);
            string encryptedText = Convert.ToBase64String(encryptedBytes);
            return encryptedText;
        }

        public async Task<string> Decrypt(string message)
        {
            if (privateRsaKey == null) await GetPublicKey();
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateRsaKey);
            byte[] encryptedBytes = Convert.FromBase64String(message);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA512);
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            return decryptedText;
        }
    }
}
