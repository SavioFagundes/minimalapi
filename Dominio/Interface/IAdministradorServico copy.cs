using Dominio.DTOs;
using Dominio.Entidades;

namespace Dominio.Interface
{
    public interface IAdministradorServico
    {
        Administrador? Login(loginDto loginDto);
    }
}
