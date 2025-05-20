using Dominio.Interface;
using Infraestrutura;
using Dominio.DTOs;
using Dominio.Entidades;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Dominio.Servicos
{
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

        public void Atualizar(Veiculo veiculo){
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }

        public Veiculo? BuscaPorId(int id){
            return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
        }

        public Veiculo Incluir(Veiculo veiculo){
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
            return veiculo;
        }
        public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null){
            var query = _contexto.Veiculos.AsQueryable();
            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => v.Nome.Contains(nome));
            }
            return query.Skip((pagina - 1) * 10).Take(10).ToList();
        }
    }
}
