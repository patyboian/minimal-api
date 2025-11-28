using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdministradorServico
{
    // o ? informa que eu também posso retornar nulo
    Administrador? Login(LoginDTO loginDTO);
    Administrador? Incluir(Administrador administrador);
    List<Administrador> Todos (int? pagina);
    Administrador? BuscaPorId(int id); // o ? pode ser nulo, posso não ter o veiculo
} 