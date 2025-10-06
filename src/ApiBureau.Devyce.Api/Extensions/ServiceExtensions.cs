using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;

namespace ApiBureau.Devyce.Api.Extensions;

/// <summary>
/// Dependency injection helpers for registering the Devyce API client.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers Devyce client services and binds <see cref="DevyceSettings"/> from configuration.
    /// </summary>
    /// <param name="services">The target service collection.</param>
    /// <param name="configuration">Application configuration used to bind settings.</param>
    /// <returns>The original service collection for chaining.</returns>
    public static IServiceCollection AddDevyce(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<DevyceSettings>()
            .Bind(configuration.GetSection(nameof(DevyceSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddHttpClient<DevyceHttpClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<DevyceSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("X-API-Key", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{settings.ApiKey}:")));
                client.DefaultRequestHeaders.Add("X-API-Key", settings.ApiKey);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("ApiBureau.Devyce.Api/1.0");
            })
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(20)))
            .AddTransientHttpErrorPolicy(pb => pb.WaitAndRetryAsync(
            [
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(3)
            ]));

        services.AddSingleton<IDevyceClient, DevyceClient>();

        return services;
    }
}