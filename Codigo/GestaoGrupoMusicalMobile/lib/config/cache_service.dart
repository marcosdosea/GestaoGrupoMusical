// ignore_for_file: avoid_print

import 'package:batala_mobile/config/cache_manager.dart';

/// Classe utilitária para gerenciar operações de cache no aplicativo
class CacheService {
  /// Força a atualização de todos os caches
  /// Limpa todos os dados em cache armazenados
  static Future<void> clearAllCaches() async {
    await CacheManager.clearAllCache();
    print('Todos os caches foram removidos');
  }

  /// Força a atualização específica de um tipo de dado
  static Future<void> clearSpecificCache(String key) async {
    await CacheManager.refreshCache(key);
    print('Cache para $key foi atualizado');
  }

  /// Retorna informações sobre o status de todos os caches
  static Future<Map<String, bool>> getCacheStatus() async {
    final caches = {
      'ensaio_list': await CacheManager.isCacheValid('ensaio_list'),
      'evento_list': await CacheManager.isCacheValid('evento_list'),
      'informativo_list': await CacheManager.isCacheValid('informativo_list'),
      'financeiro_associado': await CacheManager.isCacheValid('financeiro_associado'),
      'financeiro_campanhas': await CacheManager.isCacheValid('financeiro_campanhas'),
      'material_estudo_list': await CacheManager.isCacheValid('material_estudo_list'),
    };

    return caches;
  }

  /// Precarrega todos os dados disponíveis no cache
  /// Útil para sincronizar offline após conexão restaurada
  static Future<void> preloadAllCaches() async {
    print('Iniciando pré-carregamento de caches...');
    
    // Lista de todas as chaves de cache
    const cacheKeys = [
      'ensaio_list',
      'evento_list',
      'informativo_list',
      'financeiro_associado',
      'financeiro_campanhas',
      'material_estudo_list',
    ];

    for (String key in cacheKeys) {
      final isValid = await CacheManager.isCacheValid(key);
      print('Cache $key - Válido: $isValid');
    }

    print('Pré-carregamento concluído');
  }
}
