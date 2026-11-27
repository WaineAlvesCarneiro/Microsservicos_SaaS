using IdentityService.Domain.Entities;

namespace IdentityService.Domain.Repositories;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<IEnumerable<AuthUser>> GetAllAsync(long TenancyId = 0, CancellationToken cancellationToken = default);

    Task<(IEnumerable<AuthUser> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize, string? orderBy, string? direction,
        long TenancyId, string? userName, CancellationToken cancellationToken = default);

    Task<AuthUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task CreateAsync(AuthUser authUser, CancellationToken cancellationToken = default);

    Task UpdateAsync(AuthUser authUser, CancellationToken cancellationToken = default);

    Task DeleteAsync(AuthUser authUser, CancellationToken cancellationToken = default);

    Task<IEnumerable<AuthUser>> GetByTenancyIdAsync(long tenancyId, CancellationToken cancellationToken = default);

    Task<bool> ExisteUsuarioVinculadoNaTenancyAsync(long id, CancellationToken cancellationToken = default);
}