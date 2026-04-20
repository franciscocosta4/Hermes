using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MeuProjetoMvc.Data;

var builder = WebApplication.CreateBuilder(args);

// String de ligação ao PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configurar DbContext com PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configurar Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Regras de password (ajusta conforme necessário)
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// IMPORTANTE: ordem correta
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity UI
app.MapRazorPages();

app.Run();