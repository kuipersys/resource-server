// <copyright file="Program.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service;

using Kuiper.ResourceServer.Runtime.Data;
using Kuiper.ServiceInfra.Abstractions.Persistence;
using Kuiper.ServiceInfra.Persistence;

public class Program
{
    /*
    static bool IsLinux()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
    */

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddResourceServerRuntime();
        builder.Services.AddKuiperWebHostRuntime();
        builder.Services.AddSingleton(services => CreateLiteDbKeyValueStore(services.GetRequiredService<IConfiguration>()));

        var app = builder.Build();
        app.UseKuiperWebHostRuntime();

        app.Run();
    }

    private static IKeyValueStore CreateLiteDbKeyValueStore(IConfiguration configuration)
    {
        string dbFile = configuration["Kuiper:KeyValueStore:Path"] ?? "./data/primary_store.db";

        return new LiteDbKeyValueStore(dbFile, "resources");
    }

    private static IKeyValueStore CreateFileBasedKeyValueStore(IConfiguration configuration)
    {
        string path = configuration["Kuiper:KeyValueStore:Path"] ?? "./data";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return FileSystemKeyValueStore.Create(path);
    }
}
