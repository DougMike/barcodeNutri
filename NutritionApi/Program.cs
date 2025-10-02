using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using NutritionApi.Data;
using NutritionApi.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Banco de dados (SQLite) ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=products.db")
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

// --- HttpClient e ProductService ---
builder.Services.AddHttpClient<ProductService>();
builder.Services.AddScoped<ProductService>();

// --- Swagger sempre ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CORS para Angular (ou frontend) ---
builder.Services.AddCors(o => o.AddPolicy("AllowAngular", p => p
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

// --- Aplica migrations automaticamente ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// --- Habilita Swagger sempre (não depende do ambiente) ---
app.UseSwagger();
app.UseSwaggerUI();

// --- CORS ---
app.UseCors("AllowAngular");

// --- Endpoint público ---
app.MapGet("/products/{barcode}", async (string barcode, ProductService productService) =>
{
    var product = await productService.GetProductAsync(barcode);
    return product != null ? Results.Ok(product) : Results.NotFound(new { message = "Produto não encontrado" });
});

// --- Rodar app ---
app.Run();
