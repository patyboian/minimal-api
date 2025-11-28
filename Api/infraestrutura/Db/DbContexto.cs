using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext{
    
    private readonly IConfiguration _configuracaoAppSettings; // deixamos como readonly para ser 
    // somente leitura e não poder alterar nada
    // Criamos um construtor para poder receber a injeção de dependência
    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }
    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;


    //Método para criar um usuario adm para que possamos testar
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            }
        );
    }
   
   
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) // verifica Se o builder não foi configurado, 
        {
        var stringConexao = _configuracaoAppSettings.GetConnectionString("MySql")?.ToString();
        // esse mysql acima é o nome que damos na conexão no appsetings, pra nos retornar
        // a string de conexao
        // o ? informa que se não achar nada, ele vem vazio

        if (!string.IsNullOrEmpty(stringConexao)) // se não form nulo nem vazio ele coloca o resultado na string
        {
             optionsBuilder.UseMySql(stringConexao, 
        ServerVersion.AutoDetect(stringConexao));
        }
    }  
    }
}