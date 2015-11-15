using System.Collections.Generic;

namespace Saturn72.Core.Net.Rest
{
    public interface IRestClient
    {
        IEnumerable<TObject> SubmitRequest<TObject>(string resource, Method method, object requestData = null,
            IDictionary<string, object> queryParameters = null, IDictionary<string, string> headerParameters = null) where TObject : new();

        RestResponse SubmitRequest(string resource, Method method, object requestData = null, IDictionary<string, object> queryParameters = null, IDictionary<string, string> headerParameters = null);
    }
}