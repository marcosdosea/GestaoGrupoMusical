import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/cache_manager.dart';
import '../config/session_manager.dart';
import '../model/informativo_model.dart';
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