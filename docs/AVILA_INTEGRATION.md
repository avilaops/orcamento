# 🔌 Integração com Ecossistema Avila

## 📋 Visão Geral

O Roncav Budget está totalmente integrado ao ecossistema Avila, proporcionando:

- **🔐 Autenticação única** via api.avila.inc
- **☁️ Sincronização de dados** em nuvem
- **📊 Analytics centralizad**o
- **👤 Gestão de usuários** via portal.avila.inc
- **💳 Pagamentos** integrados
- **🎯 Marketing** unificado

---

## 🌐 APIs Integradas

### 1. **api.avila.inc** (Backend Central)

#### Endpoints Implementados:

**Autenticação** (`/auth`)
```http
POST /auth/login
POST /auth/register
POST /auth/refresh
POST /auth/logout
GET  /auth/validate
```

**Usuários** (`/users`)
```http
GET  /users/profile
PUT  /users/profile
GET  /users/permissions
DELETE /users/account
GET  /users/subscription
```

**Sincronização** (`/sync`)
```http
POST /sync/upload      # Upload dados locais
GET  /sync/download    # Download dados servidor
GET  /sync/status      # Status da sync
POST /sync/resolve-conflicts
```

**Analytics** (`/analytics`)
```http
POST /analytics/events   # Eventos de uso
POST /analytics/errors   # Log de erros
GET  /analytics/insights # IA Insights
```

**Pagamentos** (`/payments`)
```http
POST /payments/subscribe        # Assinar plano premium
POST /payments/cancel           # Cancelar assinatura
GET  /payments/invoices         # Faturas
POST /payments/update-card      # Atualizar cartão
```

---

### 2. **portal.avila.inc** (Portal do Usuário)

URL: `https://portal.avila.inc/roncav`

#### Funcionalidades no Portal:

**Dashboard Unificado**
- Visão de todos os produtos Avila que o usuário usa
- Roncav Budget como card destacado
- Quick actions: "Ver Orçamento", "Adicionar Transação"

**Gestão de Conta**
- Perfil unificado (foto, nome, email, telefone)
- Alterar senha
- Configurações de privacidade
- Exportar/Deletar dados (LGPD)

**Assinaturas**
- Gerenciar plano Roncav Budget
- Upgrade/downgrade
- Histórico de pagamentos
- Cancelamento

**Integrações**
- Conectar bancos (Open Banking)
- Conectar outros apps Avila
- Revogar acessos

**Suporte**
- Central de ajuda
- Abrir tickets
- Chat ao vivo
- Base de conhecimento

---

## 🔐 Autenticação e Segurança

### Fluxo de Autenticação

```
┌─────────────────────────────────────────────────────────┐
│                  FLUXO DE LOGIN                         │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  1. Usuário abre Roncav Budget                          │
│     ↓                                                   │
│  2. App verifica token local                            │
│     │                                                   │
│     ├─ Token válido → Acessa app                        │
│     └─ Sem token → Tela de login                        │
│                                                         │
│  3. Usuário insere email/senha                          │
│     ↓                                                   │
│  4. POST api.avila.inc/auth/login                       │
│     ↓                                                   │
│  5. API valida credenciais                              │
│     ↓                                                   │
│  6. Retorna { accessToken, refreshToken, userInfo }     │
│     ↓                                                   │
│  7. App salva tokens em SecureStorage                   │
│     ↓                                                   │
│  8. App usa accessToken em todas as requisições         │
│     │                                                   │
│     ├─ Expira em 15min → Auto-refresh com refreshToken │
│     └─ RefreshToken expira em 7 dias → Novo login      │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### Tokens

**Access Token** (JWT)
- Validade: 15 minutos
- Usado em: Authorization header (`Bearer <token>`)
- Contém: userId, email, plan, permissions

**Refresh Token** (UUID)
- Validade: 7 dias (renovado automaticamente)
- Usado para: Renovar accessToken
- Armazenado em: SecureStorage (criptografado)

### Criptografia

- **Em trânsito**: TLS 1.3
- **Em repouso**: AES-256
- **Senhas**: bcrypt (cost 12)
- **Tokens**: JWT assinado com RS256

---

## ☁️ Sincronização de Dados

### Estratégia: **Offline-First**

```
┌──────────────────────────────────────────────────┐
│          ESTRATÉGIA DE SINCRONIZAÇÃO             │
├──────────────────────────────────────────────────┤
│                                                  │
│  1. Dados SEMPRE salvos localmente primeiro     │
│     (SQLite no dispositivo)                      │
│                                                  │
│  2. Em background, app tenta sincronizar:        │
│     • A cada 5 minutos (se houver mudanças)      │
│     • Quando app volta para foreground           │
│     • Quando conexão é restaurada                │
│     • Manualmente (pull-to-refresh)              │
│                                                  │
│  3. Upload:                                      │
│     • Envia apenas dados modificados (delta)     │
│     • Marca itens como "synced" após sucesso     │
│                                                  │
│  4. Download:                                    │
│     • Recebe apenas dados novos/modificados      │
│     • Merge inteligente com dados locais         │
│                                                  │
│  5. Conflitos (raro):                            │
│     • Server-wins (padrão)                       │
│     • Client-wins (opção)                        │
│     • Manual (usuário decide)                    │
│                                                  │
└──────────────────────────────────────────────────┘
```

### Indicadores Visuais

**Status de Sync na UI:**
- 🔄 "Sincronizando..."
- ✅ "Sincronizado há 2 min"
- ⚠️ "Sem conexão (dados locais)"
- ❌ "Erro na sincronização"

---

## 📊 Analytics e Telemetria

### Eventos Rastreados

**Autenticação**
```csharp
await TrackEventAsync("user_login", new { method = "email" });
await TrackEventAsync("user_register", new { plan = "free" });
await TrackEventAsync("user_logout");
```

**Funcionalidades**
```csharp
await TrackEventAsync("transaction_created", new {
    type = "expense",
    value = 120.50,
    category = "food"
});

