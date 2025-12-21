# 🌐 Plano de Migração: Orçamento Familiar para Aplicação Online

**Projeto**: Orçamento Familiar  
**Desenvolvedor**: Nícolas Ávila  
**Objetivo**: Transformar aplicação .NET MAUI (desktop/mobile) em aplicação web online  
**Data**: 22/12/2024

---

## 🎯 Visão Geral da Migração

### Estado Atual
- ✅ Aplicação .NET MAUI (Windows, Android, iOS, macOS)
- ✅ SQLite local
- ✅ Sincronização opcional com `api.avila.inc`
- ✅ Offline-first

### Estado Futuro
- 🎯 Aplicação Web Progressive (PWA)
- 🎯 Banco de dados na nuvem (PostgreSQL/Azure SQL)
- 🎯 Online-first com cache offline
- 🎯 Acesso via navegador (multi-dispositivo)

---

## 📊 Comparação: Local vs Online

| Aspecto | Aplicação Atual (Local) | Aplicação Online |
|---------|-------------------------|------------------|
| **Instalação** | Download de 50+ MB | Acesso direto via URL |
| **Armazenamento** | SQLite local | PostgreSQL na nuvem |
| **Offline** | ✅ Total | ⚠️ Cache limitado (PWA) |
| **Multi-dispositivo** | Sincronização manual | Automático em tempo real |
| **Atualizações** | Requer reinstalação | Automáticas |
| **Backup** | Manual ou sync | Automático contínuo |
| **Colaboração** | Difícil | Nativa (multi-usuário) |
| **Custos** | Nenhum (local) | Hospedagem + BD (~R$ 50/mês) |

---

## 🏗️ Arquitetura da Aplicação Online

### Stack Tecnológica Recomendada

```
┌─────────────────────────────────────────────────────────────┐
│                    FRONTEND (Cliente)                        │
├─────────────────────────────────────────────────────────────┤
│  • Next.js 14 (React + TypeScript)                          │
│  • Tailwind CSS (estilização)                               │
│  • Zustand/Redux (state management)                         │
│  • React Query (API calls)                                  │
│  • Chart.js / Recharts (gráficos)                           │
│  • PWA (Service Workers para cache offline)                 │
└─────────────────────────────────────────────────────────────┘
                            ↕ HTTPS/REST API
┌─────────────────────────────────────────────────────────────┐
│                    BACKEND (Servidor)                        │
├─────────────────────────────────────────────────────────────┤
│  • Node.js + Express.js (API REST)                          │
│  • OU: .NET Core 9 Web API (manter stack atual)            │
│  • Autenticação: JWT + Refresh Tokens                       │
│  • Validação: Joi/Yup                                       │
│  • ORM: Prisma (Node) ou Entity Framework (.NET)           │
└─────────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────────┐
│                    BANCO DE DADOS                            │
├─────────────────────────────────────────────────────────────┤
│  • PostgreSQL (recomendado)                                 │
│  • OU: Azure SQL Database                                   │
│  • Redis (cache/sessões)                                    │
│  • Azure Blob Storage (anexos/arquivos)                     │
└─────────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────────┐
│                    HOSPEDAGEM                                │
├─────────────────────────────────────────────────────────────┤
│  Frontend: Vercel / Netlify / Azure Static Web Apps        │
│  Backend: Azure App Service / AWS / Heroku                 │
│  Database: Azure Database / AWS RDS / Supabase             │
│  CDN: Cloudflare / Azure CDN                               │
└─────────────────────────────────────────────────────────────┘
```

---

## 🚀 Plano de Migração em 4 Fases

### Fase 1: Backend API (4-6 semanas)

#### Opção A: .NET Core Web API (manter stack)
```csharp
// Estrutura do projeto
Orcamento.Familiar.API/
├── Controllers/
│   ├── ContasController.cs
│   ├── TransacoesController.cs
│   ├── OrcamentosController.cs
│   ├── MetasController.cs
│   └── AuthController.cs
├── Models/
│   ├── Conta.cs
│   ├── Transacao.cs
│   └── DTOs/
├── Services/
│   ├── IContaService.cs
│   ├── ITransacaoService.cs
│   └── IAuthService.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Middleware/
│   ├── AuthMiddleware.cs
│   └── ErrorHandlingMiddleware.cs
└── Program.cs
```

