using IdentityService.Domain.Entities;
using IdentityService.Domain.Repositories;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories;

public class TenancyRepository(ApplicationDbContext context) : ITenancyRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Tenancy>> GetAllAsync(long tenancyId = 0, CancellationToken cancellationToken = default)
    {
        var query = _context.Tenancys.AsNoTracking();

        if (tenancyId != 0)
            query = query.Where(u => u.Id == tenancyId);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Tenancy> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        string? razaoSocial, string? cnpj, CancellationToken cancellationToken = default)
    {
        var query = _context.Tenancys.AsQueryable();

        if (!string.IsNullOrWhiteSpace(razaoSocial))
            query = query.Where(x => x.RazaoSocial.Contains(razaoSocial));

        if (!string.IsNullOrWhiteSpace(cnpj))
            query = query.Where(x => x.Cnpj.Contains(cnpj));

        query = ApplyOrdering(query, orderBy!, direction!);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }


    private IQueryable<Tenancy> ApplyOrdering(IQueryable<Tenancy> query, string orderBy, string direction)
    {
        direction = direction?.ToUpper() ?? "ASC";

        return (orderBy?.ToLower(), direction) switch
        {
            ("razaoSocial", "ASC") => query.OrderBy(m => m.RazaoSocial),
            ("razaoSocial", "DESC") => query.OrderByDescending(m => m.RazaoSocial),
            ("cnpj", "ASC") => query.OrderBy(m => m.Cnpj),
            ("cnpj", "DESC") => query.OrderByDescending(m => m.Cnpj),
            (_, "DESC") => query.OrderByDescending(m => m.Id),
            _ => query.OrderBy(m => m.Id)
        };
    }

    public async Task<Tenancy?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tenancy>().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task CreateAsync(Tenancy Tenancy, CancellationToken cancellationToken = default)
    {
        await _context.Set<Tenancy>().AddAsync(Tenancy, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Tenancy Tenancy, CancellationToken cancellationToken = default)
    {
        _context.Set<Tenancy>().Update(Tenancy);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Tenancy Tenancy, CancellationToken cancellationToken = default)
    {
        _context.Set<Tenancy>().Remove(Tenancy);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuthUser>> GetByTenancyIdAsync(long tenancyId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<AuthUser>()
            .Where(x => x.TenancyId == tenancyId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteCnpjAsync(string cnpj, long id, CancellationToken cancellation = default)
    {
        return await _context.Tenancys
            .AsNoTracking()
            .AnyAsync(x => x.Cnpj == cnpj && x.Id != id, cancellation);
    }
}
