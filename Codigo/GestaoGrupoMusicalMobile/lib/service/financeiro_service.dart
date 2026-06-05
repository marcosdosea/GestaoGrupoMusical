import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/cache_manager.dart';
import '../config/session_manager.dart';
import '../model/financeiro_model.dart';

class FinanceiroService {
  static const String _cachekeyAssociado = 'financeiro_associado';
  static const String _cacheKeyAssociadoPagos = 'financeiro_associado_pagos';
  static const String _cacheKeyAdmin = 'financeiro_campanhas';
  static const String _cacheKeyAssociadoPrefix = 'financeiro_associados_';
  static const int _cachePagoDurationMinutes = 1440; // 24 horas para itens PAGO

  Future<List<FinanceiroModel>> getAll() async {
    try {
      // Tenta recuperar do cache primeiro
      final cachedData = await CacheManager.getCache(_cachekeyAssociado);
      if (cachedData != null) {
        debugPrint('Usando dados em cache para financeiro associado');
        final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
        // Mescla com items PAGO cacheados
        return _mergeWithCachedPaidItems(
          data.map((json) => FinanceiroModel.fromJson(json)).toList()
        );
      }
    } catch (e) {
      debugPrint('Erro ao recuperar cache de financeiro: $e');
    }

    try {
      final String? token = await SessionManager.getToken();
      final String url  = ApiConfig.baseUrl;

      final response = await http.get(
        Uri.parse('$url/api/Financeiro/associado'),
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
          'Authorization': 'Bearer $token', 
        },
      );

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = json.decode(response.body);
        final result = jsonList.map((json) => FinanceiroModel.fromJson(json)).toList();
        
        // Salva itens PAGO separadamente com TTL maior
        await _cachePaidItems(result);
        
        // Salva todos os dados no cache normal
        await CacheManager.saveCache(_cachekeyAssociado, jsonList);
        
        // Mescla com items PAGO cacheados
        return _mergeWithCachedPaidItems(result);
      } else {
        // Se falhar, tenta retornar cache mesmo que expirado
        try {
          final cachedData = await CacheManager.getStaleCache(_cachekeyAssociado);
          if (cachedData != null) {
            final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
            // Mescla com items PAGO cacheados
            return _mergeWithCachedPaidItems(
              data.map((json) => FinanceiroModel.fromJson(json)).toList()
            );
          }
        } catch (_) {}
        
        throw Exception("Falha ao carregar pagamentos. Status: ${response.statusCode}");
      }
    } catch (e) {
      debugPrint('Erro ao buscar financeiro: $e');
      rethrow;
    }
  }

  /// Cacheia items com status PAGO com TTL de 24 horas
  Future<void> _cachePaidItems(List<FinanceiroModel> items) async {
    final paidItems = items
        .where((item) => item.statusPagamento.toUpperCase().contains('PAGO'))
        .map((item) => {
          'id': item.id,
          'descricao': item.descricao,
          'dataInicio': item.dataInicio.toIso8601String(),
          'dataFim': item.dataFim.toIso8601String(),
          'valor': item.valor,
          'statusPagamento': item.statusPagamento,
        })
        .toList();
    
    if (paidItems.isNotEmpty) {
      await CacheManager.saveCache(
        _cacheKeyAssociadoPagos,
        paidItems,
        cacheDurationMinutes: _cachePagoDurationMinutes,
      );
      debugPrint('Items PAGO cacheados (TTL: 24h) - Qtd: ${paidItems.length}');
    }
  }

  /// Mescla items PAGO cacheados com a lista atual
  Future<List<FinanceiroModel>> _mergeWithCachedPaidItems(
    List<FinanceiroModel> currentItems
  ) async {
    try {
      final cachedPaidData = await CacheManager.getCache(_cacheKeyAssociadoPagos);
      if (cachedPaidData == null) {
        return currentItems;
      }

      final List data = cachedPaidData is List ? cachedPaidData : jsonDecode(cachedPaidData);
      final cachedPaidItems = data.map((json) => FinanceiroModel.fromJson(json)).toList();
      
      // Cria um mapa de IDs dos items atuais para evitar duplicatas
      final currentIds = currentItems.map((item) => item.id).toSet();
      
      // Adiciona items PAGO cacheados que não estão na lista atual
      final mergedItems = [...currentItems];
      for (final cachedItem in cachedPaidItems) {
        if (!currentIds.contains(cachedItem.id)) {
          mergedItems.add(cachedItem);
          debugPrint('Item PAGO recuperado do cache: ${cachedItem.descricao}');
        }
      }
      
      return mergedItems;
    } catch (e) {
      debugPrint('Erro ao mesclar items PAGO cacheados: $e');
      return currentItems;
    }
  }

  Future<bool> postFinanceiro(FinanceiroModel financeiro) async {
    final String? token = await SessionManager.getToken();
    final String url = ApiConfig.baseUrl;

    final response = await http.post(
      Uri.parse('$url/api/Financeiro'),
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $token',
      },
      body: jsonEncode({
        "id": financeiro.id,
        "descricao": financeiro.descricao,
      }),
    );

    return response.statusCode == 200 || response.statusCode == 201;
  }

  // 1. Busca todas as campanhas para o Administrador
  Future<List<CampanhaPagamentoModel>> getCampanhasAdmin() async {
    try {
      // Tenta recuperar do cache primeiro
      final cachedData = await CacheManager.getCache(_cacheKeyAdmin);
      if (cachedData != null) {
        debugPrint('Usando dados em cache para campanhas administrativas');
        final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
        return data.map((json) => CampanhaPagamentoModel.fromJson(json)).toList();
      }
    } catch (e) {
      debugPrint('Erro ao recuperar cache de campanhas: $e');
    }

    try {
      final String? token = await SessionManager.getToken();
      final String url = ApiConfig.baseUrl;

      final response = await http.get(
        Uri.parse('$url/api/Financeiro'), 
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = json.decode(response.body);
        final result = jsonList.map((json) => CampanhaPagamentoModel.fromJson(json)).toList();
        
        // Salva no cache
        await CacheManager.saveCache(_cacheKeyAdmin, jsonList);
        
        return result;
      } else {
        // Se falhar, tenta retornar cache mesmo que expirado
        try {
          final cachedData = await CacheManager.getStaleCache(_cacheKeyAdmin);
          if (cachedData != null) {
            final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
            return data.map((json) => CampanhaPagamentoModel.fromJson(json)).toList();
          }
        } catch (_) {}
        
        throw Exception("Falha ao carregar campanhas. Status: ${response.statusCode}");
      }
    } catch (e) {
      debugPrint('Erro ao buscar campanhas: $e');
      rethrow;
    }
  }

  // 2. Busca a lista de pessoas detalhada para uma campanha específica(associados)
  Future<List<AssociadoPagamentoModel>> getAssociadosDoPagamento(int idReceita) async {
    try {
      final cacheKey = '$_cacheKeyAssociadoPrefix$idReceita';
      
      // Tenta recuperar do cache primeiro
      final cachedData = await CacheManager.getCache(cacheKey);
      if (cachedData != null) {
        debugPrint('Usando dados em cache para associados do pagamento $idReceita');
        final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
        return data.map((json) => AssociadoPagamentoModel.fromJson(json)).toList();
      }
    } catch (e) {
      debugPrint('Erro ao recuperar cache de associados: $e');
    }

    try {
      final String? token = await SessionManager.getToken();
      final String url = ApiConfig.baseUrl;
      final cacheKey = '$_cacheKeyAssociadoPrefix$idReceita';

      final response = await http.get(
        Uri.parse('$url/api/Financeiro/$idReceita/associados'), 
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = json.decode(response.body);
        final result = jsonList.map((json) => AssociadoPagamentoModel.fromJson(json)).toList();
        
        // Salva no cache
        await CacheManager.saveCache(cacheKey, jsonList);
        
        return result;
      } else {
        // Se falhar, tenta retornar cache mesmo que expirado
        try {
          final cachedData = await CacheManager.getStaleCache(cacheKey);
          if (cachedData != null) {
            final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
            return data.map((json) => AssociadoPagamentoModel.fromJson(json)).toList();
          }
        } catch (_) {}
        
        throw Exception("Falha ao carregar detalhes. Status: ${response.statusCode}");
      }
    } catch (e) {
      debugPrint('Erro ao buscar associados: $e');
      rethrow;
    }
  }
}