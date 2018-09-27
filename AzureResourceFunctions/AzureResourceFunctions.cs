
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;

namespace AzureResourceFunctions
{
    public static class AzureResourceFunctions
    {
        [FunctionName("CheckAzureResourceProviders")]
        public static void Run([TimerTrigger("0 0 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            string token = GetAzureBearerToken();
            string resourceProviders = GetAzureResourceProviders(token);

            Services.ResourceRepository repo = new Services.ResourceRepository();

            Dtos.Resources resource = repo.GetLastResource();

            if (resource == null || resource.ResourcesJson != resourceProviders)
            {
                string diffs = "";

                if (resource != null)
                {
                    var jdp = new JsonDiffPatch();
                    var left = JToken.Parse(resource.ResourcesJson);
                    var right = JToken.Parse(resourceProviders);

                    JToken patch = jdp.Diff(left, right);
                    if (patch != null) diffs = patch.ToString();
                }

                Dtos.Resources res = new Dtos.Resources()
                {
                    ResourcesJson = resourceProviders,
                    Differences = diffs,
                    Timestamp = DateTime.Now
                };
                // We have resource changes changes
                repo.InsertResourceJson(res);
            }
            
            //return (new OkObjectResult(resourceProviders));
        }

        private static string GetAzureResourceProviders(string bearerToken)
        {
            string result = "";

            string subId = System.Environment.GetEnvironmentVariable("SubscriptionId");

            string Url = $"https://management.azure.com/subscriptions/{subId}/providers?api-version=2018-02-01";

            using (HttpClient client = new HttpClient())
            {
                string contentType = "application/json";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", bearerToken));

                //var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage msg = client.GetAsync(Url).Result;
                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = msg.Content.ReadAsStringAsync().Result;
                    result = JsonDataResponse;
                }
            }

            return result;
        }

        private static string GetAzureBearerToken()
        {
            string token = "";

            string clientId = System.Environment.GetEnvironmentVariable("SpClientId");
            string clientSecret = System.Environment.GetEnvironmentVariable("SpClientSecret");

            using (HttpClient client = new HttpClient())
            {
                string contentType = "application/x-www-form-urlencoded";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/oauth2/token"),
                    Content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("grant_type", "client_credentials"),
                            new KeyValuePair<string, string>("client_id", clientId),
                            new KeyValuePair<string, string>("client_secret", clientSecret),
                            new KeyValuePair<string, string>("resource", "https://management.azure.com/")
                        }),
                    Method = HttpMethod.Post
                };

                HttpResponseMessage msg = client.SendAsync(request).Result;
                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = msg.Content.ReadAsStringAsync().Result;
                    Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(JsonDataResponse);
                    if (obj != null)
                    {
                        if (obj["access_token"] != null)
                        {
                            token = obj["access_token"].ToString();
                        }
                    }
                }
            }

            return token;
        }
    }
}
