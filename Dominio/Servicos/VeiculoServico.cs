using MinimalApi.Dominio.Entidades;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;

        public VeiculoServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }
// veiculo ? pq posso não ter o veiculo
    public Veiculo? BuscaPorId(int id)
    {
        return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
        // acima busca pelo id, se não achar vai retornar nulo
    }

    public void Incluir(Veiculo veiculo)
    {
         _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _contexto.Veiculos.AsQueryable();
        if (!string.IsNullOrEmpty(nome)) // aqui diz, se eu passei o nome, então faça
        {
            // abaixo vai fazer um like pelo nome, por isso o %%
            query = query.Where(v => EF.Functions.Like(v.nome.ToLower(), $"%{nome}%"));
        }

        int itensPorPagina = 10;

        if(pagina != null)
            // o int abaixo converte para int o resultado que vier
        query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        
        return query.ToList();
    }
}