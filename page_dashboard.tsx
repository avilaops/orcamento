// ============================================================
// 📊 DASHBOARD PRINCIPAL - Orçamento Familiar
// ============================================================
// Desenvolvido por: Nícolas Ávila
// Página principal com resumo financeiro
// ============================================================

'use client';

import { useEffect, useState } from 'react';
import { useAuth } from '@/hooks/useAuth';
import { 
  getResumoFinanceiro, 
  getTransacoes, 
  getContas,
  formatCurrency, 
  formatDate,
  getTransactionIcon,
  getTransactionColor,
  type Transacao,
  type Conta
} from '@/lib/supabase';
import { useRouter } from 'next/navigation';

interface ResumoFinanceiro {
  saldoTotal: number;
  receitasMes: number;
  despesasMes: number;
  saldoMes: number;
}

export default function DashboardPage() {
  const { user, signOut, loading: authLoading } = useAuth();
  const router = useRouter();

  const [resumo, setResumo] = useState<ResumoFinanceiro | null>(null);
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [contas, setContas] = useState<Conta[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Buscar dados
  useEffect(() => {
    if (!authLoading && user) {
      carregarDados();
    } else if (!authLoading && !user) {
      router.push('/login');
    }
  }, [user, authLoading]);

  const carregarDados = async () => {
    try {
      setLoading(true);
      setError('');

      const userId = user?.id!;

      // Buscar dados em paralelo
      const [resumoData, transacoesData, contasData] = await Promise.all([
        getResumoFinanceiro(userId),
        getTransacoes(userId, 5),
        getContas(userId),
      ]);

      setResumo(resumoData);
      setTransacoes(transacoesData);
      setContas(contasData);
    } catch (err: any) {
      console.error('Erro ao carregar dados:', err);
      setError('Erro ao carregar dados. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  // Loading state
  if (authLoading || loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mb-4"></div>
          <p className="text-gray-600">Carregando...</p>
        </div>
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 p-4">
        <div className="bg-white rounded-lg shadow-md p-8 max-w-md w-full text-center">
          <span className="text-5xl mb-4 inline-block">⚠️</span>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Ops!</h2>
          <p className="text-gray-600 mb-6">{error}</p>
          <button
            onClick={carregarDados}
            className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition"
          >
            Tentar novamente
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-4">
              <span className="text-3xl">💰</span>
              <div>
                <h1 className="text-2xl font-bold text-gray-900">
                  Orçamento Familiar
                </h1>
                <p className="text-sm text-gray-600">
                  Olá, {user?.user_metadata?.nome || user?.email}! 👋
                </p>
              </div>
            </div>

            <button
              onClick={signOut}
              className="text-gray-600 hover:text-gray-900 transition"
            >
              Sair →
            </button>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Cards de Resumo */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          {/* Saldo Total */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-sm font-medium text-gray-500">Saldo Total</h3>
              <span className="text-2xl">💵</span>
            </div>
            <p className="text-3xl font-bold text-blue-600">
              {formatCurrency(resumo?.saldoTotal || 0)}
            </p>
            <p className="text-xs text-gray-500 mt-2">
              {contas.length} conta{contas.length !== 1 ? 's' : ''} ativa{contas.length !== 1 ? 's' : ''}
            </p>
          </div>

          {/* Receitas do Mês */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-sm font-medium text-gray-500">Receitas (Mês)</h3>
              <span className="text-2xl">💰</span>
            </div>
            <p className="text-3xl font-bold text-green-600">
              {formatCurrency(resumo?.receitasMes || 0)}
            </p>
            <p className="text-xs text-gray-500 mt-2">
              Mês atual
            </p>
          </div>

          {/* Despesas do Mês */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-sm font-medium text-gray-500">Despesas (Mês)</h3>
              <span className="text-2xl">💸</span>
            </div>
            <p className="text-3xl font-bold text-red-600">
              {formatCurrency(resumo?.despesasMes || 0)}
            </p>
            <p className="text-xs text-gray-500 mt-2">
              Saldo: {formatCurrency(resumo?.saldoMes || 0)}
            </p>
          </div>
        </div>

        {/* Grid Principal */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Transações Recentes */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-xl font-semibold text-gray-900">
                Transações Recentes
              </h2>
              <button 
                className="text-sm text-blue-600 hover:text-blue-700"
                onClick={() => router.push('/dashboard/transacoes')}
              >
                Ver todas →
              </button>
            </div>

            {transacoes.length === 0 ? (
              <div className="text-center py-12">
                <span className="text-6xl mb-4 inline-block">📝</span>
                <p className="text-gray-600 mb-4">
                  Nenhuma transação ainda
                </p>
                <button
                  onClick={() => router.push('/dashboard/transacoes')}
                  className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition"
                >
                  Adicionar primeira transação
                </button>
              </div>
            ) : (
              <div className="space-y-3">
                {transacoes.map((transacao) => (
                  <div 
                    key={transacao.id}
                    className="flex items-center justify-between p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition"
                  >
                    <div className="flex items-center space-x-3">
                      <span className="text-2xl">
                        {getTransactionIcon(transacao.tipo)}
                      </span>
                      <div>
                        <p className="font-medium text-gray-900">
                          {transacao.descricao}
                        </p>
                        <p className="text-xs text-gray-500">
                          {formatDate(transacao.data_transacao)}
                          {transacao.categoria && ` • ${transacao.categoria}`}
                        </p>
                      </div>
                    </div>
                    <p className={`font-bold ${getTransactionColor(transacao.tipo)}`}>
                      {transacao.tipo === 'Despesa' && '-'}
                      {formatCurrency(transacao.valor)}
                    </p>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Contas */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-xl font-semibold text-gray-900">
                Minhas Contas
              </h2>
              <button 
                className="text-sm text-blue-600 hover:text-blue-700"
                onClick={() => router.push('/dashboard/contas')}
              >
                Gerenciar →
              </button>
            </div>

            {contas.length === 0 ? (
              <div className="text-center py-12">
                <span className="text-6xl mb-4 inline-block">🏦</span>
                <p className="text-gray-600 mb-4">
                  Nenhuma conta cadastrada
                </p>
                <button
                  onClick={() => router.push('/dashboard/contas')}
                  className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition"
                >
                  Adicionar primeira conta
                </button>
              </div>
            ) : (
              <div className="space-y-3">
                {contas.slice(0, 5).map((conta) => (
                  <div 
                    key={conta.id}
                    className="flex items-center justify-between p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition"
                  >
                    <div className="flex items-center space-x-3">
                      <div 
                        className="w-3 h-3 rounded-full"
                        style={{ backgroundColor: conta.cor || '#007AFF' }}
                      ></div>
                      <div>
                        <p className="font-medium text-gray-900">
                          {conta.nome}
                        </p>
                        <p className="text-xs text-gray-500">
                          {conta.tipo_conta}
                          {conta.banco && ` • ${conta.banco}`}
                        </p>
                      </div>
                    </div>
                    <p className="font-bold text-gray-900">
                      {formatCurrency(conta.saldo_atual)}
                    </p>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Quick Actions */}
        <div className="mt-8 grid grid-cols-2 md:grid-cols-4 gap-4">
          <button
            onClick={() => router.push('/dashboard/transacoes')}
            className="bg-white p-4 rounded-lg shadow-md hover:shadow-lg transition text-center"
          >
            <span className="text-3xl mb-2 inline-block">💸</span>
            <p className="text-sm font-medium text-gray-900">
              Nova Transação
            </p>
          </button>

          <button
            onClick={() => router.push('/dashboard/contas')}
            className="bg-white p-4 rounded-lg shadow-md hover:shadow-lg transition text-center"
          >
            <span className="text-3xl mb-2 inline-block">🏦</span>
            <p className="text-sm font-medium text-gray-900">
              Contas
            </p>
          </button>

          <button
            onClick={() => router.push('/dashboard/orcamentos')}
            className="bg-white p-4 rounded-lg shadow-md hover:shadow-lg transition text-center"
          >
            <span className="text-3xl mb-2 inline-block">📊</span>
            <p className="text-sm font-medium text-gray-900">
              Orçamentos
            </p>
          </button>

          <button
            onClick={() => router.push('/dashboard/metas')}
            className="bg-white p-4 rounded-lg shadow-md hover:shadow-lg transition text-center"
          >
            <span className="text-3xl mb-2 inline-block">🎯</span>
            <p className="text-sm font-medium text-gray-900">
              Metas
            </p>
          </button>
        </div>
      </main>

      {/* Footer */}
      <footer className="bg-white border-t border-gray-200 mt-12">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <p className="text-center text-sm text-gray-600">
            Desenvolvido com ❤️ por{' '}
            <a 
              href="https://github.com/avilaops" 
              target="_blank" 
              rel="noopener noreferrer"
              className="font-medium text-blue-600 hover:text-blue-700"
            >
              Nícolas Ávila
            </a>
          </p>
        </div>
      </footer>
    </div>
  );
}
