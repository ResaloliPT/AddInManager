using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResaloliPT.AddinManager.Abstractions;

namespace ResaloliPT.AddinManager
{
    public sealed class AddinProvider : IAddinProvider
    {
        public static IAddinProvider? Instance { get; private set; }

        public static IAddinProvider LoadPlugins(IAddinManagerOptions options)
        {
            if(Instance != null)
                throw new InvalidOperationException("Provider already loaded.");

            var callingMethod = new StackTrace().GetFrame(1).GetMethod();
            if(callingMethod.Name != "Main" || !callingMethod.IsStatic || callingMethod.ReflectedType.Name != "Program")
                throw new InvalidOperationException("You must call AddinProvider.LoadPlugins() inside Program.Main().");

            Instance = new AddinProvider();
            Instance.ScanAddins(options);

            return Instance;
        }

        private AddinProvider()
        {}

        internal bool isInPipeline { private get; set; } = false;

        internal bool isInServiceContainer { private get; set; } = false;

        private readonly List<IAddin> scannedAddins = new List<IAddin>();

        public void RegisterPipelineAddins(IApplicationBuilder app, IHostEnvironment env, IConfiguration configuration)
        {
            if(!isInPipeline)
                throw new InvalidOperationException("Addin Provider was not loaded into the Pipeline!");

            scannedAddins
                .Where(scannedAddin => scannedAddin.PipelineState == AddinState.UNLOADED)
                .ToList()
                .ForEach(addin =>
                {
                    if(addin.AddinServicesDependencies.Count() == 0)
                    {
                        addin.Configure(app, env, configuration);

                        addin.PipelineState = AddinState.LOADED;

                        return;
                    }

                    RegisterPipelineRecursive(addin, app, env, configuration);
                });
        }

        public void RegisterServiceAddins(IServiceCollection services, IConfiguration configuration)
        {
            if(!isInServiceContainer)
                throw new InvalidOperationException("Addin Provider was not loaded into the Service Container!");

            scannedAddins
                .Where(scannedAddin => scannedAddin.ServicesState == AddinState.UNLOADED)
                .ToList()
                .ForEach(addin =>
                {
                    if(addin.AddinServicesDependencies.Count() == 0)
                    {
                        addin.ConfigureService(services, configuration);

                        addin.ServicesState = AddinState.LOADED;

                        return;
                    }

                    RegisterServicesRecursive(addin, services, configuration);
                });
        }

        public void ScanAddins(IAddinManagerOptions addinManagerOptions)
        {
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, addinManagerOptions.PluginsDirectory);

            var assembliesFiles = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);

            var scannedPlugins = new List<IAddin>();

            foreach(string file in assembliesFiles)
            {
                var assembly = Assembly.LoadFile(file);

                scannedPlugins = assembly
                    .GetTypes()
                    .Where(type => typeof(IAddin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .Cast<IAddin>()
                    .ToList();
            }

            scannedAddins.AddRange(scannedPlugins);
        }

        private void RegisterPipelineRecursive(IAddin addin, IApplicationBuilder app, IHostEnvironment env, IConfiguration configuration)
        {
            addin.AddinPipelineDependencies.ToList().ForEach(dependency =>
            {
                if(!scannedAddins.Any(scannedAddin => scannedAddin.AddinId == dependency))
                    throw new KeyNotFoundException($"The Pipeline Addin with the id '{dependency}' required by '{addin.AddinId}' was not found.");

            });

            scannedAddins
                .Where(scannedAddin => scannedAddin.PipelineState == AddinState.UNLOADED)
                .Where(scannedAddin => addin.AddinPipelineDependencies.Any(dependency => dependency == scannedAddin.AddinId))
                .ToList()
                .ForEach(scannedAddin =>
                {
                    RegisterPipelineRecursive(scannedAddin, app, env, configuration);

                    scannedAddin.Configure(app, env, configuration);

                    scannedAddin.PipelineState = AddinState.LOADED;
                });
        }

        private void RegisterServicesRecursive(IAddin addin, IServiceCollection services, IConfiguration configuration)
        {
            addin.AddinServicesDependencies.ToList().ForEach(dependency =>
            {
                if(!scannedAddins.Any(scannedAddin => scannedAddin.AddinId == dependency))
                    throw new KeyNotFoundException($"The Service Addin with the id '{dependency}' required by '{addin.AddinId}' was not found.");

            });

            scannedAddins
                .Where(scannedAddin => scannedAddin.PipelineState == AddinState.UNLOADED)
                .Where(scannedAddin => addin.AddinServicesDependencies.Any(dependency => dependency == scannedAddin.AddinId))
                .ToList()
                .ForEach(scannedAddin =>
                {
                    RegisterServicesRecursive(scannedAddin, services, configuration);

                    scannedAddin.ConfigureService(services, configuration);

                    scannedAddin.ServicesState = AddinState.LOADED;
                });
        }
    }
}
