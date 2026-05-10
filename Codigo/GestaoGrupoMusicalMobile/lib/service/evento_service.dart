import 'dart:convert';

import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/config/cache_manager.dart';
import 'package:batala_mobile/config/session_manager.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../model/evento_model.dart';

class EventoService {

  final String baseUrl = ApiConfig.baseUrl;
  static const String _cacheKey = 'evento_list';
  static const String _detalhesCachePrefix = 'detalhes_evento_';

  Future<List<EventoModel>> getAll() async {
    try {
      // Tenta recuperar do cache primeiro
      final cachedData = await CacheManager.getCache(_cacheKey);
      if (cachedData != null) {
        debugPrint('Usando dados em cache para eventos');
        final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
        return data.map((e) => EventoModel.fromJson(e)).toList();
      }
    } catch (e) {
      debugPrint('Erro ao recuperar cache de eventos: $e');
    }

    // Se não tem cache válido, faz requisição HTTP
    try {
      final token = await SessionManager.getToken();
      final response = await http.get(
        Uri.parse('$baseUrl/api/Evento'),
        headers: {
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode != 200) {
        throw Exception('Erro ao buscar eventos');
      }

      final List data = jsonDecode(response.body);
      final eventos = data.map((e) => EventoModel.fromJson(e)).toList();
      
      // Salva no cache
      await CacheManager.saveCache(_cacheKey, data);
      
      return eventos;
    } catch (e) {
      // Se falhar a requisição, tenta retornar o cache mesmo que expirado
      debugPrint('Erro na requisição de eventos, tentando cache expirado: $e');
      try {
        final prefs = await CacheManager.getStaleCache(_cacheKey);
        if (prefs != null) {
          final List data = prefs is List ? prefs : jsonDecode(prefs);
          return data.map((e) => EventoModel.fromJson(e)).toList();
        }
      } catch (_) {}
      rethrow;
    }
  } 
 Future<Map<String, dynamic>?> getDetalhesEvento(int idEvento) async {
    final String cacheKey = '$_detalhesCachePrefix$idEvento';

    try {
      // 1. Tenta recuperar do cache primeiro (Prioridade)
      final cachedData = await CacheManager.getCache(cacheKey);
      if (cachedData != null) {
        debugPrint('Usando cache para detalhes do evento $idEvento');
        return cachedData is Map<String, dynamic> ? cachedData : jsonDecode(cachedData);
      }
    } catch (e) {
      debugPrint("Erro ao recuperar cache de detalhes: $e");
    }

    // 2. Se não houver cache, faz a requisição HTTP
    try {
      final token = await SessionManager.getToken();
      final response = await http.get(
        Uri.parse('$baseUrl/api/Evento/Detalhes/$idEvento'),
        headers: {
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        
        // 3. Salva no cache para uso futuro
        await CacheManager.saveCache(cacheKey, data);
        
        return data;
      }
      
      // 4. Se falhar a rede, tenta o cache expirado (Stale data)
      final staleData = await CacheManager.getStaleCache(cacheKey);
      if (staleData != null) {
        return staleData is Map<String, dynamic> ? staleData : jsonDecode(staleData);
      }
    } catch (e) {
      debugPrint("Erro ao buscar detalhes do servidor: $e");
    }
    
    return null; 
  }

  // Retorna 'null' se for sucesso, ou a mensagem de erro se falhar
  Future<String?> solicitarParticipacao(int idEvento, int idTipoInstrumento) async {
    try {
      final token = await SessionManager.getToken();

      final response = await http.post(
        Uri.parse('${ApiConfig.baseUrl}/api/Evento/ResponderPresenca'),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
        body: jsonEncode({
          'idEvento': idEvento,
          'idTipoInstrumento': idTipoInstrumento,
        }),
      );

      // Se deu 200 OK, retorna null (sem erros!)
      if (response.statusCode == 200) {
        return null; 
      } 
      
      // Se deu qualquer outro status (como 409 Conflict ou 400 BadRequest)
      // Nós lemos o JSON do C# para extrair a propriedade "mensagem"
      else {
        final data = jsonDecode(response.body);
        return data['mensagem'] ?? "Ocorreu um erro inesperado ao se inscrever.";
      }
    } catch (e) {
      debugPrint("Erro ao responder presença: $e");
      return "Erro de conexão com o servidor.";
    }
  }
  Future<bool> cancelarSolicitacao(int idEvento) async {
    try {
      final token = await SessionManager.getToken();

      final response = await http.post( 
        Uri.parse('${ApiConfig.baseUrl}/api/Evento/CancelarPresenca/$idEvento'), 
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      return response.statusCode == 200;
    } catch (e) {
      debugPrint("Erro ao cancelar presença: $e");
      return false;
    }
  }
}