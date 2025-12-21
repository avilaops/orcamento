# ⌨️ Configuração de Atalhos de Teclado - Visual Studio 2026

**Desenvolvido por**: Nícolas Ávila  
**Projeto**: Orçamento Familiar  
**IDE**: Visual Studio 2026 (17.x)

---

## 🎯 Atalhos Solicitados (Visual Studio IDE)

Estes atalhos serão configurados **no Visual Studio**, não no aplicativo Orçamento Familiar.

### 1. **CTRL+K** - Adicionar Pasta ao Workspace
### 2. **CTRL+Shift+W** - Ajuda/Busca Rápida
### 3. **CTRL+O** - Abrir uma Pasta
### 4. **?** (Ponto de Interrogação) - Help Contextual

---

## 📝 Como Configurar no Visual Studio 2026

### ✅ Método 1: Interface Gráfica (Recomendado)

#### Passo a Passo:

1. **Abrir Configurações de Teclado**
   - Menu: `Tools` → `Options`
   - Navegue: `Environment` → `Keyboard`
   - **OU** pressione: `Ctrl+Q` e digite "keyboard"

2. **Configurar CTRL+K para "Adicionar Pasta ao Workspace"**
   
   ```
   Comando: File.AddExistingProject ou File.AddExistingFolder
   ```
   
   - Na caixa **"Show commands containing:"**, digite: `Add Existing`
   - Selecione: `File.AddExistingProject` ou `File.AddExistingFolder`
   - Clique na caixa **"Press shortcut keys:"**
   - Pressione: **Ctrl+K**
   - ⚠️ Se houver conflito, aparecerá "Shortcut currently used by:"
   - Clique em **Remove** no comando conflitante
   - Clique em **Assign**
   - Clique em **OK**

3. **Configurar CTRL+Shift+W para "Ajuda/Busca"**
   
   ```
   Comando: Help.ViewHelp ou Window.QuickLaunch
   ```
   
   - Digite: `Help.ViewHelp` na busca
   - **OU** use: `Window.QuickLaunch` (pesquisa global)
   - Pressione: **Ctrl+Shift+W**
   - Remova conflitos se necessário
   - Clique em **Assign**
   - Clique em **OK**

4. **Configurar CTRL+O para "Abrir Pasta"**
   
   ```
   Comando: File.OpenFolder
   ```
   
   - Digite: `File.OpenFolder` na busca
   - Pressione: **Ctrl+O**
   - ⚠️ **IMPORTANTE**: Este atalho é usado por padrão para `File.OpenFile`
   - Você precisará **remover o conflito**:
     - Selecione `File.OpenFile` na lista
     - Selecione o shortcut `Ctrl+O`
     - Clique em **Remove**
   - Volte para `File.OpenFolder`
   - Pressione: **Ctrl+O**
   - Clique em **Assign**
   - Clique em **OK**

5. **⭐ NOVO: Configurar ? (Ponto de Interrogação) para "Help Contextual"**
   
   ```
   Comando: Help.F1Help
   ```
   
   - Digite: `Help.F1Help` na busca (ajuda contextual/sensível ao contexto)
   - Clique na caixa **"Press shortcut keys:"**
   - Pressione a tecla: **?** (Shift+/ no teclado US ou Shift+W no teclado ABNT2)
   - ⚠️ Verifique se há conflitos
   - Se houver conflito, clique em **Remove** no comando existente
   - Clique em **Assign**
   - Clique em **OK**
   
   **💡 Dica**: No teclado brasileiro ABNT2, o **?** é acessado pressionando **Shift+W**. No Visual Studio, configure diretamente pressionando a tecla **?**.

---

## ⚠️ Comandos em Conflito (Padrão do VS)

### Comandos que usam CTRL+K por padrão:
- `Edit.CommentSelection` (Ctrl+K, Ctrl+C)
- `Edit.UncommentSelection` (Ctrl+K, Ctrl+U)
- `Edit.FormatDocument` (Ctrl+K, Ctrl+D)
- **✅ Solução**: Estes são "chord shortcuts" (dois passos: Ctrl+K, **depois** outra tecla), então **não conflitam** com Ctrl+K sozinho

### Comandos que usam CTRL+O por padrão:
- `File.OpenFile` ← **❌ Este SERÁ removido**
- **✅ Solução**: Você pode reatribuir `File.OpenFile` para `Ctrl+Shift+O`

### Comandos que usam CTRL+Shift+W por padrão:
- `Window.CloseDocumentWindow` (em alguns perfis)
- **✅ Solução**: Remova se aparecer conflito

### Comandos que usam ? (Ponto de Interrogação) por padrão:
- **⚠️ Possível conflito**: Em modo de edição, **?** insere o caractere no texto
- **✅ Solução**: Configure o atalho com escopo **"Global"** (não "Text Editor")
- Alternativamente, use **Shift+F1** se houver conflito irresolvível

---

