# 🆓 Plano de Implementação GRATUITO - Orçamento Familiar Online

**Desenvolvedor**: Nícolas Ávila  
**Projeto**: Orçamento Familiar  
**Investimento**: R$ 0,00 (100% Free Tier)  
**Data**: 22/12/2024

---

## 🎯 Stack 100% Gratuita

### Frontend
- ✅ **Vercel** (Free Tier)
  - Hospedagem ilimitada
  - Deploy automático via GitHub
  - SSL grátis
  - 100GB bandwidth/mês

### Backend + Database
- ✅ **Supabase** (Free Tier)
  - PostgreSQL: 500MB
  - Autenticação completa
  - Storage: 1GB
  - Realtime API
  - 50.000 usuários ativos/mês

### CDN e Performance
- ✅ **Cloudflare** (Free Tier)
  - CDN global ilimitado
  - DDoS protection
  - SSL/TLS
  - Cache inteligente

### Monitoramento
- ✅ **Sentry** (Free Tier)
  - 5.000 eventos/mês
  - Error tracking
  - Performance monitoring

### Analytics
- ✅ **Google Analytics 4** (Free)
  - Analytics completo
  - Sem limites

---

## 🚀 Implementação em 5 Passos

### Passo 1: Configurar Supabase (Backend + DB)

#### 1.1 Criar Conta e Projeto
```bash
# 1. Acesse: https://supabase.com
# 2. Login com GitHub
# 3. Create New Project
#    - Name: orcamento-familiar
#    - Database Password: (escolha uma senha forte)
#    - Region: South America (São Paulo)
```

#### 1.2 Criar Schema do Banco

```sql
-- Execute no SQL Editor do Supabase

-- Tabela de usuários (já existe, apenas habilitar)
-- Supabase cria automaticamente

-- Tabela de contas
CREATE TABLE contas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE,
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

-- Tabela de transações
CREATE TABLE transacoes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE,
    conta_id UUID REFERENCES contas(id) ON DELETE CASCADE,
    descricao VARCHAR(200) NOT NULL,
    valor DECIMAL(15,2) NOT NULL,
    tipo VARCHAR(20) NOT NULL,
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

-- Tabela de orçamentos
CREATE TABLE orcamentos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE,
    categoria VARCHAR(50) NOT NULL,
    valor_planejado DECIMAL(15,2) NOT NULL,
    mes INT NOT NULL,
    ano INT NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(user_id, categoria, mes, ano)
);

-- Tabela de metas
CREATE TABLE metas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE,
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

-- Habilitar Row Level Security (RLS)
ALTER TABLE contas ENABLE ROW LEVEL SECURITY;
ALTER TABLE transacoes ENABLE ROW LEVEL SECURITY;
ALTER TABLE orcamentos ENABLE ROW LEVEL SECURITY;
ALTER TABLE metas ENABLE ROW LEVEL SECURITY;

-- Políticas de segurança (usuário só vê seus próprios dados)
-- Contas
CREATE POLICY "Usuários podem ver suas próprias contas"
    ON contas FOR SELECT
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem inserir suas próprias contas"
    ON contas FOR INSERT
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem atualizar suas próprias contas"
    ON contas FOR UPDATE
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem deletar suas próprias contas"
    ON contas FOR DELETE
    USING (auth.uid() = user_id);

-- Transações
CREATE POLICY "Usuários podem ver suas próprias transações"
    ON transacoes FOR SELECT
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem inserir suas próprias transações"
    ON transacoes FOR INSERT
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem atualizar suas próprias transações"
    ON transacoes FOR UPDATE
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem deletar suas próprias transações"
    ON transacoes FOR DELETE
    USING (auth.uid() = user_id);

-- Orçamentos
CREATE POLICY "Usuários podem ver seus próprios orçamentos"
    ON orcamentos FOR SELECT
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem inserir seus próprios orçamentos"
    ON orcamentos FOR INSERT
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem atualizar seus próprios orçamentos"
    ON orcamentos FOR UPDATE
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem deletar seus próprios orçamentos"
    ON orcamentos FOR DELETE
    USING (auth.uid() = user_id);

-- Metas
CREATE POLICY "Usuários podem ver suas próprias metas"
    ON metas FOR SELECT
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem inserir suas próprias metas"
    ON metas FOR INSERT
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem atualizar suas próprias metas"
    ON metas FOR UPDATE
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem deletar suas próprias metas"
    ON metas FOR DELETE
    USING (auth.uid() = user_id);

-- Índices para performance
CREATE INDEX idx_transacoes_user ON transacoes(user_id);
CREATE INDEX idx_transacoes_data ON transacoes(data_transacao);
CREATE INDEX idx_transacoes_conta ON transacoes(conta_id);
CREATE INDEX idx_contas_user ON contas(user_id);
CREATE INDEX idx_orcamentos_user ON orcamentos(user_id);
CREATE INDEX idx_metas_user ON metas(user_id);
```

