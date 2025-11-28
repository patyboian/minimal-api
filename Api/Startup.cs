using MinimalApi;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViewes;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Reflection.Metadata;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
        
    }

    private string key = "";

    public IConfiguration? Configuration{get; set;} = default!;

    // tudo o que temos de configuração de service colocaremos aqui
        public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters    
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

services.AddAuthorization();

services.AddScoped<IAdministradorServico, AdministradorServico>();
services.AddScoped<IVeiculoServico, VeiculoServico>();

// add o swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition ("Bearer", new OpenApiSecurityScheme
    {
        Name = "Autorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[]{}
    }
    });

});


services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
    );
}
);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
       //Configurando o Swagger
        app.UseSwagger();
        app.UseSwaggerUI(); // pra poder ter a interface do swagger UI

        // Configuração do Jwt, tem que ser nessa ordem, primeiro autenticar depois autorizar
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
           #region Home
// antes era assim para aparecer apenas o Hello World na nossa home
//app.MapGet("/", () => "Hello World!");
    // agora mudamos para
    // o .WithTags("Home") deixa separado por sessão no Swagger
endpoints.MapGet("/", () => Results.Json (new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Administradores
string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    // acima ele faz a criptografia dos dados

    var claims = new List<Claim>()
    {
     new Claim("Email", administrador.Email),
     new Claim(ClaimTypes.Role, administrador.Perfil),
     new Claim("Perfil", administrador.Perfil) ,  
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires : DateTime.Now.AddDays(1), // ele vai expirar em 1 dia
        signingCredentials : credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}
// FromBody informa que vamos receber este dado via corpo do request no postman
endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.Login(loginDTO);

    if(adm != null){
    string token = GerarTokenJwt(adm);
        return Results.Ok(new AdmistradorLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else 
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");
// Colocando o AllowAnonymous() informamos que essa rota não tem restrição


// FromBody informa que vamos receber este dado via query
endpoints.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach (var adm in administradores)
    {
        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
        return Results.Ok(adms);

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Administradores");

// FromRoute informa que vamos receber este dado via rota
endpoints.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscaPorId(id);
    if(administrador  == null) return Results.NotFound();
    return Results.Ok(new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
    
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Administradores");


// FromBody informa que vamos receber este dado via corpo do request no postman
endpoints.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao{
        Mensagens = new List<string>()
    };

    if(string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("Email não pode ser vazio");

         if(string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagens.Add("Senha não pode ser vazia");

         if(administradorDTO.Perfil == null)
        validacao.Mensagens.Add("Perfil não pode ser vazio");

        if(validacao.Mensagens.Count() > 0)
    return Results.BadRequest(validacao);

    var administrador = new Administrador{
        Email = administradorDTO.Email,
         Senha = administradorDTO.Senha,
          Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
          // acima o ?? significa, se vier nulo, vamos colocar o padrão editor
    };
    administradorServico.Incluir(administrador);

    return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
        
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Administradores");

#endregion

#region Veiculos

    ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO){

        var validacao = new ErrosDeValidacao(){
        Mensagens = new List<string>()
        };
// se o nome for vazio, vai abrir outra tela com a mensagem de erro
    if(string.IsNullOrEmpty(veiculoDTO.Nome))
    validacao.Mensagens.Add("O nome não pode ser vazio");

        if(string.IsNullOrEmpty(veiculoDTO.Marca))
    validacao.Mensagens.Add("A marca não pode ficar em branco");

        if(veiculoDTO.Ano < 1950)
    validacao.Mensagens.Add("Veiculo muito antigo, aceito somente anos superiores à 1950.");
   
   return validacao;
    }

// FromBody informa que vamos receber este dado via corpo do request no postman
endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
       
    var validacao = validaDTO(veiculoDTO);

        if(validacao.Mensagens.Count() > 0)
    return Results.BadRequest(validacao);

    var veiculo = new Veiculo{
        nome = veiculoDTO.Nome,
         Marca = veiculoDTO.Marca,
          Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
    
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm, Editor"})
.WithTags("Veiculos");

// FromQuery informa que vamos receber este dado via query
endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
    
}).RequireAuthorization().WithTags("Veiculos");

// FromRoute informa que vamos receber este dado via rota
endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    return Results.Ok(veiculo);
    
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm, Editor"}).WithTags("Veiculos");

// FromRoute informa que vamos receber este dado via rota
endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
     var veiculo = veiculoServico.BuscaPorId(id);
 if(veiculo == null) return Results.NotFound();

     var validacao = validaDTO(veiculoDTO);

        if(validacao.Mensagens.Count() > 0)
    return Results.BadRequest(validacao);

        veiculo.nome = veiculoDTO.Nome;
         veiculo.Marca = veiculoDTO.Marca;
          veiculo.Ano = veiculoDTO.Ano;

          veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
    
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Veiculos");

// FromRoute informa que vamos receber este dado via rota
endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
 var veiculo = veiculoServico.BuscaPorId(id);
 if(veiculo == null) return Results.NotFound();

          veiculoServico.Apagar(veiculo);

    return Results.NoContent();
    
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Veiculos");

#endregion 
        });

    }

}