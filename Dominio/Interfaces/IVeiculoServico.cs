using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IVeiculoServico
{
    // o ? informa que eu também posso retornar nulo
    List<Veiculo> Todos(int? pagina = 1, string? nome=null, string? marca=null);
    Veiculo? BuscaPorId(int id); // o ? pode ser nulo, posso não ter o veiculo

    void Incluir(Veiculo veiculo);

    void Atualizar(Veiculo veiculo);

    void Apagar(Veiculo veiculo);
} 