// ============================================================
// 🔐 CLIENTE SUPABASE - ORÇAMENTO FAMILIAR
// ============================================================
// Desenvolvido por: Nícolas Ávila
// Configuração do cliente Supabase para Next.js
// ============================================================

import { createClient } from '@supabase/supabase-js';

// Validação das variáveis de ambiente
const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL;
const supabaseAnonKey = process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY;

if (!supabaseUrl || !supabaseAnonKey) {
  throw new Error(
    '⚠️ Faltando credenciais do Supabase! ' +
    'Configure NEXT_PUBLIC_SUPABASE_URL e NEXT_PUBLIC_SUPABASE_ANON_KEY no arquivo .env.local'
  );
}

// Cliente Supabase
export const supabase = createClient(supabaseUrl, supabaseAnonKey);

// ============================================================
// TIPOS TYPESCRIPT
// ============================================================

export type Conta = {
  id: string;
  user_id: string;
  nome: string;
  tipo_conta: 'Corrente' | 'Poupanca' | 'Investimento' | 'Carteira' | 'Outro';
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

export type Transacao = {
  id: string;
  user_id: string;
  conta_id: string;
  descricao: string;
  valor: number;
  tipo: 'Receita' | 'Despesa' | 'Transferencia';
  categoria?: string;
  data_transacao: string;
  observacoes?: string;
  recorrente: boolean;
  frequencia_recorrencia?: 'Diaria' | 'Semanal' | 'Mensal' | 'Anual';
  parcela_atual?: number;
  total_parcelas?: number;
  created_at: string;
  updated_at: string;
};

export type Orcamento = {
  id: string;
  user_id: string;
  categoria: string;
  valor_planejado: number;
  mes: number;
  ano: number;
  created_at: string;
  updated_at: string;
};

export type Meta = {
  id: string;
  user_id: string;
  titulo: string;
  descricao?: string;
  valor_alvo: number;
  valor_atual: number;
  data_inicio: string;
  data_alvo: string;
  prioridade: 'Baixa' | 'Media' | 'Alta';
  status: 'Ativa' | 'Concluida' | 'Cancelada';
  created_at: string;
  updated_at: string;
};

export type Database = {
  public: {
    Tables: {
      contas: {
        Row: Conta;
        Insert: Omit<Conta, 'id' | 'created_at' | 'updated_at'>;
        Update: Partial<Omit<Conta, 'id' | 'created_at' | 'updated_at'>>;
      };
      transacoes: {
        Row: Transacao;
        Insert: Omit<Transacao, 'id' | 'created_at' | 'updated_at'>;
        Update: Partial<Omit<Transacao, 'id' | 'created_at' | 'updated_at'>>;
      };
      orcamentos: {
        Row: Orcamento;
        Insert: Omit<Orcamento, 'id' | 'created_at' | 'updated_at'>;
        Update: Partial<Omit<Orcamento, 'id' | 'created_at' | 'updated_at'>>;
      };
      metas: {
        Row: Meta;
        Insert: Omit<Meta, 'id' | 'created_at' | 'updated_at'>;
        Update: Partial<Omit<Meta, 'id' | 'created_at' | 'updated_at'>>;
      };
    };
  };
};

// ============================================================
// FUNÇÕES AUXILIARES
// ============================================================

/**
 * Formata valor monetário para exibição
 */
export function formatCurrency(value: number): string {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  }).format(value);
}

/**
 * Formata data para exibição
 */
export function formatDate(date: string): string {
  return new Date(date).toLocaleDateString('pt-BR');
}

/**
 * Calcula porcentagem
 */
export function calculatePercentage(current: number, total: number): number {
  if (total === 0) return 0;
  return Math.round((current / total) * 100);
}

/**
 * Retorna cor baseada no tipo de transação
 */
export function getTransactionColor(tipo: Transacao['tipo']): string {
  const colors = {
    Receita: 'text-green-600',
    Despesa: 'text-red-600',
    Transferencia: 'text-blue-600',
  };
  return colors[tipo];
}

/**
 * Retorna ícone baseado no tipo de transação
 */
export function getTransactionIcon(tipo: Transacao['tipo']): string {
  const icons = {
    Receita: '💰',
    Despesa: '💸',
    Transferencia: '🔄',
  };
  return icons[tipo];
}

// ============================================================
// API FUNCTIONS
// ============================================================

/**
 * Buscar todas as contas do usuário
 */
export async function getContas(userId: string) {
  const { data, error } = await supabase
    .from('contas')
    .select('*')
    .eq('user_id', userId)
    .order('created_at', { ascending: false });

  if (error) throw error;
  return data as Conta[];
}

/**
 * Buscar transações do usuário
 */
export async function getTransacoes(userId: string, limit?: number) {
  let query = supabase
    .from('transacoes')
    .select('*')
    .eq('user_id', userId)
    .order('data_transacao', { ascending: false });

  if (limit) {
    query = query.limit(limit);
  }

  const { data, error } = await query;

  if (error) throw error;
  return data as Transacao[];
}

/**
 * Buscar resumo financeiro
 */
export async function getResumoFinanceiro(userId: string) {
  // Buscar contas ativas
  const { data: contas, error: contasError } = await supabase
    .from('contas')
    .select('saldo_atual, incluir_no_total, ativa')
    .eq('user_id', userId);

  if (contasError) throw contasError;

  const saldoTotal = contas
    ?.filter((c) => c.incluir_no_total && c.ativa)
    .reduce((acc, c) => acc + Number(c.saldo_atual), 0) || 0;

  // Buscar transações do mês
  const hoje = new Date();
  const primeiroDiaMes = new Date(hoje.getFullYear(), hoje.getMonth(), 1);
  const ultimoDiaMes = new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0);

  const { data: transacoes, error: transacoesError } = await supabase
    .from('transacoes')
    .select('tipo, valor')
    .eq('user_id', userId)
    .gte('data_transacao', primeiroDiaMes.toISOString())
    .lte('data_transacao', ultimoDiaMes.toISOString());

  if (transacoesError) throw transacoesError;

  const receitasMes = transacoes
    ?.filter((t) => t.tipo === 'Receita')
    .reduce((acc, t) => acc + Number(t.valor), 0) || 0;

  const despesasMes = transacoes
    ?.filter((t) => t.tipo === 'Despesa')
    .reduce((acc, t) => acc + Number(t.valor), 0) || 0;

  return {
    saldoTotal,
    receitasMes,
    despesasMes,
    saldoMes: receitasMes - despesasMes,
  };
}

export default supabase;
