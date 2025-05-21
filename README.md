# Minimal API - Sistema de Gerenciamento de Veículos

Uma API REST minimalista desenvolvida em .NET 9 para gerenciamento de veículos e administradores, utilizando autenticação JWT e MySQL como banco de dados.

## 🚀 Funcionalidades

- Autenticação JWT
- Gerenciamento de Administradores
- Gerenciamento de Veículos
- Controle de Acesso Baseado em Perfis (RBAC)
- Documentação com Swagger
- Paginação de resultados
- Validação de dados
- Containerização com Docker

## 📋 Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [MySQL](https://www.mysql.com/downloads/) (opcional, se não usar Docker)

## 🔧 Instalação

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/minimal-api.git
cd minimal-api
```

2. Inicie o container do MySQL usando Docker Compose:
```bash
docker-compose up -d
```

3. Restaure as dependências do projeto:
```bash
dotnet restore
```

4. Execute as migrações do banco de dados:
```bash
dotnet ef database update
```

5. Inicie a aplicação:
```bash
dotnet run
```

A API estará disponível em `https://localhost:5001` e a documentação Swagger em `https://localhost:5001/swagger`

## 🛠️ Como Usar

### Autenticação

1. Faça login para obter o token JWT:
```http
POST /administradores/login
Content-Type: application/json

{
    "email": "admin@exemplo.com",
    "senha": "123456"
}
```

2. Use o token retornado no header das requisições:
```
Authorization: Bearer {seu-token}
```

### Endpoints Principais

#### Administradores
- `POST /administradores/login` - Login
- `GET /administradores` - Lista administradores (requer perfil Adm)
- `GET /administradores/{id}` - Busca administrador por ID (requer perfil Adm)
- `POST /administradores` - Cria novo administrador (requer perfil Adm)

#### Veículos
- `GET /veiculos` - Lista veículos
- `GET /veiculos/{id}` - Busca veículo por ID
- `POST /veiculos` - Cria novo veículo (requer perfil Adm ou Editor)
- `PUT /veiculos/{id}` - Atualiza veículo (requer perfil Adm)
- `DELETE /veiculos/{id}` - Remove veículo (requer perfil Adm)

## 🤝 Como Contribuir

1. Faça um Fork do projeto
2. Crie uma Branch para sua Feature (`git checkout -b feature/AmazingFeature`)
3. Faça o Commit das suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Faça o Push para a Branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Padrões de Código

- Siga as convenções de nomenclatura do C#
- Mantenha o código limpo e bem documentado
- Escreva testes unitários para novas funcionalidades
- Atualize a documentação quando necessário

## 🔄 Melhorias Futuras

- [ ] Implementar cache com Redis
- [ ] Adicionar logging estruturado
- [ ] Implementar rate limiting
- [ ] Adicionar testes de integração
- [ ] Implementar CI/CD
- [ ] Adicionar monitoramento com Application Insights
- [ ] Implementar versionamento da API
- [ ] Adicionar suporte a múltiplos idiomas
- [ ] Implementar sistema de notificações
- [ ] Adicionar documentação mais detalhada

## 🤝 Colaboradores

Agradecemos às seguintes pessoas que contribuíram para este projeto:

<table>
  <tr>
    <td align="center">
      <a href="#" title="defina o título do link">
        <img src="https://avatars.githubusercontent.com/u/144637977?s=400&u=f379d5610f40bc3b58d0d2bf2ab6bafb0e294cd8&v=4" width="100px;" alt="Foto do Savio no GitHub"/><br>
        <sub>
          <b>Sávio Fagundes</b>
        </sub>
      </a>
    </td>
  </tr>
</table>
## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## ✨ Agradecimentos

- [.NET](https://dotnet.microsoft.com/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [MySQL](https://www.mysql.com/)
- [Docker](https://www.docker.com/)
- [Swagger](https://swagger.io/)
