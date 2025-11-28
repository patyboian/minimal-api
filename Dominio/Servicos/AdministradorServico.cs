using MinimalApi.Dominio.Entidades;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;

        public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Administrador? BuscaPorId(int id)
    {
         return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
        // acima busca pelo id, se não achar vai retornar nulo
    }

    public Administrador? Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    // o ? informa que eu também posso retornar nulo
    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = (_contexto.Administradores.Where(a => a.Email == loginDTO.Email
        && a.Senha == loginDTO.Senha).FirstOrDefault());
        // o FirstOrDefault retorna o primeiro valor ou nulo
        return adm; 
    }

    public List<Administrador> Todos(int? pagina)
    {
       var query = _contexto.Administradores.AsQueryable();
 
        int itensPorPagina = 10;

        if(pagina != null)
            // o int abaixo converte para int o resultado que vier
        query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        
        return query.ToList();
    }
}