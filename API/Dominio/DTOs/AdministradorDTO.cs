using Dominio.Enums;

namespace Dominio.DTOs
{
    public class AdministradorDTO
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public Perfils? Perfil { get; set; } = default!;
    }
}