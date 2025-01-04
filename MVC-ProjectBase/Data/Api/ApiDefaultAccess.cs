using Domain.Extensions;
using Domain.Models.ApplicationModels;
using System.Text;

namespace Data.Api
{
    public class ApiDefaultAccess
    {
        public async Task<HttpResponseMessage> RestApiRequest(RestApiRequestModel ApiRequest)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpMethod Method = ApiRequest.TypeRequest.ToUpper().Trim() switch
                {
                    "GET" => HttpMethod.Get,
                    "POST" => HttpMethod.Post,
                    "PUT" => HttpMethod.Put,
                    "DELETE" => HttpMethod.Delete,
                    "HEAD" => HttpMethod.Head,
                    "OPTIONS" => HttpMethod.Options,
                    "TRACE" => HttpMethod.Trace,
                    "PATCH" => HttpMethod.Patch,
                    _ => throw new Exception("Nenhum Método Aceito"),
                };
                var request = new HttpRequestMessage(Method, ApiRequest.Url);



                if (ApiRequest.TimeOut != null)
                {
                    httpClient.Timeout = TimeSpan.FromSeconds((double)ApiRequest.TimeOut);
                }
                if (ApiRequest.Auth != null)
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(ApiRequest.Auth.Type, ApiRequest.Auth.Authorization);
                }
                if (ApiRequest.Headers != null)
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
            RestApiRequestModel request = new RestApiRequestModel()
            {
                Url = GraphQlRequest.Url,
                TypeRequest = "POST",
                Auth = GraphQlRequest.Auth,
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

            returnObj.StatusCode = (int)response.StatusCode;

            return returnObj;
        }
    }
}
