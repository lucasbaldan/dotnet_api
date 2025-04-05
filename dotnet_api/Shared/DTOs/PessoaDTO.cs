namespace dotnet_api.Shared.DTOs;

public class PessoaDTO
{
    public int Id { get; }

    public string? Nome { get; set; }

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public string? Cep { get; set; }

    public string? Logradouro { get; set; }

    public string? Cidade { get; set; }

    public string? Estado { get; set; }
}
