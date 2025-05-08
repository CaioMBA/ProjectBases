using Domain.Enums;
using Domain.Extensions;
using Domain.Models.ApplicationConfigurationModels.ApiDefaultModels;
using Org.BouncyCastle.Security;
using System.Text;
using System.Web;

namespace Data.Api
{
    public class DefaultApiAccess
    {
        public static async Task<HttpResponseMessage> RestApiRequest(RestApiRequestModel ApiRequest)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpMethod Method = ApiRequest.Method switch
                {
                    ApiRequestMethod.GET => HttpMethod.Get,
                    ApiRequestMethod.POST => HttpMethod.Post,
                    ApiRequestMethod.PUT => HttpMethod.Put,
                    ApiRequestMethod.DELETE => HttpMethod.Delete,
                    ApiRequestMethod.HEAD => HttpMethod.Head,
                    ApiRequestMethod.OPTIONS => HttpMethod.Options,
                    ApiRequestMethod.TRACE => HttpMethod.Trace,
                    ApiRequestMethod.PATCH => HttpMethod.Patch,
                    _ => throw new InvalidKeyException($"{ApiRequest.Method.ToString()} is not a valid Api Type"),
                };

                var uriBuilder = new UriBuilder(ApiRequest.Url);

                if (ApiRequest.QueryParameters != null && ApiRequest.QueryParameters.Any())
                {
                    var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                    foreach (var item in ApiRequest.QueryParameters)
                    {
                        query[item.Key] = item.Value;
                    }
                    uriBuilder.Query = query.ToString();
                }

                var request = new HttpRequestMessage(Method, uriBuilder.ToString());



                if (ApiRequest.TimeOut != null)
                {
                    httpClient.Timeout = TimeSpan.FromSeconds((double)ApiRequest.TimeOut);
                }
                if (ApiRequest.Authentication != null)
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(ApiRequest.Authentication.Type.ToString(), ApiRequest.Authentication.Authorization);
                }
                if (ApiRequest.Headers != null && ApiRequest.Headers.Any())
                {
                    foreach (var header in ApiRequest.Headers!)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                if (!String.IsNullOrEmpty(ApiRequest.Body))
                {
                    request.Content = new StringContent(ApiRequest.Body, Encoding.UTF8, "application/json");
                }

                try
                {
                    HttpResponseMessage? response = await httpClient.SendAsync(request);
                    return response;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<GraphQlApiResponseModel> GraphQlApiRequest(GraphQlApiRequestModel GraphQlRequest)
        {
            RestApiRequestModel request = new()
            {
                Url = GraphQlRequest.Url,
                Method = ApiRequestMethod.POST,
                Authentication = GraphQlRequest.Authentication,
                Headers = null,
                Body = new
                {
                    query = GraphQlRequest.Query,
                    variables = GraphQlRequest.Variables ?? new Dictionary<string, object?>()
                }.ToJson(),
                TimeOut = 120
            };

            HttpResponseMessage response = await RestApiRequest(request);

            string content = await response.Content.ReadAsStringAsync();

            GraphQlApiResponseModel? returnObj = content.ToObject<GraphQlApiResponseModel>();
            if (returnObj == null)
            {
                returnObj = new GraphQlApiResponseModel();
            }
            returnObj.StatusCode = (int)response.StatusCode;
            return returnObj;
        }
    }
}
