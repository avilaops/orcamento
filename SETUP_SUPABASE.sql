-- ============================================================
-- 🗄️ SETUP BANCO DE DADOS - ORÇAMENTO FAMILIAR
-- ============================================================
-- Desenvolvido por: Nícolas Ávila
-- Projeto: Orçamento Familiar
-- Database: PostgreSQL (Supabase)
-- Data: 22/12/2024
-- ============================================================

-- INSTRUÇÕES:
-- 1. Acesse: https://supabase.com/dashboard
-- 2. Selecione seu projeto
-- 3. Vá em: SQL Editor
-- 4. Cole este script completo
-- 5. Clique em "Run"

-- ============================================================
-- TABELAS PRINCIPAIS
-- ============================================================

-- Tabela de contas bancárias
CREATE TABLE IF NOT EXISTS contas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE NOT NULL,
    nome VARCHAR(100) NOT NULL,
    tipo_conta VARCHAR(20) NOT NULL CHECK (tipo_conta IN ('Corrente', 'Poupanca', 'Investimento', 'Carteira', 'Outro')),
    saldo_inicial DECIMAL(15,2) DEFAULT 0.00,
    saldo_atual DECIMAL(15,2) DEFAULT 0.00,
    banco VARCHAR(50),
    agencia VARCHAR(20),
    numero_conta VARCHAR(30),
    cor VARCHAR(7) DEFAULT '#007AFF',
    ativa BOOLEAN DEFAULT TRUE,
    incluir_no_total BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Tabela de transações
CREATE TABLE IF NOT EXISTS transacoes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE NOT NULL,
    conta_id UUID REFERENCES contas(id) ON DELETE CASCADE NOT NULL,
    descricao VARCHAR(200) NOT NULL,
    valor DECIMAL(15,2) NOT NULL,
    tipo VARCHAR(20) NOT NULL CHECK (tipo IN ('Receita', 'Despesa', 'Transferencia')),
    categoria VARCHAR(50),
    data_transacao DATE NOT NULL DEFAULT CURRENT_DATE,
    observacoes TEXT,
    recorrente BOOLEAN DEFAULT FALSE,
    frequencia_recorrencia VARCHAR(20) CHECK (frequencia_recorrencia IN ('Diaria', 'Semanal', 'Mensal', 'Anual')),
    parcela_atual INTEGER,
    total_parcelas INTEGER,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Tabela de orçamentos
CREATE TABLE IF NOT EXISTS orcamentos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE NOT NULL,
    categoria VARCHAR(50) NOT NULL,
    valor_planejado DECIMAL(15,2) NOT NULL,
    mes INTEGER NOT NULL CHECK (mes BETWEEN 1 AND 12),
    ano INTEGER NOT NULL CHECK (ano >= 2000),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(user_id, categoria, mes, ano)
);

-- Tabela de metas financeiras
CREATE TABLE IF NOT EXISTS metas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE NOT NULL,
    titulo VARCHAR(100) NOT NULL,
    descricao TEXT,
    valor_alvo DECIMAL(15,2) NOT NULL,
    valor_atual DECIMAL(15,2) DEFAULT 0.00,
    data_inicio DATE NOT NULL DEFAULT CURRENT_DATE,
    data_alvo DATE NOT NULL,
    prioridade VARCHAR(20) CHECK (prioridade IN ('Baixa', 'Media', 'Alta')) DEFAULT 'Media',
    status VARCHAR(20) CHECK (status IN ('Ativa', 'Concluida', 'Cancelada')) DEFAULT 'Ativa',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ============================================================
-- ÍNDICES PARA PERFORMANCE
-- ============================================================

CREATE INDEX IF NOT EXISTS idx_contas_user_id ON contas(user_id);
CREATE INDEX IF NOT EXISTS idx_contas_ativa ON contas(ativa) WHERE ativa = TRUE;

CREATE INDEX IF NOT EXISTS idx_transacoes_user_id ON transacoes(user_id);
CREATE INDEX IF NOT EXISTS idx_transacoes_conta_id ON transacoes(conta_id);
CREATE INDEX IF NOT EXISTS idx_transacoes_data ON transacoes(data_transacao);
CREATE INDEX IF NOT EXISTS idx_transacoes_tipo ON transacoes(tipo);
CREATE INDEX IF NOT EXISTS idx_transacoes_categoria ON transacoes(categoria);

