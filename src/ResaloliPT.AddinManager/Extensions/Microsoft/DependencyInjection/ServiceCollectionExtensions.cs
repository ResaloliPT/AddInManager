using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ResaloliPT.AddInManager.Extensions.Microsoft.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseAddIns(this IServiceCollection applicationBuilder, IConfiguration configuration)
        {
            if(applicationBuilder is null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            if(AddInProvider.Instance == null)
                throw new InvalidOperationException("You need to load the AddIn Provider to be able to register Service AddIns.");

            AddInProvider.Instance.RegisterServiceAddIns(applicationBuilder, configuration);

            return applicationBuilder;
        }
    }
}
