using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Infraestrutura.Db;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Identity;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using Test.Mocks;
using Microsoft.Extensions.DependencyInjection;
namespace Test.Helpers;

public class Setup{

    public const string PORT = "5001";
    public static TestContext testContext = default!;
    public static WebApplicationFactory<Startup> http = default!;
    public static HttpClient client = default!;

    public static void ClassInit(TestContext testContext)
    {
        Setup.testContext = testContext;
        Setup.http = new WebApplicationFactory<Startup>();

        Setup.http = Setup.http.WithWebHostBuilder(UriBuilder =>
        {
            builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAdministradorServico, AdministradorServicoMock>();
                //services.AddScoped<ILogin<Administrador>,AdministradorServicoMock>();
           
            });
        }
        );
        Setup.client = Setup.http.CreateClient();
    }
}