#### 1.3 Configurar Autenticação

```bash
# No dashboard do Supabase:
# 1. Authentication → Settings
# 2. Habilitar: Email/Password
# 3. Site URL: https://orcamento-familiar.vercel.app (depois que criar)
# 4. Redirect URLs: 
#    - http://localhost:3000/**
#    - https://orcamento-familiar.vercel.app/**
```

---

### Passo 2: Criar Frontend Next.js

#### 2.1 Criar Projeto

```bash
# No terminal do Windows
cd C:\Users\Administrador\source\repos\

# Criar novo projeto Next.js
npx create-next-app@latest orcamento-familiar-web --typescript --tailwind --app --no-src-dir

cd orcamento-familiar-web

# Instalar dependências do Supabase
npm install @supabase/supabase-js @supabase/auth-helpers-nextjs

# Instalar bibliotecas de UI e utilidades
npm install zustand react-hook-form zod @hookform/resolvers
npm install date-fns lucide-react recharts
npm install clsx tailwind-merge class-variance-authority

# Instalar shadcn/ui (componentes prontos)
npx shadcn-ui@latest init
```

#### 2.2 Configurar Variáveis de Ambiente

```env
# .env.local
NEXT_PUBLIC_SUPABASE_URL=https://seu-projeto.supabase.co
NEXT_PUBLIC_SUPABASE_ANON_KEY=sua-chave-anonima

# Pegar essas credenciais em:
# Supabase Dashboard → Settings → API
```

#### 2.3 Estrutura do Projeto

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
│   │   ├── page.tsx              # Dashboard principal
│   │   ├── contas/
│   │   │   └── page.tsx
│   │   ├── transacoes/
│   │   │   └── page.tsx
│   │   ├── orcamentos/
│   │   │   └── page.tsx
│   │   └── metas/
│   │       └── page.tsx
│   ├── layout.tsx
│   └── page.tsx                  # Landing page
├── components/
│   ├── ui/                       # Componentes shadcn/ui
│   ├── auth/
│   │   ├── LoginForm.tsx
│   │   └── RegisterForm.tsx
│   ├── dashboard/
│   │   ├── BalanceCard.tsx
│   │   ├── TransactionsList.tsx
│   │   └── Charts.tsx
│   └── layout/
│       ├── Header.tsx
│       └── Sidebar.tsx
├── lib/
│   ├── supabase.ts              # Cliente Supabase
│   └── utils.ts
├── hooks/
│   ├── useAuth.ts
│   └── useTransactions.ts
├── store/
│   └── authStore.ts
├── .env.local
├── next.config.js
├── tailwind.config.ts
└── package.json
```

#### 2.4 Configurar Cliente Supabase

```typescript
// lib/supabase.ts
import { createClient } from '@supabase/supabase-js';

const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL!;
const supabaseAnonKey = process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY!;

export const supabase = createClient(supabaseUrl, supabaseAnonKey);

// Tipos para o TypeScript
export type Tables = {
  contas: {
    id: string;
    user_id: string;
    nome: string;
    tipo_conta: string;
    saldo_inicial: number;
    saldo_atual: number;
    banco?: string;
    agencia?: string;
    numero_conta?: string;
    cor?: string;
    ativa: boolean;
    incluir_no_total: boolean;
    created_at: string;
    updated_at: string;
  };
  transacoes: {
    id: string;
    user_id: string;
    conta_id: string;
    descricao: string;
    valor: number;
    tipo: string;
    categoria?: string;
    data_transacao: string;
    observacoes?: string;
    recorrente: boolean;
    frequencia_recorrencia?: string;
    parcela_atual?: number;
    total_parcelas?: number;
    created_at: string;
    updated_at: string;
  };
  // ... outros tipos
};
```

#### 2.5 Hook de Autenticação

```typescript
// hooks/useAuth.ts
import { useEffect, useState } from 'react';
import { supabase } from '@/lib/supabase';
import { User } from '@supabase/supabase-js';

