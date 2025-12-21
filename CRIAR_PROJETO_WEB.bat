@echo off
chcp 65001 > nul
echo ============================================================
echo 🚀 CRIANDO ORÇAMENTO FAMILIAR WEB - 100%% GRATUITO
echo ============================================================
echo.
echo Desenvolvido por: Nícolas Ávila
echo Projeto: Orçamento Familiar
echo Custo: R$ 0,00 (FREE TIER)
echo.

cd /d "%~dp0"

echo [1/6] 📂 Criando pasta do projeto web...
if not exist "orcamento-familiar-web" (
    echo Criando projeto Next.js...
    call npx create-next-app@latest orcamento-familiar-web --typescript --tailwind --app --no-src-dir --import-alias "@/*" --yes
    echo ✅ Projeto criado!
) else (
    echo ℹ️  Projeto já existe
)

echo.
echo [2/6] 📦 Instalando dependências do Supabase...
cd orcamento-familiar-web
call npm install @supabase/supabase-js @supabase/auth-helpers-nextjs

echo.
echo [3/6] 🎨 Instalando bibliotecas de UI...
call npm install zustand react-hook-form zod @hookform/resolvers
call npm install date-fns lucide-react recharts
call npm install clsx tailwind-merge class-variance-authority

echo.
echo [4/6] 🎯 Instalando shadcn/ui...
call npx shadcn-ui@latest init -y --defaults

echo.
echo [5/6] 📝 Criando arquivo de configuração...
(
echo NEXT_PUBLIC_SUPABASE_URL=sua-url-aqui
echo NEXT_PUBLIC_SUPABASE_ANON_KEY=sua-chave-aqui
echo.
echo # Copie as credenciais do Supabase:
echo # 1. Acesse: https://supabase.com/dashboard
echo # 2. Selecione seu projeto
echo # 3. Settings ^> API
echo # 4. Copie "Project URL" e "anon public" key
) > .env.local

echo ✅ Arquivo .env.local criado!

echo.
echo [6/6] 🎉 Criando estrutura de pastas...
mkdir app\(auth)\login 2>nul
mkdir app\(auth)\register 2>nul
mkdir app\(dashboard) 2>nul
mkdir app\(dashboard)\contas 2>nul
mkdir app\(dashboard)\transacoes 2>nul
mkdir app\(dashboard)\orcamentos 2>nul
mkdir app\(dashboard)\metas 2>nul
mkdir components\ui 2>nul
mkdir components\auth 2>nul
mkdir components\dashboard 2>nul
mkdir lib 2>nul
mkdir hooks 2>nul
mkdir store 2>nul

echo.
echo ============================================================
echo ✅ PROJETO CRIADO COM SUCESSO!
echo ============================================================
echo.
echo 📋 PRÓXIMOS PASSOS:
echo.
echo 1. Configure o Supabase:
echo    - Acesse: https://supabase.com
echo    - Crie um projeto novo
echo    - Execute o SQL (arquivo SETUP_SUPABASE.sql)
echo    - Copie as credenciais para .env.local
echo.
echo 2. Execute o projeto:
echo    cd orcamento-familiar-web
echo    npm run dev
echo.
echo 3. Acesse: http://localhost:3000
echo.
echo 4. Deploy no Vercel (quando pronto):
echo    - Instale: npm install -g vercel
echo    - Execute: vercel --prod
echo.
echo ============================================================
pause
