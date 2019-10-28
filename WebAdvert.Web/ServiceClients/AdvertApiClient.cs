using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient: IAdvertApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

        public AdvertApiClient(IConfiguration configuration, HttpClient client,IMapper mapper)
        {
            _configuration = configuration;
            _client = client;
            _mapper = mapper;

            var apiConfig = _configuration.GetSection(("AdvertApi"));
            var createUrl = apiConfig.GetValue<string>("BaseUrl");
            _client.BaseAddress= new Uri(createUrl);
            
              }
        public async Task<AdvertResponse> Create(CreateAdvertModel model)
        {
            try
            {
                var advertApiModel = _mapper.Map<AdvertModel>(model);
                var jsonmodel = JsonConvert.SerializeObject(advertApiModel);
                var response = await _client.PostAsync(new Uri($"{_client.BaseAddress}/Create"),
                    new StringContent(jsonmodel, Encoding.UTF8, "application/json")).ConfigureAwait(false);

                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var createAdvertResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(responseJson);
                var advertResponse = _mapper.Map<AdvertResponse>(createAdvertResponse);

                return advertResponse;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           

        }

        public async Task<bool> Confirm(ConfirmAdvertRequest model)
        {
            var advertModel = _mapper.Map<ConfirmAdvertModel>(model);
            var jsonmodel = JsonConvert.SerializeObject(advertModel);
            var response = await _client.PutAsync( new Uri($"{_client.BaseAddress}/Confirm"),
                new StringContent(jsonmodel, Encoding.UTF8, "application/json")).ConfigureAwait(false);


            return response.StatusCode == HttpStatusCode.OK;

        }
    }
}
