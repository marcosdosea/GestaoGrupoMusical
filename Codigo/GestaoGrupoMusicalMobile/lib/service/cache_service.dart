import 'package:shared_preferences/shared_preferences.dart';
import 'dart:convert';
import '../model/informativo_model.dart';

class CacheService {
  static const String _informativoCacheKey = 'informativos_cache_v2';
  static const String _informativoCacheMetadataKey = 'informativos_cache_metadata';
  static const String _informativoCacheTimestampKey = 'informativos_cache_timestamp';
  static const int _cacheExpirationMinutes = 120; // Cache expira em 2 horas
  static const int _maxCacheItems = 500; // Limite máximo de itens em cache

  /// Salva lista de informativos no cache local com metadados
  static Future<void> cacheInformativos(
    List<InformativoModel> informativos, {
    int pageNumber = 1,
    int pageSize = 20,
  }) async {
    final prefs = await SharedPreferences.getInstance();
    
    try {
      // Carrega dados existentes
      List<InformativoModel> allInformativos = await getInformativosCache() ?? [];
      
      // Mantém apenas os últimos N itens para economizar espaço
      final combined = <int, InformativoModel>{};
      
      // Adiciona informativos existentes
      for (var info in allInformativos) {
        combined[info.id] = info;
      }
      
      // Adiciona novos informativos
      for (var info in informativos) {
        combined[info.id] = info;
      }
      
      // Limita tamanho do cache
      final sorted = combined.values.toList();
      sorted.sort((a, b) => b.dataInicio.compareTo(a.dataInicio));
      
      if (sorted.length > _maxCacheItems) {
        sorted.removeRange(_maxCacheItems, sorted.length);
      }
      
      // Salva dados
      final jsonList = sorted.map((e) => {
        'id': e.id,
        'data': e.dataInicio.toIso8601String(),
        'mensagem': e.mensagem,
      }).toList();
      
      await prefs.setString(_informativoCacheKey, jsonEncode(jsonList));
      
      // Salva metadados
      final metadata = {
        'totalItems': sorted.length,
        'lastUpdated': DateTime.now().millisecondsSinceEpoch,
        'lastPage': pageNumber,
        'pageSize': pageSize,
        'lastSync': DateTime.now().toIso8601String(),
      };
      
      await prefs.setString(_informativoCacheMetadataKey, jsonEncode(metadata));
      await prefs.setInt(_informativoCacheTimestampKey, DateTime.now().millisecondsSinceEpoch);
    } catch (e) {
      print('Erro ao cachear informativos: $e');
    }
  }

  /// Recupera informativos do cache local
  static Future<List<InformativoModel>?> getInformativosCache() async {
    final prefs = await SharedPreferences.getInstance();
    final cachedData = prefs.getString(_informativoCacheKey);
    
    if (cachedData == null) {
      return null;
    }

    try {
      final List<dynamic> jsonList = jsonDecode(cachedData);
      return jsonList
          .map((e) => InformativoModel.fromJson(e as Map<String, dynamic>))
          .toList();
    } catch (e) {
      print('Erro ao decodificar cache: $e');
      return null;
    }
  }

  /// Verifica se o cache ainda é válido
  static Future<bool> isCacheValid() async {
    final prefs = await SharedPreferences.getInstance();
    final timestamp = prefs.getInt(_informativoCacheTimestampKey);
    
    if (timestamp == null) {
      return false;
    }

    final cacheTime = DateTime.fromMillisecondsSinceEpoch(timestamp);
    final difference = DateTime.now().difference(cacheTime).inMinutes;
    
    return difference < _cacheExpirationMinutes;
  }

  /// Obtém metadados do cache
  static Future<Map<String, dynamic>> getCacheMetadata() async {
    final prefs = await SharedPreferences.getInstance();
    final metadata = prefs.getString(_informativoCacheMetadataKey);
    
    if (metadata == null) {
      return {};
    }
    
    try {
      return jsonDecode(metadata);
    } catch (e) {
      return {};
    }
  }

  /// Limpa cache de informativos
  static Future<void> clearInformativosCache() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_informativoCacheKey);
    await prefs.remove(_informativoCacheMetadataKey);
    await prefs.remove(_informativoCacheTimestampKey);
  }

  /// Obtém informações sobre o cache (para debug e UI)
  static Future<Map<String, dynamic>> getCacheInfo() async {
    final prefs = await SharedPreferences.getInstance();
    final timestamp = prefs.getInt(_informativoCacheTimestampKey);
    final cachedData = prefs.getString(_informativoCacheKey);
    final metadata = await getCacheMetadata();
    
    if (timestamp == null || cachedData == null) {
      return {
        'cached': false,
        'itemCount': 0,
        'ageMinutes': 0,
        'isValid': false,
        'dataSaved': '0 KB',
      };
    }

    final cacheTime = DateTime.fromMillisecondsSinceEpoch(timestamp);
    final age = DateTime.now().difference(cacheTime).inMinutes;
    
    try {
      final List<dynamic> jsonList = jsonDecode(cachedData);
      
      // Calcula tamanho aproximado dos dados salvos
      final approximateSize = cachedData.length ~/ 1024; // KB
      
      return {
        'cached': true,
        'itemCount': jsonList.length,
        'ageMinutes': age,
        'isValid': age < _cacheExpirationMinutes,
        'dataSaved': '$approximateSize KB',
        'lastSync': metadata['lastSync'] ?? 'Desconhecido',
      };
    } catch (e) {
      return {
        'cached': false,
        'itemCount': 0,
        'ageMinutes': 0,
        'isValid': false,
        'dataSaved': '0 KB',
      };
    }
  }
}
