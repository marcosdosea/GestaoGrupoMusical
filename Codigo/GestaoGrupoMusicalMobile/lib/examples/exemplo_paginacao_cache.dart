/// Exemplos práticos de uso do sistema de paginação e cache de informativos
/// 
/// Este arquivo demonstra como usar o InformativoService, CacheService
/// e InformativoView para implementar paginação com cache local.

// ============================================================================
// EXEMPLO 1: Inicialização básica na View
// ============================================================================

import 'package:flutter/material.dart';
import 'service/informativo_service.dart';
import 'model/informativo_model.dart';

class ExemploInformativoView extends StatefulWidget {
  @override
  _ExemploInformativoViewState createState() => _ExemploInformativoViewState();
}

class _ExemploInformativoViewState extends State<ExemploInformativoView> {
  late InformativoService _service;
  List<InformativoModel> _informativos = [];
  int _currentPage = 1;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _service = InformativoService();
    _carregarPrimeiraPagina();
  }

  // ============================================================================
  // EXEMPLO 2: Carregar primeira página (com cache)
  // ============================================================================
  
  Future<void> _carregarPrimeiraPagina() async {
    setState(() => _isLoading = true);

    try {
      // Carrega página 1 com 20 itens
      // Se cache válido (< 2h), retorna do cache (0 bytes de rede!)
      // Se cache inválido, busca da API e salva em cache
      final resultado = await _service.getAllPaginated(
        pageNumber: 1,
        pageSize: 20,
        forceRefresh: false, // Usa cache se válido
      );

      setState(() {
        _informativos = resultado.items;
        _currentPage = resultado.pageNumber;
        _isLoading = false;
      });
    } catch (e) {
      print('Erro: $e');
      setState(() => _isLoading = false);
    }
  }

  // ============================================================================
  // EXEMPLO 3: Carregar próxima página (paginação infinita)
  // ============================================================================
  
  Future<void> _carregarProxima() async {
    try {
      final resultado = await _service.getAllPaginated(
        pageNumber: _currentPage + 1,
        pageSize: 20,
        forceRefresh: false,
      );

      setState(() {
        // IMPORTANTE: addAll, não replace! Mantém itens anteriores
        _informativos.addAll(resultado.items);
        _currentPage = resultado.pageNumber;
      });
    } catch (e) {
      print('Erro ao carregar mais: $e');
    }
  }

  // ============================================================================
  // EXEMPLO 4: Refresh manual (força sincronização com servidor)
  // ============================================================================
  
  Future<void> _sincronizarComServidor() async {
    try {
      // forceRefresh=true ignora cache e busca API sempre
      final resultado = await _service.getAllPaginated(
        pageNumber: 1,
        pageSize: 20,
        forceRefresh: true, // FORÇA sincronização
      );

      setState(() {
        _informativos = resultado.items;
        _currentPage = resultado.pageNumber;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('✅ Sincronizado com servidor')),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('❌ Erro: $e')),
      );
    }
  }

  // ============================================================================
  // EXEMPLO 5: Verificar informações do cache
  // ============================================================================
  
  Future<void> _verificarCache() async {
    final cacheInfo = await _service.getCacheInfo();
    
    print('=== INFO DO CACHE ===');
    print('Em cache: ${cacheInfo['cached']}');
    print('Itens em cache: ${cacheInfo['itemCount']}');
    print('Minutos desde atualização: ${cacheInfo['ageMinutes']}');
    print('Cache ainda válido: ${cacheInfo['isValid']}');
    print('Dados economizados: ${cacheInfo['dataSaved']}');
    print('Última sincronização: ${cacheInfo['lastSync']}');
  }

  // ============================================================================
  // EXEMPLO 6: Calcular economia de dados
  // ============================================================================
  
  Future<void> _mostrarEconomia() async {
    final economia = await _service.getDataSavingsEstimate();
    
    // Exemplo de output: "18 KB"
    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text('💾 Economia de Dados'),
        content: Text(
          'Você economizou $economia de dados ao usar cache local!',
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

  // ============================================================================
  // EXEMPLO 7: Limpar cache manualmente
  // ============================================================================
  
  Future<void> _limparCache() async {
    await _service.clearCache();
    
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('🗑️ Cache limpo')),
    );
  }

  // ============================================================================
  // EXEMPLO 8: Build com indicadores de cache e paginação
  // ============================================================================
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Informativos'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _sincronizarComServidor,
            tooltip: 'Sincronizar com servidor',
          ),
          IconButton(
            icon: const Icon(Icons.info),
            onPressed: _verificarCache,
            tooltip: 'Ver info do cache',
          ),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: _sincronizarComServidor,
        child: Column(
          children: [
            // Indicador de cache
            FutureBuilder<Map<String, dynamic>>(
              future: _service.getCacheInfo(),
              builder: (context, snapshot) {
                if (!snapshot.hasData) return const SizedBox();
                
                final info = snapshot.data!;
                final status = info['isValid'] ? '✅' : '⚠️';
                
                return Container(
                  color: Colors.blue.shade50,
                  padding: const EdgeInsets.all(8),
                  child: Text(
                    '$status ${info['itemCount']} avisos | ${info['dataSaved']} economizados',
                    style: const TextStyle(fontSize: 12),
                  ),
                );
              },
            ),
            
            // Lista
            Expanded(
              child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : ListView.builder(
                    itemCount: _informativos.length,
                    itemBuilder: (context, index) {
                      final info = _informativos[index];
                      return ListTile(
                        title: Text(info.mensagem),
                        subtitle: Text(
                          '${info.dataInicio.day}/${info.dataInicio.month}/${info.dataInicio.year}',
                        ),
                      );
                    },
                  ),
            ),
            
            // Botão "carregar mais"
            Padding(
              padding: const EdgeInsets.all(16),
              child: ElevatedButton(
                onPressed: _carregarProxima,
                child: const Text('Carregar Mais Avisos'),
              ),
            ),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _mostrarEconomia,
        tooltip: 'Mostrar economia',
        child: const Icon(Icons.savings),
      ),
    );
  }
}

