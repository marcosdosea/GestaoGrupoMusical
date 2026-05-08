import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/cache_manager.dart';
import '../config/session_manager.dart';
import '../model/informativo_model.dart';

class InformativoService {
  static const String _cacheKey = 'informativo_list';
  
  Future<List<InformativoModel>> getAll() async {
  try {
    // Tenta recuperar do cache primeiro
    final cachedData = await CacheManager.getCache(_cacheKey);
    if (cachedData != null) {
      debugPrint('Usando dados em cache para informativos');
      final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
      return data.map((e) => InformativoModel.fromJson(e)).toList();
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
      
      return informativos;
    } else {
      // Se falhar, tenta retornar cache mesmo que expirado
      try {
        final cachedData = await CacheManager.getCache(_cacheKey);
        if (cachedData != null) {
          final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
          return data.map((e) => InformativoModel.fromJson(e)).toList();
        }
      } catch (_) {}
      
      throw Exception('Erro: ${response.statusCode}');
    }
  } catch (e) {
    debugPrint('Erro ao buscar informativos: $e');
    rethrow;
  }
}
}