**Endpoints principais**:
```csharp
// Autenticação
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/refresh
POST   /api/auth/logout

// Contas
GET    /api/contas
GET    /api/contas/{id}
POST   /api/contas
PUT    /api/contas/{id}
DELETE /api/contas/{id}

// Transações
GET    /api/transacoes
GET    /api/transacoes/{id}
POST   /api/transacoes
PUT    /api/transacoes/{id}
DELETE /api/transacoes/{id}
GET    /api/transacoes/extrato?mes=1&ano=2024

// Orçamentos
GET    /api/orcamentos
GET    /api/orcamentos/{id}
POST   /api/orcamentos
PUT    /api/orcamentos/{id}
DELETE /api/orcamentos/{id}

// Metas
GET    /api/metas
GET    /api/metas/{id}
POST   /api/metas
PUT    /api/metas/{id}
DELETE /api/metas/{id}

// Dashboard
GET    /api/dashboard/resumo
GET    /api/dashboard/graficos
GET    /api/relatorios/mensal?mes=1&ano=2024
```

#### Migração do Banco de Dados

**SQLite (atual) → PostgreSQL (online)**

```sql
-- Schema PostgreSQL
CREATE TABLE usuarios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    senha_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE contas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID REFERENCES usuarios(id) ON DELETE CASCADE,
    nome VARCHAR(100) NOT NULL,
    tipo_conta VARCHAR(20) NOT NULL,
    saldo_inicial DECIMAL(15,2) DEFAULT 0,
    saldo_atual DECIMAL(15,2) DEFAULT 0,
    banco VARCHAR(50),
    agencia VARCHAR(20),
    numero_conta VARCHAR(30),
    cor VARCHAR(7),
    ativa BOOLEAN DEFAULT TRUE,
    incluir_no_total BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE transacoes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID REFERENCES usuarios(id) ON DELETE CASCADE,
    conta_id UUID REFERENCES contas(id) ON DELETE CASCADE,
    descricao VARCHAR(200) NOT NULL,
    valor DECIMAL(15,2) NOT NULL,
    tipo VARCHAR(20) NOT NULL, -- 'Receita', 'Despesa', 'Transferencia'
    categoria VARCHAR(50),
    data_transacao DATE NOT NULL,
    observacoes TEXT,
    recorrente BOOLEAN DEFAULT FALSE,
    frequencia_recorrencia VARCHAR(20),
    parcela_atual INT,
    total_parcelas INT,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE orcamentos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID REFERENCES usuarios(id) ON DELETE CASCADE,
    categoria VARCHAR(50) NOT NULL,
    valor_planejado DECIMAL(15,2) NOT NULL,
    mes INT NOT NULL,
    ano INT NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(usuario_id, categoria, mes, ano)
);

CREATE TABLE metas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID REFERENCES usuarios(id) ON DELETE CASCADE,
    titulo VARCHAR(100) NOT NULL,
    descricao TEXT,
    valor_alvo DECIMAL(15,2) NOT NULL,
    valor_atual DECIMAL(15,2) DEFAULT 0,
    data_inicio DATE NOT NULL,
    data_alvo DATE NOT NULL,
    prioridade VARCHAR(20),
    status VARCHAR(20),
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

-- Índices para performance
CREATE INDEX idx_transacoes_usuario ON transacoes(usuario_id);
CREATE INDEX idx_transacoes_data ON transacoes(data_transacao);
CREATE INDEX idx_transacoes_conta ON transacoes(conta_id);
CREATE INDEX idx_contas_usuario ON contas(usuario_id);
```

---

### Fase 2: Frontend Web (6-8 semanas)

#### Estrutura Next.js

