using Asp.Versioning;
using ComplianceCrm.Application.Customers.Dtos;
using ComplianceCrm.Application.Customers.Services;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceCrm.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _svc;
    public CustomersController(ICustomerService svc) => _svc = svc;

    /// <summary>List customers (paged).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var items = await _svc.ListAsync(page, pageSize, ct);
        return Ok(items);
    }

    /// <summary>Create a new customer.</summary>
    [HttpPost]
    public async Task<ActionResult<long>> Create([FromBody] CreateCustomerRequest req, CancellationToken ct)
    {
        var id = await _svc.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id, version = "1.0" }, id);
    }

    /// <summary>Get a customer by id. (Not implemented yet in Application.)</summary>
    [HttpGet("{id:long}")]
    public Task<ActionResult> GetById(long id)
        => Task.FromResult<ActionResult>(StatusCode(501)); // TODO: add ICustomerService.GetByIdAsync when needed
}
