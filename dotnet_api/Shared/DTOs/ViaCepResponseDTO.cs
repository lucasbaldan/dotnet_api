namespace dotnet_api.Shared.DTOs;

public class ViaCepResponseDTO
{
    public string? Cep { get; set; }
    public string? Logradouro { get; set; }
    public string? Bairro { get; set; }
    public string? Localidade { get; set; }
    public string? Uf { get; set; }
}