```
orcamento-familiar-web/
├── app/
│   ├── (auth)/
│   │   ├── login/
│   │   │   └── page.tsx
│   │   └── register/
│   │       └── page.tsx
│   ├── (dashboard)/
│   │   ├── layout.tsx
│   │   ├── page.tsx              // Dashboard
│   │   ├── contas/
│   │   │   ├── page.tsx
│   │   │   └── [id]/
│   │   │       └── page.tsx
│   │   ├── transacoes/
│   │   │   ├── page.tsx
│   │   │   └── nova/
│   │   │       └── page.tsx
│   │   ├── orcamentos/
│   │   │   └── page.tsx
│   │   ├── metas/
│   │   │   └── page.tsx
│   │   └── relatorios/
│   │       └── page.tsx
│   ├── layout.tsx
│   └── page.tsx                  // Landing page
├── components/
│   ├── ui/                       // Componentes base (shadcn/ui)
│   ├── charts/
│   │   ├── BalanceChart.tsx
│   │   ├── ExpenseChart.tsx
│   │   └── CategoryChart.tsx
│   ├── forms/
│   │   ├── TransactionForm.tsx
│   │   ├── AccountForm.tsx
│   │   └── BudgetForm.tsx
│   └── layout/
│       ├── Header.tsx
│       ├── Sidebar.tsx
│       └── Footer.tsx
├── lib/
│   ├── api.ts                    // Cliente API
│   ├── auth.ts                   // Autenticação
│   └── utils.ts
├── hooks/
│   ├── useAuth.ts
│   ├── useTransactions.ts
│   └── useAccounts.ts
├── store/
│   └── authStore.ts              // Zustand
├── public/
│   ├── manifest.json             // PWA
│   └── service-worker.js
├── styles/
│   └── globals.css
├── next.config.js
├── tailwind.config.js
└── package.json
```

#### Exemplo de Componente: Dashboard

```tsx
// app/(dashboard)/page.tsx
'use client';

import { useState, useEffect } from 'react';
import { useAuth } from '@/hooks/useAuth';
import { api } from '@/lib/api';
import { BalanceChart } from '@/components/charts/BalanceChart';
import { ExpenseChart } from '@/components/charts/ExpenseChart';
import { RecentTransactions } from '@/components/RecentTransactions';

interface DashboardData {
  saldoTotal: number;
  receitasMes: number;
  despesasMes: number;
  transacoesRecentes: Transaction[];
  despesasPorCategoria: CategoryExpense[];
}

export default function DashboardPage() {
  const { user } = useAuth();
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchDashboard() {
      try {
        const response = await api.get('/dashboard/resumo');
        setData(response.data);
      } catch (error) {
        console.error('Erro ao carregar dashboard:', error);
      } finally {
        setLoading(false);
      }
    }

    fetchDashboard();
  }, []);

  if (loading) {
    return <div className="flex justify-center items-center h-screen">
      <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
    </div>;
  }

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">
          Olá, {user?.nome}! 👋
        </h1>
        <p className="text-gray-600">Aqui está o resumo das suas finanças</p>
      </div>

      {/* Cards de Resumo */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-white p-6 rounded-lg shadow-md">
          <h3 className="text-sm font-medium text-gray-500">Saldo Total</h3>
          <p className="text-3xl font-bold text-blue-600">
            {data?.saldoTotal.toLocaleString('pt-BR', { 
              style: 'currency', 
              currency: 'BRL' 
            })}
          </p>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-md">
          <h3 className="text-sm font-medium text-gray-500">Receitas (Mês)</h3>
          <p className="text-3xl font-bold text-green-600">
            {data?.receitasMes.toLocaleString('pt-BR', { 
              style: 'currency', 
              currency: 'BRL' 
            })}
          </p>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-md">
          <h3 className="text-sm font-medium text-gray-500">Despesas (Mês)</h3>
          <p className="text-3xl font-bold text-red-600">
            {data?.despesasMes.toLocaleString('pt-BR', { 
              style: 'currency', 
              currency: 'BRL' 
            })}
          </p>
        </div>
      </div>

      {/* Gráficos */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white p-6 rounded-lg shadow-md">
          <h2 className="text-xl font-semibold mb-4">Evolução do Saldo</h2>
          <BalanceChart />
        </div>

        <div className="bg-white p-6 rounded-lg shadow-md">
          <h2 className="text-xl font-semibold mb-4">Despesas por Categoria</h2>
          <ExpenseChart data={data?.despesasPorCategoria || []} />
        </div>
      </div>

      {/* Transações Recentes */}
      <div className="bg-white p-6 rounded-lg shadow-md">
        <h2 className="text-xl font-semibold mb-4">Transações Recentes</h2>
        <RecentTransactions transactions={data?.transacoesRecentes || []} />
      </div>
    </div>
  );
}
```

---

### Fase 3: PWA e Offline Support (2-3 semanas)

#### Service Worker para Cache Offline

