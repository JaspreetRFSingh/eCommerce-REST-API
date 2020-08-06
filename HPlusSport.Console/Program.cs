using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HPlusSport.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();

            var discoveryDocument = await client.GetDiscoveryDocumentAsync(
                "http://localhost:61884");

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "client",
                    ClientSecret = "H+ Sport",
                    Scope = "hps-api"
                });
            System.Console.WriteLine($"Token: {tokenResponse.AccessToken}");
        }
    }
}
