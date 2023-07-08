using Microsoft.Extensions.Configuration;

namespace ExchangeSimulator.Shared.Options;

public static class Extensions
{
    /// <summary>
    /// Gets data from appsettings.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="sectionName">Name of section in appsettings.</param>
    /// <returns>Data from appsettings.</returns>
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string sectionName) where TOptions : new()
    {
        var options = new TOptions();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }
}