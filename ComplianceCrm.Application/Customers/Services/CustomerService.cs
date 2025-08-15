using ComplianceCrm.Application.Abstractions.Persistence;
using ComplianceCrm.Application.Abstractions.Providers;
using ComplianceCrm.Application.Abstractions.Services;
using ComplianceCrm.Application.Common.Paging;
using ComplianceCrm.Application.Customers.Dtos;
using ComplianceCrm.Domain.Audit.Enums;
using ComplianceCrm.Domain.Customers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ComplianceCrm.Application.Customers.Services;

public class CustomerService(
    IAppDbContext db,
    ITenantProvider tenant,
    ICurrentUserService currentUser,
    IDateTimeProvider clock,
    IBusinessAuditService audit,
    IValidator<CreateCustomerRequest> createValidator,
    IValidator<PagingParams> pagingValidator) : ICustomerService
{
    private readonly IAppDbContext _db = db;
    private readonly ITenantProvider _tenant = tenant;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IDateTimeProvider _clock = clock;
    private readonly IBusinessAuditService _audit = audit;
    private readonly IValidator<CreateCustomerRequest> _createValidator = createValidator;
    private readonly IValidator<PagingParams> _pagingValidator = pagingValidator;

    public async Task<IReadOnlyList<CustomerDto>> ListAsync(int page, int pageSize, CancellationToken ct = default)
    {
        // Validate paging
        await _pagingValidator.ValidateAndThrowAsync(new PagingParams(page, pageSize), ct);

        var tenantId = _tenant.GetTenantId();

        var items = await _db.Customers
            .Where(c => c.TenantId == tenantId)
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .Select(c => new CustomerDto(c.Id, c.Name, c.Email, c.Phone, c.Consented, c.ConsentDateUtc))
            .ToListAsync(ct);

        return items;
    }

    public async Task<long> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default)
    {
        // Validate input
        await _createValidator.ValidateAndThrowAsync(request, ct);

        var tenantId = _tenant.GetTenantId();
        var now = _clock.UtcNow;
        var userId = _currentUser.GetUserId();

        var entity = new Customer
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Consented = request.Consented,
            ConsentDateUtc = request.Consented ? now : null,
            CreatedAtUtc = now,
            CreatedByUserId = userId
        };

        _db.Customers.Add(entity);
        await _db.SaveChangesAsync(ct);

        await _audit.WriteAsync(
            targetType: TargetType.Customer,
            targetId: entity.Id,
            action: BusinessAction.Created,
            notes: "Customer created",
            ct: ct);

        return entity.Id;
    }
}
