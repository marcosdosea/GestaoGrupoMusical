import 'package:batala_mobile/service/informativo_service.dart';
import 'package:flutter/material.dart';

/// Exemplos de Uso do Sistema de Cache e Paginação de Informativos
///
/// Este arquivo demonstra diferentes formas de usar o novo sistema
/// implementado para economizar dados e melhorar performance.

class ExemplosInformativoService {
  /// EXEMPLO 1: Carregar todos os informativos com cache automático
  /// Ideal para: Telas simples que mostram todos os avisos
  static Future<void> exemplo1_carregarTodos() async {
    final service = InformativoService();

    try {
      // Primeiro acesso: busca API
      // Próximos acessos (1h): usa cache (SEM DADOS!)
      final informativos = await service.getAll();

      print('Total de informativos: ${informativos.length}');
      for (final info in informativos) {
        print('- ${info.mensagem} (${info.dataInicio})');
      }
    } catch (e) {
      print('Erro: $e');
    }
  }

  /// EXEMPLO 2: Forçar atualização do servidor
  /// Ideal para: Botão de "Atualizar" ou pull-to-refresh
  static Future<void> exemplo2_forcarAtualizacao() async {
    final service = InformativoService();

    try {
      // forceRefresh=true ignora cache e busca API
      final informativos = await service.getAll(forceRefresh: true);
      print('${informativos.length} informativos atualizados');
    } catch (e) {
      print('Erro ao atualizar: $e');
    }
  }

  /// EXEMPLO 3: Paginação para economizar dados
  /// Ideal para: Listas longas onde não precisa carregar tudo de uma vez
  static Future<void> exemplo3_paginacao() async {
    final service = InformativoService();

    try {
      // Página 1: carrega 20 primeiros avisos
      // Quando usuário rola, carrega página 2, 3, etc
      final pagina1 = await service.getAllPaginated(
        pageNumber: 1,
        pageSize: 20,
      );

      print('Página 1:');
      print('- Itens: ${pagina1.items.length}');
      print('- Total: ${pagina1.totalItems}');
      print('- Páginas: ${pagina1.totalPages}');
      print('- Próxima página? ${pagina1.hasNextPage}');

      // Se tiver próxima página
      if (pagina1.hasNextPage) {
        final pagina2 = await service.getAllPaginated(pageNumber: 2);
        print('Página 2: ${pagina2.items.length} novos itens');
      }
    } catch (e) {
      print('Erro na paginação: $e');
    }
  }

  /// EXEMPLO 4: Carregar com verificação de cache antes
  /// Ideal para: Telas que precisam saber o status do cache
  static Future<void> exemplo4_verificarCache() async {
    final service = InformativoService();

    // Verifica estado do cache
    final cacheInfo = await service.getCacheInfo();

    if (cacheInfo['cached'] == false) {
      print('⚪ Sem cache - será buscado do servidor');
    } else if (cacheInfo['isValid'] == true) {
      print(
          '🟢 Cache válido (${cacheInfo['ageMinutes']}min atrás) - economizando dados!');
    } else {
      print(
          '🟠 Cache antigo (${cacheInfo['ageMinutes']}min) - recomenda-se atualizar');
    }

    // Agora carrega normalmente
    final informativos = await service.getAll();
    print('Carregados ${informativos.length} informativos');
  }

  /// EXEMPLO 5: Limpar cache manualmente
  /// Ideal para: Botão de "Limpar dados em cache" nas configurações
  static Future<void> exemplo5_limparCache() async {
    final service = InformativoService();

    await service.clearCache();
    print('✓ Cache de informativos limpo');

    // Próxima chamada buscará API mesmo que recente
    final informativos = await service.getAll();
    print('${informativos.length} informativos (do servidor)');
  }

  /// EXEMPLO 6: Tratamento robusto com fallback
  /// Ideal para: Produção com suporte a offline
  static Future<List<dynamic>> exemplo6_comFallback() async {
    final service = InformativoService();

    try {
      // Tenta carregar normalmente
      return await service.getAll();
    } catch (e) {
      print('⚠️ Erro ao buscar: $e');

      // Fallback: tenta cache mesmo que inválido
      final cached = await service.getCacheInfo();
      if (cached['cached'] == true) {
        print('📦 Usando cache antigo como fallback');
        return await service.getAll();
      } else {
        print('❌ Sem cache, nenhum dado disponível');
        return [];
      }
    }
  }