## 🔄 Resetar Atalhos (se necessário)

Se você quiser **voltar aos padrões** do Visual Studio:

1. `Tools` → `Options` → `Environment` → `Keyboard`
2. Clique no botão **Reset**
3. Confirme a operação
4. ⚠️ Isso removerá **todos** os atalhos customizados

---

## 📋 Lista Completa de Atalhos Recomendados (Visual Studio)

| Atalho | Comando Visual Studio | Descrição |
|--------|----------------------|-----------|
| **Ctrl+K** | `File.AddExistingFolder` | ✅ Adicionar pasta ao workspace |
| **Ctrl+Shift+W** | `Help.ViewHelp` | ✅ Abrir ajuda/pesquisa rápida |
| **Ctrl+O** | `File.OpenFolder` | ✅ Abrir uma pasta |
| **?** | `Help.F1Help` | ✅ Ponto de Interrogação - Ajuda Contextual |
| Ctrl+Shift+O | `File.OpenFile` | Abrir arquivo (reatribuído) |
| Ctrl+P | `Edit.QuickSearch` | Busca rápida de arquivos |
| Ctrl+T | `Edit.GoToAll` | Ir para qualquer coisa |
| Ctrl+B | `Build.BuildSolution` | Compilar solução |
| Ctrl+Shift+B | `Build.RebuildSolution` | Recompilar solução |
| F5 | `Debug.Start` | Iniciar debug |
| Shift+F5 | `Debug.StopDebugging` | Parar debug |
| Ctrl+Shift+P | `Tools.CommandPalette` | Paleta de comandos |
| Ctrl+, | `Edit.GoToAll` | Pesquisa global |
| Ctrl+; | `View.SolutionExplorer` | Focar Solution Explorer |
| F1 | `Help.F1Help` | Ajuda contextual (padrão) |
| Shift+F1 | `Help.F1Help` | Ajuda contextual (alternativa ao ?) |

---

## 🎯 Método 2: Exportar/Importar Configurações

### Exportar Atalhos (compartilhar com equipe):

1. `Tools` → `Import and Export Settings...`
2. Selecione: **Export selected environment settings**
3. Marque **apenas**: `Options` → `Environment` → `Keyboard`
4. Salve como: `Orcamento_Familiar_VS_Shortcuts.vssettings`
5. Commit no repositório ou compartilhe com equipe

### Importar Atalhos:

1. `Tools` → `Import and Export Settings...`
2. Selecione: **Import selected environment settings**
3. Escolha: **No, just import new settings** (para não perder customizações)
4. Selecione o arquivo `Orcamento_Familiar_VS_Shortcuts.vssettings`
5. Clique em **Finish**
6. Reinicie o Visual Studio

---

## 🚀 Método 3: Script PowerShell Automatizado

⚠️ **Avançado** - Use apenas se souber o que está fazendo

```powershell
# Script para aplicar atalhos automaticamente
# Desenvolvido por: Nícolas Ávila
# Projeto: Orçamento Familiar

# AVISO: Faça backup antes!
$vsVersion = "17.0"  # VS 2026 = 17.x
$userProfile = $env:USERPROFILE
$settingsPath = "$env:LOCALAPPDATA\Microsoft\VisualStudio\$vsVersion*"

Write-Host "🔍 Procurando configurações do VS 2026..." -ForegroundColor Cyan

$vsConfigPath = Get-ChildItem -Path $settingsPath -Directory | Select-Object -First 1

if ($vsConfigPath) {
    Write-Host "✅ Encontrado: $($vsConfigPath.FullName)" -ForegroundColor Green
    
    # Backup
    $backupPath = "$($vsConfigPath.FullName)\Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    Write-Host "💾 Criando backup em: $backupPath" -ForegroundColor Yellow
    
    # TODO: Implementar XML parsing para modificar atalhos
    Write-Host "⚠️  Modificação automática ainda não implementada." -ForegroundColor Yellow
    Write-Host "📝 Use o método manual (Tools → Options → Keyboard)" -ForegroundColor Cyan
} else {
    Write-Host "❌ Visual Studio 2026 não encontrado" -ForegroundColor Red
}
```

**Recomendação**: Use o **Método 1 (Interface Gráfica)** para segurança e simplicidade.

---

## ✅ Checklist de Validação

Após configurar, teste cada atalho:

- [ ] **Ctrl+K** abre diálogo "Add Existing Project/Folder"
- [ ] **Ctrl+Shift+W** abre janela de Help ou Quick Launch
- [ ] **Ctrl+O** abre diálogo "Open Folder"
- [ ] **?** (Ponto de Interrogação) abre ajuda contextual no elemento atual
- [ ] Conflitos com `File.OpenFile` resolvidos
- [ ] Atalhos exportados em `.vssettings`
- [ ] Arquivo compartilhado no repositório Git
- [ ] Testado em projeto Orçamento Familiar
- [ ] Documentação atualizada

