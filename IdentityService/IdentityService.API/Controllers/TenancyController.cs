using IdentityService.API.Controllers.ApiBase;
using IdentityService.Application.Features.Tenancys.Commands.Create;
using IdentityService.Application.Features.Tenancys.Commands.Delete;
using IdentityService.Application.Features.Tenancys.Commands.Update;
using IdentityService.Application.Features.Tenancys.Queries.GetAll;
using IdentityService.Application.Features.Tenancys.Queries.GetAllPaged;
using IdentityService.Application.Features.Tenancys.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TenancyController(IMediator mediator) : ApiBaseController
{
    [Authorize(Policy = "AdminPolicy")]
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromQuery] long tenancyId = 0)
    {
        var result = await mediator.Send(new GetAllQueryTenancy(TenancyId: Convert.ToInt64(tenancyId)), cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "TenancyAdmin")]
    [HttpGet("paginado")]
    public async Task<IActionResult> GetAllPagedAsync(CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = "Id",
        [FromQuery] string? direction = "ASC",
        [FromQuery] string? razaoSocial = null,
        [FromQuery] string? cnpj = null)
    {
        var query = new GetAllPagedQueryTenancy(
            Page: page,
            PageSize: pageSize,
            SortBy: sortBy ?? "Id",
            Direction: direction ?? "ASC",
            RazaoSocial: razaoSocial,
            Cnpj: cnpj);

        var result = await mediator.Send(query, cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "TenancyAdmin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetByIdQueryTenancy(id), cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : NotFound(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "TenancyAdmin")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCommandTenancy command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.Sucesso)
            return BadRequest(new { sucesso = false, erro = result.Mensagem });

        return CreatedAtAction(nameof(GetById), new { id = result.Dados!.Id }, new
        {
            sucesso = true,
            dados = result.Dados
        });
    }

    [Authorize(Policy = "TenancyAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(long id, [FromBody] UpdateCommandTenancy command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("O ID da URL não corresponde ao ID do corpo da requisição.");
        }

        var result = await mediator.Send(command, cancellationToken);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "TenancyAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteCommandTenancy(id), cancellationToken);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }
}