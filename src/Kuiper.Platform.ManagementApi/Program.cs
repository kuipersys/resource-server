// <copyright file="Program.cs" company="Kuiper Microsystems, LLC">
// � Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kuiper.Platform.ManagementApi.Middleware;

namespace Kuiper.Platform.ManagementApi;

public class Program
{
    static bool IsLinux()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // builder.Configuration.AddKuiperPlatformConfiguration();

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddKuiperServices();
        //builder.Services.AddAuthentication(options =>
        //{
        //    options.AddScheme<MutualTlsAuthenticationHandler>(MutualTlsAuthenticationHandler.SchemeName, MutualTlsAuthenticationHandler.DisplayName);
        //});

        //builder.Services.AddResourceHandlers();
        //builder.Services.AddScoped<IKeyValueStore, KvStoreDbContext>();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.SerializerOptions.IgnoreReadOnlyFields = true;
            options.SerializerOptions.IgnoreReadOnlyProperties = false;
            options.SerializerOptions.IncludeFields = true;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        //builder.Services.AddDbContext<KvStoreDbContext>(options =>
        //{
        //    // options.UseInMemoryDatabase("kvstore");

        //    // Get application data directory
        //    var application_data = Environment.SpecialFolder.ApplicationData;

        //    // Create a directory for the application data
        //    string application_data_path = Path.Combine(Environment.GetFolderPath(application_data), "kuiper_data");

        //    if (IsLinux())
        //    {
        //        application_data_path = "/var/lib/kuiperdb";
        //    }

        //    var system_db_path = Path.Combine(application_data_path, "system.db");

        //    Console.WriteLine($"Database Path: {system_db_path}");

        //    if (!Directory.Exists(application_data_path))
        //    {
        //        Console.WriteLine("Creating data directory ...");
        //        Directory.CreateDirectory(application_data_path);
        //    }

        //    // Configure the database to use SQLite
        //    options.UseSqlite($"Data Source={system_db_path}");
        //});

        var app = builder.Build();
        // Configure the HTTP request pipeline.

        //app.UseAuthentication();
        app.UseAuthorization();

        //app
        //    .MapKuiperServicesEndpoints(app)
        //    .MapKuiperResources();

        app.MapKuiperServicesEndpoints();

        app.Run();
    }
}
