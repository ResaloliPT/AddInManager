using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ResaloliPT.AddinManager.Extensions.Microsoft.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAddins(this IApplicationBuilder applicationBuilder, IHostEnvironment env, IConfiguration configuration)
        {
            if(applicationBuilder is null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            if(AddinProvider.Instance == null)
                throw new InvalidOperationException("You need to load the Addin Provider to be able to register Pipeline Addins.");

            AddinProvider.Instance.RegisterPipelineAddins(applicationBuilder, env, configuration);

            return applicationBuilder;
        }
    }
}
