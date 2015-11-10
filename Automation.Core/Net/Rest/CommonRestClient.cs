using System;
using System.Collections.Generic;
using Automation.Extensions;
using RestSharp;
using RestSharp.Deserializers;

namespace Automation.Core.Net.Rest
{
    public class CommonRestClient : IRestClient
    {
        private readonly Uri _serverUri;

        public CommonRestClient(string serverUri)
        {
            _serverUri = new Uri(serverUri);
        }

        public virtual Uri ServerUri
        {
            get { return _serverUri; }
        }

        public IEnumerable<TObject> SubmitRequest<TObject>(string resource, Method method, object requestData = null, IDictionary<string, object> queryParameters = null, IDictionary<string, string> headerParameters = null) where TObject : new()
        {
            Guard.NotEmpty(resource, "resource");
            var request = BuildRequest(resource, method, requestData, queryParameters, headerParameters);
            var response = GetRestClient().Execute(request);

            var jsonDeserializer = new JsonDeserializer();
            return jsonDeserializer.Deserialize<List<TObject>>(response);
        }

        public RestResponse SubmitRequest(string resource, Method method, object requestData = null, IDictionary<string, object> queryParameters = null, IDictionary<string, string> headerParameters = null)
        {
            Guard.NotEmpty(resource, "resource");
            var request = BuildRequest(resource, method, requestData, queryParameters, headerParameters);
            var rowResponse = GetRestClient().Execute(request);

            return ToRestResponse(rowResponse);
        }

        private RestRequest BuildRequest(string resource, Method method, object requestObject, IDictionary<string, object> queryParameters, IDictionary<string, string> headerParameters)
        {
            var request = new RestRequest
            {
                Resource = resource,
                Method = ToRestSharpMethod(method),
                RequestFormat = DataFormat.Json,
                Timeout = 300000,
            };

            if (requestObject.NotNull())
                request.AddBody(requestObject);

            if (queryParameters.NotEmpty())
                queryParameters.ForEachItem(x => request.AddParameter(x.Key, x.Value, ParameterType.QueryString));

            if(headerParameters.NotEmpty())
                headerParameters.ForEachItem(x=>request.AddHeader(x.Key,x.Value));
            return request;
        }

        private RestSharp.Method ToRestSharpMethod(Method method)
        {
            switch (method)
            {
                case Method.Get:
                    return RestSharp.Method.GET;
                case Method.Post:
                    return RestSharp.Method.POST;
                case Method.Delete:
                    return RestSharp.Method.DELETE;
                case Method.Put:
                    return RestSharp.Method.PUT;
            }
            return RestSharp.Method.GET;
        }

        protected virtual RestResponse ToRestResponse(IRestResponse restSharpResponse)
        {
            return new RestResponse
            {
                Content = restSharpResponse.Content,
                ContentEncoding = restSharpResponse.ContentEncoding,
                ContentLength = restSharpResponse.ContentLength,
                ContentType = restSharpResponse.ContentType
            };
        }

        private RestSharp.IRestClient GetRestClient()
        {
            return new RestClient(ServerUri);
        }
    }
}