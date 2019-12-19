using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ResaloliPT.AddinManager.Extensions.Microsoft.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseAddins(this IServiceCollection applicationBuilder, IConfiguration configuration)
        {
            if(applicationBuilder is null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            if(AddinProvider.Instance == null)
                throw new InvalidOperationException("You need to load the Addin Provider to be able to register Service Addins.");

            AddinProvider.Instance.RegisterServiceAddins(applicationBuilder, configuration);

            return applicationBuilder;
        }
    }
}
