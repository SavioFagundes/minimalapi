using Infraestrutura;
using Dominio.DTOs;
using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Dominio.Interface;
using Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using Dominio.ModelViews;
using Dominio.Enums;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Adiciona o Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

app.MapPost("/administradores/login", ([FromBody] loginDto loginDto, IAdministradorServico administradorServico) => {
    if (administradorServico.Login(loginDto) != null)
    {
        return Results.Ok(new { message = "Login realizado com sucesso" });
    }
    return Results.Unauthorized();
}).WithTags("Administradores");

app.MapGet("/administradores/{id}",([FromRoute] int id,IAdministradorServico administradorServico) => {

    var administrador = administradorServico.BuscaPorId(id);
    if(administrador == null) return Results.NotFound();
    return Results.Ok(administrador);

}).WithTags("Administradores");

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
}).WithTags("Administradores");


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
}).WithTags("Administradores");


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

}).WithTags("Veiculos");

app.MapGet("/veiculos",([FromQuery] int pagina,IVeiculoServico veiculoServico) => {

    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);

}).WithTags("Veiculos");


app.MapGet("/veiculos/{id}",([FromRoute] int id,IVeiculoServico veiculoServico) => {

    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}",([FromRoute] int id,VeiculoDTO veiculoDto,IVeiculoServico veiculoServico) => {

    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    veiculo.Nome = veiculoDto.Nome;
    veiculo.Marca = veiculoDto.Marca;
    veiculo.Ano = veiculoDto.Ano;
    veiculoServico.Atualizar(veiculo);
    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}",([FromRoute] int id,IVeiculoServico veiculoServico) => {

    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    veiculoServico.Apagar(veiculo);
    return Results.NoContent();

}).WithTags("Veiculos");

app.Run();


