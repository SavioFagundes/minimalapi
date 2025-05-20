using Dominio.Interface;
using Infraestrutura;
using Dominio.DTOs;
using Dominio.Entidades;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _contexto;

        public AdministradorServico(DbContexto contexto)
        {
            _contexto = contexto;
        }
        
        public Administrador? Login(loginDto loginDto)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDto.Email && a.Senha == loginDto.Senha).FirstOrDefault();
            return adm;
        }

        public Administrador Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();
            return administrador;
        }

        public List<Administrador> Todos (int? pagina)
        {
            var query = _contexto.Administradores.AsQueryable();
            if(pagina.HasValue)
            {
                query = query.Skip((pagina.Value - 1) * 10).Take(10);
            }
            return query.ToList();
        }

        public Administrador? BuscaPorId(int id)
        {
            return _contexto.Administradores.Find(id);
        }
    }
}
