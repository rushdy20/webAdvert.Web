using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;
using WebAdvert.Web.Models.Adverts;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiProfileMapper: Profile
    {
        public AdvertApiProfileMapper()
        {
            CreateMap<AdvertModel, CreateAdvertModel>().ReverseMap();
            CreateMap<CreateAdvertResponse, AdvertResponse>().ReverseMap();
            CreateMap<ConfirmAdvertModel, ConfirmAdvertRequest>().ReverseMap();
            CreateMap<CreateAdvertViewModel, CreateAdvertModel>().ReverseMap();

        }
    }
}
