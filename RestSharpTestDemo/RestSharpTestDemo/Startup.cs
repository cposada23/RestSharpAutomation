using Microsoft.Extensions.DependencyInjection;
using RestSharpTestDemo.Base;

namespace RestSharpTestDemo;

public class Startup
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IRestLibrary, RestLibrary>()
            .AddScoped<IRestFactory, RestFactory>()
            .AddScoped<IRestBuilder, RestBuilder>();
    }
}