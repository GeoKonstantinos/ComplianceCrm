using Asp.Versioning;
using ComplianceCrm.Application.Common.Paging;
using ComplianceCrm.Application.Tasks.Dtos;
using ComplianceCrm.Application.Tasks.Services;
using ComplianceCrm.Domain.Customers;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceCrm.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _svc;
    public TasksController(ITaskService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> List(
        [FromQuery] long? customerId,
        [FromQuery] TaskType? type,
        [FromQuery] Domain.Customers.TaskStatus? status,
        [FromQuery] DateTime? dueFromUtc,
        [FromQuery] DateTime? dueToUtc,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var q = new TaskQuery(new PagingParams(page, pageSize), customerId, type, status, dueFromUtc, dueToUtc);
        var items = await _svc.ListAsync(q, ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<long>> Create([FromBody] CreateTaskRequest req, CancellationToken ct)
    {
        var id = await _svc.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id, version = "1.0" }, id);
    }

    [HttpGet("{id:long}")]
    public Task<ActionResult> GetById(long id) => System.Threading.Tasks.Task.FromResult<ActionResult>(StatusCode(501)); // add later

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTaskRequest req, CancellationToken ct)
    {
        await _svc.UpdateAsync(id, req, ct);
        return NoContent();
    }

    [HttpPut("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateTaskStatusRequest req, CancellationToken ct)
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