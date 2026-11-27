using IdentityService.Domain.Entities;

namespace IdentityService.Domain.Repositories;

public interface ITenancyRepository
{
    Task<IEnumerable<Tenancy>> GetAllAsync(long tenancyId = 0, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Tenancy> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        string? razaoSocial, string? cnpj, CancellationToken cancellationToken = default);
    Task<Tenancy?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task CreateAsync(Tenancy tenancy, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tenancy tenancy, CancellationToken cancellationToken = default);
    Task DeleteAsync(Tenancy tenancy, CancellationToken cancellationToken = default);
    Task<bool> ExisteCnpjAsync(string cnpj, long id,CancellationToken cancellationToken = default);
}