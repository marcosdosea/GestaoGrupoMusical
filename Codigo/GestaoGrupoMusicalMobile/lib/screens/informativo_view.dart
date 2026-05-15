import 'package:batala_mobile/model/informativo_model.dart';
import 'package:batala_mobile/service/informativo_service.dart';
import 'package:flutter/material.dart';

class InformativoView extends StatefulWidget {
  const InformativoView({super.key});

  @override
  State<InformativoView> createState() => _InformativoViewState();
}

class _InformativoViewState extends State<InformativoView> {
  final InformativoService _service = InformativoService();
  final ScrollController _scrollController = ScrollController();

  final List<InformativoModel> _informativos = [];
  bool _isLoading = false;
  bool _isLoadingMore = false;
  bool _hasMore = true;
  int _pageNumber = 1;

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
    _loadFirstPage();
  }

  @override
  void dispose() {
    _scrollController.removeListener(_onScroll);
    _scrollController.dispose();
    super.dispose();
  }

  Future<void> _loadFirstPage() async {
    setState(() {
      _isLoading = true;
      _pageNumber = 1;
      _hasMore = true;
      _informativos.clear();
    });

    await _loadPage(pageNumber: 1, resetList: true);

    if (mounted) {
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future<void> _loadPage({required int pageNumber, bool resetList = false}) async {
    if (_isLoadingMore) return;

    setState(() {
      _isLoadingMore = true;
    });

    try {
      final result = await _service.getPage(pageNumber: pageNumber, pageSize: 10);
      if (!mounted) return;

      setState(() {
        if (resetList) {
          _informativos
            ..clear()
            ..addAll(result.items);
        } else {
          _informativos.addAll(result.items);
        }
        _pageNumber = result.pageNumber;
        _hasMore = result.hasNextPage;
      });
    } catch (e) {
      if (!mounted) return;
      if (_informativos.isEmpty) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Erro ao carregar avisos: $e')),
        );
      }
    } finally {
      if (mounted) {
        setState(() {
          _isLoadingMore = false;
        });
      }
    }
  }

  Future<void> _onRefresh() async {
    await _loadFirstPage();
  }

  void _onScroll() {
    if (!_scrollController.hasClients || !_hasMore || _isLoadingMore) return;

    final threshold = _scrollController.position.maxScrollExtent - 200;
    if (_scrollController.position.pixels >= threshold) {
      _loadPage(pageNumber: _pageNumber + 1);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading && _informativos.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_informativos.isEmpty) {
      return RefreshIndicator(
        onRefresh: _onRefresh,
        child: ListView(
          controller: _scrollController,
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.only(top: 10, bottom: 100),
          children: const [
            SizedBox(height: 120),
            Center(child: Text('Nenhum informativo encontrado')),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _onRefresh,
      child: ListView.builder(
        controller: _scrollController,
        padding: const EdgeInsets.only(top: 10, bottom: 100),
        itemCount: _informativos.length + (_hasMore ? 1 : 0),
        itemBuilder: (context, index) {
          if (index == _informativos.length) {
            return const Padding(
              padding: EdgeInsets.symmetric(vertical: 16),
              child: Center(child: CircularProgressIndicator()),
            );
          }

          final informativo = _informativos[index];
          return ListTile(
            leading: const Icon(Icons.info_outline, color: Color(0xFFD64550)),
            title: Text(
              informativo.mensagem,
              style: const TextStyle(fontWeight: FontWeight.bold),
            ),
            subtitle: Text(
              "Postado em: ${informativo.dataInicio.day.toString().padLeft(2, '0')}/${informativo.dataInicio.month.toString().padLeft(2, '0')}/${informativo.dataInicio.year} às ${informativo.dataInicio.hour.toString().padLeft(2, '0')}:${informativo.dataInicio.minute.toString().padLeft(2, '0')}",
            ),
          );
        },
      ),
    );
  }
}