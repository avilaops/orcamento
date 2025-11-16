# 🤖 Instruções para GitHub Copilot - Roncav Budget

## 📋 Visão Geral do Projeto

**Roncav Budget** é uma aplicação .NET MAUI multiplataforma para gerenciamento de orçamentos e controle financeiro.

### Tecnologias
- **.NET MAUI** - Framework multiplataforma
- **C#** - Linguagem principal
- **XAML** - Interface de usuário
- **SQLite** - Banco de dados local

### Plataformas Suportadas
- ✅ Windows (WinUI)
- ✅ Android
- ✅ iOS
- ✅ macOS

---

## 🔍 Análise e Revisão de Código

### Antes de Fazer Alterações

1. **Ler a arquitetura existente**
   - Verificar `Readme_Roncav_Budget.md`
   - Revisar `Como_Executar.md` e `Executar_Agora.md`
   - Consultar `Guia_Visual_Completo.md` para entender o design

2. **Analisar dependências**
   - Verificar `Roncav_Budget.sln` para estrutura do solution
   - Revisar arquivos `.csproj` de cada projeto
   - Identificar NuGet packages instalados

3. **Verificar compilação atual**
   ```powershell
   dotnet build Roncav_Budget.sln
   ```

### Padrões de Código

- **MVVM Pattern**: Seguir Model-View-ViewModel
- **Naming Conventions**:
  - Views: `*Page.xaml`
  - ViewModels: `*ViewModel.cs`
  - Models: `*Model.cs`
  - Services: `*Service.cs`

- **Async/Await**: Sempre usar para operações I/O
- **Dependency Injection**: Registrar serviços no `MauiProgram.cs`

---

## 🌐 Integração com Ecossistema Avila

### APIs e Serviços Centralizados

Este projeto faz parte do **ecossistema Avila** e deve seguir os padrões de integração corporativos:

#### 🔗 APIs Centralizadas da Avila

**Base URLs por Ambiente:**
- **Produção**: `https://api.avila.inc`
- **Staging**: `https://api-staging.avila.inc`
- **Desenvolvimento**: `https://localhost:7000` ou Cloudflare Tunnel

**Serviços Disponíveis:**

1. **Authentication API** (`/auth`)
   - `POST /auth/login` - Autenticação de usuários
   - `POST /auth/refresh` - Refresh de tokens JWT
   - `POST /auth/logout` - Invalidar sessão
   - `GET /auth/validate` - Validar token

2. **User Management API** (`/users`)
   - `GET /users/profile` - Perfil do usuário
   - `PUT /users/profile` - Atualizar perfil
   - `GET /users/permissions` - Permissões do usuário

3. **Sync API** (`/sync`)
   - `POST /sync/upload` - Enviar dados locais para nuvem
   - `GET /sync/download` - Baixar dados da nuvem
   - `GET /sync/status` - Status de sincronização
   - `POST /sync/resolve-conflicts` - Resolver conflitos

4. **Analytics API** (`/analytics`)
   - `POST /analytics/events` - Registrar eventos de uso
   - `POST /analytics/errors` - Reportar erros
   - `GET /analytics/insights` - Obter insights de dados

### 📡 Implementação de Integração

#### 1. Configuração de Serviço HTTP

```csharp
// Services/AvilaApiService.cs
public class AvilaApiService
{
    private readonly HttpClient _httpClient;
    private readonly ISecureStorage _secureStorage;

    public AvilaApiService(HttpClient httpClient, ISecureStorage secureStorage)
    {
        _httpClient = httpClient;
        _secureStorage = secureStorage;

        // Base URL configurável por ambiente
        #if DEBUG
            _httpClient.BaseAddress = new Uri("https://localhost:7000");
        #else
            _httpClient.BaseAddress = new Uri("https://api.avila.inc");
        #endif
    }

    public async Task<bool> AuthenticateAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/auth/login", new
        {
            email,
            password,
            clientId = "roncav-budget",
            platform = DeviceInfo.Platform.ToString()
        });

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadFromJsonAsync<AuthToken>();
            await _secureStorage.SetAsync("auth_token", token.AccessToken);
            await _secureStorage.SetAsync("refresh_token", token.RefreshToken);
            return true;
        }

        return false;
    }
}
```

#### 2. Registrar Serviço no MauiProgram.cs

```csharp
// MauiProgram.cs
builder.Services.AddHttpClient<AvilaApiService>(client =>
{
    client.DefaultRequestHeaders.Add("X-Client-App", "Roncav-Budget");
    client.DefaultRequestHeaders.Add("X-Client-Version", AppInfo.VersionString);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddSingleton<ISyncService, SyncService>();
builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();
```

