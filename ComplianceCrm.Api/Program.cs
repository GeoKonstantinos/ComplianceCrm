using Asp.Versioning;
using ComplianceCrm.Api.Support;
using ComplianceCrm.Application;
using ComplianceCrm.Infrastructure;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();



builder.Services.AddControllers();

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p => p.AddPolicy("dev",
    b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));


builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("ComplianceCrm.Api"))
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation();
        t.AddHttpClientInstrumentation(); 
        t.AddEntityFrameworkCoreInstrumentation(); 
        t.AddNpgsql(); 
        t.AddOtlpExporter(); 
    })
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation();
        m.AddHttpClientInstrumentation();
        m.AddRuntimeInstrumentation();
        m.AddOtlpExporter();
    });

var app = builder.Build();

app.UseCors("dev");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseMiddleware<CorrelationHeaderMiddleware>();
//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
