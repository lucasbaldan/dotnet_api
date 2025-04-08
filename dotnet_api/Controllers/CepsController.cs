using AutoMapper;
using Asp.Versioning;
using dotnet_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using dotnet_api.Shared.DTOs;
using dotnet_api.Services;

namespace dotnet_api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    //[Authorize]
    [EnableRateLimiting("fixed")]
    public class CepsController(IViaCepService viaCepService) : ControllerBase
    {
        private readonly IViaCepService _viaCepService = viaCepService;

        [HttpGet("{cep:int:min(1)}")]
        public async Task<ActionResult<ViaCepResponseDTO>> Get(string cep)
        {

            if (cep.Length != 8) throw new ArgumentException("O CEP a ser consultado deve conter 8 dígitos.");

            var consultaApi = await _viaCepService.GetEnderecoByCEP(cep);

            return Ok(consultaApi);
        }
    }
}
