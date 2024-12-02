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
        private readonly HostModel _hostModel;
        private readonly HttpClient _client = new HttpClient();

        public RequestController()
        {
            _hostModel = new HostModel();
        }

        public async Task<string> RequestHandle(string url)
        {
            _hostModel.Url = url;

            using (HttpResponseMessage response = await _client.GetAsync(_hostModel.Url))
            {
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
    }
}