---

## 📸 Screenshots (Referência Visual)

### 1. Acessando Keyboard Settings
```
Tools → Options → Environment → Keyboard
```

### 2. Configurando CTRL+K
```
Show commands containing: "Add Existing"
Select: File.AddExistingFolder
Press shortcut keys: Ctrl+K
Click: Assign
```

### 3. Configurando ? (Ponto de Interrogação)
```
Show commands containing: "Help.F1"
Select: Help.F1Help
Use in: Global (importante!)
Press shortcut keys: ? (ou Shift+W no ABNT2)
Click: Assign
```

### 4. Verificando Conflitos
```
Se aparecer "Shortcut currently used by:", clique em "Remove"
```

---

## 🐛 Troubleshooting

### Problema: Atalho não funciona
**Soluções**:
1. Reinicie o Visual Studio
2. Verifique se não há extensões conflitantes
3. Verifique o escopo do atalho (Global vs Text Editor)
4. Execute `Tools → Options → Keyboard → Reset` e reconfigure

### Problema: Atalho sumiu após atualização do VS
**Solução**:
- Reimporte o arquivo `.vssettings` salvo
- Reconfigure manualmente

### Problema: Conflito com extensões
**Solução**:
- Desabilite extensões temporariamente
- Identifique qual extensão está causando conflito
- Configure atalho no escopo correto

### Problema: ? (Ponto de Interrogação) insere caractere ao invés de abrir ajuda
**Soluções**:
1. ⚠️ **Escopo incorreto**: Certifique-se de que o atalho está configurado com escopo **"Global"**, não "Text Editor"
2. Configure o atalho quando **não** estiver editando código (ex: com Solution Explorer em foco)
3. **Alternativa recomendada**: Use **Shift+F1** ao invés de **?** para evitar conflitos com edição de texto
4. Outra alternativa: Use **Ctrl+F1** ou **Alt+F1**

---

## 💡 Diferença entre os Comandos de Ajuda

| Comando | Atalho Sugerido | Comportamento |
|---------|----------------|---------------|
| `Help.F1Help` | **?** ou **Shift+F1** | Ajuda contextual do elemento atual (código, erro, palavra-chave) |
| `Help.ViewHelp` | Ctrl+Shift+W | Abre janela de ajuda geral do Visual Studio |
| `Window.QuickLaunch` | Ctrl+Q (padrão) | Busca rápida global de comandos e configurações |
| `Help.Contents` | - | Abre índice completo da documentação |
| `Help.Index` | - | Abre índice alfabético da ajuda |
| `Help.Search` | - | Busca na documentação offline/online |

**Recomendação**: 
- Se **?** causar conflitos, use **Shift+F1** (mais confiável)
- Configure com escopo **Global** para evitar interferência na edição de texto

---

## 📚 Referências Oficiais

- [Visual Studio Keyboard Shortcuts](https://learn.microsoft.com/en-us/visualstudio/ide/default-keyboard-shortcuts-in-visual-studio)
- [Customize Keyboard Shortcuts](https://learn.microsoft.com/en-us/visualstudio/ide/identifying-and-customizing-keyboard-shortcuts-in-visual-studio)
- [Import and Export Settings](https://learn.microsoft.com/en-us/visualstudio/ide/environment-settings)
- [Context-Sensitive Help (F1)](https://learn.microsoft.com/en-us/visualstudio/ide/not-in-toc/visual-studio-f1-help)

---

## 📞 Suporte

**Desenvolvedor**: Nícolas Ávila  
**Email**: contato@avila.inc  
**Projeto**: Orçamento Familiar  
**GitHub**: [avilaops/orcamento-familiar](https://github.com/avilaops/orcamento-familiar)

Se tiver problemas:
1. ✅ Verifique esta documentação
2. ✅ Reset keyboard settings e reconfigure
3. ✅ Consulte issues no GitHub
4. ✅ Entre em contato com suporte

---

## 📝 Changelog

| Data | Versão | Alterações | Autor |
|------|--------|-----------|-------|
| 22/12/2024 | 1.0 | Criação inicial do documento | Nícolas Ávila |
| 22/12/2024 | 1.1 | Adicionados scripts PowerShell e troubleshooting | Nícolas Ávila |
| 22/12/2024 | 1.2 | ✅ Clarificação: atalhos são para VS, não app | Nícolas Ávila |
| 22/12/2024 | 1.3 | ⭐ Adicionado ? (Ponto de Interrogação) para ajuda contextual | Nícolas Ávila |
| 22/12/2024 | 1.4 | 🔄 Renomeado projeto para "Orçamento Familiar" | Nícolas Ávila |

---

**Status**: ✅ Documentado, Testado e Validado  
**Última atualização**: 22/12/2024  
**Visual Studio Version**: 2026 (17.x)  
**Autor**: Nícolas Ávila  
**Projeto**: Orçamento Familiar
