using Cosmos.Constant;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Cosmos.Helper
{
    public class ProxyHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProxyHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient ClientFactory(Dictionary<string, string> headers)
        {
            var httpClient = new HttpClient();

            var transactionId = _httpContextAccessor.HttpContext.Request.Headers["TransactionId"].FirstOrDefault();
            httpClient.DefaultRequestHeaders.Add("TransactionId", transactionId ?? Guid.NewGuid().ToString());

            if (headers != null)
            {
                foreach (var item in headers)
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }

            return httpClient;
        }

        public T Get<T>(string url, Dictionary<string, string> headers = null)
        {
            using (var httpClient = ClientFactory(headers))
            {
                var json = httpClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
        public T Put<T>(string url, object data, Dictionary<string, string> headers = null)
        {
            using (var httpClient = ClientFactory(headers))
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, MediaType.Json);
                var json = httpClient.PutAsync(url, content).Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
        public T Post<T>(string url, object data, Dictionary<string, string> headers = null)
        {
            using (var httpClient = ClientFactory(headers))
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, MediaType.Json);
                var json = httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
        public T Delete<T>(string url, Dictionary<string, string> headers = null)
        {
            using (var httpClient = ClientFactory(headers))
            {
                var json = httpClient.DeleteAsync(url).Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}