using UserService.Configurations;
using UserService.Repositories;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);


var token = Environment.GetEnvironmentVariable("VAULT_TOKEN");
var endPoint = Environment.GetEnvironmentVariable("VaultEndPoint");
// Hent ConnectionString fra Vault
var vaultRepository = new VaultRepository(endPoint, token);
var connectionString = await vaultRepository.GetSecretAsync("ConnectionString");
Console.WriteLine($"ConnectionString er: {connectionString}");

// Tilføj ConnectionString til konfigurationen
builder.Configuration.AddInMemoryCollection(new[]
{
    new KeyValuePair<string, string>("MongoDbSettings:ConnectionString", connectionString)
});

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(typeof(MongoRepository<>)); // Registrer repository før controllerne

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
