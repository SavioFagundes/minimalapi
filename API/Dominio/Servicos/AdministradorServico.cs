// Serviço responsável pela lógica de negócios relacionada aos administradores
// Implementa a interface IAdministradorServico para gerenciar operações CRUD e autenticação

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
        // Contexto do banco de dados injetado via construtor
        private readonly DbContexto _contexto;

        public AdministradorServico(DbContexto contexto)
        {
            _contexto = contexto;
        }
        
        // Método para autenticação de administrador
        // Retorna o administrador se as credenciais forem válidas
        public Administrador? Login(loginDto loginDto)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDto.Email && a.Senha == loginDto.Senha).FirstOrDefault();
            return adm;
        }

        // Método para incluir um novo administrador no sistema
        // Retorna o administrador criado com ID gerado
        public Administrador Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();
            return administrador;
        }

        // Método para listar todos os administradores com suporte a paginação
        // Retorna uma lista paginada de administradores
        public List<Administrador> Todos (int? pagina)
        {
            var query = _contexto.Administradores.AsQueryable();
            if(pagina.HasValue)
            {
                query = query.Skip((pagina.Value - 1) * 10).Take(10);
            }
            return query.ToList();
        }

        // Método para buscar um administrador específico por ID
        // Retorna null se não encontrar
        public Administrador? BuscaPorId(int id)
        {
            return _contexto.Administradores.Find(id);
        }
    }
}
