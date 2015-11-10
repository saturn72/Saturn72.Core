using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Automation.Core.Net
{
    public abstract class HttpClientBase : HttpClient
    {
        protected abstract string HttpServiceAddress { get; }

        public async Task<HttpContent> HttpRequest(string requestString)
        {
            return await HttpRequestSubmitter(requestString);
        }

        public async Task HttpRequest(string requestString, Action<HttpContent> callback)
        {
            var content = await HttpRequestSubmitter(requestString);
            callback(content);
        }

        public async Task HttpRequest(string requestString, Action callback)
        {
            await HttpRequestSubmitter(requestString);
            callback();
        }

        public async Task<TResult> HttpRequest<TResult>(string requestString, Func<HttpContent, TResult> callback)
        {
            var content = await HttpRequestSubmitter(requestString);
            return callback(content);
        }

        #region Utilities

        private async Task<HttpContent> HttpRequestSubmitter(string requestString)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.GetAsync(requestString);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
                return response.Content;
            }
        }

        #endregion

        protected virtual HttpClient CreateHttpClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(HttpServiceAddress)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}