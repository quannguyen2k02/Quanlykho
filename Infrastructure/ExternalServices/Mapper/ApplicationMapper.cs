
using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.ExternalServices.Mapper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            //CreateMap<LedResult, LedResultRequest>();
            CreateMap<OrderRequest, Order>();
            CreateMap<Order, OrderResponse>();
            CreateMap<SlipRequest, ExportSlip>();
            CreateMap<ExportSlip, SlipResponse>();
        }
    }
}
