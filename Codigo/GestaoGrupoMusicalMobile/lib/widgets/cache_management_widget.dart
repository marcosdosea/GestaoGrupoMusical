// ignore_for_file: unnecessary_to_list_in_spreads

import 'package:flutter/material.dart';
import '../config/cache_manager.dart';
import '../config/cache_service.dart';

/// Widget para gerenciar cache manualmente
/// Pode ser adicionado em um menu de settings ou usado isoladamente
class CacheManagementWidget extends StatefulWidget {
  const CacheManagementWidget({super.key});

  @override
  State<CacheManagementWidget> createState() => _CacheManagementWidgetState();
}

class _CacheManagementWidgetState extends State<CacheManagementWidget> {
  Map<String, bool> _cacheStatus = {};
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _loadCacheStatus();
  }

  Future<void> _loadCacheStatus() async {
    setState(() => _isLoading = true);
    try {
      final status = await CacheService.getCacheStatus();
      setState(() => _cacheStatus = status);
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Erro ao carregar status: $e')),
        );
      }
    } finally {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _clearAllCaches() async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Limpar Cache'),
        content: const Text(
          'Isso removerá todos os dados em cache. '
          'Os dados serão recarregados da internet quando necessário.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancelar'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Limpar', style: TextStyle(color: Colors.red)),
          ),
        ],
      ),
    );

    if (confirm == true) {
      await CacheService.clearAllCaches();
      await _loadCacheStatus();
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Cache limpo com sucesso!')),
        );
      }
    }
  }

  Future<void> _clearSpecificCache(String key) async {
    await CacheManager.clearCache(key);
    await _loadCacheStatus();
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Cache de $key removido!')),
      );
    }
  }

  String _getCacheLabel(String key) {
    const labels = {
      'ensaio_list': 'Ensaios',
      'evento_list': 'Eventos',
      'informativo_list': 'Informativos',
      'financeiro_associado': 'Financeiro (Associado)',
      'financeiro_campanhas': 'Campanhas de Pagamento',
      'material_estudo_list': 'Materiais de Estudo',
    };
    return labels[key] ?? key;
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Text(
                    'Gerenciamento de Cache',
                    style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  if (_isLoading)
                    const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    ),
                ],
              ),
              const SizedBox(height: 8),
              const Text(
                'O cache permite consultar dados offline após a primeira carga.',
                style: TextStyle(fontSize: 12, color: Colors.grey),
              ),
            ],
          ),
        ),
        Expanded(
          child: _isLoading
              ? const Center(child: CircularProgressIndicator())
              : ListView(
                  children: [
                    ..._cacheStatus.entries.map((entry) {
                      final key = entry.key;
                      final isValid = entry.value;
                      return ListTile(
                        leading: Icon(
                          isValid ? Icons.check_circle : Icons.schedule,
                          color: isValid ? Colors.green : Colors.orange,
                        ),
                        title: Text(_getCacheLabel(key)),
                        subtitle: Text(
                          isValid ? 'Cache válido' : 'Cache expirado',
                          style: TextStyle(
                            fontSize: 12,
                            color: isValid ? Colors.green : Colors.orange,
                          ),
                        ),
                        trailing: IconButton(
                          icon: const Icon(Icons.delete_outline),
                          onPressed: () => _clearSpecificCache(key),
                        ),
                      );
                    }).toList(),
                    const Divider(),
                    Padding(
                      padding: const EdgeInsets.all(16),
                      child: ElevatedButton.icon(
                        onPressed: _clearAllCaches,
                        icon: const Icon(Icons.delete),
                        label: const Text('Limpar Todo o Cache'),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.red.shade600,
                          foregroundColor: Colors.white,
                        ),
                      ),
                    ),
                  ],
                ),
        ),
      ],
    );
  }
}

/// Widget simples para atualizar dados com indicador de sincronização
class CacheRefreshButton extends StatefulWidget {
  final Future<void> Function() onRefresh;
  final bool isLoading;

  const CacheRefreshButton({
    super.key,
    required this.onRefresh,
    this.isLoading = false,
  });

  @override
  State<CacheRefreshButton> createState() => _CacheRefreshButtonState();
}

class _CacheRefreshButtonState extends State<CacheRefreshButton>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  bool _isRefreshing = false;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    );
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<void> _handleRefresh() async {
    if (_isRefreshing) return;

    setState(() => _isRefreshing = true);
    _controller.repeat();

    try {
      // Limpa todos os caches para forçar nova requisição
      await CacheManager.clearAllCache();
      // Executa função de refresh
      await widget.onRefresh();
    } finally {
      _controller.stop();
      if (mounted) {
        setState(() => _isRefreshing = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return IconButton(
      icon: RotationTransition(
        turns: _controller,
        child: const Icon(Icons.refresh),
      ),
      onPressed: _isRefreshing ? null : _handleRefresh,
      tooltip: 'Atualizar dados (limpar cache)',
    );
  }
}

/// Builder function para adicionar cache info em qualquer tela
void showCacheInfoDialog(BuildContext context, String dataType) {
  showDialog(
    context: context,
    builder: (context) => AlertDialog(
      title: const Text('Cache de Dados'),
      content: Text(
        'Estes dados estão armazenados em cache.\n\n'
        'Se está offline, os dados anteriormente carregados '
        'estarão disponíveis por 30 minutos após a última '
        'sincronização.\n\n'
        'Tipo de dado: $dataType',
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
