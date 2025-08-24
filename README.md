# README — Client (MVC) — Parte 1: Controllers, DTOs/ViewModels e Views (pareados com a API, sem autorização no Client)

Este documento descreve como implementar **Controllers, DTOs/ViewModels e Views** no repositório **Client**, consumindo endpoints do repositório **API**.  
**Sem uso de autorização no Client por enquanto** (iremos tratar autenticação/roles quando a estratégia estiver definida). O objetivo é **gerar telas funcionais** a partir de endpoints já expostos pela API.

---

## Conexão com a API — ApiBaseUrl (já configurado)
<details>
<summary>Ver detalhes</summary>

- O Client usa o valor de **ApiBaseUrl** definido em `appsettings.json`.  
- Este valor **já está configurado e funcionando**.

Exemplo (já existente no `appsettings.json` do Client):
```json
{
  "ApiBaseUrl": "https://localhost:7140/"
}
```

> [!NOTE]  
> Não é necessário alterar essa configuração. O Client já está apto a se conectar à API.
</details>

---

## Escopo do Client e Relação com a API
<details>
<summary>Ver escopo e princípios</summary>

- O **Client** é um aplicativo MVC que **renderiza páginas** e **consome endpoints da API** via `HttpClient` nomeado **"Api"**.  
- **Models de domínio** e regras de negócio residem na **API**.  
- No Client, crie:  
  - **DTOs de consumo**: classes que espelham a **resposta da API**.  
  - **ViewModels**: classes para **renderização** nas Views (podem agregar dados, mensagens de UI, paginação etc.).
- **Controllers do Client**:
  1) Chamam endpoints da API;  
  2) Transformam DTOs em ViewModels;  
  3) Retornam Views fortemente tipadas;  
  4) **Sem `[Authorize]` no Client** por enquanto.
</details>

---

## Padrão de Controller do Client pareado com endpoint da API
<details>
<summary>Ver instruções e exemplo</summary>

### Convenções
- Nome do controller: **`[Nome]Controller`** (termina com `Controller`).  
  Ex.: `UsersController` no Client para consumir `/api/users` na API.
- Pasta: `Controllers/`.

### Exemplo — Listagem básica
Arquivo: `Controllers/UsersController.cs`
```csharp
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Client.Models;

namespace Client.Controllers
{
    // Sem [Authorize] no Client por enquanto
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public UsersController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var client = _httpFactory.CreateClient("Api");

            // Substitua "api/users" pela rota real da API se necessário
            var users = await client.GetFromJsonAsync<List<UserViewModel>>("api/users");

            // Garanta uma lista válida para a View
            return View(users ?? new List<UserViewModel>());
        }

        // POST: /Users/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");

            // Substitua a rota conforme a API
            var resp = await client.DeleteAsync($"api/users/{id}");
            if (!resp.IsSuccessStatusCode)
                TempData["Error"] = "Failed to delete user.";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Users/SetAdmin
        // Exemplo de POST simples com body JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAdmin(Guid id, bool isAdmin)
        {
            var client = _httpFactory.CreateClient("Api");

            var resp = await client.PostAsJsonAsync($"api/users/{id}/set-admin", isAdmin);
            if (!resp.IsSuccessStatusCode)
                TempData["Error"] = "Failed to update admin status.";

            return RedirectToAction(nameof(Index));
        }
    }
}
```

> [!NOTE]  
> Este exemplo assume que a API oferece as rotas `GET api/users`, `DELETE api/users/{id}` e `POST api/users/{id}/set-admin`. Ajuste os caminhos conforme a sua API.
</details>

---

## DTOs (consumo da API) e ViewModels (renderização)
<details>
<summary>Ver como projetar classes no Client</summary>

- **DTOs**: correspondem ao **payload recebido** da API.  
- **ViewModels**: dados que a View realmente precisa (podem agrupar e adaptar o DTO).

Exemplo mínimo para a lista de usuários:  
Arquivo: `Models/UserViewModel.cs`
```csharp
namespace Client.Models
{
    public sealed class UserViewModel
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
```
> [!TIP]  
> Mantenha DTOs/ViewModels **enxutos** e **específicos** para a tela. Evite carregar campos que a View não usa.
</details>

---

## Criando a View da Action
<details>
<summary>Ver passo a passo para criar a View</summary>

### Passos
1. No **Controller**, garanta uma action pública que retorne `IActionResult` e `return View(model)`.
2. Clique com o botão direito em **cima do nome da action** (ex.: `Index`) → **Add View...**
3. Parâmetros da janela:  
   - **Nome da View** = o mesmo nome da action (ex.: `Index`).  
   - **Template** = `Empty` (ou outro se desejar scaffolding).  
   - **Clique em Add**.
4. O VS criará o arquivo no caminho:  
   ```
   Views/[NOME_DO_CONTROLLER]/[NOME_DA_ACTION].cshtml
   ```
5. Abra o `.cshtml` e defina o modelo fortemente tipado, exemplo:

