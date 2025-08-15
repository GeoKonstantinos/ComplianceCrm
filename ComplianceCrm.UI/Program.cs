using ComplianceCrm.UI.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// ===== API HTTP CLIENT =====
// ���� ��� �� URL ��� API ��� (�.�. https://localhost:5001)
var apiBase = builder.Configuration.GetValue<string>("Api:BaseUrl") ?? "https://localhost:5001";

builder.Services.AddHttpClient("ComplianceApi", client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    // ��������� ������� Tenant ��� dev � ���� ���� ��� GUID
    client.DefaultRequestHeaders.Add("X-Tenant-Id", "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
})
// ����������� ���� delegating handler ��� Correlation Id
.AddHttpMessageHandler(() => new CorrelationIdHandler());

builder.Services.AddScoped<ApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


// ===== Delegating Handler: ��������� X-Correlation - Id �� ���� ����� =====
public sealed class CorrelationIdHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.Headers.Contains("X-Correlation-Id"))
            request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());

        return base.SendAsync(request, cancellationToken);
    }
}