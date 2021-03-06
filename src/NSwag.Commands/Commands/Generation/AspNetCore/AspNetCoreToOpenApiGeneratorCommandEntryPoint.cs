﻿//-----------------------------------------------------------------------
// <copyright file="AspNetCoreToSwaggerGeneratorCommandEntryPoint.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace NSwag.Commands.Generation.AspNetCore
{
    /// <summary>In-process entry point for the aspnetcore2swagger command.</summary>
    internal class AspNetCoreToOpenApiGeneratorCommandEntryPoint
    {
        public static void Process(string commandContent, string outputFile, string applicationName)
        {
            var command = JsonConvert.DeserializeObject<AspNetCoreToSwaggerCommand>(commandContent);

            var previousWorkingDirectory = command.ChangeWorkingDirectoryAndSetAspNetCoreEnvironment();
            var webHost = GetWebHost(applicationName);

            var assemblyLoader = new AssemblyLoader.AssemblyLoader();
            var document = Task.Run(async () =>
                await command.GenerateDocumentAsync(assemblyLoader, webHost, previousWorkingDirectory)).GetAwaiter().GetResult();

            var json = command.UseDocumentProvider ? document.ToJson() : document.ToJson(command.OutputType);

            var outputPathDirectory = Path.GetDirectoryName(outputFile);
            Directory.CreateDirectory(outputPathDirectory);
            File.WriteAllText(outputFile, json);
        }

        private static IWebHost GetWebHost(string applicationName)
        {
            var assemblyName = new AssemblyName(applicationName);
            var assembly = Assembly.Load(assemblyName);

            if (assembly.EntryPoint == null)
            {
                throw new InvalidOperationException($"Unable to locate the program entry point for {assemblyName}.");
            }

            var entryPointType = assembly.EntryPoint.DeclaringType;
            var buildWebHostMethod = entryPointType.GetMethod("BuildWebHost");
            var args = new string[0];

            IWebHost webHost = null;
            if (buildWebHostMethod != null)
            {
                var result = buildWebHostMethod.Invoke(null, new object[] { args });
                webHost = (IWebHost)result;
            }
            else
            {
                var createWebHostMethod = entryPointType?.GetMethod("CreateWebHostBuilder");
                if (createWebHostMethod != null)
                {
                    var webHostBuilder = (IWebHostBuilder)createWebHostMethod.Invoke(null, new object[] { args });
                    webHost = webHostBuilder.Build();
                }
            }

            if (webHost != null)
            {
                return webHost;
            }

            throw new InvalidOperationException($"aspnet2swaggercommand requires the entry point type {entryPointType.FullName} to have " +
                                                $"either an BuildWebHost or CreateWebHostBuilder method. " +
                                                $"See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/hosting?tabs=aspnetcore2x " +
                                                $"for suggestions on ways to refactor your startup type.");
        }
    }
}
