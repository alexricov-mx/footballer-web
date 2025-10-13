using FootballerWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Authentication services - conectando a API externa
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:8090");
});
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtClientService, JwtClientService>();
builder.Services.AddScoped<IAuthStateService, AuthStateService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<LigaService>();
builder.Services.AddScoped<EquipoService>();

// HttpClient para componentes Blazor
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:8090")
});

// Session and authentication
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