```javascript
// public/service-worker.js
const CACHE_NAME = 'orcamento-familiar-v1';
const urlsToCache = [
  '/',
  '/dashboard',
  '/transacoes',
  '/contas',
  '/styles/globals.css',
  '/manifest.json'
];

// Instalação - cachear recursos
self.addEventListener('install', (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then((cache) => cache.addAll(urlsToCache))
  );
});

// Fetch - servir do cache quando offline
self.addEventListener('fetch', (event) => {
  event.respondWith(
    caches.match(event.request)
      .then((response) => {
        // Cache hit - retornar do cache
        if (response) {
          return response;
        }

        // Não está no cache - buscar da rede
        return fetch(event.request).then(
          (response) => {
            // Cachear nova requisição
            if (!response || response.status !== 200) {
              return response;
            }

            const responseToCache = response.clone();
            caches.open(CACHE_NAME)
              .then((cache) => {
                cache.put(event.request, responseToCache);
              });

            return response;
          }
        );
      })
  );
});
```

#### Manifest.json para PWA

```json
{
  "name": "Orçamento Familiar",
  "short_name": "Orçamento",
  "description": "Controle suas finanças de forma simples e eficiente",
  "start_url": "/",
  "display": "standalone",
  "background_color": "#ffffff",
  "theme_color": "#007AFF",
  "icons": [
    {
      "src": "/icon-192.png",
      "sizes": "192x192",
      "type": "image/png"
    },
    {
      "src": "/icon-512.png",
      "sizes": "512x512",
      "type": "image/png"
    }
  ]
}
```

---

### Fase 4: Deploy e Infraestrutura (1-2 semanas)

#### Opção 1: Stack Azure (Recomendado para .NET)

```yaml
# Custo estimado: R$ 150-300/mês

Frontend:
  - Azure Static Web Apps: R$ 0 (tier Free)
  - CDN: R$ 20/mês
  
Backend:
  - Azure App Service (B1): R$ 50/mês
  - Azure Functions (consumo): R$ 10/mês
  
Database:
  - Azure Database for PostgreSQL (Basic): R$ 80/mês
  - Redis Cache (Basic): R$ 20/mês
  
Storage:
  - Azure Blob Storage: R$ 10/mês
  
Total: ~R$ 190/mês
```

#### Opção 2: Stack Vercel + Supabase (Recomendado para Next.js)

```yaml
# Custo estimado: R$ 50-150/mês

Frontend:
  - Vercel Pro: $20/mês (~R$ 100)
  
Backend + Database:
  - Supabase Pro: $25/mês (~R$ 125)
    (PostgreSQL + Auth + Storage + Realtime)
  
CDN:
  - Cloudflare Free: R$ 0
  
Total: ~R$ 225/mês
```

#### Opção 3: Stack Low-Cost

```yaml
# Custo estimado: R$ 20-50/mês

Frontend:
  - Vercel Hobby: R$ 0
  - Netlify Free: R$ 0
  
Backend:
  - Railway: $5/mês (~R$ 25)
  - Render Free tier: R$ 0
  
Database:
  - Supabase Free: R$ 0 (até 500MB)
  - Neon.tech Free: R$ 0
  
Total: ~R$ 25/mês
```

---

## 📋 Checklist de Migração

### Backend API
- [ ] Criar projeto .NET Core Web API
- [ ] Configurar Entity Framework + PostgreSQL
- [ ] Implementar autenticação JWT
- [ ] Criar todos os endpoints CRUD
- [ ] Adicionar validações e tratamento de erros
- [ ] Implementar middleware de autenticação
- [ ] Configurar CORS
- [ ] Documentar API com Swagger
- [ ] Testes unitários (80%+ cobertura)
- [ ] Deploy em Azure/AWS

### Frontend Web
- [ ] Inicializar projeto Next.js
- [ ] Configurar Tailwind CSS
- [ ] Implementar autenticação (login/register)
- [ ] Criar layout principal (Header, Sidebar, Footer)
- [ ] Página: Dashboard
- [ ] Página: Transações (lista + nova)
- [ ] Página: Contas (lista + nova)
- [ ] Página: Orçamentos
- [ ] Página: Metas
- [ ] Página: Relatórios
- [ ] Implementar gráficos (Chart.js)
- [ ] Configurar PWA (Service Worker + Manifest)
- [ ] Testes E2E (Cypress/Playwright)
- [ ] Deploy em Vercel/Netlify