#### 3. Sincronização de Dados

```csharp
// Services/SyncService.cs
public class SyncService : ISyncService
{
    private readonly AvilaApiService _apiService;
    private readonly ILocalDatabase _localDb;

    public async Task<SyncResult> SyncAsync()
    {
        try
        {
            // 1. Upload dados locais modificados
            var localChanges = await _localDb.GetPendingChangesAsync();
            if (localChanges.Any())
            {
                await _apiService.UploadChangesAsync(localChanges);
            }

            // 2. Download dados do servidor
            var serverData = await _apiService.GetUpdatesAsync();
            await _localDb.MergeServerDataAsync(serverData);

            // 3. Resolver conflitos
            var conflicts = await _localDb.GetConflictsAsync();
            if (conflicts.Any())
            {
                await ResolveConflictsAsync(conflicts);
            }

            return SyncResult.Success();
        }
        catch (Exception ex)
        {
            // Log para Analytics API
            await _apiService.LogErrorAsync(ex);
            return SyncResult.Failed(ex.Message);
        }
    }
}
```

### 🏢 Governança e Cultura Corporativa

#### Padrões Obrigatórios da Avila

1. **Autenticação e Autorização**
   - ✅ Sempre usar JWT tokens da API central
   - ✅ Implementar refresh token automático
   - ✅ Validar permissões antes de operações críticas
   - ❌ NUNCA criar sistema de auth próprio

2. **Logging e Telemetria**
   - ✅ Todos os erros devem ir para Analytics API
   - ✅ Registrar eventos de uso importantes (login, sync, transações)
   - ✅ Incluir contexto: userId, deviceId, appVersion
   - ✅ Usar níveis: Debug, Info, Warning, Error, Critical

3. **Tratamento de Dados**
   - ✅ Dados sensíveis SEMPRE criptografados (SecureStorage)
   - ✅ LGPD/GDPR compliance: permitir exportação e exclusão
   - ✅ Sincronização bidirecional quando online
   - ✅ Modo offline funcional (local-first)

4. **UI/UX Consistente**
   - ✅ Seguir Material Design 3 / Fluent Design
   - ✅ Cores da marca Avila:
     - Primary: `#1E88E5` (Azul Avila)
     - Secondary: `#FF6F00` (Laranja Destaque)
     - Error: `#D32F2F`
     - Success: `#388E3C`
   - ✅ Fontes: Segoe UI (Windows), SF Pro (iOS), Roboto (Android)
   - ✅ Ícones: Material Icons ou Fluent UI Icons

5. **Versionamento e Deploy**
   - ✅ Semantic Versioning: `MAJOR.MINOR.PATCH`
   - ✅ Tag git para cada release: `v1.2.3`
   - ✅ CHANGELOG.md atualizado
   - ✅ Deploy em staging antes de produção

#### 📊 Estrutura de Dados Padrão Avila

```csharp
// Models/Base/AvilaEntity.cs
public abstract class AvilaEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } // UserId
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public long Version { get; set; } = 1; // Para controle de conflitos
    public bool IsSynced { get; set; } = false;
}
```

#### 🔄 Ciclo de Sincronização

```
┌─────────────────────────────────────────────────────────────┐
│                    FLUXO DE SINCRONIZAÇÃO                   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  App Local                    API Avila                     │
│  ┌─────────┐                 ┌─────────┐                   │
│  │ SQLite  │────Upload────▶  │ MongoDB │                   │
│  │         │                 │ Cosmos  │                   │
│  │         │◀───Download───  │         │                   │
│  └─────────┘                 └─────────┘                   │
│      │                            │                         │
│      │  Conflito?                 │                         │
│      └────────┬───────────────────┘                         │
│               ▼                                             │
│         Resolução:                                          │
│         • Last-Write-Wins (padrão)                          │
│         • Server-Wins (dados críticos)                      │
│         • Manual (UI de resolução)                          │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### 🔐 Segurança Corporativa

#### Secrets Management

```csharp
// appsettings.json (NÃO commitar valores reais)
{
  "AvilaApi": {
    "BaseUrl": "https://api.avila.inc",
    "ClientId": "roncav-budget-{ENV}",
    "Timeout": 30
  },
  "Features": {
    "EnableSync": true,
    "EnableAnalytics": true,
    "OfflineMode": true
  }
}

