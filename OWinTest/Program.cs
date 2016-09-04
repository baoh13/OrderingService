using System;
using System.Collections.Generic;
using System.Net.Http;

namespace OWinTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = "http://localhost:63715";
            var token = new Token();
            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string>
                {
                    {"grant_type", "password"},
                    {"username", "jignesh"},
                    {"password", "user123456"},
                };

                var tokenResponse = client.PostAsync(baseAddress + "/oauth/token", new FormUrlEncodedContent(form))
                                          .Result;

                token = tokenResponse.Content.ReadAsAsync<Token>().Result;
            }

            // Next Request 
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseAddress);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
                var response = httpClient.GetAsync("/api/orders").Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success");
                }
                var message = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("URL responese : " + message);
            }

            Console.Read();
        }
    }
}
