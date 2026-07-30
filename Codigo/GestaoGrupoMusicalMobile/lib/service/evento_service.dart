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
  final http.Client _client;

  EventoService({http.Client? client}) : _client = client ?? http.Client();

  Future<List<EventoModel>> getAll() async {
    // Usamos o ID da pessoa para o cache ser persistente mesmo se o token mudar
    final userId = (await SessionManager.getIdPessoa())?.toString();

    try {
      final cachedData = await CacheManager.getCache(_cacheKey, userId: userId);
      if (cachedData != null) {
        debugPrint('Usando dados em cache isolados para o usuário $userId');
        final List data =
            cachedData is List ? cachedData : jsonDecode(cachedData);
        return data.map((e) => EventoModel.fromJson(e)).toList();
      }
    } catch (e) {
      debugPrint('Erro ao recuperar cache de eventos: $e');
    }

    try {
      final token = await SessionManager.getToken();
      final response = await _client.get(
        Uri.parse('$baseUrl/api/Evento'),
        headers: {
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final List data = jsonDecode(response.body);
        await CacheManager.saveCache(_cacheKey, data, userId: userId);
        return data.map((e) => EventoModel.fromJson(e)).toList();
      } else {
        throw Exception('Status ${response.statusCode}');
      }
    } catch (e) {
      debugPrint('Erro na rede, tentando fallback de cache: $e');
      final staleData = await CacheManager.getCache(_cacheKey, userId: userId);
      if (staleData != null) {
        final List data = staleData is List ? staleData : jsonDecode(staleData);
        return data.map((e) => EventoModel.fromJson(e)).toList();
      }
      rethrow;
    }
  }

  Future<Map<String, dynamic>?> getDetalhesEvento(int idEvento) async {
    final token = await SessionManager.getToken();
    final userId = (await SessionManager.getIdPessoa())?.toString();
    final String cacheKey = '$_detalhesCachePrefix$idEvento';

    try {
      final response = await _client.get(
        Uri.parse('$baseUrl/api/Evento/Detalhes/$idEvento'),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        debugPrint("JSON RECEBIDO DA API: ${response.body}");

        final Map<String, dynamic> data = jsonDecode(response.body);

        // Verifica se os campos básicos existem no Map antes de salvar
        if (data.containsKey('id') || data.containsKey('Id')) {
          await CacheManager.saveCache(cacheKey, data, userId: userId);
          return data;
        } else {
          debugPrint("ERRO: O JSON chegou, mas não tem a estrutura esperada.");
          return null;
        }
      } else {
        debugPrint(
            "ERRO API: Status ${response.statusCode} - ${response.body}");
        return null;
      }
    } catch (e) {
      debugPrint("EXCEÇÃO NO SERVICE: $e");
      // Fallback para cache se a rede falhar
      return await CacheManager.getCache(cacheKey, userId: userId);
    }
  }

  // Métodos de POST (Participação e Cancelamento)
  Future<String?> solicitarParticipacao(
      int idEvento, int idTipoInstrumento) async {
    try {
      final token = await SessionManager.getToken();
      final response = await _client.post(
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

      if (response.statusCode == 200) return null;
      final data = jsonDecode(response.body);
      return data['mensagem'] ?? "Erro ao se inscrever.";
    } catch (e) {
      return "Erro de conexão.";
    }
  }

  Future<bool> cancelarSolicitacao(int idEvento) async {
    try {
      final token = await SessionManager.getToken();
      final response = await _client.post(
        Uri.parse('${ApiConfig.baseUrl}/api/Evento/CancelarPresenca/$idEvento'),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );
      return response.statusCode == 200;
    } catch (e) {
      return false;
    }
  }

  // Novo método para forçar a limpeza do cache de eventos
  Future<void> limparCacheListagem() async {
    final userId = (await SessionManager.getIdPessoa())?.toString();

    try {
      await CacheManager.clearCache(_cacheKey, userId: userId);
    } catch (e) {
      debugPrint('Erro ao limpar cache de eventos: $e');
    }
  }

  // POST: Enviar justificativa de ausência para o evento
  Future<String?> justificarAusencia(int idEvento, String justificativa) async {
    try {
      final token = await SessionManager.getToken();
      final response = await _client.post(
        Uri.parse('$baseUrl/api/Evento/JustificarAusencia'),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
        body: jsonEncode({
          'idEvento': idEvento,
          'justificativa': justificativa,
        }),
      );

      if (response.statusCode == 200) return null;
      return _mensagemErro(response);
    } catch (e) {
      debugPrint('Erro ao enviar justificativa de evento: $e');
      return 'Não foi possível conectar à API para enviar a justificativa.';
    }
  }

  Future<void> limparCacheDetalhes(int idEvento) async {
    final userId = (await SessionManager.getIdPessoa())?.toString();
    await CacheManager.clearCache('$_detalhesCachePrefix$idEvento',
        userId: userId);
  }

  String _mensagemErro(http.Response response) {
    if (response.statusCode == 401) {
      return 'Sua sessão expirou. Entre novamente.';
    }

    if (response.statusCode == 403) {
      return 'Você não tem permissão para justificar esta ausência.';
    }

    try {
      final data = jsonDecode(response.body) as Map<String, dynamic>;
      final mensagem = data['mensagem'] as String?;
      if (mensagem != null && mensagem.isNotEmpty) return mensagem;
    } catch (_) {}

    return 'Não foi possível enviar a justificativa (HTTP ${response.statusCode}).';
  }

  // GET: Consultar os detalhes/inscrição para verificar a justificativa enviada
  Future<Map<String, dynamic>?> getMinhaInscricao(int idEvento) async {
    return await getDetalhesEvento(idEvento);
  }
}