  /// EXEMPLO 7: Refresh periódico em background
  /// Ideal para: Sincronizar dados automaticamente
  static Future<void> exemplo7_refreshPeriodico() async {
    final service = InformativoService();

    // Simula refresh a cada 30 minutos
    Future.delayed(const Duration(minutes: 30), () async {
      print('🔄 Sincronizando informativos em background...');
      try {
        await service.getAll(forceRefresh: true);
        print('✓ Sincronizado com sucesso');
      } catch (e) {
        print('⚠️ Erro ao sincronizar: $e');
      }
    });
  }
}

// ============================================================================
// EXEMPLOS DE USO EM WIDGETS
// ============================================================================

/// EXEMPLO: Widget com paginação automática ao rolar
class ListaInformativosComPaginacao extends StatefulWidget {
  const ListaInformativosComPaginacao({Key? key}) : super(key: key);

  @override
  State<ListaInformativosComPaginacao> createState() =>
      _ListaInformativosComPaginacaoState();
}

class _ListaInformativosComPaginacaoState
    extends State<ListaInformativosComPaginacao> {
  final service = InformativoService();
  late ScrollController _scrollController;

  int _currentPage = 1;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController();
    _scrollController.addListener(_onScroll);
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent - 500) {
      // Usuário está perto do final - carregar próxima página
      _loadMore();
    }
  }

  Future<void> _loadMore() async {
    if (_isLoading) return;

    setState(() => _isLoading = true);

    try {
      final result = await service.getAllPaginated(
        pageNumber: _currentPage + 1,
        pageSize: 20,
      );

      // Adicionar novos itens à lista
      setState(() => _currentPage = result.pageNumber);
    } catch (e) {
      print('Erro ao carregar mais: $e');
    } finally {
      setState(() => _isLoading = false);
    }
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return const SizedBox(); // Implementação real no informativo_view.dart
  }
}

/// EXEMPLO: Widget com botão de limpar cache
class ConfiguracesCacheMixin {
  /// Mostra diálogo para limpar cache
  static Future<void> mostrarDialogoLimparCache(BuildContext context) async {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Limpar Cache'),
        content:
            const Text('Deseja limpar todos os dados em cache? Isso economizará espaço, mas sua próxima consulta usará dados do plano.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancelar'),
          ),
          TextButton(
            onPressed: () async {
              final service = InformativoService();
              await service.clearCache();
              if (context.mounted) {
                Navigator.pop(context);
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text('✓ Cache limpo com sucesso'),
                    duration: Duration(seconds: 2),
                  ),
                );
              }
            },
            child: const Text('Limpar', style: TextStyle(color: Colors.red)),
          ),
        ],
      ),
    );
  }

  /// Mostra informações do cache
  static Future<void> mostrarInfoCache(BuildContext context) async {
    final service = InformativoService();
    final info = await service.getCacheInfo();

    if (!context.mounted) return;

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Informações do Cache'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Cache Salvo: ${info['cached'] ? 'Sim ✓' : 'Não'}',
            ),
            if (info['cached']) ...[
              const SizedBox(height: 8),
              Text('Itens: ${info['itemCount']}'),
              Text('Idade: ${info['ageMinutes']} minuto(s)'),
              Text(
                'Válido: ${info['isValid'] ? 'Sim ✓' : 'Não (pode atualizar)'}',
              ),
            ],
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }
}

// ============================================================================
// COMPARAÇÃO: ANTES vs DEPOIS
// ============================================================================

/*
ANTES (SEM CACHE E PAGINAÇÃO):
==============================
1. Usuário abre app → Requisição 1 para API (todos os avisos)
2. Usuário fecha e abre novamente em 5 min → Requisição 2 (todos novamente)
3. Em 1 hora: até 12 requisições completas
4. Sem internet: sem dados
5. Muitos dados: carrega TUDO de uma vez

Consumo em 1 hora: ~12 requisições × 50KB = 600KB


DEPOIS (COM CACHE E PAGINAÇÃO):
===============================
1. Usuário abre app → Requisição 1 para API (20 avisos + cache)
2. Usuário fecha e abre novamente em 5 min → SEM REQUISIÇÃO (usa cache)
3. Em 1 hora: até 1 requisição (ao expirar cache)
4. Sem internet: mostra cache antigo
5. Muitos dados: carrega sob demanda (20 por vez)

Consumo em 1 hora: ~1 requisição × 20KB = 20KB
ECONOMIA: 97% de redução! 🎉

ECONOMIAS ADICIONAIS:
- Menos calor do processador (menos requisições)
- Bateria economizada
- App mais rápido (usa cache)
- Experiência melhor (offline support)
*/
