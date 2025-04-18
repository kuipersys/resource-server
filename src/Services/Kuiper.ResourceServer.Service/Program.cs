// <copyright file="Program.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using System.Runtime.InteropServices;

using Kuiper.Platform.Runtime.Abstractions.Command;
using Kuiper.ResourceServer.Service.CommandHandlers;
using Kuiper.ResourceServer.Service.Core;
using Kuiper.ServiceInfra.Abstractions.Persistence;
using Kuiper.ServiceInfra.Persistence;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kuiper.ResourceServer.Service;

public class Program
{
    static bool IsLinux()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddKuiperWebHostRuntime();
        builder.Services.AddSingleton(services => FileSystemKeyValueStore.Create("./data"));

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommandHandler, GetResourceCommandHandler>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommandHandler, ListResourceCommandHandler>());

        var app = builder.Build();
        app.UseKuiperWebHostRuntime();

        ResourceManager resourceManager = new ResourceManager(app.Services.GetRequiredService<IKeyValueStore>());
        await resourceManager.InitializeAsync();

        app.Run();
    }
}
