using Microsoft.EntityFrameworkCore;
using SistemaPDVAssadosTiaDri.Models;
using SistemaPDVAssadosTiaDri.Services;

var builder = WebApplication.CreateBuilder(args);

// Configura o DbContext com a string de conexão
builder.Services.AddDbContext<PDVContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PDVConnection")));

// Adiciona suporte para MVC e Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Adiciona suporte a sessões
builder.Services.AddDistributedMemoryCache(); // Necessário para armazenar dados da sessão na memória
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de expiração da sessão
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Adiciona serviços personalizados
builder.Services.AddScoped<VendaService>();
builder.Services.AddHttpContextAccessor(); // Necessário para acessar HttpContext na aplicação

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

app.UseSession(); // Habilita o uso de sessões na aplicação

app.UseAuthorization();

// Mapeia os controladores MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Produtos}/{action=Index}/{id?}");

// Mapeia as páginas Razor
app.MapRazorPages();

app.Run();