```cshtml
@model List<Client.Models.UserViewModel>

<h1>Users</h1>

@if (TempData["Error"] is string err && !string.IsNullOrWhiteSpace(err))
{
    <div class="alert alert-danger">@err</div>
}

@if (Model.Count == 0)
{
    <p>No users found.</p>
}
else
{
    <table class="table table-hover align-middle">
        <thead class="table-light">
            <tr>
                <th>Email</th>
                <th>User Name</th>
                <th>Admin</th>
                <th class="text-end">Actions</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var user in Model)
            {
                <tr>
                    <td>@user.Email</td>
                    <td>@user.UserName</td>
                    <td>
                        <form asp-action="SetAdmin" method="post" class="d-inline">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="id" value="@user.Id" />
                            <input type="hidden" name="isAdmin" value="@(user.IsAdmin ? "false" : "true")" />
                            <button type="submit" class="btn btn-sm @(user.IsAdmin ? "btn-success" : "btn-outline-secondary")">
                                @(user.IsAdmin ? "Admin" : "Make Admin")
                            </button>
                        </form>
                    </td>
                    <td class="text-end">
                        <form asp-action="Delete" method="post" class="d-inline" onsubmit="return confirm('Delete this user?');">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="id" value="@user.Id" />
                            <button type="submit" class="btn btn-sm btn-danger">Delete</button>
                        </form>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}
```
</details>

# README — Client (MVC) — Parte 2: Layout, Bootstrap, Program.cs, Rotas, Erros e Checklist (sem autorização no Client)

Instruções para **layout**, **recursos estáticos**, **roteamento**, **boas práticas de views**, **tratamento de erros** e um **checklist** antes de rodar — mantendo o Client **sem autorização** por enquanto.

---

## Como as Views usam `_Layout.cshtml`
<details>
<summary>Ver explicação e boas práticas</summary>

- `_Layout.cshtml` (em `Views/Shared/`) define a estrutura base (cabeçalho, navegação, rodapé, scripts, estilos).  
- Views são renderizadas dentro de `@RenderBody()`; `_ViewStart.cshtml` normalmente aplica o layout.  
- Para forçar um layout específico em uma view:
  ```cshtml
  @{
      Layout = "_Layout";
  }
  ```

> [!WARNING]  
> Mudanças no `_Layout.cshtml` afetam todas as Views que o utilizam. Planeje e revise.
</details>

---

## Uso de Bootstrap nas Views
<details>
<summary>Ver instruções e recomendações</summary>

- Bootstrap é referenciado no layout (link/script).  
- Utilize classes (`.container`, `.row`, `.col-*`, `.btn`, `.table`, etc.).  
- **Não** altere arquivos em `wwwroot/lib/bootstrap`. Para customizações, use `wwwroot/css/site.css`.  
- Bootstrap Icons podem ser referenciados via CDN, se necessário.

> [!TIP]  
> Padronize botões, tabelas e títulos para consistência visual.
</details>

---

## `Program.cs` do Client — configuração base (sem autorização)
<details>
<summary>Ver pontos relevantes</summary>

- `AddControllersWithViews()` já está presente — Controllers/Views **funcionam sem ajustes**.  
- Sessão (`AddDistributedMemoryCache()` e `AddSession(...)`) está habilitada — útil para mensagens (`TempData`).  
- `AddHttpContextAccessor()` disponível — permite acesso a contexto em views/layouts.  
- `AddHttpClient("Api", ...)` — named client para chamadas à API, usando `ApiBaseUrl` do `appsettings.json`:
  ```csharp
  builder.Services.AddHttpClient("Api", client =>
  {
      client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
  });
  ```
- **Sem autenticação no Client por enquanto** (não usar `[Authorize]` em controllers ou checagens de role no layout).
</details>

---

## Rotas e convenções
<details>
<summary>Ver comportamento padrão</summary>

- Padrão do Client: `/{controller=Home}/{action=Index}/{id?}`.  
- Exemplo: `/Users/Index` (ou apenas `/Users`).  
- Na API (referência): `[Route("api/[controller]")]` → `/api/users` para `UsersController` na API.
</details>

---

## Tratamento de erros em chamadas à API
<details>
<summary>Ver padrões simples e previsíveis</summary>

- Após chamadas à API, verifique `IsSuccessStatusCode` e defina mensagens em `TempData` ou no ViewModel.  
- Para GET de listagem, evite quebrar a tela: retorne lista vazia e uma mensagem.

Exemplo:
```csharp
var client = _httpFactory.CreateClient("Api");
var response = await client.GetAsync("api/users");
if (!response.IsSuccessStatusCode)
{
    TempData["Error"] = $"Failed to load users ({(int)response.StatusCode}).";
    return View(new List<UserViewModel>());
}
var users = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
return View(users ?? new List<UserViewModel>());
```
</details>

---

## Boas práticas de Views
<details>
<summary>Ver recomendações</summary>

- Use **View fortemente tipada** (`@model`).  
- Reutilize com **Partial Views** e **View Components**.  
- Use **Tag Helpers** (`asp-for`, `asp-action`, `asp-controller`).  
- Validação cliente/servidor via **DataAnnotations** no **ViewModel**.  
- **Sem lógica de negócio** nas Views — apenas apresentação.
</details>

---

## Checklist antes de rodar
<details>
<summary>Ver lista final</summary>

1) API rodando e acessível (confirme rota `/api/users`).  
2) `ApiBaseUrl` já configurado em `appsettings.json` do Client.  
3) Controller do Client criado (ex.: `UsersController`) chamando a rota correta da API.  
4) View criada no caminho `Views/[Controller]/[Action].cshtml`.  
5) Build limpo: se necessário, `Build > Clean` e `Build > Rebuild` no Visual Studio.

[!IMPORTANT]  
> Para agora, **não use `[Authorize]` no Client** nem verificações de role no layout. Quando a estratégia de autenticação estiver definida (ex.: JWT), incluiremos essa parte.
</details>

