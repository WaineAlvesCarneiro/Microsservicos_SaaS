using IdentityService.Application.Features.Auth;
using IdentityService.Application.Features.Auth.Commands.Create;
using IdentityService.Application.Features.Auth.Commands.DefinirSenha;
using IdentityService.Application.Features.Auth.Commands.Delete;
using IdentityService.Application.Features.Auth.Commands.Update;
using IdentityService.Application.Features.Auth.Queries.GetAll;
using IdentityService.Application.Features.Auth.Queries.GetAllPaged;
using IdentityService.Application.Features.Auth.Queries.GetById;
using IdentityService.Application.Helpers;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Infrastructure.ServicesJWT;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IMediator mediator, TokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] AuthLoginRequest request, CancellationToken cancellationToken)
    {
        var query = new AuthLoginQuery { Username = request.Username, Password = request.Password };
        var user = await mediator.Send(query, cancellationToken);

        if (user == null)
            return Unauthorized();

        if (user.TenancyAtiva != TipoTenancyAtivo.Ativo)
            return BadRequest(new { sucesso = false, erro = "O acesso para este Condomínio está suspenso. Procure o suporte." });

        if (user.Ativo != TipoUserAtivo.Ativo)
            return BadRequest(new { sucesso = false, erro = "Seu usuário está inativo." });

        var token = tokenService.GenerateToken(
            user.UserName,
            (TipoRole)user.Role,
            user.TenancyId,
            user.PrimeiroAcesso,
            (TipoUserAtivo)user.Ativo,
            (TipoTenancyAtivo)user.TenancyAtiva
        );

        return Ok(new { token, sucesso = true, primeiroAcesso = user.PrimeiroAcesso });
    }

    [Authorize(Policy = "PermitirTrocaSenha")]
    [HttpPost("definir-senha-permanente")]
    public async Task<IActionResult> DefinirSenha([FromBody] DefinirSenhaRequest request, CancellationToken cancellationToken)
    {
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username))
            return Unauthorized(new { sucesso = false, erro = "Sessão inválida." });

        var command = new DefinirSenhaCommand
        {
            UserName = username,
            NovaSenha = request.NovaSenha
        };

        var result = await mediator.Send(command, cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados, mensagem = result.Mensagem })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    public record DefinirSenhaRequest(string NovaSenha);

    [Authorize(Policy = "TenancyAdmin")]
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromQuery] long TenancyId = 0)
    {
        var result = await mediator.Send(new GetAllQueryAuthUser(TenancyId: TenancyId), cancellationToken);

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
        [FromQuery] long TenancyId = 0,
        [FromQuery] string? userName = null)
    {
        var query = new GetAllPagedQueryAuthUser(
            Page: page,
            PageSize: pageSize,
            SortBy: sortBy ?? "Id",
            Direction: direction ?? "ASC",
            TenancyId: TenancyId,
            UserName: userName);

        var result = await mediator.Send(query, cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "TenancyAdmin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetByIdQueryAuthUser(id), cancellationToken);

        return result.Sucesso
            ? Ok(new { sucesso = true, dados = result.Dados })
            : NotFound(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "TenancyAdmin")]
    [HttpPost("criar-usuario")]
    public async Task<IActionResult> Post([FromBody] CreateCommandAuthUser command, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateCommandAuthUser command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("O ID da URL não corresponde ao ID do corpo da requisição.");

        if (!User.IsSuporte())
            command.Role = null;

        var result = await mediator.Send(command, cancellationToken);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }

    [Authorize(Policy = "TenancyAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteCommandAuthUser(id), cancellationToken);

        return result.Sucesso
            ? NoContent()
            : BadRequest(new { sucesso = false, erro = result.Mensagem });
    }
}