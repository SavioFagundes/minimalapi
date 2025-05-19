var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "aaaaaaaaa");

app.MapPost("/login", (loginDto loginDto) => {
    if (loginDto.Email == "adm@teste.com" && loginDto.Password == "123456")
    {
        return Results.Ok(new { message = "Login realizado com sucesso" });
    }
    return Results.Unauthorized();
});

app.Run();


