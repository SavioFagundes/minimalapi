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
    }
}
