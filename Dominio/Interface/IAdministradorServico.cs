using Dominio.DTOs;
using Dominio.Entidades;

namespace Dominio.Interface
{
    public interface IAdministradorServico
    {
        Administrador? Login(loginDto loginDto);
        Administrador Incluir(Administrador administrador);
        List<Administrador> Todos(int? pagina );
        Administrador? BuscaPorId(int id);
    }
}
