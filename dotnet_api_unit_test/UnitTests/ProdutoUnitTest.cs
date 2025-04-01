﻿using dotnet_api.Controllers;
using dotnet_api.Shared.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api_unit_test.UnitTests;

public class ProdutoUnitTest(XunitUnitTests controller) : IClassFixture<XunitUnitTests>
{
    private readonly ProdutosController _controller = new(controller.repository, controller.mapper);

    [Fact]
    public async Task GetProduto_Return_ProdutoDTO_OK()
    {
        var result = await _controller.Get(3);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ProdutoDTO>();
    }

    [Fact]
    public async Task GetProduto_Return_NotFound()
    {
        var result = await _controller.Get(999);

        result.Result.Should().BeOfType<NotFoundResult>()
            .Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateProduto_Return_Ok_Created()
    {
        var result = await _controller.Create(new ProdutoDTO
        {
            Nome = "Produto Teste",
            Descricao = "Descrição do produto teste",
            Preco = 10.00m,
            CategoriaId = 1,
            Estoque = 1      
        });

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().NotBeNull();

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.StatusCode.Should().Be(201);
    }
}
