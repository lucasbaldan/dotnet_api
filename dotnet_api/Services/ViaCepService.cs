using dotnet_api.Shared.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
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

            using var jsonDoc = JsonDocument.Parse(result);
            if (jsonDoc.RootElement.TryGetProperty("erro", out var erro))
            {
                throw new ArgumentException("CEP com formato inválido ao padrão nacional");
            }

            var retorno = JsonSerializer.Deserialize<ViaCepResponseDTO>(result, _options);

            return retorno ?? throw new Exception("Erro ao processar informações do CEP desejado");
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
            throw new KeyNotFoundException("CEP não encontrado na base nacional");

        else
            throw new Exception("Ocorreu um erro ao consultar o CEP desejado na base Nacional.");
    }

    public Task<ViaCepResponseDTO> GetAndSaveAsync(ViaCepResponseDTO endereco)
    {
        throw new NotImplementedException();
    }
}

public interface IViaCepService
{
    Task<ViaCepResponseDTO> GetEnderecoByCEP(string cep);

    Task<ViaCepResponseDTO> GetAndSaveAsync(ViaCepResponseDTO endereco);
}
