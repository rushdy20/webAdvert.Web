using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AdvertApi.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient: IAdvertApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public AdvertApiClient(IConfiguration configuration, HttpClient client)
        {
            _configuration = configuration;
            _client = client;

            var apiConfig = _configuration.GetSection(("AdvertApi"));
            var createUrl = apiConfig.GetValue<string>("CreateUrl");
            _client.BaseAddress= new Uri(createUrl);
            _client.DefaultRequestHeaders.Add("Content-type","application/json");
        }
        public async Task<AdvertResponse> Create(AdvertModel model)
        {
            var advertApiModel = new AdvertModel(); //automappper to model 
            var jsonmodel = JsonConvert.SerializeObject(model);
            var response = await _client.PostAsync(_client.BaseAddress, new StringContent(jsonmodel))
                .ConfigureAwait(false);
            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var createAdvertResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(responseJson);
            var advertResponse = new AdvertResponse();//automapper

            return advertResponse;

        }
    }
}
