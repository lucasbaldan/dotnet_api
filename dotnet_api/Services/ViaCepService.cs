using dotnet_api.Shared.DTOs;
using System.Text.Json;

namespace dotnet_api.Services;

public class ViaCepService : IViaCepService
{
    private readonly JsonSerializerOptions _options;
    private readonly IHttpClientFactory _clientFactory;

    public ViaCepService(IHttpClientFactory clientFactory)
    {
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _clientFactory = clientFactory;
    }

    public async Task<ViaCepResponseDTO> GetEnderecoByCEP(string cep)
    {
        var client = _clientFactory.CreateClient("ViaCep");
        using var response = await client.GetAsync($"{cep}/json");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var retorno = JsonSerializer.Deserialize<ViaCepResponseDTO>(result, _options);

            return retorno ?? throw new Exception("Erro ao processar informações do CEP desejado");
        }
        else
        {
            throw new Exception("Erro ao consultar o CEP");
        }
    }

    public Task<ViaCepResponseDTO> SaveAsync(ViaCepResponseDTO endereco)
    {
        throw new NotImplementedException();
    }
}

public interface IViaCepService
{
    Task<ViaCepResponseDTO> GetEnderecoByCEP(string cep);

    Task<ViaCepResponseDTO> SaveAsync(ViaCepResponseDTO endereco);
}