// Usar User Secrets em desenvolvimento
// dotnet user-secrets set "AvilaApi:ApiKey" "dev-key-xxx"
```

#### Azure Key Vault (Produção)

```csharp
// Program.cs - Configuração para produção
#if RELEASE
builder.Configuration.AddAzureKeyVault(
    new Uri("https://avila-keyvault.vault.azure.net/"),
    new DefaultAzureCredential()
);
#endif
```

### 📋 Checklist de Conformidade Avila

Antes de qualquer deploy, garantir:

- [ ] **Integração com Auth API** funcionando
- [ ] **Sync bidirecional** implementado e testado
- [ ] **Logs enviados para Analytics API**
- [ ] **Tratamento de erros** global implementado
- [ ] **Modo offline** funcional
- [ ] **UI/UX** segue padrões da marca Avila
- [ ] **Dados sensíveis** criptografados
- [ ] **Compliance LGPD**: exportação/exclusão de dados
- [ ] **Versionamento** correto (tag git + CHANGELOG)
- [ ] **Testes** em staging antes de produção

### 🎨 Design System Avila

```xml
<!-- Resources/Styles/AvilaColors.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui">
    <!-- Cores Primárias -->
    <Color x:Key="AvilaPrimary">#1E88E5</Color>
    <Color x:Key="AvilaSecondary">#FF6F00</Color>

    <!-- Status -->
    <Color x:Key="AvilaSuccess">#388E3C</Color>
    <Color x:Key="AvilaWarning">#F57C00</Color>
    <Color x:Key="AvilaError">#D32F2F</Color>
    <Color x:Key="AvilaInfo">#1976D2</Color>

    <!-- Neutros -->
    <Color x:Key="AvilaTextPrimary">#212121</Color>
    <Color x:Key="AvilaTextSecondary">#757575</Color>
    <Color x:Key="AvilaBackground">#FAFAFA</Color>
    <Color x:Key="AvilaSurface">#FFFFFF</Color>
</ResourceDictionary>
```

---

## 🚀 Deploy e Publicação

### Pré-requisitos de Deploy

1. **Verificar configuração de build**
   ```powershell
   # Verificar se todos os projetos compilam
   dotnet build -c Release
   ```

2. **Testar em modo Release**
   ```powershell
   # Windows
   dotnet build Roncav_Budget.winui/Roncav_Budget.winui.csproj -c Release

   # Android
   dotnet build Roncav_Budget.droid/Roncav_Budget.droid.csproj -c Release
   ```

### Deploy por Plataforma

#### 🪟 Windows (WinUI)

```powershell
# Publicar para Windows
dotnet publish Roncav_Budget.winui/Roncav_Budget.winui.csproj `
  -c Release `
  -f net8.0-windows10.0.19041.0 `
  -p:RuntimeIdentifierOverride=win10-x64 `
  -p:WindowsPackageType=MSIX `
  -p:GenerateAppxPackageOnBuild=true
```

**Arquivos gerados**: `Roncav_Budget.winui/AppPackages/`

#### 🤖 Android

```powershell
# Publicar APK
dotnet publish Roncav_Budget.droid/Roncav_Budget.droid.csproj `
  -c Release `
  -f net8.0-android `
  -p:AndroidPackageFormat=apk

# Publicar AAB (Google Play)
dotnet publish Roncav_Budget.droid/Roncav_Budget.droid.csproj `
  -c Release `
  -f net8.0-android `
  -p:AndroidPackageFormat=aab
```

**Arquivos gerados**: `Roncav_Budget.droid/bin/Release/`

#### 🍎 iOS

```powershell
# Publicar para iOS (requer macOS)
dotnet publish Roncav_Budget.ios/Roncav_Budget.ios.csproj `
  -c Release `
  -f net8.0-ios
```

**Nota**: Deploy iOS requer certificados Apple Developer

#### 💻 macOS

```powershell
# Publicar para macOS
dotnet publish Roncav_Budget.mac/Roncav_Budget.mac.csproj `
  -c Release `
  -f net8.0-maccatalyst
```

---

## ✅ Checklist de Deploy

Antes de fazer deploy, verificar:

- [ ] **Compilação limpa sem warnings**
  ```powershell
  dotnet clean
  dotnet build -c Release --no-incremental
  ```

- [ ] **Versão atualizada**
  - Incrementar versão em `Directory.Build.props` ou `.csproj`
  - Formato: `<ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>`

- [ ] **Testes executados**
  ```powershell
  dotnet test
  ```

- [ ] **Assets e recursos verificados**
  - Ícones da aplicação
  - Splash screens
  - Imagens e fontes