### Database
- [ ] Criar schema PostgreSQL
- [ ] Configurar migrations
- [ ] Importar dados de teste
- [ ] Configurar backup automático
- [ ] Otimizar índices
- [ ] Configurar replicação (opcional)

### DevOps
- [ ] Configurar CI/CD (GitHub Actions)
- [ ] Configurar ambientes (Dev, Staging, Prod)
- [ ] Configurar monitoramento (Application Insights)
- [ ] Configurar logging (Sentry/LogRocket)
- [ ] Configurar alertas
- [ ] Documentar processo de deploy

---

## 🎯 Cronograma Estimado

### Timeline Realista: 3-4 meses

| Fase | Duração | Semanas |
|------|---------|---------|
| **Fase 1: Backend API** | 4-6 semanas | 1-6 |
| **Fase 2: Frontend Web** | 6-8 semanas | 7-14 |
| **Fase 3: PWA/Offline** | 2-3 semanas | 15-17 |
| **Fase 4: Deploy** | 1-2 semanas | 18-19 |
| **Testes e Ajustes** | 2-3 semanas | 20-22 |
| **Documentação** | 1 semana | 23 |

**Total**: ~23 semanas (5-6 meses)

---

## 💰 Investimento Necessário

### Desenvolvimento
- **Solo (Nícolas Ávila)**: 400-500 horas
- **Com 1 desenvolvedor adicional**: 200-300 horas cada
- **Custo estimado** (freelancer R$ 80-150/h): R$ 32.000 - R$ 75.000

### Hospedagem (mensal)
- **Opção Econômica**: R$ 25-50/mês
- **Opção Profissional**: R$ 150-300/mês
- **Primeiro ano**: R$ 300-3.600

### Total Investimento Ano 1
- Desenvolvimento: R$ 32.000 - R$ 75.000
- Hospedagem: R$ 300 - R$ 3.600
- **Total**: R$ 32.300 - R$ 78.600

---

## 🚦 Decisão: Local ou Online?

### Mantenha Local Se:
- ✅ Privacidade dos dados é prioridade máxima
- ✅ Usuário único (sem necessidade de compartilhar)
- ✅ Baixo orçamento
- ✅ Acesso offline frequente

### Migre para Online Se:
- ✅ Precisa acessar de múltiplos dispositivos
- ✅ Quer compartilhar com família/cônjuge
- ✅ Deseja backup automático
- ✅ Quer atualizações automáticas
- ✅ Planeja monetizar (assinaturas/SaaS)

---

## 🔄 Alternativa: Híbrido (Melhor dos Dois Mundos)

### Manter Aplicação Local + Adicionar Web

```
Desktop App (.NET MAUI)  ←→  API Backend  ←→  Web App (Next.js)
        ↓                        ↓                    ↓
   SQLite Local          PostgreSQL Cloud      Browser Cache (PWA)
```

**Vantagens**:
- Usuários podem escolher (desktop ou web)
- Ambos sincronizam com mesma API
- Máxima flexibilidade
- Sem perder base de usuários desktop

**Investimento**: +30% no desenvolvimento (web adicional)

---

## 📞 Próximos Passos

### Decisão Imediata
1. ✅ Definir: 100% Online ou Híbrido?
2. ✅ Escolher stack: .NET Core ou Next.js?
3. ✅ Definir orçamento disponível
4. ✅ Estabelecer timeline

### Começar Desenvolvimento
1. Criar repositório Git para web
2. Configurar projeto backend
3. Modelar banco de dados PostgreSQL
4. Implementar primeiro endpoint (auth)
5. Criar primeira tela (login)

---

**Autor**: Nícolas Ávila  
**Projeto**: Orçamento Familiar  
**Data**: 22/12/2024  
**Versão**: 1.0

---

## 📎 Recursos Adicionais

- [Next.js Documentation](https://nextjs.org/docs)
- [.NET Core Web API Tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api)
- [PostgreSQL Best Practices](https://wiki.postgresql.org/wiki/Don%27t_Do_This)
- [PWA Guide](https://web.dev/progressive-web-apps/)
- [Azure Deployment Guide](https://learn.microsoft.com/en-us/azure/app-service/)

---

**Status**: 📋 Planejamento Completo  
**Pronto para**: Discussão de escopo e início de desenvolvimento
