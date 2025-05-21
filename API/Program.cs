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

var key = builder.Configuration.GetSection("Jwt")["Key"] ?? "12345678901234567890123456789012";

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

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Adiciona o Swagger
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

// Configura o Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

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

app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

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

app.MapGet("/administradores/{id}",([FromRoute] int id,IAdministradorServico administradorServico) => {

    var administrador = administradorServico.BuscaPorId(id);
    if(administrador == null) return Results.NotFound();
    return Results.Ok(administrador);

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Administradores");

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

app.MapGet("/veiculos",([FromQuery] int pagina,IVeiculoServico veiculoServico) => {

    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);

}).RequireAuthorization().WithTags("Veiculos");


app.MapGet("/veiculos/{id}",([FromRoute] int id,IVeiculoServico veiculoServico) => {

    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    return Results.Ok(veiculo);

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm,Editor"}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}",([FromRoute] int id,VeiculoDTO veiculoDto,IVeiculoServico veiculoServico) => {

    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    veiculo.Nome = veiculoDto.Nome;
    veiculo.Marca = veiculoDto.Marca;
    veiculo.Ano = veiculoDto.Ano;
    veiculoServico.Atualizar(veiculo);
    return Results.Ok(veiculo);

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}",([FromRoute] int id,IVeiculoServico veiculoServico) => {

    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    veiculoServico.Apagar(veiculo);
    return Results.NoContent();

}).RequireAuthorization().WithTags("Veiculos");

app.Run();


