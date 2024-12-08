using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XssVader.Models;

namespace XssVader.Controllers
{
    internal class RequestController
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _url;

        public RequestController(string url)
        {
            _url = url;
        }
        public async Task<string> RequestHandle(string url)
        {
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            using (HttpResponseMessage response = await _client.GetAsync(url))

            {
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
        public async Task<string> GetHeader()
        {
            using (HttpResponseMessage response = await _client.GetAsync(_url))
            {
                var headers = response.Headers;
                var headerString = new StringBuilder();

                foreach (var header in headers)
                {
                    headerString.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }

                return headerString.ToString();
            }
        }
    }
}
