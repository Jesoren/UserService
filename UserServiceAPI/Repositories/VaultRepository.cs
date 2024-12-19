using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;

namespace UserService.Repositories
{
    public class VaultRepository
    {
        //private readonly string EndPoint = "https://localhost:8201/";
        private readonly string Token;
        private IVaultClient _vaultClient;
        public VaultRepository(string endPoint, string token)
        {
            var EndPoint = endPoint;
            Token = token; 

            // Ignorer SSL-certifikatfejl (ikke anbefalet i produktion)
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };

            // Konfigurer Vault-klienten
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(Token);
            var vaultClientSettings = new VaultClientSettings(EndPoint, authMethod)
            {
                MyHttpClientProviderFunc = handler => new HttpClient(httpClientHandler)
                {
                    BaseAddress = new Uri(EndPoint)
                }
            };

            _vaultClient = new VaultClient(vaultClientSettings);
        }
        public async Task<string> GetSecretAsync(string secret)
        {

            try
            {
                // Hent hemmeligheden fra Vault
                Secret<SecretData> kv2Secret = await _vaultClient.V1.Secrets.KeyValue.V2
                    .ReadSecretAsync(path: "Hemmeligheder", mountPoint: "secret");
                var value = kv2Secret.Data.Data[secret].ToString();

                return value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved hentning af ConnectionString: {ex.Message}");
                throw;
            }
        }
    }
}
