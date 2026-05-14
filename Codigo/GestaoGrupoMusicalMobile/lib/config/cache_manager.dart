// ignore_for_file: avoid_print

import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'session_manager.dart';

class CacheManager {
  static const String _cachePrefix = 'cache_';
  static const String _timestampPrefix = 'timestamp_';
  static const int _defaultCacheDurationMinutes = 30;

  static String _cacheKey(String key, [String? userId]) => 
    userId != null ? '$_cachePrefix${userId}_$key' : '$_cachePrefix$key';

  static String _timestampKey(String key, [String? userId]) => 
    userId != null ? '$_timestampPrefix${userId}_$key' : '$_timestampPrefix$key';

  static int? _getCacheAgeMinutes(SharedPreferences prefs, String key, [String? userId]) {
    final timestamp = prefs.getInt(_timestampKey(key, userId));
    if (timestamp == null) return null;

    final cacheAge = DateTime.now().millisecondsSinceEpoch - timestamp;
    return cacheAge ~/ (1000 * 60);
  }

  /// Salva dados no cache com um timestamp isolado por usuário
  static Future<void> saveCache(
    String key,
    dynamic data, {
    int cacheDurationMinutes = _defaultCacheDurationMinutes,
    String? userId,
  }) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final effectiveUserId = userId ?? (await SessionManager.getToken())?.toString();

      final cacheKey = _cacheKey(key, effectiveUserId);
      final timestampKey = _timestampKey(key, effectiveUserId);

      final jsonString = jsonEncode(data);
      await prefs.setString(cacheKey, jsonString);

      final timestamp = DateTime.now().millisecondsSinceEpoch;
      await prefs.setInt(timestampKey, timestamp);
    } catch (e) {
      print('Erro ao salvar cache: $e');
    }
  }

  /// Recupera dados do cache somente se ainda estiverem frescos
  static Future<dynamic> getCache(String key, {String? userId}) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final effectiveUserId = userId ?? (await SessionManager.getToken())?.toString();
      final cacheKey = _cacheKey(key, effectiveUserId);

      final jsonString = prefs.getString(cacheKey);
      if (jsonString == null) return null;

      final cacheAgeMinutes = _getCacheAgeMinutes(prefs, key, effectiveUserId);
      if (cacheAgeMinutes != null && cacheAgeMinutes > _defaultCacheDurationMinutes) {
        return null;
      }

      return jsonDecode(jsonString);
    } catch (e) {
      print('Erro ao recuperar cache: $e');
      return null;
    }
  }

  /// Recupera dados do cache independentemente da idade (Fallback)
  static Future<dynamic> getStaleCache(String key, {String? userId}) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final effectiveUserId = userId ?? (await SessionManager.getToken())?.toString();
      final jsonString = prefs.getString(_cacheKey(key, effectiveUserId));
      
      if (jsonString == null) return null;
      return jsonDecode(jsonString);
    } catch (e) {
      print('Erro ao recuperar cache expirado: $e');
      return null;
    }
  }

  /// Verifica se um cache existe e ainda é válido (ADICIONADO)
  static Future<bool> isCacheValid(String key, {String? userId}) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final effectiveUserId = userId ?? (await SessionManager.getToken())?.toString();
      final cacheKey = _cacheKey(key, effectiveUserId);

      if (!prefs.containsKey(cacheKey)) return false;

      final cacheAgeMinutes = _getCacheAgeMinutes(prefs, key, effectiveUserId);
      if (cacheAgeMinutes == null) return false;

      return cacheAgeMinutes <= _defaultCacheDurationMinutes;
    } catch (e) {
      print('Erro ao verificar validade: $e');
      return false;
    }
  }

  /// Força a limpeza de um cache específico (ADICIONADO)
  static Future<void> refreshCache(String key, {String? userId}) async {
    await clearCache(key, userId: userId);
  }

  /// Remove um cache específico
  static Future<void> clearCache(String key, {String? userId}) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final effectiveUserId = userId ?? (await SessionManager.getToken())?.toString();

      await prefs.remove(_cacheKey(key, effectiveUserId));
      await prefs.remove(_timestampKey(key, effectiveUserId));
    } catch (e) {
      print('Erro ao limpar cache: $e');
    }
  }

  /// Remove TODOS os caches salvos no dispositivo
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
}