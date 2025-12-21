# 🚀 COMO EXECUTAR O RONCAV BUDGET - WINUI

## ✅ Status do Projeto
- ✔️ Arquivo de solução configurado com plataformas x64, x86, ARM64
- ✔️ Projeto WinUI compilado com sucesso (Debug|x64)
- ✔️ Executável gerado em: `Roncav_Budget.winui\bin\x64\Debug\net9.0-windows10.0.19041.0\Roncav_Budget.winui.exe`
- ✔️ Configurações de startup preparadas

---

## 🎯 MÉTODO 1: Usando o Script Automático (RECOMENDADO)

1. **Execute o arquivo batch:**
   ```
   ConfigurarEExecutar.bat
   ```
   
   Este script irá:
   - Limpar todo o cache do Visual Studio
   - Deletar pastas bin/obj
   - Restaurar pacotes NuGet
   - Fazer build do projeto WinUI
   - Exibir instruções finais

2. **Depois que o script terminar:**
   - Abra o **Visual Studio 2026**
   - Abra a solução `Roncav_Budget.sln`
   - Na barra de ferramentas:
     - **Configuration:** `Debug`
     - **Platform:** `x64`
   - Na **Solution Explorer**:
     - Clique com botão direito em `Roncav_Budget.winui`
     - Selecione **"Set as Startup Project"**
   - Pressione **F5** ou clique no botão **▶ Start**

---

## 🔧 MÉTODO 2: Manual no Visual Studio

### Passo 1: Fechar o Visual Studio (se estiver aberto)

### Passo 2: Limpar cache (IMPORTANTE)
1. Delete a pasta `.vs` na raiz da solução
2. Delete as pastas `bin` e `obj` em:
   - `Roncav_Budget.winui\bin`
   - `Roncav_Budget.winui\obj`
   - `Roncav_Budget\bin`
   - `Roncav_Budget\obj`

### Passo 3: Reabrir o Visual Studio
1. Abra `Roncav_Budget.sln`

### Passo 4: Configurar a Plataforma
1. Na barra de ferramentas do Visual Studio:
   - **Configuration:** Selecione `Debug`
   - **Platform:** Selecione `x64` (NÃO use "Any CPU" ou "arm64")

### Passo 5: Definir Startup Project
1. Na **Solution Explorer**
2. Clique com botão direito em `Roncav_Budget.winui`
3. Selecione **"Set as Startup Project"**
4. O projeto deve aparecer em **negrito**

### Passo 6: Executar
1. Pressione **F5**
2. OU clique no botão **▶ Roncav_Budget.winui** na barra de ferramentas

---

## ⚠️ TROUBLESHOOTING

### Erro: "Could not find file '...arm64ec\Release\...'"
**Causa:** Visual Studio está tentando usar configuração antiga/incorreta

**Solução:**
1. Feche o Visual Studio
2. Delete a pasta `.vs` completamente
3. Execute o script `ConfigurarEExecutar.bat`
4. Reabra o Visual Studio
5. Configure manualmente: `Debug | x64`
6. Set as Startup Project novamente

### Erro: "Select Startup Item..."
**Causa:** Nenhum projeto de startup foi definido

**Solução:**
1. Na Solution Explorer
2. Clique com botão direito em `Roncav_Budget.winui`
3. Selecione "Set as Startup Project"

### Erro: Build falha ou falta DLLs
**Solução:**
```powershell
dotnet clean Roncav_Budget.sln
dotnet restore Roncav_Budget.sln
dotnet build "Roncav_Budget.winui\Roncav_Budget.winui.csproj" -c Debug /p:Platform=x64
```

---

## 📋 Arquivos de Configuração Criados

Os seguintes arquivos foram criados/modificados para facilitar a execução:

1. **Roncav_Budget.sln** - Atualizado com configurações de plataforma x64, x86, ARM64
2. **Roncav_Budget.winui\Roncav_Budget.winui.csproj.user** - Configurações de debug
3. **ConfigurarEExecutar.bat** - Script de configuração automática
4. **COMO_EXECUTAR.md** - Este arquivo de instruções

---

## 🎮 Plataformas Suportadas

- ✅ **x64** (Recomendado - Windows 64-bit moderno)
- ✅ **x86** (Windows 32-bit)
- ✅ **ARM64** (Dispositivos ARM como Surface Pro X)

---

## 📦 Estrutura do Projeto

```
Orcamento-Familiar/
├── Roncav_Budget/              # Projeto principal .NET MAUI
├── Roncav_Budget.winui/        # ⭐ Projeto WinUI (STARTUP)
├── Roncav_Budget.droid/        # Android
├── Roncav_Budget.ios/          # iOS
├── Roncav_Budget.mac/          # macOS
├── Roncav_Budget.sln           # Solução configurada
└── ConfigurarEExecutar.bat     # Script de setup
```

---

## 💡 Dicas

- Sempre use **Debug | x64** para desenvolvimento
- O projeto **Roncav_Budget.winui** deve estar em **negrito** na Solution Explorer
- Se tiver problemas, execute `ConfigurarEExecutar.bat` para reset completo
- O executável final fica em: `Roncav_Budget.winui\bin\x64\Debug\net9.0-windows10.0.19041.0\`

---

## ✨ Pronto para usar!

Tudo foi configurado e testado. O projeto compilou com sucesso. Siga qualquer um dos métodos acima e o aplicativo deve iniciar sem problemas!

**Boa sorte! 🎉**
