using Asp.Versioning;
using ComplianceCrm.Application.Common.Paging;
using ComplianceCrm.Application.Documents.Dtos;
using ComplianceCrm.Application.Documents.Services;
using ComplianceCrm.Domain.Documents;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceCrm.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/documents")]
public sealed class DocumentsController : ControllerBase
{
    private readonly IDocumentService _svc;
    public DocumentsController(IDocumentService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DocumentDto>>> List(
        [FromQuery] long? customerId,
        [FromQuery] DocumentType? type,
        [FromQuery] DocumentStatus? status,
        [FromQuery] DateTime? createdFromUtc,
        [FromQuery] DateTime? createdToUtc,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var q = new DocumentQuery(new PagingParams(page, pageSize), customerId, type, status, createdFromUtc, createdToUtc);
        var items = await _svc.ListAsync(q, ct);
        return Ok(items);
    }

    [HttpGet("by-customer/{customerId:long}")]
    public async Task<ActionResult<IReadOnlyList<DocumentDto>>> ListByCustomer(long customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var items = await _svc.ListAsync(
            new DocumentQuery(new PagingParams(page, pageSize), customerId),
            ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<long>> Create([FromBody] CreateDocumentRequest req, CancellationToken ct)
    {
        var id = await _svc.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id, version = "1.0" }, id);
    }

    [HttpGet("{id:long}")]
    public Task<ActionResult> GetById(long id) => Task.FromResult<ActionResult>(StatusCode(501)); // add later

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateMetadata(long id, [FromBody] UpdateDocumentMetadataRequest req, CancellationToken ct)
    {
        await _svc.UpdateMetadataAsync(id, req, ct);
        return NoContent();
    }

    [HttpPut("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateDocumentStatusRequest req, CancellationToken ct)
    {
        await _svc.UpdateStatusAsync(id, req, ct);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
