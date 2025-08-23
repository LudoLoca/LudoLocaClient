// Configuração principal do projeto cliente MVC (.NET 8)
// Este arquivo define todos os serviços essenciais, middlewares e integrações necessárias para o funcionamento do frontend.

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte a controllers e views (MVC padrão)
builder.Services.AddControllersWithViews();

// Configuração de sessão em memória (apenas para desenvolvimento)
// Permite uso de TempData, autenticação baseada em sessão, etc.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de expiração da sessão
    options.Cookie.HttpOnly = true;                 // Cookie não acessível via JS
    options.Cookie.IsEssential = true;              // Necessário para funcionamento do app
});

// Permite acesso ao HttpContext em views e layouts (ex: autenticação, sessão)
builder.Services.AddHttpContextAccessor();

// Configuração do HttpClient para chamadas à API
// Usa o valor de ApiBaseUrl definido no appsettings.json ou variáveis de ambiente
builder.Services.AddHttpClient("Api", client =>
{
    // ATENÇÃO: Certifique-se de que "ApiBaseUrl" está definido corretamente no appsettings.json
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
    // TODO: Adicionar tratamento para caso ApiBaseUrl não esteja definido
});

// Configuração de autenticação baseada em cookie para o MVC (não utiliza banco de dados ou JWT)
// Apenas para proteger rotas do lado do cliente, não interfere na autenticação da API
builder.Services.AddAuthentication("AppCookie")
    .AddCookie("AppCookie", o =>
    {
        o.LoginPath = "/Account/Login";
        o.LogoutPath = "/Account/Logout";
        o.AccessDeniedPath = "/Account/AccessDenied";
        o.Cookie.HttpOnly = true;
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        o.SlidingExpiration = true;
    });

// Adiciona suporte a autorização (usado com [Authorize] em controllers)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configuração de tratamento de erros e HSTS para produção
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redireciona HTTP para HTTPS
app.UseHttpsRedirection();

// Habilita arquivos estáticos (wwwroot, css, js, imagens)
app.UseStaticFiles();

// Define o roteamento padrão do MVC
app.UseRouting();

// Habilita sessão (deve vir antes de autenticação/autorização)
app.UseSession();

// Habilita autenticação baseada em cookie
app.UseAuthentication();

// Habilita autorização ([Authorize] em controllers)
app.UseAuthorization();

// Define a rota padrão: Home/Index/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//debug
