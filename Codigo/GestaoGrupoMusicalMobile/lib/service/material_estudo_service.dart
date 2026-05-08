import 'dart:convert';
import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/config/cache_manager.dart';
import 'package:batala_mobile/config/session_manager.dart';
import 'package:batala_mobile/model/material_estudo_model.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

class MaterialestudoService {

   final String baseUrl = ApiConfig.baseUrl;
   static const String _cacheKey = 'material_estudo_list';

Future<List<MaterialestudoModel>> getAll() async {
  try {
    // Tenta recuperar do cache primeiro
    final cachedData = await CacheManager.getCache(_cacheKey);
    if (cachedData != null) {
      debugPrint('Usando dados em cache para materiais de estudo');
      final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
      return data.map((e) => MaterialestudoModel.fromJson(e)).toList();
    }
  } catch (e) {
    debugPrint('Erro ao recuperar cache de materiais: $e');
  }

  try {
    final String? token = await SessionManager.getToken();

    // A rota agora é apenas /api/MaterialEstudo
    final response = await http.get(
      Uri.parse('${ApiConfig.baseUrl}/api/MaterialEstudo'),
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer $token', 
      },
    );

    if (response.statusCode == 200) {
      final List data = jsonDecode(response.body);
      final materiais = data.map((e) => MaterialestudoModel.fromJson(e)).toList();
      
      // Salva no cache
      await CacheManager.saveCache(_cacheKey, data);
      
      return materiais;
    } else {
      // Se falhar, tenta retornar cache mesmo que expirado
      try {
        final cachedData = await CacheManager.getCache(_cacheKey);
        if (cachedData != null) {
          final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
          return data.map((e) => MaterialestudoModel.fromJson(e)).toList();
        }
      } catch (_) {}
      
      throw Exception('Erro: ${response.statusCode}');
    }
  } catch (e) {
    debugPrint('Erro ao buscar materiais de estudo: $e');
    rethrow;
  }
}
}