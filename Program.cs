using Infraestrutura;
using Dominio.DTOs;
using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Dominio.Interface;
using Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using Dominio.ModelViews;



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

app.MapGet("/", () => Results.Json(new Home()));

app.MapPost("/administradores/login", ([FromBody] loginDto loginDto, IAdministradorServico administradorServico) => {
    if (administradorServico.Login(loginDto) != null)
    {
        return Results.Ok(new { message = "Login realizado com sucesso" });
    }
    return Results.Unauthorized();
});


app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDto, IVeiculoServico veiculoServico) => {

    var veiculo = new Veiculo{
        Nome = veiculoDto.Nome,
        Marca = veiculoDto.Marca,
        Ano = veiculoDto.Ano
    };
    veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);

});

app.Run();


