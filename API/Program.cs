// Configuração da API Minimal usando .NET 9
// Este arquivo contém a configuração principal da API, incluindo autenticação JWT, endpoints e serviços

using Infraestrutura;
using Dominio.DTOs;
using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Dominio.Interface;
using Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using Dominio.ModelViews;
using Dominio.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Configuração da chave JWT para autenticação
var key = builder.Configuration.GetSection("Jwt")["Key"] ?? "12345678901234567890123456789012";

// Configuração da autenticação JWT
builder.Services.AddAuthentication(options =>{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>{
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Configuração da autorização
builder.Services.AddAuthorization();

// Registro dos serviços na injeção de dependência
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

// Configuração do contexto do banco de dados MySQL
builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Configuração do Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new string[] {}
        }
    });
});

var app = builder.Build();

// Configuração do Swagger em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Função para gerar token JWT
string GerarTokenJwt(Administrador administrador){
    if(string.IsNullOrEmpty(key)) return string.Empty;
    var ssecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(ssecurityKey, SecurityAlgorithms.HmacSha256);
    var claims = new List<Claim>{
        new Claim(ClaimTypes.Email, administrador.Email),
        new Claim(ClaimTypes.Role, administrador.Perfil),
        new Claim("Perfil", administrador.Perfil)
    };
    var token = new JwtSecurityToken(
        expires: DateTime.Now.AddDays(1),
        claims: claims,
        signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}

// Endpoint da página inicial
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

// Endpoints de Administradores
// Login de administrador
app.MapPost("/administradores/login", ([FromBody] loginDto loginDto, IAdministradorServico administradorServico) => {
    var adm = administradorServico.Login(loginDto);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdministradorLogado{
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");

// Buscar administrador por ID
app.MapGet("/administradores/{id}",([FromRoute] int id,IAdministradorServico administradorServico) => {
    var administrador = administradorServico.BuscaPorId(id);
    if(administrador == null) return Results.NotFound();
    return Results.Ok(administrador);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Administradores");

// Listar todos os administradores com paginação
app.MapGet("/administradores", ([FromQuery] int pagina, IAdministradorServico administradorServico) => {
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach(var adm in administradores)
    {
        adms.Add(new AdministradorModelView{
            Id = adm.Id,
            Nome = adm.Nome,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Administradores");

// Validação de dados do administrador
ErrosDeValidacao ValidarAdministrador(AdministradorDTO administradorDto)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administradorDto.Email))
    {
        validacao.Mensagens.Add("O email do administrador é obrigatório");
    }

    return validacao;
}

// Criar novo administrador
app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDto, IAdministradorServico administradorServico) => {
    var validacao = ValidarAdministrador(administradorDto);
    if(validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    if(string.IsNullOrEmpty(administradorDto.Senha))
    {
        validacao.Mensagens.Add("A senha do administrador é obrigatória");
    }
    if(administradorDto.Perfil == null)
    {
        validacao.Mensagens.Add("O perfil do administrador é obrigatório");
    }
    if(validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    var administrador = new Administrador{
        Email = administradorDto.Email,
        Senha = administradorDto.Senha,
        Perfil = administradorDto.Perfil.Value.ToString()
    };
    administradorServico.Incluir(administrador);
    return Results.Created($"/administrador/{administrador.Id}", administrador);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Administradores");

// Validação de dados do veículo
ErrosDeValidacao ValidarVeiculo(VeiculoDTO veiculoDto){
    var validacao = new ErrosDeValidacao{
        Mensagens = new List<string>()
    };

    if(string.IsNullOrEmpty(veiculoDto.Nome))
    {
        validacao.Mensagens.Add("O nome do veiculo é obrigatório");
    }
    if(string.IsNullOrEmpty(veiculoDto.Marca))
    {
        validacao.Mensagens.Add("A marca do veiculo é obrigatória");
    }
    if(veiculoDto.Ano <= 0)
    {
        validacao.Mensagens.Add("O ano do veiculo é obrigatório");
    }

    return validacao;
}

// Endpoints de Veículos
// Criar novo veículo
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDto, IVeiculoServico veiculoServico) => {
    var validacao = ValidarVeiculo(veiculoDto);
    if(validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo{
        Nome = veiculoDto.Nome,
        Marca = veiculoDto.Marca,
        Ano = veiculoDto.Ano
    };
    veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm,Editor"}).WithTags("Veiculos");

// Listar todos os veículos com paginação
app.MapGet("/veiculos",([FromQuery] int pagina,IVeiculoServico veiculoServico) => {
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veiculos");

// Buscar veículo por ID
app.MapGet("/veiculos/{id}",([FromRoute] int id,IVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    return Results.Ok(veiculo);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm,Editor"}).WithTags("Veiculos");

// Atualizar veículo existente
app.MapPut("/veiculos/{id}",([FromRoute] int id,VeiculoDTO veiculoDto,IVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    veiculo.Nome = veiculoDto.Nome;
    veiculo.Marca = veiculoDto.Marca;
    veiculo.Ano = veiculoDto.Ano;
    veiculoServico.Atualizar(veiculo);
    return Results.Ok(veiculo);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Veiculos");

// Excluir veículo
app.MapDelete("/veiculos/{id}",([FromRoute] int id,IVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    veiculoServico.Apagar(veiculo);
    return Results.NoContent();
}).RequireAuthorization().WithTags("Veiculos");

app.Run();


