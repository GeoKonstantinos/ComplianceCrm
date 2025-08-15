using FluentValidation;
using ComplianceCrm.Application.Customers.Services;
using ComplianceCrm.Application.Documents.Services;
using ComplianceCrm.Application.Tasks.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ComplianceCrm.Application;

// <summary>
/// Registers Application layer services to the DI container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ITaskService, TaskService>(); // ή ICustomerTaskService αν μετονομαστεί



        // Validators (FluentValidation)
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}