export function useAuth() {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Verificar sessão atual
    supabase.auth.getSession().then(({ data: { session } }) => {
      setUser(session?.user ?? null);
      setLoading(false);
    });

    // Escutar mudanças de autenticação
    const {
      data: { subscription },
    } = supabase.auth.onAuthStateChange((_event, session) => {
      setUser(session?.user ?? null);
    });

    return () => subscription.unsubscribe();
  }, []);

  const signIn = async (email: string, password: string) => {
    const { data, error } = await supabase.auth.signInWithPassword({
      email,
      password,
    });
    return { data, error };
  };

  const signUp = async (email: string, password: string, nome: string) => {
    const { data, error } = await supabase.auth.signUp({
      email,
      password,
      options: {
        data: {
          nome,
        },
      },
    });
    return { data, error };
  };

  const signOut = async () => {
    const { error } = await supabase.auth.signOut();
    return { error };
  };

  return {
    user,
    loading,
    signIn,
    signUp,
    signOut,
  };
}
```

#### 2.6 Página de Login

```typescript
// app/(auth)/login/page.tsx
'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';

export default function LoginPage() {
  const router = useRouter();
  const { signIn } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    const { data, error } = await signIn(email, password);

    if (error) {
      setError(error.message);
      setLoading(false);
    } else {
      router.push('/dashboard');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="max-w-md w-full space-y-8 p-8 bg-white rounded-lg shadow-md">
        <div>
          <h2 className="text-3xl font-bold text-center text-gray-900">
            Orçamento Familiar
          </h2>
          <p className="mt-2 text-center text-gray-600">
            Entre na sua conta
          </p>
        </div>

        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
              {error}
            </div>
          )}

          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700">
              Email
            </label>
            <input
              id="email"
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-700">
              Senha
            </label>
            <input
              id="password"
              type="password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
          >
            {loading ? 'Entrando...' : 'Entrar'}
          </button>

          <p className="text-center text-sm text-gray-600">
            Não tem conta?{' '}
            <a href="/register" className="font-medium text-blue-600 hover:text-blue-500">
              Cadastre-se
            </a>
          </p>
        </form>
      </div>
    </div>
  );
}
```

---

### Passo 3: Deploy no Vercel (Gratuito)

#### 3.1 Preparar Deploy

```bash
# Inicializar Git (se ainda não tiver)
git init
git add .
git commit -m "Initial commit - Orçamento Familiar Web"

# Criar repositório no GitHub
# 1. Acesse: https://github.com/new
# 2. Nome: orcamento-familiar-web
# 3. Criar repositório

# Push para GitHub
git remote add origin https://github.com/avilaops/orcamento-familiar-web.git
git branch -M main
git push -u origin main
```

#### 3.2 Deploy na Vercel

```bash
# Opção 1: Via Dashboard
# 1. Acesse: https://vercel.com
# 2. Login com GitHub
# 3. New Project
# 4. Importar: avilaops/orcamento-familiar-web
# 5. Configurar variáveis de ambiente:
#    - NEXT_PUBLIC_SUPABASE_URL
#    - NEXT_PUBLIC_SUPABASE_ANON_KEY
# 6. Deploy!

# Opção 2: Via CLI
npm install -g vercel
vercel login
vercel --prod
```

#### 3.3 Configurar Domínio (Opcional mas Gratuito)

```bash
# No Vercel Dashboard:
# 1. Settings → Domains
# 2. Add Domain: orcamento-familiar.vercel.app (gratuito)
# OU
# 3. Usar domínio próprio (se tiver)
```

---

### Passo 4: Configurar Cloudflare CDN (Gratuito)

```bash
# 1. Criar conta: https://cloudflare.com
# 2. Add Site: orcamento-familiar.vercel.app
# 3. Escolher plano Free
# 4. Copiar nameservers
# 5. Configurar no Vercel (se usar domínio customizado)
# 6. Ativar:
#    - Auto Minify (HTML, CSS, JS)
#    - Brotli compression
#    - Cache Level: Standard
```

---

### Passo 5: Monitoramento com Sentry (Gratuito)

```bash
# 1. Instalar Sentry
npm install @sentry/nextjs

# 2. Configurar
npx @sentry/wizard -i nextjs

