using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Plugin.Connectivity;
using TrackingApp.Classes;

namespace TrackingApp.Services
{
    public class ApiService
    {
        #region Services
        DialogService dialogService;
        NavigationService navigationService;
        #endregion

        #region Constructor
        public ApiService()
        {
            dialogService = new DialogService();
            navigationService = new NavigationService();
        }
        #endregion

        public async Task<bool> CheckConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                dialogService.ShortToast("Active su conexión a internet.");
                return false;
            }
            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable)
            {
                dialogService.ShortToast("Sin conexión a internet.");
                return false;
            }
            return true;
        }

        public async Task<Response> Autentication<T>(string urlBase, string servicePrefix, string controller)
        {
            try
            {
                if (await CheckConnection())
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri(urlBase);
                    var url = string.Format("{0}{1}", servicePrefix, controller);
                    var response = await client.GetAsync(urlBase + url);

                    if (!response.IsSuccessStatusCode || response.StatusCode.ToString() == "BadRequest")
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    var validate = JsonConvert.DeserializeObject<T>(result);
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Ok",
                        Result = validate,
                    };
                }
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
            }
            return new Response
            {
                IsSuccess = false,
            };
        }

        public async Task<Response> GetOne<T>(string urlBase, string servicePrefix, string controller)
        {
            if (await CheckConnection())
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.GetAsync(urlBase + url);

                if (!response.IsSuccessStatusCode || response.StatusCode.ToString() == "BadRequest")
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }

                var result = await response.Content.ReadAsStringAsync();
                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.DateFormatString = "dd/MM/yyyy";
                object resultGet = null;
                try
                {
                    resultGet = JsonConvert.DeserializeObject<T>(result, jsonSerializerSettings);
                }catch(Exception e){
                    return new Response { IsSuccess = false };
                }

                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = resultGet,
                };
            }
            return new Response { IsSuccess = false };
        }

        public async Task<Response> Get<T>(string urlBase, string servicePrefix, string controller)
        {
            if (await CheckConnection())
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.GetAsync(urlBase + url);

                if (!response.IsSuccessStatusCode || response.StatusCode.ToString() == "BadRequest")
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }

                var result = await response.Content.ReadAsStringAsync();
                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.DateFormatString = "dd/MM/yyyy";
                var list = JsonConvert.DeserializeObject<List<T>>(result, jsonSerializerSettings);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = list,
                };
            }
            return new Response { IsSuccess = false };
        }

        public async Task<Response> Post<T>(string urlBase, string servicePrefix, string controller, T model)
        {
            if (await CheckConnection())
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");

                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.PostAsync(urlBase + url, content);

                if (!response.IsSuccessStatusCode || response.StatusCode.ToString() == "BadRequest")
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }

                var result = await response.Content.ReadAsStringAsync();
                var newRecord = JsonConvert.DeserializeObject<T>(result);

                return new Response
                {
                    IsSuccess = true,
                    Message = "Transacción procesada con éxito.",
                    Result = newRecord,
                };
            }
            return new Response { IsSuccess = false };
        }

        public async Task<Response> Put<T>(string urlBase, string servicePrefix, string controller, T model)
        {
            if (await CheckConnection())
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.PutAsync(urlBase + url, content);

                if (!response.IsSuccessStatusCode || response.StatusCode.ToString() == "BadRequest")
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }
                return new Response
                {
                    IsSuccess = true,
                    Message = response.Content.ReadAsStringAsync().Result
                };
            }
            return new Response { IsSuccess = false };
        }

        public async Task<Response> Delete(string urlBase, string servicePrefix, string controller)
        {
            if (await CheckConnection())
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.DeleteAsync(urlBase + url);

                if (!response.IsSuccessStatusCode || response.StatusCode.ToString() == "BadRequest")
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }
                return new Response
                {
                    IsSuccess = true,
                    Message = "Transacción procesada con éxito.",
                };
            }
            return new Response { IsSuccess = false };
        }

    }
}
