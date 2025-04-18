﻿using AutoMapper;
using Asp.Versioning;
using dotnet_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using dotnet_api.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using dotnet_api.Shared.Utilities;
using dotnet_api.Shared.Utilities.FilterClasses;
using dotnet_api.Shared.DTOs;
using dotnet_api.Shared.Filters;
using dotnet_api.Shared.Enums;
using dotnet_api.Services;

namespace dotnet_api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    //[Authorize]
    [EnableRateLimiting("fixed")]
    public class PessoasController(ITransaction transaction, IMapper mapper, IViaCepService viaCep) : ControllerBase
    {
        private readonly ITransaction _transaction = transaction;
        private readonly IMapper _mapper = mapper;
        private readonly IViaCepService _viaCep = viaCep;

        [PermissionAuthorize((int)PermissoesEnum.produtosRead)]
        [HttpPost("getAll")]

        public Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] Pagination paginacao, [FromBody] ProdutoFilter? filtro)
        {
            //var produtos = await _transaction.ProdutoRepository.Get(paginacao, filtro);
            //if (produtos == null) throw new Exception("Ocorreu um erro inesperado ao consultar os registro na base de dados");

            //return Ok(_mapper.Map<IEnumerable<ProdutoDTO>>(produtos));

            throw new NotImplementedException("");
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<PessoaDTO>> Get(int id)
        {
            var consulta = await _transaction.PessoaRepository.Get(id);
            if (consulta == null) return NotFound();

            return Ok(_mapper.Map<PessoaDTO>(consulta));
        }

        [HttpPost]
        public async Task<ActionResult> Create(PessoaDTO produto)
        {
            Pessoa pesssoaCriada = _transaction.PessoaRepository.Create(_mapper.Map<Pessoa>(produto));

            if (pesssoaCriada == null) return BadRequest();

            await _transaction.Commit();
            return CreatedAtAction(nameof(Get), new { id = pesssoaCriada.Id }, new CreatedResponseDTO() { Id = pesssoaCriada.Id.ToString() });
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<ActionResult> Update(int id, PessoaDTO pessoa)
        {
            if (id != pessoa.Id) throw new FormatException("A requisição foi enviada de maneira incorreta");

            Pessoa pessoaAtualizado = _transaction.PessoaRepository.Update(_mapper.Map<Pessoa>(pessoa));

            await _transaction.Commit();
            return Ok(pessoaAtualizado);
        }


        [HttpPut("updateEndereco/{id:int:min(1)}")]
        public async Task<ActionResult> UpdateEndereco(int id, UpdateEnderecoDTO atualizacaoDTO)
        {
            if (ModelState.IsValid == false) return BadRequest(new ErrorResponse()
            {
                Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage),
                Message = "Ocorreu um erro ao processar a operação desejada",
                StatusCode = StatusCodes.Status400BadRequest
            });

            if (id != atualizacaoDTO.Id_pessoa) throw new FormatException("A requisição foi efetuada de maneira incorreta");

            var resultViaCep = await _viaCep.GetEnderecoByCEP(atualizacaoDTO.Cep!);

            if (resultViaCep == null) return BadRequest(new ErrorResponse()
            {
                Message = "Ocorreu um erro ao processar a operação desejada",
                StatusCode = StatusCodes.Status404NotFound
            });

            var pessoa = await _transaction.PessoaRepository.Get(id) ?? throw new KeyNotFoundException("Registro de Pessoa não encotrado na base de dados");
            
            pessoa.Cep = resultViaCep.Cep;
            pessoa.Logradouro = resultViaCep.Logradouro;
            pessoa.Cidade = resultViaCep.Localidade;
            pessoa.Estado = resultViaCep.Uf;

            Pessoa pessoaAtualizado = _transaction.PessoaRepository.Update(pessoa);

            await _transaction.Commit();
            return Ok(_mapper.Map<PessoaDTO>(pessoaAtualizado));
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<ActionResult> Delete(int id)
        {
            Pessoa? pessoa = await _transaction.PessoaRepository.Get(id);
            if (pessoa == null) return NotFound();

            _transaction.PessoaRepository.Delete(pessoa);

            await _transaction.Commit();

            return Ok();
        }

    }
}
