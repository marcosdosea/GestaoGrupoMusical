import 'dart:convert';

import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/config/cache_manager.dart';
import 'package:batala_mobile/model/ensaio_model.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

class EnsaioService {

   final String baseUrl = ApiConfig.baseUrl;
   static const String _cacheKey = 'ensaio_list';

  Future<List<EnsaioModel>> getAll() async {
    try {
      // Tenta recuperar do cache primeiro
      final cachedData = await CacheManager.getCache(_cacheKey);
      if (cachedData != null) {
        debugPrint('Usando dados em cache para ensaios');
        final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
        return data.map((e) => EnsaioModel.fromJson(e)).toList();
      }
    } catch (e) {
      debugPrint('Erro ao recuperar cache de ensaios: $e');
    }

    // Se não tem cache válido, faz requisição HTTP
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/api/Ensaio'),
        headers: {
          'Accept': 'application/json',
        },
      );

      if (response.statusCode != 200) {
        throw Exception('Erro ao buscar eventos');
      }

      final List data = jsonDecode(response.body);
      final ensaios = data.map((e) => EnsaioModel.fromJson(e)).toList();
      
      // Salva no cache
      await CacheManager.saveCache(_cacheKey, data);
      
      return ensaios;
    } catch (e) {
      // Se falhar a requisição, tenta retornar o cache mesmo que expirado
      debugPrint('Erro na requisição de ensaios, tentando cache expirado: $e');
      try {
        final prefs = await CacheManager.getStaleCache(_cacheKey);
        if (prefs != null) {
          final List data = prefs is List ? prefs : jsonDecode(prefs);
          return data.map((e) => EnsaioModel.fromJson(e)).toList();
        }
      } catch (_) {}
      rethrow;
    }
  }
}
