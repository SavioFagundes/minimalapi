// Interface que define os contratos para o serviço de administradores
// Define os métodos necessários para gerenciar administradores no sistema

using Dominio.DTOs;
using Dominio.Entidades;

namespace Dominio.Interface
{
    public interface IAdministradorServico
    {
        // Método para autenticação de administrador
        // Retorna o administrador se as credenciais forem válidas, null caso contrário
        Administrador? Login(loginDto loginDto);

        // Método para incluir um novo administrador no sistema
        // Retorna o administrador criado com ID gerado
        Administrador Incluir(Administrador administrador);

        // Método para listar todos os administradores com suporte a paginação
        // Retorna uma lista paginada de administradores
        List<Administrador> Todos(int? pagina);

        // Método para buscar um administrador específico por ID
        // Retorna null se não encontrar
        Administrador? BuscaPorId(int id);
    }
}
