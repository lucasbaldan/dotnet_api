using AutoMapper;
using dotnet_api.Models;

namespace dotnet_api.DTOs.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Produto, ProdutoDTO>().ReverseMap();
    }
}
