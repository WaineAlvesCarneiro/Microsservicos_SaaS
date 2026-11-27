using IdentityService.Application.DTOs;
using IdentityService.Application.Mappings;
using IdentityService.Domain.Common;
using IdentityService.Domain.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Auth.Queries.GetById;

public class GetByIdQueryHandlerAuthUser(IAuthUserRepository repository)
    : IRequestHandler<GetByIdQueryAuthUser, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(GetByIdQueryAuthUser request, CancellationToken cancellationToken)
    {
        var dado = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (dado is null) return Result<AuthUserDto>.Failure("Usuário não encontrado.");

        return Result<AuthUserDto>.Success(dado.ToDto());
    }
}