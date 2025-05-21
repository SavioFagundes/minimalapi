// Modelo de visualização para representação de dados do administrador
// Utilizado para transferência de dados entre camadas da aplicação
// Contém apenas as propriedades necessárias para exibição

namespace Dominio.ModelViews
{
    public record AdministradorModelView
    {
        // Identificador único do administrador
        public int Id { get; set; }

        // Nome do administrador
        public string Nome { get; set; }

        // Email do administrador
        public string Email { get; set; }

        // Perfil de acesso do administrador (Adm ou Editor)
        public string Perfil { get; set; }
    }
}