- [ ] **Configurações de release**
  - `appsettings.json` para produção
  - Connection strings corretas
  - API keys configuradas

- [ ] **Assinatura de código** (se aplicável)
  - Certificado Windows para MSIX
  - Keystore Android configurado
  - Perfil de provisionamento iOS

- [ ] **Integração Avila validada**
  - Auth API conectada
  - Sync funcionando
  - Analytics configurado
  - Cores/fontes da marca aplicadas

---

## 🐛 Solução de Problemas Comuns

### Erro: "Workload not installed"
```powershell
# Instalar workloads MAUI
dotnet workload install maui
dotnet workload install android
dotnet workload install ios
dotnet workload install maccatalyst
```

### Erro: "SDK not found"
- Verificar se .NET 8 SDK está instalado: `dotnet --version`
- Instalar: https://dotnet.microsoft.com/download

### Erro de dependências NuGet
```powershell
# Limpar cache e restaurar
dotnet nuget locals all --clear
dotnet restore Roncav_Budget.sln
```

### Build lento ou travando
```powershell
# Limpar bin/obj
Get-ChildItem -Recurse -Directory -Filter "bin" | Remove-Item -Recurse -Force
Get-ChildItem -Recurse -Directory -Filter "obj" | Remove-Item -Recurse -Force
dotnet restore
```

---

## 📝 Comandos Úteis

### Desenvolvimento
```powershell
# Rodar no Windows
dotnet run --project Roncav_Budget.winui

# Rodar no Android (emulador)
dotnet build -t:Run -f net8.0-android

# Listar dispositivos Android
adb devices

# Hot Reload ativado
dotnet watch run --project Roncav_Budget.winui
```

### Análise de Código
```powershell
# Análise de código
dotnet format --verify-no-changes
dotnet build /p:TreatWarningsAsErrors=true

# Verificar estilo
dotnet format --severity info
```

### Informações do Projeto
```powershell
# Ver workloads instalados
dotnet workload list

# Ver SDKs instalados
dotnet --list-sdks

# Ver runtimes instalados
dotnet --list-runtimes
```

---

## 🎯 Workflow de Revisão Recomendado

### 1. Análise Inicial
- Ler todos os `.md` da raiz do projeto
- Mapear estrutura de pastas e projetos
- Identificar padrões de código existentes

### 2. Compilação e Testes
- Compilar em Debug e Release
- Executar testes automatizados
- Testar em pelo menos 2 plataformas

### 3. Revisão de Código
- Verificar SOLID principles
- Garantir exception handling adequado
- Validar async/await patterns
- Checar memory leaks potenciais
- **Validar integração com APIs Avila**
- **Verificar conformidade com padrões corporativos**

### 4. Otimizações
- Analisar performance com profiler
- Otimizar queries ao banco de dados
- Reduzir tamanho do pacote final
- Implementar lazy loading onde aplicável
- **Minimizar chamadas à API (cache local)**
- **Otimizar sincronização (delta sync)**

### 5. Deploy
- Seguir checklist de deploy acima
- Gerar builds para todas as plataformas
- Documentar breaking changes
- Atualizar CHANGELOG.md (se existir)
- **Testar em staging.avila.inc primeiro**
- **Validar com equipe de QA**

---

## 📚 Referências

- [.NET MAUI Docs](https://learn.microsoft.com/dotnet/maui/)
- [XAML Controls](https://learn.microsoft.com/dotnet/maui/user-interface/controls/)
- [Publishing Guide](https://learn.microsoft.com/dotnet/maui/deployment/)
- [Best Practices](https://learn.microsoft.com/dotnet/maui/fundamentals/best-practices)
- **[Avila API Documentation](https://api.avila.inc/docs)** - Documentação completa das APIs
- **[Avila Design System](https://design.avila.inc)** - Guia de UI/UX corporativo
- **[Avila Developer Portal](https://dev.avila.inc)** - Portal do desenvolvedor

---

## 🔐 Segurança

- **Nunca commitar**:
  - API keys em código
  - Senhas ou tokens
  - Keystores/certificados privados

- **Usar**:
  - User Secrets para desenvolvimento
  - Azure Key Vault para produção
  - Variáveis de ambiente para CI/CD

---

## 📞 Suporte

Para dúvidas específicas do projeto:
1. Verificar documentação em `/docs` (se existir)
2. Consultar arquivos `.md` na raiz
3. Revisar issues no repositório GitHub

---

**Última atualização**: 2025-11-16
**Versão das instruções**: 1.0
