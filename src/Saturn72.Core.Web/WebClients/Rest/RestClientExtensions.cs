using System.Collections.Generic;
using System.Linq;
using Saturn72.Extensions;

namespace Saturn72.Core.Net.Rest
{
    public static class RestClientExtensions
    {
        public static RestResponse SubmitDeleteRequest(this IRestClient restClient, string resource, object id,
            IDictionary<string, string> headerParameters = null)
        {
            var queryParameters = new[] {new {Key = "id", Value = id}}.ToDictionary(x => x.Key, x => x.Value);

            return restClient.SubmitRequest(resource, Method.Delete, queryParameters: queryParameters, headerParameters: headerParameters);
        }

        public static RestResponse SubmitPostRequest(this IRestClient restClient, string resource, object tObject,
            IDictionary<string, string> headerParameters = null)
        {
            return restClient.SubmitRequest(resource, Method.Post, tObject, headerParameters: headerParameters);
        }

        public static RestResponse SubmitGetRequest(this IRestClient restClient, string resource, object tObject)
        {
            return restClient.SubmitRequest(resource, Method.Get, tObject);
        }

        public static IEnumerable<TObject> SubmitPostRequest<TObject>(this IRestClient restClient, string resource,
            object tObject = null) where TObject : new()
        {
            return restClient.SubmitRequest<TObject>(resource, Method.Post, tObject);
        }

        public static IEnumerable<TObject> SubmitGetRequest<TObject>(this IRestClient restClient, string resource,
            object tObject = null) where TObject : new()
        {
            var requestParameters = ToRequestParams(tObject);
            return restClient.SubmitRequest<TObject>(resource, Method.Get, null, requestParameters);
        }

        private static Dictionary<string, object> ToRequestParams(object tObject)
        {
            if (tObject.IsNull())
                return null;

            var type = tObject.GetType();
            return type.GetProperties().ToDictionary(x => x.Name, x => x.GetValue(tObject, null));
        }
    }
}