CREATE INDEX IF NOT EXISTS idx_orcamentos_user_id ON orcamentos(user_id);
CREATE INDEX IF NOT EXISTS idx_orcamentos_periodo ON orcamentos(user_id, ano, mes);

CREATE INDEX IF NOT EXISTS idx_metas_user_id ON metas(user_id);
CREATE INDEX IF NOT EXISTS idx_metas_status ON metas(status);

-- ============================================================
-- FUNÇÕES AUXILIARES
-- ============================================================

-- Função para atualizar updated_at automaticamente
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Triggers para atualizar updated_at
CREATE TRIGGER update_contas_updated_at
    BEFORE UPDATE ON contas
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_transacoes_updated_at
    BEFORE UPDATE ON transacoes
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_orcamentos_updated_at
    BEFORE UPDATE ON orcamentos
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_metas_updated_at
    BEFORE UPDATE ON metas
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- ============================================================
-- ROW LEVEL SECURITY (RLS)
-- ============================================================

-- Habilitar RLS em todas as tabelas
ALTER TABLE contas ENABLE ROW LEVEL SECURITY;
ALTER TABLE transacoes ENABLE ROW LEVEL SECURITY;
ALTER TABLE orcamentos ENABLE ROW LEVEL SECURITY;
ALTER TABLE metas ENABLE ROW LEVEL SECURITY;

-- ============================================================
-- POLÍTICAS DE SEGURANÇA - CONTAS
-- ============================================================

CREATE POLICY "Usuários podem ver suas próprias contas"
    ON contas FOR SELECT
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem inserir suas próprias contas"
    ON contas FOR INSERT
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem atualizar suas próprias contas"
    ON contas FOR UPDATE
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem deletar suas próprias contas"
    ON contas FOR DELETE
    USING (auth.uid() = user_id);

-- ============================================================
-- POLÍTICAS DE SEGURANÇA - TRANSAÇÕES
-- ============================================================

CREATE POLICY "Usuários podem ver suas próprias transações"
    ON transacoes FOR SELECT
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem inserir suas próprias transações"
    ON transacoes FOR INSERT
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem atualizar suas próprias transações"
    ON transacoes FOR UPDATE
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem deletar suas próprias transações"
    ON transacoes FOR DELETE
    USING (auth.uid() = user_id);

-- ============================================================
-- POLÍTICAS DE SEGURANÇA - ORÇAMENTOS
-- ============================================================

CREATE POLICY "Usuários podem ver seus próprios orçamentos"
    ON orcamentos FOR SELECT
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem inserir seus próprios orçamentos"
    ON orcamentos FOR INSERT
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem atualizar seus próprios orçamentos"
    ON orcamentos FOR UPDATE
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem deletar seus próprios orçamentos"
    ON orcamentos FOR DELETE
    USING (auth.uid() = user_id);

-- ============================================================
-- POLÍTICAS DE SEGURANÇA - METAS
-- ============================================================

CREATE POLICY "Usuários podem ver suas próprias metas"
    ON metas FOR SELECT
    USING (auth.uid() = user_id);

CREATE POLICY "Usuários podem inserir suas próprias metas"
    ON metas FOR INSERT
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem atualizar suas próprias metas"
    ON metas FOR UPDATE
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Usuários podem deletar suas próprias metas"
    ON metas FOR DELETE
    USING (auth.uid() = user_id);

-- ============================================================
-- VIEWS ÚTEIS
-- ============================================================

-- View: Resumo financeiro por usuário
CREATE OR REPLACE VIEW resumo_financeiro AS
SELECT 
    c.user_id,
    SUM(CASE WHEN c.incluir_no_total AND c.ativa THEN c.saldo_atual ELSE 0 END) as saldo_total,
    COUNT(DISTINCT c.id) as total_contas,
    COUNT(DISTINCT t.id) as total_transacoes
