import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';

class CacheManager {
  static const String _cachePrefix = 'cache_';
  static const String _timestampPrefix = 'timestamp_';
  static const int _defaultCacheDurationMinutes = 30; // Duração padrão do cache

  /// Salva dados no cache com um timestamp
  static Future<void> saveCache(
    String key,
    dynamic data, {
    int cacheDurationMinutes = _defaultCacheDurationMinutes,
  }) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final cacheKey = '$_cachePrefix$key';
      final timestampKey = '$_timestampPrefix$key';

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

  /// Recupera dados do cache se ainda forem válidos
  static Future<dynamic> getCache(String key) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final cacheKey = '$_cachePrefix$key';
      final timestampKey = '$_timestampPrefix$key';

      final jsonString = prefs.getString(cacheKey);
      if (jsonString == null) {
        return null; // Cache não existe
      }

      // Verifica se o cache ainda é válido
      final timestamp = prefs.getInt(timestampKey);
      if (timestamp != null) {
        final cacheAge = DateTime.now().millisecondsSinceEpoch - timestamp;
        final cacheAgeMinutes = cacheAge / (1000 * 60);

        if (cacheAgeMinutes > _defaultCacheDurationMinutes) {
          // Cache expirou, remove e retorna null
          await clearCache(key);
          return null;
        }
      }

      // Retorna o cache ainda válido
      return jsonDecode(jsonString);
    } catch (e) {
      print('Erro ao recuperar cache: $e');
      return null;
    }
  }

  /// Remove um cache específico
  static Future<void> clearCache(String key) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final cacheKey = '$_cachePrefix$key';
      final timestampKey = '$_timestampPrefix$key';

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
      final cacheKey = '$_cachePrefix$key';
      final timestampKey = '$_timestampPrefix$key';

      if (!prefs.containsKey(cacheKey)) {
        return false;
      }

      final timestamp = prefs.getInt(timestampKey);
      if (timestamp == null) {
        return false;
      }

      final cacheAge = DateTime.now().millisecondsSinceEpoch - timestamp;
      final cacheAgeMinutes = cacheAge / (1000 * 60);

      return cacheAgeMinutes <= _defaultCacheDurationMinutes;
    } catch (e) {
      print('Erro ao verificar validade do cache: $e');
      return false;
    }
  }
}
