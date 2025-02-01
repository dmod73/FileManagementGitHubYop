using FileManagementGitHub.Components;
using FileManagementGitHub.Services; // Se corrigió el namespace

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Inyectar el servicio de archivos
builder.Services.AddSingleton<FileService>();

var app = builder.Build();

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
