import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/cache_manager.dart';
import '../config/session_manager.dart';
import '../model/informativo_model.dart';
<<<<<<< HEAD
import '../model/informativo_page_result.dart';

class InformativoService {
  static const int _defaultPageSize = 10;

  String _pageCacheKey(int pageNumber, int pageSize) => 'informativo_page_${pageSize}_$pageNumber';
  
  Future<List<InformativoModel>> getAll() async {
    final List<InformativoModel> informativos = [];
    var pageNumber = 1;
    var hasNextPage = true;

    while (hasNextPage) {
      final pageResult = await getPage(pageNumber: pageNumber, pageSize: _defaultPageSize);
      informativos.addAll(pageResult.items);
      hasNextPage = pageResult.hasNextPage;
      pageNumber++;
    }

    return informativos;
  }

  Future<InformativoPageResult> getPage({
    required int pageNumber,
    int pageSize = _defaultPageSize,
    bool forceRefresh = false,
  }) async {
    final cacheKey = _pageCacheKey(pageNumber, pageSize);

    if (!forceRefresh) {
      try {
        final cachedData = await CacheManager.getCache(cacheKey);
        if (cachedData != null) {
          debugPrint('Usando pagina em cache para informativos: $pageNumber');
          return InformativoPageResult.fromJson(
            cachedData is Map<String, dynamic> ? cachedData : jsonDecode(cachedData),
          );
        }
      } catch (e) {
        debugPrint('Erro ao recuperar cache de informativos: $e');
      }
    }

    try {
      final String? token = await SessionManager.getToken();
      final response = await http.get(
        Uri.parse('${ApiConfig.baseUrl}/api/Informativo/Grupo/Paginado?pageNumber=$pageNumber&pageSize=$pageSize'),
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final Map<String, dynamic> data = jsonDecode(response.body) as Map<String, dynamic>;
        final pageResult = InformativoPageResult.fromJson(data);

        await CacheManager.saveCache(cacheKey, data);

        return pageResult;
      }

      try {
        final cachedData = await CacheManager.getStaleCache(cacheKey);
        if (cachedData != null) {
          return InformativoPageResult.fromJson(
            cachedData is Map<String, dynamic> ? cachedData : jsonDecode(cachedData),
          );
        }
      } catch (_) {}

      throw Exception('Erro: ${response.statusCode}');
    } catch (e) {
      debugPrint('Erro ao buscar informativos: $e');
      rethrow;
    }
  }
}
=======
import 'cache_service.dart';

class InformativoService {
  static const int _pageSize = 20; // Número de itens por página
  
  // Controla se já tentou sincronizar nesta sessão
  static bool _hasAttemptedSync = false;

  /// Obtém todos os informativos com suporte a cache e paginação
  /// Prioriza cache local se disponível e válido
  /// Sincroniza com servidor apenas se necessário
  Future<List<InformativoModel>> getAll({bool forceRefresh = false}) async {
    try {
      // Se não forçar refresh, tenta carregar do cache
      if (!forceRefresh) {
        final isValid = await CacheService.isCacheValid();
        if (isValid) {
          final cached = await CacheService.getInformativosCache();
          if (cached != null && cached.isNotEmpty) {
            return _sortByDateDescending(cached);
          }
        }
      }

      // Se cache não está disponível, busca da API
      final informativos = await _fetchFromApi();
      
      // Salva no cache para futuras consultas
      await CacheService.cacheInformativos(informativos);
      
      return _sortByDateDescending(informativos);
    } catch (e) {
      // Se houver erro na API, tenta retornar do cache mesmo que inválido
      final cached = await CacheService.getInformativosCache();
      if (cached != null && cached.isNotEmpty) {
        return _sortByDateDescending(cached);
      }
      rethrow;
    }
  }

  /// Obtém informativos com paginação
  /// pageNumber: número da página (começando em 1)
  /// pageSize: número de itens por página (padrão: 20)
  /// forceRefresh: força sincronização com servidor
  /// 
  /// Estratégia de economia de dados:
  /// 1. Primeiro verifica se há cache válido
  /// 2. Se cache é válido, retorna dados paginados do cache
  /// 3. Se cache é inválido/ausente, busca da API
  /// 4. Mescla dados da API com cache local para maximizar disponibilidade
  Future<PaginatedInformativos> getAllPaginated({
    int pageNumber = 1,
    int pageSize = _pageSize,
    bool forceRefresh = false,
  }) async {
    try {
      // Estratégia 1: Se não forçar refresh, tenta cache
      if (!forceRefresh) {
        final isValid = await CacheService.isCacheValid();
        if (isValid) {
          final cached = await CacheService.getInformativosCache();
          if (cached != null && cached.isNotEmpty) {
            return _paginateInformativos(
              _sortByDateDescending(cached),
              pageNumber,
              pageSize,
            );
          }
        }
      }

      // Estratégia 2: Cache inválido/ausente, busca da API
      final informativos = await _fetchFromApi();
      
      // Estratégia 3: Mescla com cache para evitar perda de dados
      List<InformativoModel> mergedData = informativos;
      if (!forceRefresh) {
        final cachedData = await CacheService.getInformativosCache();
        if (cachedData != null && cachedData.isNotEmpty) {
          mergedData = _mergeInformativos(informativos, cachedData);
        }
      }
      
      // Salva dados mesclados no cache
      await CacheService.cacheInformativos(
        mergedData,
        pageNumber: pageNumber,
        pageSize: pageSize,
      );
      
      return _paginateInformativos(
        _sortByDateDescending(mergedData),
        pageNumber,
        pageSize,
      );
    } catch (e) {
      // Fallback: retorna cache mesmo que inválido (melhor que erro)
      final cached = await CacheService.getInformativosCache();
      if (cached != null && cached.isNotEmpty) {
        return _paginateInformativos(
          _sortByDateDescending(cached),
          pageNumber,
          pageSize,
        );
      }
      rethrow;
    }
  }

