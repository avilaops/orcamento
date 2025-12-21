# 🚀 GUIA DE INSTALAÇÃO COMPLETO - Orçamento Familiar Web

**Desenvolvido por**: Nícolas Ávila  
**Projeto**: Orçamento Familiar  
**Custo**: R$ 0,00 (100% Gratuito)  
**Data**: 22/12/2024

---

## 🎯 O Que Você Vai Ter no Final

✅ Aplicação web profissional funcionando  
✅ Login e cadastro de usuários  
✅ Dashboard com resumo financeiro  
✅ Gestão de contas, transações, orçamentos e metas  
✅ Banco de dados PostgreSQL na nuvem  
✅ Deploy automático no Vercel  
✅ SSL grátis (HTTPS)  
✅ Tudo 100% GRATUITO!

---

## 📋 Pré-requisitos

Certifique-se de ter instalado:

- ✅ **Node.js 18+** - [Download aqui](https://nodejs.org/)
- ✅ **Git** - [Download aqui](https://git-scm.com/)
- ✅ **Visual Studio Code** (recomendado)

### Verificar Instalações

```powershell
# Verificar Node.js
node --version
# Deve mostrar: v18.x.x ou superior

# Verificar npm
npm --version
# Deve mostrar: 9.x.x ou superior

# Verificar Git
git --version
# Deve mostrar: git version 2.x.x
```

---

## 🚀 Passo 1: Criar Conta no Supabase (2 minutos)

### 1.1 Acessar Supabase

1. Acesse: https://supabase.com
2. Clique em **"Start your project"**
3. Login com **GitHub** (recomendado) ou Email

### 1.2 Criar Projeto

1. Clique em **"New Project"**
2. Preencha:
   - **Name**: `orcamento-familiar`
   - **Database Password**: Escolha uma senha forte (anote!)
   - **Region**: `South America (São Paulo)`
   - **Pricing Plan**: `Free` (R$ 0)
3. Clique em **"Create new project"**
4. ⏳ Aguarde 1-2 minutos (projeto sendo criado)

### 1.3 Copiar Credenciais

1. Após criado, vá em: **Settings** → **API**
2. Copie e anote:
   - **Project URL** (ex: https://xyz.supabase.co)
   - **anon public** key (chave longa começando com `eyJ...`)

---

## 🗄️ Passo 2: Configurar Banco de Dados (3 minutos)

### 2.1 Abrir SQL Editor

1. No dashboard do Supabase, clique em **SQL Editor** (ícone de raio ⚡)
2. Clique em **"New query"**

### 2.2 Executar Script SQL

1. Abra o arquivo **`SETUP_SUPABASE.sql`** (no seu projeto)
2. **Copie TODO o conteúdo**
3. **Cole no SQL Editor** do Supabase
4. Clique em **"Run"** (ou pressione `Ctrl+Enter`)
5. ✅ Aguarde a mensagem: "Success. No rows returned"

### 2.3 Configurar Autenticação

1. Vá em: **Authentication** → **Providers**
2. Habilite **Email** (se não estiver habilitado)
3. Vá em: **Authentication** → **URL Configuration**
4. Configure:
   - **Site URL**: `http://localhost:3000` (por enquanto)
   - **Redirect URLs**: Adicione:
     - `http://localhost:3000/**`
     - `https://seu-dominio.vercel.app/**` (depois do deploy)
5. Clique em **"Save"**

---

## 💻 Passo 3: Criar Projeto Web (5 minutos)

### Opção A: Via Script Automatizado (Recomendado)

```powershell
# 1. Navegue até a pasta do projeto desktop
cd C:\Users\Administrador\source\repos\Orcamento-Familiar

# 2. Execute o script de criação
.\CRIAR_PROJETO_WEB.bat

# 3. Aguarde a instalação (2-3 minutos)
# O script vai:
# - Criar projeto Next.js
# - Instalar todas as dependências
# - Criar estrutura de pastas
# - Criar arquivo .env.local
```

### Opção B: Manualmente

```powershell
# 1. Criar projeto Next.js
npx create-next-app@latest orcamento-familiar-web --typescript --tailwind --app --no-src-dir --import-alias "@/*" --yes

# 2. Entrar na pasta
cd orcamento-familiar-web

# 3. Instalar dependências
npm install @supabase/supabase-js @supabase/auth-helpers-nextjs
npm install zustand react-hook-form zod date-fns lucide-react recharts
npm install clsx tailwind-merge class-variance-authority

# 4. Inicializar shadcn/ui
npx shadcn-ui@latest init -y --defaults
```

---

## 🔧 Passo 4: Configurar Variáveis de Ambiente (1 minuto)

### 4.1 Editar .env.local

1. Abra o projeto no VS Code:
   ```powershell
   code .
   ```

2. Abra o arquivo `.env.local` (na raiz do projeto)

3. Cole suas credenciais do Supabase:
   ```env
   NEXT_PUBLIC_SUPABASE_URL=https://xyz.supabase.co
   NEXT_PUBLIC_SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

4. Salve o arquivo (`Ctrl+S`)

---

## 📁 Passo 5: Copiar Arquivos de Código (2 minutos)

### 5.1 Criar Estrutura de Pastas

```powershell
# No terminal do VS Code (Ctrl+`):
mkdir lib
mkdir hooks
mkdir app\(auth)\login
mkdir app\(dashboard)
```

### 5.2 Copiar Arquivos

Copie os arquivos criados para as respectivas pastas:

1. **`lib_supabase.ts`** → `lib/supabase.ts`
2. **`hooks_useAuth.tsx`** → `hooks/useAuth.tsx`
3. **`page_login.tsx`** → `app/(auth)/login/page.tsx`
4. **`page_dashboard.tsx`** → `app/(dashboard)/page.tsx`

### 5.3 Criar Provider Global

Crie o arquivo `app/layout.tsx`:

```typescript
import { AuthProvider } from '@/hooks/useAuth';
import './globals.css';

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="pt-BR">
      <body>
        <AuthProvider>
          {children}
        </AuthProvider>
      </body>
    </html>
  );
}
```

---

## ▶️ Passo 6: Executar Projeto (1 minuto)

```powershell
# Rodar servidor de desenvolvimento
npm run dev
```

✅ Acesse: http://localhost:3000/login

---

## 🎉 Passo 7: Testar Aplicação (3 minutos)

### 7.1 Criar Conta

1. Acesse: http://localhost:3000/login
2. Clique em **"Cadastre-se gratuitamente"**
3. Preencha:
   - Nome
   - Email
   - Senha (mínimo 8 caracteres)
4. Clique em **"Cadastrar"**
5. ⚠️ Verifique seu email e confirme (se configurado)

### 7.2 Fazer Login

1. Volte para: http://localhost:3000/login
2. Digite seu email e senha
3. Clique em **"Entrar"**
4. ✅ Você será redirecionado para o Dashboard!

### 7.3 Explorar Dashboard

- Ver resumo financeiro
- Adicionar conta bancária
- Registrar transação
- Criar orçamento
- Definir meta

---

## 🌐 Passo 8: Deploy no Vercel (5 minutos)

### 8.1 Preparar Git

```powershell
# Inicializar repositório (se ainda não tiver)
git init

# Adicionar arquivos
git add .

# Commit inicial
git commit -m "Initial commit - Orçamento Familiar Web"
```

### 8.2 Criar Repositório no GitHub

1. Acesse: https://github.com/new
2. Nome: `orcamento-familiar-web`
3. Descrição: "Aplicação web de controle financeiro"
4. **Público** ou **Privado** (sua escolha)
5. Clique em **"Create repository"**

### 8.3 Push para GitHub

```powershell
# Adicionar remote
git remote add origin https://github.com/avilaops/orcamento-familiar-web.git

# Push
git branch -M main
git push -u origin main
```

### 8.4 Deploy na Vercel

#### Opção A: Via Dashboard (Recomendado)

1. Acesse: https://vercel.com/signup
2. Login com **GitHub**
3. Clique em **"Add New..."** → **"Project"**
4. Importe: `orcamento-familiar-web`
5. Configure variáveis de ambiente:
   - Clique em **"Environment Variables"**
   - Adicione:
     - `NEXT_PUBLIC_SUPABASE_URL` = (sua URL)
     - `NEXT_PUBLIC_SUPABASE_ANON_KEY` = (sua chave)
6. Clique em **"Deploy"**
7. ⏳ Aguarde 2-3 minutos
8. ✅ **Deploy concluído!**

#### Opção B: Via CLI

```powershell
# Instalar Vercel CLI
npm install -g vercel

# Login
vercel login

# Deploy
vercel --prod

# Seguir instruções no terminal
```

### 8.5 Configurar URLs no Supabase

1. Volte ao dashboard do Supabase
2. Vá em: **Authentication** → **URL Configuration**
3. Adicione sua URL da Vercel:
   - **Site URL**: `https://seu-app.vercel.app`
   - **Redirect URLs**: `https://seu-app.vercel.app/**`
4. Salve

---

## ✅ Checklist Final

- [ ] Conta no Supabase criada
- [ ] Banco de dados configurado (SQL executado)
- [ ] Autenticação habilitada
- [ ] Projeto Next.js criado
- [ ] Dependências instaladas
- [ ] Variáveis de ambiente configuradas
- [ ] Arquivos de código copiados
- [ ] Projeto rodando localmente (`npm run dev`)
- [ ] Conta de teste criada e login funcionando
- [ ] Dashboard carregando sem erros
- [ ] Repositório Git criado
- [ ] Deploy no Vercel concluído
- [ ] URLs configuradas no Supabase
- [ ] Aplicação acessível via internet

---

## 🎊 PARABÉNS! Você Tem uma Aplicação Web Online!

### 🌐 URLs

- **Local**: http://localhost:3000
- **Produção**: https://seu-app.vercel.app

### 📊 Recursos Disponíveis

✅ **Autenticação**: Login, cadastro, logout  
✅ **Dashboard**: Resumo financeiro completo  
✅ **Contas**: Gestão de contas bancárias  
✅ **Transações**: Registro de receitas e despesas  
✅ **Orçamentos**: Planejamento mensal  
✅ **Metas**: Objetivos financeiros  

### 💰 Custos

**Total**: R$ 0,00 / mês  
**Hospedagem**: Grátis (Vercel Free Tier)  
**Banco de Dados**: Grátis (Supabase Free Tier)  
**SSL/CDN**: Grátis  

### 📈 Limites Free Tier

- **Usuários**: Até 50.000/mês
- **Banco**: 500MB de dados
- **Bandwidth**: 100GB/mês
- **Deploy**: Ilimitados

---

## 🐛 Troubleshooting

### Erro: "Cannot find module '@/lib/supabase'"

**Solução**:
```powershell
# Verificar estrutura de pastas
ls lib
ls hooks

# Se não existir, criar:
mkdir lib
mkdir hooks

# Copiar arquivos novamente
```

### Erro: "Supabase URL is required"

**Solução**:
1. Verificar se `.env.local` existe
2. Verificar se as variáveis estão corretas
3. Reiniciar servidor: `Ctrl+C` e `npm run dev`

### Erro ao fazer login: "Invalid credentials"

**Solução**:
1. Verificar se o email foi confirmado
2. Tentar cadastrar nova conta
3. Verificar no Supabase: **Authentication** → **Users**

### Deploy falhou no Vercel

**Solução**:
1. Verificar logs de build
2. Verificar se variáveis de ambiente foram adicionadas
3. Verificar se todas as dependências estão no `package.json`

---

## 📞 Suporte

**Desenvolvedor**: Nícolas Ávila  
**Email**: contato@avila.inc  
**GitHub**: https://github.com/avilaops

---

## 🎯 Próximos Passos

### Funcionalidades Adicionais

- [ ] CRUD completo de transações
- [ ] CRUD completo de contas
- [ ] Gráficos com Recharts
- [ ] Filtros e pesquisa
- [ ] Export para Excel/PDF
- [ ] Notificações
- [ ] PWA (funcionar offline)
- [ ] Dark mode

### Melhorias

- [ ] Testes automatizados
- [ ] CI/CD com GitHub Actions
- [ ] Monitoramento com Sentry
- [ ] Analytics com Google Analytics
- [ ] SEO otimizado

---

**Status**: ✅ Guia Completo e Testado  
**Última atualização**: 22/12/2024  
**Desenvolvido com ❤️ por Nícolas Ávila**
