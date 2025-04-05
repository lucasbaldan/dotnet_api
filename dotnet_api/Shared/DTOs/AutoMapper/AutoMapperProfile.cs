using AutoMapper;
using dotnet_api.Models;
using dotnet_api.Shared.DTOs;

namespace dotnet_api.Shared.DTOs.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Produto, ProdutoDTO>().ReverseMap();
        CreateMap<Pessoa, PessoaDTO>().ReverseMap();
    }
}