  /// Busca informativos da API - operação de rede
  Future<List<InformativoModel>> _fetchFromApi() async {
    final String? token = await SessionManager.getToken();
    final response = await http.get(
      Uri.parse('${ApiConfig.baseUrl}/api/Informativo/Grupo'),
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer $token',
      },
    ).timeout(
      const Duration(seconds: 10),
      onTimeout: () => throw Exception('Timeout ao buscar informativos'),
    );

    if (response.statusCode == 200) {
      final List data = jsonDecode(response.body);
      return data.map((e) => InformativoModel.fromJson(e)).toList();
    } else if (response.statusCode == 401) {
      throw Exception('Sessão expirada. Por favor, faça login novamente.');
    } else {
      throw Exception('Erro ${response.statusCode} ao buscar informativos');
    }
  }

  /// Mescla informativos da API com cache, priorizando dados mais recentes
  List<InformativoModel> _mergeInformativos(
    List<InformativoModel> apiData,
    List<InformativoModel> cachedData,
  ) {
    final Map<int, InformativoModel> merged = {};
    
    // Adiciona dados do cache
    for (var item in cachedData) {
      merged[item.id] = item;
    }
    
    // Sobrescreve/adiciona dados da API (dados mais frescos)
    for (var item in apiData) {
      merged[item.id] = item;
    }
    
    return merged.values.toList();
  }

  /// Ordena informativos por data (mais recentes primeiro)
  List<InformativoModel> _sortByDateDescending(List<InformativoModel> informativos) {
    final sorted = List<InformativoModel>.from(informativos);
    sorted.sort((a, b) => b.dataInicio.compareTo(a.dataInicio));
    return sorted;
  }

  /// Pagina lista de informativos na memória (não faz requisição)
  PaginatedInformativos _paginateInformativos(
    List<InformativoModel> informativos,
    int pageNumber,
    int pageSize,
  ) {
    final startIndex = (pageNumber - 1) * pageSize;
    final endIndex = (startIndex + pageSize).clamp(0, informativos.length);

    final hasNextPage = endIndex < informativos.length;
    final hasPreviousPage = pageNumber > 1;

    final items = informativos.sublist(
      startIndex.clamp(0, informativos.length),
      endIndex.clamp(0, informativos.length),
    );

    return PaginatedInformativos(
      items: items,
      pageNumber: pageNumber,
      pageSize: pageSize,
      totalItems: informativos.length,
      totalPages: (informativos.length / pageSize).ceil(),
      hasNextPage: hasNextPage,
      hasPreviousPage: hasPreviousPage,
    );
  }

  /// Limpa cache de informativos
  Future<void> clearCache() async {
    await CacheService.clearInformativosCache();
  }

  /// Obtém informações sobre o cache para exibir na UI
  Future<Map<String, dynamic>> getCacheInfo() async {
    return await CacheService.getCacheInfo();
  }

  /// Limpa cache de informativos
  Future<void> clearCache() async {
    await CacheService.clearInformativosCache();
  }

  /// Calcula estimativa de dados economizados
  /// Retorna quantos bytes foram poupados ao usar cache
  Future<String> getDataSavingsEstimate() async {
    final cacheInfo = await getCacheInfo();
    final itemCount = cacheInfo['itemCount'] ?? 0;
    
    // Estimativa: ~200 bytes por item (JSON + overhead)
    final estimatedBytes = itemCount * 200;
    
    if (estimatedBytes < 1024) {
      return '$estimatedBytes B';
    } else if (estimatedBytes < 1024 * 1024) {
      return '${(estimatedBytes / 1024).toStringAsFixed(1)} KB';
    } else {
      return '${(estimatedBytes / (1024 * 1024)).toStringAsFixed(1)} MB';
    }
  }
}

/// Classe para retornar informativos paginados
>>>>>>> f87ee16efe656d2fea696dac95770ac346e04776
