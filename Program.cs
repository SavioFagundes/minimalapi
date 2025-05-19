using Infraestrutura;
using Dominio.DTOs;
using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Dominio.Interface;
using Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
var app = builder.Build();

app.MapGet("/", () => "aaaaaaaaa");

app.MapPost("/login", ([FromBody] loginDto loginDto, IAdministradorServico administradorServico) => {
    if (administradorServico.Login(loginDto) != null)
    {
        return Results.Ok(new { message = "Login realizado com sucesso" });
    }
    return Results.Unauthorized();
});

app.Run();


