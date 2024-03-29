﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ResaloliPT.AddInManager.Abstractions
{
    public abstract class AddIn : IAddIn
    {
        public AddInState PipelineState { get; set; } = AddInState.UNLOADED;

        public AddInState ServicesState { get; set; } = AddInState.UNLOADED;

        public string AddInId { get; private set; }

        public IEnumerable<string> AddInPipelineDependencies { get; private set; } = new List<string>();

        public IEnumerable<string> AddInServicesDependencies { get; private set; } = new List<string>();

        protected AddIn(string addinId)
        {
            AddInId = addinId;

            AddInPipelineDependencies = AddPipelineDependencies();

            AddInServicesDependencies = AddServicesDependencies();
        }

        public virtual IEnumerable<string> AddPipelineDependencies()
        {
            return Array.Empty<string>();
        }

        public virtual IEnumerable<string> AddServicesDependencies()
        {
            return Array.Empty<string>();
        }

        public virtual void ConfigureService(IServiceCollection services, IConfiguration configuration)
        {}

        public virtual void Configure(IApplicationBuilder app, IHostEnvironment env, IConfiguration configuration)
        {}
    }
}
