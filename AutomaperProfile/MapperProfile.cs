using AdvanAPI.Data.OBT;
using AdvanAPI.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.AutomaperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterModel, Account>().ReverseMap();
            //CreateMap<PaymentAdvice, PaymentAdvice>().ReverseMap();
            //CreateMap<CustomerInfo, Customer>().ReverseMap();

        }
    }
}
