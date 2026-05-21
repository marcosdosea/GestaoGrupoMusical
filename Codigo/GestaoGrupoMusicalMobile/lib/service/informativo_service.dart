import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/cache_manager.dart';
import '../config/session_manager.dart';
import '../model/informativo_model.dart';

class PaginatedInformativeResult {
  final List<InformativoModel> items;
  final int pageNumber;
  final int pageSize;
  final int totalItems;
  final bool hasMorePages;
  final bool isFromCache;

  PaginatedInformativeResult({
    required this.items,
    required this.pageNumber,
    required this.pageSize,
    required this.totalItems,
    required this.hasMorePages,
    required this.isFromCache,
  });
}

class InformativoService {
  static const String _cacheKey = 'informativo_list';
  static const int _defaultPageSize = 10;

  /// Busca todos os informativos (sem paginação - para cache/sincronização)
  Future<List<InformativoModel>> getAll() async {
    try {
      // Tenta recuperar do cache primeiro
      final cachedData = await CacheManager.getCache(_cacheKey);
      if (cachedData != null) {
        debugPrint('Usando dados em cache para informativos');
        final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
        return _ordenarPorDataDecrescente(
          data.map((e) => InformativoModel.fromJson(e)).toList()
        );
      }
    } catch (e) {
      debugPrint('Erro ao recuperar cache de informativos: $e');
    }

    try {
      final String? token = await SessionManager.getToken();
      final response = await http.get(
        Uri.parse('${ApiConfig.baseUrl}/api/Informativo/Grupo'),
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final List data = jsonDecode(response.body);
        final informativos = data.map((e) => InformativoModel.fromJson(e)).toList();

        // Salva no cache
        await CacheManager.saveCache(_cacheKey, data);

        return _ordenarPorDataDecrescente(informativos);
      } else {
        // Se falhar, tenta retornar cache mesmo que expirado
        try {
          final cachedData = await CacheManager.getStaleCache(_cacheKey);
          if (cachedData != null) {
            final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
            return _ordenarPorDataDecrescente(
              data.map((e) => InformativoModel.fromJson(e)).toList()
            );
          }
        } catch (_) {}

        throw Exception('Erro: ${response.statusCode}');
      }
    } catch (e) {
      debugPrint('Erro ao buscar informativos: $e');
      rethrow;
    }
  }

  /// Busca informativos com paginação (otimizado para economizar dados)
  /// Carrega todos os dados uma vez e implementa paginação localmente
  Future<PaginatedInformativeResult> getPaginated({
    int pageNumber = 1,
    int pageSize = _defaultPageSize,
  }) async {
    try {
      // Carrega todos os informativos (usa cache quando disponível)
      final allInformativos = await getAll();

      // Calcula índices de paginação
      final totalItems = allInformativos.length;
      final startIndex = (pageNumber - 1) * pageSize;
      final endIndex = (startIndex + pageSize).clamp(0, totalItems);

      // Valida índices
      if (startIndex >= totalItems && totalItems > 0) {
        return PaginatedInformativeResult(
          items: [],
          pageNumber: pageNumber,
          pageSize: pageSize,
          totalItems: totalItems,
          hasMorePages: false,
          isFromCache: true,
        );
      }

      // Extrai página de dados
      final pageItems = allInformativos.sublist(
        startIndex,
        endIndex,
      );

      // Verifica se há mais páginas
      final hasMorePages = endIndex < totalItems;

      // Determina se dados vieram do cache
      final isCacheValid = await CacheManager.isCacheValid(_cacheKey);

      return PaginatedInformativeResult(
        items: pageItems,
        pageNumber: pageNumber,
        pageSize: pageSize,
        totalItems: totalItems,
        hasMorePages: hasMorePages,
        isFromCache: isCacheValid,
      );
    } catch (e) {
      debugPrint('Erro ao buscar informativos paginados: $e');
      rethrow;
    }
  }

  /// Ordena informativos por data decrescente (mais recentes primeiro)
  List<InformativoModel> _ordenarPorDataDecrescente(List<InformativoModel> informativos) {
    final lista = List<InformativoModel>.from(informativos);
    lista.sort((a, b) => b.dataInicio.compareTo(a.dataInicio));
    return lista;
  }

  /// Força refresh dos dados (limpa cache e busca novamente)
  /// Use com moderação para economizar dados
  Future<void> refreshCache() async {
    await CacheManager.refreshCache(_cacheKey);
  }
}