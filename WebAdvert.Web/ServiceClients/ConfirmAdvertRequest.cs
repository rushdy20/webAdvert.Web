﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;

namespace WebAdvert.Web.ServiceClients
{
    public class ConfirmAdvertRequest
    {
        public string Id { get; set; }

        public AdvertStatus Status { get; set; }
    }
}
