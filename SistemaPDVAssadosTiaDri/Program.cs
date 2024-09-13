using Microsoft.EntityFrameworkCore;
using SistemaPDVAssadosTiaDri.Models;
using SistemaPDVAssadosTiaDri.Services; // Certifique-se de que o namespace do VendaService esteja correto

var builder = WebApplication.CreateBuilder(args);

// Configura o DbContext com a string de conexão
builder.Services.AddDbContext<PDVContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PDVConnection")));

// Adiciona suporte para MVC e Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


builder.Services.AddScoped<VendaService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configura o pipeline de requisições HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Mapeia os controladores MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Produtos}/{action=Index}/{id?}");

// Mapeia as páginas Razor
app.MapRazorPages();

app.Run();
