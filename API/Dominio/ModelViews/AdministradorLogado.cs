// Modelo de visualização para representação dos dados do administrador após login
// Contém as informações necessárias para autenticação e autorização

namespace Dominio.ModelViews;

public class AdministradorLogado{
    // Email do administrador logado
    public string Email {get; set;} = string.Empty;

    // Perfil de acesso do administrador logado
    public string Perfil {get; set;} = string.Empty;

    // Token JWT gerado para autenticação
    public string Token {get; set;} = string.Empty;
}
