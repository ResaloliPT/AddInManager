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
using ResaloliPT.AddInManager.Abstractions;

namespace ResaloliPT.AddInManager
{
    public sealed class AddInProvider : IAddInProvider
    {
        public static IAddInProvider? Instance { get; private set; }

        public static IAddInProvider LoadPlugins(IAddInManagerOptions options)
        {
            if(Instance != null)
                throw new InvalidOperationException("Provider already loaded.");

            var callingMethod = new StackTrace().GetFrame(1).GetMethod();
            if(callingMethod.Name != "Main" || !callingMethod.IsStatic || callingMethod.ReflectedType.Name != "Program")
                throw new InvalidOperationException("You must call AddInProvider.LoadPlugins() inside Program.Main().");

            Instance = new AddInProvider();
            Instance.ScanAddIns(options);

            return Instance;
        }

        private AddInProvider()
        {}

        internal bool isInPipeline { private get; set; } = false;

        internal bool isInServiceContainer { private get; set; } = false;

        private readonly List<IAddIn> scannedAddIns = new List<IAddIn>();

        public void RegisterPipelineAddIns(IApplicationBuilder app, IHostEnvironment env, IConfiguration configuration)
        {
            if(!isInPipeline)
                throw new InvalidOperationException("AddIn Provider was not loaded into the Pipeline!");

            scannedAddIns
                .Where(scannedAddIn => scannedAddIn.PipelineState == AddInState.UNLOADED)
                .ToList()
                .ForEach(addin =>
                {
                    if(addin.AddInServicesDependencies.Count() == 0)
                    {
                        addin.Configure(app, env, configuration);

                        addin.PipelineState = AddInState.LOADED;

                        return;
                    }

                    RegisterPipelineRecursive(addin, app, env, configuration);
                });
        }

        public void RegisterServiceAddIns(IServiceCollection services, IConfiguration configuration)
        {
            if(!isInServiceContainer)
                throw new InvalidOperationException("AddIn Provider was not loaded into the Service Container!");

            scannedAddIns
                .Where(scannedAddIn => scannedAddIn.ServicesState == AddInState.UNLOADED)
                .ToList()
                .ForEach(addin =>
                {
                    if(addin.AddInServicesDependencies.Count() == 0)
                    {
                        addin.ConfigureService(services, configuration);

                        addin.ServicesState = AddInState.LOADED;

                        return;
                    }

                    RegisterServicesRecursive(addin, services, configuration);
                });
        }

        public void ScanAddIns(IAddInManagerOptions addinManagerOptions)
        {
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, addinManagerOptions.PluginsDirectory);

            var externalAssembliesFiles = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);

            var scannedPlugins = new List<IAddIn>();

            foreach(string file in externalAssembliesFiles)
            {
                var assembly = Assembly.LoadFile(file);

                scannedPlugins = assembly
                    .GetTypes()
                    .Where(type => typeof(IAddIn).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .Cast<IAddIn>()
                    .ToList();
            }

            scannedAddIns.AddRange(scannedPlugins); //Adds External AddIns

            AppDomain.CurrentDomain
                .GetAssemblies()
                .ToList()
                .ForEach(assembly =>
                {
                    var localAddIns = assembly
                        .GetTypes()
                        .Where(type => typeof(IAddIn).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        .Select(Activator.CreateInstance)
                        .Cast<IAddIn>()
                        .ToList();

                    scannedAddIns.AddRange(localAddIns); //Adds Internal AddIns
                });


            var distinctAddIns = scannedAddIns.Distinct().ToList(); //Ensure same plugin is loaded once!

            scannedAddIns.Clear();
            scannedAddIns.AddRange(distinctAddIns);
        }

        private void RegisterPipelineRecursive(IAddIn addin, IApplicationBuilder app, IHostEnvironment env, IConfiguration configuration)
        {
            addin.AddInPipelineDependencies.ToList().ForEach(dependency =>
            {
                if(!scannedAddIns.Any(scannedAddIn => scannedAddIn.AddInId == dependency))
                    throw new KeyNotFoundException($"The Pipeline AddIn with the id '{dependency}' required by '{addin.AddInId}' was not found.");

            });

            scannedAddIns
                .Where(scannedAddIn => scannedAddIn.PipelineState == AddInState.UNLOADED)
                .Where(scannedAddIn => addin.AddInPipelineDependencies.Any(dependency => dependency == scannedAddIn.AddInId))
                .ToList()
                .ForEach(scannedAddIn =>
                {
                    RegisterPipelineRecursive(scannedAddIn, app, env, configuration);

                    scannedAddIn.Configure(app, env, configuration);

                    scannedAddIn.PipelineState = AddInState.LOADED;
                });
        }

        private void RegisterServicesRecursive(IAddIn addin, IServiceCollection services, IConfiguration configuration)
        {
            addin.AddInServicesDependencies.ToList().ForEach(dependency =>
            {
                if(!scannedAddIns.Any(scannedAddIn => scannedAddIn.AddInId == dependency))
                    throw new KeyNotFoundException($"The Service AddIn with the id '{dependency}' required by '{addin.AddInId}' was not found.");

            });

            scannedAddIns
                .Where(scannedAddIn => scannedAddIn.PipelineState == AddInState.UNLOADED)
                .Where(scannedAddIn => addin.AddInServicesDependencies.Any(dependency => dependency == scannedAddIn.AddInId))
                .ToList()
                .ForEach(scannedAddIn =>
                {
                    RegisterServicesRecursive(scannedAddIn, services, configuration);

                    scannedAddIn.ConfigureService(services, configuration);

                    scannedAddIn.ServicesState = AddInState.LOADED;
                });
        }
    }
}
