using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Servicos;
 // Teste com erro, pq ele usou o my sql por terminal para fazer um dump,
 // no qual eu não consegui replicar devido á versão do Bd baixada
[TestClass]
public sealed class AdministradorServicoTest
{
    private DbContexto CriarContextoDeTeste()
    {
        //abaixo é o codigo para ele pegar o settings do test e não o principal
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));
        // esse comando acima faz ele voltar algumas pastas

        var builder = new ConfigurationBuilder()
        .SetBasePath(path ?? Directory.GetCurrentDirectory()) // se vier nulo pega do projeto original
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }

    [TestMethod]
    // Teste para criar um administrador
    public void TestandoSalvarAdministrador()
    {
        // Arrange - São todas as variaveis que criaremos para validação
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        // acima limpamos a base toda vez que o teste for executado

        var adm = new Administrador();
        
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";
        var admnistradorServico = new AdministradorServico(context);

        // Act ação que será executada
        admnistradorServico.Incluir(adm);

        // Assert - validação dos dados
        Assert.AreEqual(1, admnistradorServico.Todos(1).Count());
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Senha);
        Assert.AreEqual("Adm", adm.Perfil);
    }

    [TestMethod]
    // Teste para criar um administrador e buscar ele por ID
    public void TestandoBuscaPorId()
    {
        // Arrange - São todas as variaveis que criaremos para validação
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        // acima limpamos a base toda vez que o teste for executado

        var adm = new Administrador();
        
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";
        var admnistradorServico = new AdministradorServico(context);

        // Act ação que será executada
        admnistradorServico.Incluir(adm);
        var admDoBanco = admnistradorServico.BuscaPorId(adm.Id);

        // Assert - validação dos dados
        Assert.AreEqual(1, admDoBanco?.Id);
   
    }
}