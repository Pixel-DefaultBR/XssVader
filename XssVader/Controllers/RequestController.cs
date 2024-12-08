using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
            SetDefaultRequestHeaders();
            return await GetResponseBody(url);
        }

        public void SetDefaultRequestHeaders()
        {
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            _client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            _client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
            _client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            _client.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
        }
        private async Task<string> GetResponseBody(string url)
        {
            using (HttpResponseMessage response = await _client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> GetHeader()
        {
            using (HttpResponseMessage response = await _client.GetAsync(_url))
            {
                return FormatHeaders(response.Headers);
            }
        }

        private string FormatHeaders(HttpResponseHeaders headers)
        {
            var headerString = new StringBuilder();

            foreach (var header in headers)
            {
                headerString.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            return headerString.ToString();
        }
    }
}