FROM contas c
LEFT JOIN transacoes t ON t.conta_id = c.id
GROUP BY c.user_id;

-- View: Transações do mês atual
CREATE OR REPLACE VIEW transacoes_mes_atual AS
SELECT 
    t.*,
    c.nome as conta_nome,
    c.banco as conta_banco
FROM transacoes t
JOIN contas c ON c.id = t.conta_id
WHERE 
    EXTRACT(MONTH FROM t.data_transacao) = EXTRACT(MONTH FROM CURRENT_DATE)
    AND EXTRACT(YEAR FROM t.data_transacao) = EXTRACT(YEAR FROM CURRENT_DATE);

-- View: Despesas por categoria (mês atual)
CREATE OR REPLACE VIEW despesas_por_categoria AS
SELECT 
    user_id,
    categoria,
    SUM(valor) as total,
    COUNT(*) as quantidade
FROM transacoes
WHERE 
    tipo = 'Despesa'
    AND EXTRACT(MONTH FROM data_transacao) = EXTRACT(MONTH FROM CURRENT_DATE)
    AND EXTRACT(YEAR FROM data_transacao) = EXTRACT(YEAR FROM CURRENT_DATE)
GROUP BY user_id, categoria;

-- ============================================================
-- DADOS DE EXEMPLO (OPCIONAL - DESCOMENTAR SE QUISER)
-- ============================================================

/*
-- Inserir conta de exemplo para testes
-- IMPORTANTE: Substitua 'SEU_USER_ID' pelo ID do usuário após criar conta

INSERT INTO contas (user_id, nome, tipo_conta, saldo_inicial, saldo_atual, banco, cor)
VALUES 
    ('SEU_USER_ID', 'Conta Corrente Principal', 'Corrente', 1000.00, 1000.00, 'Banco do Brasil', '#007AFF'),
    ('SEU_USER_ID', 'Poupança', 'Poupanca', 5000.00, 5000.00, 'Caixa', '#34C759');

-- Inserir transações de exemplo
INSERT INTO transacoes (user_id, conta_id, descricao, valor, tipo, categoria, data_transacao)
SELECT 
    'SEU_USER_ID',
    id,
    'Salário',
    3500.00,
    'Receita',
    'Salário',
    CURRENT_DATE
FROM contas WHERE nome = 'Conta Corrente Principal' LIMIT 1;

INSERT INTO transacoes (user_id, conta_id, descricao, valor, tipo, categoria, data_transacao)
SELECT 
    'SEU_USER_ID',
    id,
    'Supermercado',
    250.00,
    'Despesa',
    'Alimentação',
    CURRENT_DATE
FROM contas WHERE nome = 'Conta Corrente Principal' LIMIT 1;
*/

-- ============================================================
-- VERIFICAÇÃO FINAL
-- ============================================================

-- Verificar se todas as tabelas foram criadas
DO $$
BEGIN
    RAISE NOTICE 'Verificando tabelas...';
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'contas') THEN
        RAISE NOTICE '✅ Tabela "contas" criada com sucesso';
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'transacoes') THEN
        RAISE NOTICE '✅ Tabela "transacoes" criada com sucesso';
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'orcamentos') THEN
        RAISE NOTICE '✅ Tabela "orcamentos" criada com sucesso';
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'metas') THEN
        RAISE NOTICE '✅ Tabela "metas" criada com sucesso';
    END IF;
    
    RAISE NOTICE '';
    RAISE NOTICE '============================================================';
    RAISE NOTICE '🎉 BANCO DE DADOS CONFIGURADO COM SUCESSO!';
    RAISE NOTICE '============================================================';
    RAISE NOTICE '';
    RAISE NOTICE '📋 Próximos passos:';
    RAISE NOTICE '1. Vá em: Authentication > Providers';
    RAISE NOTICE '2. Habilite: Email provider';
    RAISE NOTICE '3. Configure: Site URL e Redirect URLs';
    RAISE NOTICE '4. Copie as credenciais (Settings > API)';
    RAISE NOTICE '5. Cole no arquivo .env.local do projeto';
    RAISE NOTICE '';
END $$;
