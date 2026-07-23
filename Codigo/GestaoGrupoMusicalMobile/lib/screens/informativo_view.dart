import 'package:batala_mobile/model/informativo_model.dart';
import 'package:batala_mobile/service/informativo_service.dart';
import 'package:flutter/material.dart';

class InformativoView extends StatefulWidget {
  const InformativoView({super.key});

  @override
  State<InformativoView> createState() => _InformativoViewState();
}

class _InformativoViewState extends State<InformativoView> {
  late InformativoService service;
  final ScrollController _scrollController = ScrollController();
  
  int _currentPage = 1;
  final int _pageSize = 10;
  List<InformativoModel> _allItems = [];
  bool _isLoadingMore = false;
  bool _hasMorePages = true;
  bool _isInitialLoading = true;
  bool _isFromCache = false;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    service = InformativoService();
    _loadFirstPage();
    _scrollController.addListener(_onScroll);
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  Future<void> _loadFirstPage() async {
    setState(() {
      _isInitialLoading = true;
      _errorMessage = null;
      _allItems = [];
      _currentPage = 1;
      _hasMorePages = true;
    });

    try {
      final result = await service.getPaginated(
        pageNumber: 1,
        pageSize: _pageSize,
      );

      setState(() {
        _allItems = result.items;
        _hasMorePages = result.hasMorePages;
        _isFromCache = result.isFromCache;
        _isInitialLoading = false;
      });
    } catch (e) {
      setState(() {
        _errorMessage = 'Erro ao carregar avisos: $e';
        _isInitialLoading = false;
      });
      debugPrint('Erro ao carregar primeira página: $e');
    }
  }

  Future<void> _loadMorePages() async {
    if (_isLoadingMore || !_hasMorePages) return;

    setState(() {
      _isLoadingMore = true;
    });

    try {
      final nextPage = _currentPage + 1;
      final result = await service.getPaginated(
        pageNumber: nextPage,
        pageSize: _pageSize,
      );

      // Filtro para prevenir mensagens duplicadas caso a paginação da API sofra deslocamento
      // Ele verifica se já existe uma mensagem idêntica (mesmo texto e mesma data) na lista.
      final novosItens = result.items.where((novo) {
        return !_allItems.any((existente) => 
            existente.mensagem == novo.mensagem && 
            existente.dataInicio == novo.dataInicio);
      }).toList();

      setState(() {
        _allItems.addAll(novosItens);
        _hasMorePages = result.hasMorePages;
        _currentPage = nextPage;
        _isLoadingMore = false;
      });
    } catch (e) {
      setState(() {
        _isLoadingMore = false;
      });
      debugPrint('Erro ao carregar página ${_currentPage + 1}: $e');

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Erro ao carregar mais avisos: $e'),
            duration: const Duration(seconds: 2),
          ),
        );
      }
    }
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
            _scrollController.position.maxScrollExtent * 0.8 &&
        !_isLoadingMore &&
        _hasMorePages) {
      _loadMorePages();
    }
  }

  Future<void> _refreshData() async {
    await service.refreshCache();
    await _loadFirstPage();
  }

  Widget _buildCacheBadge() {
    if (_isFromCache) {
      return Container(
        margin: const EdgeInsets.only(top: 8, left: 16, right: 16, bottom: 8),
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
        decoration: BoxDecoration(
          color: Colors.blue.shade100,
          borderRadius: BorderRadius.circular(20),
          border: Border.all(color: Colors.blue.shade300),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.storage, size: 14, color: Colors.blue.shade700),
            const SizedBox(width: 6),
            Text(
              'Carregado do cache (dados locais)',
              style: TextStyle(
                fontSize: 12,
                color: Colors.blue.shade700,
              ),
            ),
          ],
        ),
      );
    }
    return const SizedBox.shrink();
  }

  @override
  Widget build(BuildContext context) {
    if (_isInitialLoading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_errorMessage != null) {
      return RefreshIndicator(
        onRefresh: _refreshData,
        child: CustomScrollView(
          physics: const AlwaysScrollableScrollPhysics(),
          slivers: [
            SliverFillRemaining(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text('Erro: $_errorMessage'),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: _loadFirstPage,
                    child: const Text('Tentar Novamente'),
                  ),
                ],
              ),
            ),
          ],
        ),
      );
    }

    if (_allItems.isEmpty) {
      return RefreshIndicator(
        onRefresh: _refreshData,
        child: CustomScrollView(
          physics: const AlwaysScrollableScrollPhysics(),
          slivers: [
            SliverFillRemaining(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.info_outline, size: 48, color: Colors.grey.shade400),
                  const SizedBox(height: 16),
                  Text(
                    'Nenhum aviso encontrado',
                    style: TextStyle(color: Colors.grey.shade600, fontSize: 16),
                  ),
                  const SizedBox(height: 16),
                  ElevatedButton.icon(
                    onPressed: _refreshData,
                    icon: const Icon(Icons.refresh),
                    label: const Text('Atualizar'),
                  ),
                ],
              ),
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _refreshData,
      child: ListView.builder(
        controller: _scrollController,
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.only(top: 10, bottom: 100),
        // Adiciona 1 na contagem total se houver mais páginas (para renderizar o loader)
        itemCount: _allItems.length + (_hasMorePages ? 1 : 0),
        itemBuilder: (context, index) {
          
          // Se chegamos no final da lista e ainda tem mais páginas, renderizamos o Loader/Botão
          if (index == _allItems.length) {
            return Padding(
              padding: const EdgeInsets.symmetric(vertical: 16),
              child: _isLoadingMore
                  ? const Center(child: CircularProgressIndicator())
                  : Center(
                      child: ElevatedButton.icon(
                        onPressed: _loadMorePages,
                        icon: const Icon(Icons.expand_more),
                        label: const Text('Carregar Mais'),
                      ),
                    ),
            );
          }

          final informativo = _allItems[index];

          // Se for o primeiro item da lista e os dados vierem do cache, mostra a badge em cima dele
          if (index == 0 && _isFromCache) {
            return Column(
              children: [
                _buildCacheBadge(),
                _buildInformativoTile(informativo),
              ],
            );
          }

          // Renderização normal para os outros itens
          return _buildInformativoTile(informativo);
        },
      ),
    );
  }

  Widget _buildInformativoTile(InformativoModel informativo) {
    return ListTile(
      leading: const Icon(Icons.info_outline, color: Color(0xFFD64550)),
      title: Text(
        informativo.mensagem,
        style: const TextStyle(fontWeight: FontWeight.bold),
        maxLines: 2,
        overflow: TextOverflow.ellipsis,
      ),
      subtitle: Text(
        "Postado em: ${informativo.dataInicio.day.toString().padLeft(2, '0')}/${informativo.dataInicio.month.toString().padLeft(2, '0')}/${informativo.dataInicio.year} às ${informativo.dataInicio.hour.toString().padLeft(2, '0')}:${informativo.dataInicio.minute.toString().padLeft(2, '0')}",
      ),
      isThreeLine: true,
    );
  }
}