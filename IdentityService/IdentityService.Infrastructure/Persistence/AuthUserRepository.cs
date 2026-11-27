using IdentityService.Domain.Entities;
using IdentityService.Domain.Repositories;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories;

public class AuthUserRepository(ApplicationDbContext context) : IAuthUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<AuthUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.AuthUsers.FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
    }

    public async Task<IEnumerable<AuthUser>> GetAllAsync(long TenancyId = 0, CancellationToken cancellationToken = default)
    {
        IQueryable<AuthUser> query = MontaQueryComTenancyId(TenancyId);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<AuthUser> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, string? orderBy, string? direction,
        long TenancyId, string? userName, CancellationToken cancellationToken = default)
    {
        IQueryable<AuthUser> query = MontaQueryComTenancyId(TenancyId);

        if (!string.IsNullOrWhiteSpace(userName))
            query = query.Where(x => x.UserName.Contains(userName));

        query = ApplyOrdering(query, orderBy!, direction!);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private IQueryable<AuthUser> MontaQueryComTenancyId(long TenancyId)
    {
        IQueryable<AuthUser> query = _context.AuthUsers;

        if (TenancyId != 0)
            query = query.Where(u => u.TenancyId == TenancyId);
        return query;
    }

    private IQueryable<AuthUser> ApplyOrdering(IQueryable<AuthUser> query, string orderBy, string direction)
    {
        direction = direction?.ToUpper() ?? "ASC";

        return (orderBy?.ToLower(), direction) switch
        {
            ("username", "asc") => query.OrderBy(m => m.UserName),
            ("username", "desc") => query.OrderByDescending(m => m.UserName),
            ("email", "asc") => query.OrderBy(m => m.Email),
            ("email", "desc") => query.OrderByDescending(m => m.Email),
            (_, "DESC") => query.OrderByDescending(m => m.Id),
            _ => query.OrderBy(m => m.Id)
        };
    }

    public async Task<AuthUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<AuthUser>().AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task CreateAsync(AuthUser authUser, CancellationToken cancellationToken = default)
    {
        await _context.Set<AuthUser>().AddAsync(authUser, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AuthUser authUser, CancellationToken cancellationToken = default)
    {
        _context.Set<AuthUser>().Update(authUser);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AuthUser authUser, CancellationToken cancellationToken = default)
    {
        _context.Set<AuthUser>().Remove(authUser);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuthUser>> GetByTenancyIdAsync(long tenancyId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<AuthUser>()
            .Where(x => x.TenancyId == tenancyId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteUsuarioVinculadoNaTenancyAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.AuthUsers.AsNoTracking().AnyAsync(m => m.TenancyId == id, cancellationToken);
    }
}