await TrackEventAsync("account_added", new {
    type = "checking",
    bank = "nubank"
});

await TrackEventAsync("budget_created", new {
    category = "transport",
    amount = 500
});

await TrackEventAsync("goal_achieved", new {
    goalName = "Emergency Fund"
});
```

**Engajamento**
```csharp
await TrackEventAsync("app_opened");
await TrackEventAsync("screen_viewed", new { screen = "Dashboard" });
await TrackEventAsync("feature_used", new { feature = "Reports" });
```

**Conversão**
```csharp
await TrackEventAsync("upgrade_clicked");
await TrackEventAsync("subscription_started", new { plan = "premium" });
await TrackEventAsync("trial_started");
```

**Erros**
```csharp
await LogErrorAsync("Sync", new Exception("Network timeout"));
```

### Dashboard de Analytics

Disponível em: `https://portal.avila.inc/analytics/roncav`

Métricas exibidas:
- DAU/MAU
- Retention (7d, 30d)
- Conversion rate
- Churn rate
- Top features
- User journeys
- Error rate
- Performance (load times)

---

## 💳 Integração de Pagamentos

### Fluxo de Upgrade

```
User clica "Assinar Premium"
  ↓
App redireciona para portal.avila.inc/subscribe
  ↓
Usuário escolhe plano e insere cartão
  ↓
Portal processa pagamento via Stripe
  ↓
Sucesso → Atualiza plano no servidor
  ↓
Webhook notifica app
  ↓
App atualiza permissões localmente
```

### Planos Disponíveis

| Plano | Preço/Mês | Preço/Ano | Stripe Plan ID |
|-------|-----------|-----------|----------------|
| Free | R$ 0 | R$ 0 | - |
| Premium | R$ 14,90 | R$ 149 | `plan_premium_monthly` |
| Família | R$ 24,90 | R$ 249 | `plan_family_monthly` |
| Business MEI | R$ 29,90 | R$ 299 | `plan_business_monthly` |

### Gerenciamento

**No App:**
- Ver plano atual
- Botão "Fazer Upgrade"
- Link para "Gerenciar Assinatura" → abre portal.avila.inc

**No Portal:**
- Upgrade/downgrade
- Cancelar (com retenção flow)
- Atualizar forma de pagamento
- Ver faturas
- Solicitar reembolso (até 7 dias)

---

## 🔗 Deep Links

### Esquema de URLs

```
roncav://auth/login
roncav://auth/register
roncav://dashboard
roncav://transactions
roncav://transactions/new
roncav://accounts
roncav://budgets
roncav://goals
roncav://settings
roncav://subscribe?plan=premium
```

### Uso

**Do Portal para o App:**
```html
<a href="roncav://transactions/new">
  Adicionar Transação
</a>
```

**Do App para o Portal:**
```csharp
await Browser.OpenAsync("https://portal.avila.inc/roncav/settings");
```

---

## 📱 Single Sign-On (SSO)

### Fluxo SSO

Se usuário já está logado em outro produto Avila:

```
1. App detecta token Avila no keychain
2. Valida token via /auth/validate
3. Se válido → Login automático
4. Se inválido → Tela de login normal
```

### Logout Universal

Quando usuário faz logout:
```
1. POST /auth/logout (invalida todos os tokens)
2. Remove tokens locais
3. Notifica outros apps Avila via broadcast
```

---

## 🛠️ Ferramentas de Debug

### Console de Desenvolvedor

Disponível em: `https://portal.avila.inc/dev/console`

**Features:**
- Ver logs de API em tempo real
- Testar endpoints manualmente
- Simular webhooks
- Visualizar payload de sync
- Resetar dados de teste

### Ambiente de Staging

- **API**: `https://api-staging.avila.inc`
- **Portal**: `https://portal-staging.avila.inc`
- **Dados**: Isolados de produção
- **Pagamentos**: Modo teste (Stripe Test)

---

## ✅ Checklist de Integração

### Desenvolvimento
- [x] AvilaApiService implementado
- [x] SyncService implementado
- [x] Autenticação JWT funcionando
- [x] SecureStorage configurado
- [x] Analytics rastreando eventos
- [x] Deep links configurados
- [ ] Testes unitários (80%+ cobertura)
- [ ] Testes de integração com API

### UI/UX
- [ ] Tela de login/registro
- [ ] Indicador de status de sync
- [ ] Mensagens de erro amigáveis
- [ ] Skeleton loaders durante sync
- [ ] Botão "Fazer Upgrade"
- [ ] Link para portal em Configurações

### Produção
- [ ] Variáveis de ambiente configuradas
- [ ] Certificado SSL válido
- [ ] Rate limiting respeitado
- [ ] Retry logic implementado
- [ ] Monitoramento configurado (Sentry)
- [ ] Documentação atualizada

---

## 📞 Suporte

**Equipe de Integração Avila**
- Email: dev@avila.inc
- Slack: #roncav-budget
- Docs: https://docs.api.avila.inc

**Status da API**
- Monitor: https://status.avila.inc
- Uptime: 99.9% SLA

---

**Última atualização**: 2025-11-16
**Responsável**: Squad de Platform Avila