# 3. Adicionar ao .env.local
NEXT_PUBLIC_SENTRY_DSN=https://seu-dsn@sentry.io/projeto
```

---

## 📊 Limites dos Planos Gratuitos

### Supabase Free
- ✅ 500MB de banco de dados
- ✅ 1GB de storage
- ✅ 50.000 usuários ativos/mês
- ✅ 2GB de transferência/mês
- ⚠️ Pausado após 1 semana de inatividade (reativa automático)

### Vercel Free
- ✅ 100 deploys/dia
- ✅ 100GB de bandwidth/mês
- ✅ SSL grátis
- ✅ Domínio .vercel.app gratuito

### Cloudflare Free
- ✅ CDN ilimitado
- ✅ DDoS protection
- ✅ Cache ilimitado

**Total de usuários suportados GRÁTIS**: ~1.000-5.000 usuários ativos

---

## 🎯 Próximos Passos IMEDIATOS

### 1. Agora Mesmo (5 minutos)
```bash
# Criar conta Supabase
https://supabase.com/dashboard/sign-up

# Criar conta Vercel
https://vercel.com/signup
```

### 2. Hoje (1 hora)
```bash
# Criar projeto Next.js
cd C:\Users\Administrador\source\repos\
npx create-next-app@latest orcamento-familiar-web --typescript --tailwind --app

# Configurar Supabase
# Copiar SQL acima e executar
```

### 3. Esta Semana (10-15 horas)
- [ ] Implementar autenticação
- [ ] Criar dashboard básico
- [ ] CRUD de transações
- [ ] Deploy na Vercel

### 4. Próximas 2 Semanas (20-30 horas)
- [ ] CRUD de contas
- [ ] CRUD de orçamentos
- [ ] Gráficos e relatórios
- [ ] PWA (offline support)

---

## 💰 Quando Você Precisará Pagar?

### Supabase Pro (~R$ 125/mês)
Quando atingir:
- Mais de 500MB de dados
- Mais de 50.000 usuários ativos
- Precisar de backup automático
- Quiser suporte prioritário

### Vercel Pro (~R$ 100/mês)
Quando precisar:
- Mais de 100GB de bandwidth
- Domínio customizado comercial
- Suporte prioritário
- Analytics avançado

**Resumo**: Você pode ter **milhares de usuários** sem pagar nada!

---

## 🚀 Comandos Rápidos para Começar AGORA

```powershell
# Crie este arquivo: start-projeto-web.ps1

# 1. Criar projeto
cd C:\Users\Administrador\source\repos\
npx create-next-app@latest orcamento-familiar-web --typescript --tailwind --app --no-src-dir

# 2. Entrar no projeto
cd orcamento-familiar-web

# 3. Instalar dependências
npm install @supabase/supabase-js @supabase/auth-helpers-nextjs
npm install zustand react-hook-form zod recharts date-fns lucide-react

# 4. Criar .env.local
@"
NEXT_PUBLIC_SUPABASE_URL=cole-aqui-depois
NEXT_PUBLIC_SUPABASE_ANON_KEY=cole-aqui-depois
"@ | Out-File -FilePath .env.local

# 5. Abrir no VS Code
code .

Write-Host "✅ Projeto criado! Agora:" -ForegroundColor Green
Write-Host "1. Configure Supabase e copie as credenciais para .env.local"
Write-Host "2. Execute: npm run dev"
Write-Host "3. Acesse: http://localhost:3000"
```

---

## ✅ Checklist de Início Rápido

- [ ] Criar conta no Supabase
- [ ] Criar conta no Vercel
- [ ] Criar conta no Cloudflare (opcional)
- [ ] Executar `start-projeto-web.ps1`
- [ ] Copiar credenciais do Supabase para `.env.local`
- [ ] Executar SQL para criar tabelas
- [ ] Rodar `npm run dev` e ver o projeto funcionando
- [ ] Começar a desenvolver! 🚀

---

**Autor**: Nícolas Ávila  
**Projeto**: Orçamento Familiar  
**Custo**: R$ 0,00  
**Data**: 22/12/2024

---

## 🎉 Resultado Final

Você terá:
- ✅ App web profissional
- ✅ Banco de dados PostgreSQL
- ✅ Autenticação completa
- ✅ SSL grátis
- ✅ CDN global
- ✅ Deploy automático
- ✅ Monitoramento de erros
- ✅ Backup automático (Supabase)

**TUDO DE GRAÇA!** 🎊

Quer que eu crie os arquivos de código prontos para você copiar e colar? 🚀
