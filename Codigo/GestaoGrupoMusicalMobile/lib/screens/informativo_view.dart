import 'package:batala_mobile/model/informativo_model.dart';
import 'package:batala_mobile/service/informativo_service.dart';
import 'package:flutter/material.dart';

class InformativoView extends StatefulWidget {
  const InformativoView({super.key});

  @override
  State<InformativoView> createState() => _InformativoViewState();
}

class _InformativoViewState extends State<InformativoView> {
  late InformativoService _service;
  late ScrollController _scrollController;
  
  List<InformativoModel> _informativos = [];
  int _currentPage = 1;
  int _totalPages = 1;
  bool _isLoading = false;
  bool _isLoadingMore = false;
  bool _isSyncingWithServer = false;
  String? _errorMessage;
  String _dataSavings = '0 KB'; // Para mostrar economia de dados

  @override
  void initState() {
    super.initState();
    _service = InformativoService();
    _scrollController = ScrollController();
    _scrollController.addListener(_onScroll);
    _loadFirstPage();
    _updateDataSavings(); // Atualiza indicador de economia
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  /// Atualiza o indicador de economia de dados
  Future<void> _updateDataSavings() async {
    try {
      final savings = await _service.getDataSavingsEstimate();
      if (mounted) {
        setState(() => _dataSavings = savings);
      }
    } catch (e) {
      // Silenciosamente ignora erros no cálculo de economia
    }
  }

  /// Carrega primeira página de informativos
  Future<void> _loadFirstPage({bool forceRefresh = false}) async {
    if (!mounted) return;
    
    setState(() {
      _isLoading = true;
      _isSyncingWithServer = forceRefresh;
      _errorMessage = null;
    });

    try {
      final result = await _service.getAllPaginated(
        pageNumber: 1,
        pageSize: 20,
        forceRefresh: forceRefresh,
      );

      if (mounted) {
        setState(() {
          _informativos = result.items;
          _currentPage = result.pageNumber;
          _totalPages = result.totalPages;
          _isLoading = false;
          _isSyncingWithServer = false;
        });
        _updateDataSavings(); // Atualiza economia após carregar
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _isLoading = false;
          _isSyncingWithServer = false;
          _errorMessage = 'Erro ao carregar informativos: $e';
        });
      }
    }
  }

  /// Carrega próxima página quando usuário faz scroll
  Future<void> _loadNextPage() async {
    if (_isLoadingMore || _currentPage >= _totalPages) return;

    if (!mounted) return;
    
    setState(() {
      _isLoadingMore = true;
    });

    try {
      final result = await _service.getAllPaginated(
        pageNumber: _currentPage + 1,
        pageSize: 20,
        forceRefresh: false,
      );

      if (mounted) {
        setState(() {
          _informativos.addAll(result.items);
          _currentPage = result.pageNumber;
          _totalPages = result.totalPages;
          _isLoadingMore = false;
        });
        _updateDataSavings(); // Atualiza economia após carregar
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _isLoadingMore = false;
          _errorMessage = 'Erro ao carregar mais informativos: $e';
        });
      }
    }
  }

  /// Detecta quando o usuário fez scroll perto do final
  void _onScroll() {
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent - 500) {
      _loadNextPage();
    }
  }

  /// Retorna indicador de status do cache e economia de dados
  Widget _buildCacheIndicator() {
    return FutureBuilder<Map<String, dynamic>>(
      future: _service.getCacheInfo(),
      builder: (context, snapshot) {
        if (!snapshot.hasData) {
          return const SizedBox.shrink();
        }

        final cacheInfo = snapshot.data!;
        if (!cacheInfo['cached']) {
          return Padding(
            padding: const EdgeInsets.only(bottom: 8.0),
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              decoration: BoxDecoration(
                color: Colors.blue.shade50,
                border: Border.all(color: Colors.blue.shade300),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Row(
                children: [
                  const Icon(Icons.cloud_download_outlined, color: Colors.blue, size: 18),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: const [
                        Text(
                          'Dados carregados do servidor',
                          style: TextStyle(fontSize: 12, color: Colors.blue, fontWeight: FontWeight.w600),
                        ),
                        Text(
                          'Toque para atualizar quando tiver conexão',
                          style: TextStyle(fontSize: 10, color: Colors.blue),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          );
        }

        final isValid = cacheInfo['isValid'] as bool;
        final ageMinutes = cacheInfo['ageMinutes'] as int;
        final itemCount = cacheInfo['itemCount'] ?? 0;

        return Padding(
          padding: const EdgeInsets.only(bottom: 8.0),
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
            decoration: BoxDecoration(
              color: isValid ? Colors.green.shade50 : Colors.orange.shade50,
              border: Border.all(
                color: isValid ? Colors.green.shade300 : Colors.orange.shade300,
              ),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Row(
              children: [
                Icon(
                  Icons.storage,
                  color: isValid ? Colors.green : Colors.orange,
                  size: 18,
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        isValid
                            ? '💾 Cache Local - Economizando dados'
                            : '⏰ Cache antigo (${ageMinutes}min) - Toque para atualizar',
                        style: TextStyle(
                          fontSize: 12,
                          fontWeight: FontWeight.w600,
                          color: isValid ? Colors.green.shade700 : Colors.orange.shade700,
                        ),
                      ),
                      Text(
                        '$itemCount avisos em cache • $_dataSavings economizados',
                        style: TextStyle(
                          fontSize: 10,
                          color: isValid ? Colors.green.shade600 : Colors.orange.shade600,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  /// Constrói lista de informativos
  Widget _buildInformativosList() {
    if (_informativos.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.info_outline,
              size: 48,
              color: Colors.grey.shade400,
            ),
            const SizedBox(height: 16),
            Text(
              'Nenhum informativo encontrado',
              style: TextStyle(
                fontSize: 16,
                color: Colors.grey.shade600,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              'Puxe para baixo para sincronizar',
              style: TextStyle(
                fontSize: 12,
                color: Colors.grey.shade500,
              ),
            ),
          ],
        ),
      );
    }

    return Column(
      children: [
        // Indicador de paginação
        if (_totalPages > 1)
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 8.0),
            child: Text(
              'Página $_currentPage de $_totalPages • ${_informativos.length} avisos carregados',
              style: TextStyle(
                fontSize: 11,
                color: Colors.grey.shade600,
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
        Expanded(
          child: ListView.builder(
            controller: _scrollController,
            padding: const EdgeInsets.only(top: 10, bottom: 100),
            itemCount: _informativos.length + (_isLoadingMore ? 1 : 0),
            itemBuilder: (context, index) {
              if (index == _informativos.length) {
                return Padding(
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  child: Center(
                    child: SizedBox(
                      width: 24,
                      height: 24,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        valueColor: AlwaysStoppedAnimation(Colors.grey.shade400),
                      ),
                    ),
                  ),
                );
              }

              final info = _informativos[index];
              return Card(
                margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                child: ListTile(
                  leading: Icon(
                    Icons.info_outline,
                    color: const Color(0xFFD64550),
                  ),
                  title: Text(
                    info.mensagem,
                    style: const TextStyle(fontWeight: FontWeight.bold),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                  subtitle: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const SizedBox(height: 4),
                      Text(
                        'Postado em: ${info.dataInicio.day.toString().padLeft(2, '0')}/${info.dataInicio.month.toString().padLeft(2, '0')}/${info.dataInicio.year} às ${info.dataInicio.hour.toString().padLeft(2, '0')}:${info.dataInicio.minute.toString().padLeft(2, '0')}',
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey.shade600,
                        ),
                      ),
                    ],
                  ),
                  isThreeLine: true,
                ),
              );
            },
          ),
        ),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return RefreshIndicator(
      onRefresh: () => _loadFirstPage(forceRefresh: true),
      child: Column(
        children: [
          if (_errorMessage != null)
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: Container(
                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                decoration: BoxDecoration(
                  color: Colors.red.shade50,
                  border: Border.all(color: Colors.red.shade300),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    Icon(Icons.error_outline, color: Colors.red.shade700, size: 18),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        _errorMessage!,
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.red.shade700,
                        ),
                      ),
                    ),
                    GestureDetector(
                      onTap: () {
                        setState(() => _errorMessage = null);
                      },
                      child: Icon(Icons.close, color: Colors.red.shade700, size: 18),
                    ),
                  ],
                ),
              ),
            ),
          Padding(
            padding: const EdgeInsets.all(8.0),
            child: Column(
              children: [
                _buildCacheIndicator(),
                if (_isSyncingWithServer)
                  Padding(
                    padding: const EdgeInsets.only(top: 8.0),
                    child: Container(
                      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                      decoration: BoxDecoration(
                        color: Colors.blue.shade50,
                        border: Border.all(color: Colors.blue.shade300),
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Row(
                        children: [
                          SizedBox(
                            width: 16,
                            height: 16,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              valueColor: AlwaysStoppedAnimation(Colors.blue.shade600),
                            ),
                          ),
                          const SizedBox(width: 8),
                          Expanded(
                            child: Text(
                              'Sincronizando com servidor...',
                              style: TextStyle(
                                fontSize: 12,
                                color: Colors.blue.shade700,
                                fontWeight: FontWeight.w500,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
              ],
            ),
          ),
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _buildInformativosList(),
          ),
          if (_currentPage < _totalPages && !_isLoadingMore)
            Padding(
              padding: const EdgeInsets.all(16),
              child: SizedBox(
                width: double.infinity,
                child: ElevatedButton.icon(
                  onPressed: _loadNextPage,
                  icon: const Icon(Icons.expand_more),
                  label: Text('Carregar mais avisos (página ${_currentPage + 1}/$_totalPages)'),
                ),
              ),
            ),
        ],
      ),
    );
  }
}