using System.ComponentModel.DataAnnotations;


namespace Dominio.DTOs
{
    public class VeiculoDTO
    {

        [MaxLength(100)]
        public string Nome { get; set; } = default!;

        [MaxLength(100)]
        public string Marca { get; set; } = default!;

        public int Ano { get; set; } = default!;
    }
}