// ============================================================================
// EXEMPLO 9: Usar em outro contexto (fora de StatefulWidget)
// ============================================================================

class ExemploUsoSimples {
  static Future<void> exemploCarregarAvisos() async {
    final service = InformativoService();
    
    try {
      // Simples - carrega primeira página
      final resultado = await service.getAllPaginated(pageNumber: 1);
      
      print('Carregados ${resultado.items.length} avisos');
      print('Página ${resultado.pageNumber} de ${resultado.totalPages}');
      
      for (var aviso in resultado.items) {
        print('- ${aviso.mensagem}');
      }
    } catch (e) {
      print('Erro: $e');
    }
  }
}

// ============================================================================
// EXEMPLO 10: Scenario - Modo offline
// ============================================================================

/// Exemplo de como o sistema funciona offline
class ExemploModoOffline {
  static Future<void> demonstracao() async {
    final service = InformativoService();
    
    // Primeira vez: requisição normal
    // (assume que o usuário estava online)
    print('1ª vez - Carrega e cachea dados da API');
    
    // Modo offline agora: sem internet
    print('Modo offline - Tenta requisição, falha, retorna cache');
    
    try {
      final resultado = await service.getAllPaginated(pageNumber: 1);
      
      // Funcionará se houver cache, mesmo offline!
      print('✅ Avisos carregados do cache (modo offline)');
      print('Avisos disponíveis: ${resultado.totalItems}');
    } catch (e) {
      print('❌ Erro: Sem internet e sem cache disponível');
    }
  }
}

// ============================================================================
// EXEMPLO 11: Melhores práticas
// ============================================================================

class MelhoresPraticas {
  /// ✅ BOM: Reutilizar instância de serviço
  final InformativoService _service = InformativoService();
  
  /// ❌ RUIM: Criar nova instância a cada requisição
  // void ruim() => InformativoService().getAllPaginated(pageNumber: 1);
  
  /// ✅ BOM: Chamar com forceRefresh=true apenas em refresh manual
  Future<void> bom() async {
    await _service.getAllPaginated(
      pageNumber: 1,
      forceRefresh: true, // Apenas em pull-to-refresh
    );
  }
  
  /// ✅ BOM: Verificar se há mais páginas antes de carregar
  Future<void> verificarPaginacao(int currentPage, int totalPages) async {
    if (currentPage < totalPages) {
      await _service.getAllPaginated(pageNumber: currentPage + 1);
    }
  }
  
  /// ✅ BOM: Tratamento de erros com fallback para cache
  Future<void> tratamentoErro() async {
    try {
      final resultado = await _service.getAllPaginated(pageNumber: 1);
      print('Dados: ${resultado.items.length}');
    } on Exception catch (e) {
      // Cache ainda disponível será retornado automaticamente
      print('Erro: $e (mas cache pode estar disponível)');
    }
  }
}

// ============================================================================
// EXEMPLO 12: Monitorar economia de dados
// ============================================================================

class MonitorarEconomia {
  static Future<void> exibirRelatorio() async {
    final service = InformativoService();
    
    // Simula 10 visitas
    for (int i = 1; i <= 10; i++) {
      final cacheInfo = await service.getCacheInfo();
      final economia = await service.getDataSavingsEstimate();
      
      print(
        'Visita $i: ${cacheInfo['itemCount']} itens, '
        '$economia economizados, '
        'válido: ${cacheInfo['isValid']}'
      );
      
      // Aguarda um pouco entre visitas
      await Future.delayed(const Duration(seconds: 1));
    }
  }
}
