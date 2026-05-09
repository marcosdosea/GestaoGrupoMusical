import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';

class CacheManager {
  static const String _cachePrefix = 'cache_';
  static const String _timestampPrefix = 'timestamp_';
  static const int _defaultCacheDurationMinutes = 30; // Duração padrão do cache

  static String _cacheKey(String key) => '$_cachePrefix$key';
  static String _timestampKey(String key) => '$_timestampPrefix$key';

  static int? _getCacheAgeMinutes(SharedPreferences prefs, String key) {
    final timestamp = prefs.getInt(_timestampKey(key));
    if (timestamp == null) {
      return null;
    }

    final cacheAge = DateTime.now().millisecondsSinceEpoch - timestamp;
    return cacheAge ~/ (1000 * 60);
  }

  /// Salva dados no cache com um timestamp
  static Future<void> saveCache(
    String key,
    dynamic data, {
    int cacheDurationMinutes = _defaultCacheDurationMinutes,
  }) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final cacheKey = _cacheKey(key);
      final timestampKey = _timestampKey(key);

      // Salva os dados como JSON string
      final jsonString = jsonEncode(data);
      await prefs.setString(cacheKey, jsonString);

      // Salva o timestamp da criação do cache
      final timestamp = DateTime.now().millisecondsSinceEpoch;
      await prefs.setInt(timestampKey, timestamp);
    } catch (e) {
      print('Erro ao salvar cache: $e');
    }
  }

  /// Recupera dados do cache somente se ainda estiverem frescos.
  static Future<dynamic> getCache(String key) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final cacheKey = _cacheKey(key);

      final jsonString = prefs.getString(cacheKey);
      if (jsonString == null) {
        return null;
      }

      final cacheAgeMinutes = _getCacheAgeMinutes(prefs, key);
      if (cacheAgeMinutes != null && cacheAgeMinutes > _defaultCacheDurationMinutes) {
        return null;
      }

      return jsonDecode(jsonString);
    } catch (e) {
      print('Erro ao recuperar cache: $e');
      return null;
    }
  }

  /// Recupera dados do cache independentemente da idade.
  /// Use isso como fallback quando não houver conectividade.
  static Future<dynamic> getStaleCache(String key) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final jsonString = prefs.getString(_cacheKey(key));
      if (jsonString == null) {
        return null;
      }

      return jsonDecode(jsonString);
    } catch (e) {
      print('Erro ao recuperar cache expirado: $e');
      return null;
    }
  }

  /// Remove um cache específico
  static Future<void> clearCache(String key) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final cacheKey = _cacheKey(key);
      final timestampKey = _timestampKey(key);

      await prefs.remove(cacheKey);
      await prefs.remove(timestampKey);
    } catch (e) {
      print('Erro ao limpar cache: $e');
    }
  }

  /// Remove todos os caches
  static Future<void> clearAllCache() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final keys = prefs.getKeys();

      for (String key in keys) {
        if (key.startsWith(_cachePrefix) || key.startsWith(_timestampPrefix)) {
          await prefs.remove(key);
        }
      }
    } catch (e) {
      print('Erro ao limpar todos os caches: $e');
    }
  }

  /// Força a atualização do cache (mesmo que ainda seja válido)
  static Future<void> refreshCache(String key) async {
    await clearCache(key);
  }

  /// Verifica se um cache existe e ainda é válido
  static Future<bool> isCacheValid(String key) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final cacheKey = _cacheKey(key);

      if (!prefs.containsKey(cacheKey)) {
        return false;
      }

      final cacheAgeMinutes = _getCacheAgeMinutes(prefs, key);
      if (cacheAgeMinutes == null) {
        return false;
      }

      return cacheAgeMinutes <= _defaultCacheDurationMinutes;
    } catch (e) {
      print('Erro ao verificar validade do cache: $e');
      return false;
    }
  }

  /// Indica se existe qualquer cache salvo, mesmo que esteja expirado.
  static Future<bool> hasAnyCache(String key) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      return prefs.containsKey(_cacheKey(key));
    } catch (e) {
      print('Erro ao verificar existência do cache: $e');
      return false;
    }
  }
}
