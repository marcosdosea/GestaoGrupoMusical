import 'package:batala_mobile/config/session_manager.dart';
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
    final userId = (await SessionManager.getIdPessoa())?.toString();

    try {
      final cachedData = await CacheManager.getCache(_cacheKey, userId: userId);
      if (cachedData != null) {
        debugPrint(
            'Usando dados em cache isolados para ensaios do usuário $userId');
        final List data =
            cachedData is List ? cachedData : jsonDecode(cachedData);
        return data.map((e) => EnsaioModel.fromJson(e)).toList();
      }
    } catch (e) {
      debugPrint('Erro ao recuperar cache de ensaios: $e');
    }

    try {
      final token = await SessionManager.getToken();
      final response = await http.get(
        Uri.parse('$baseUrl/api/Ensaio'),
        headers: {
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode != 200) {
        throw Exception('Erro ao buscar ensaios');
      }

      final List data = jsonDecode(response.body);
      final ensaios = data.map((e) => EnsaioModel.fromJson(e)).toList();

      await CacheManager.saveCache(_cacheKey, data);

      return ensaios;
    } catch (e) {
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

  // GET: Buscar a frequência/justificativa do associado no ensaio
  Future<Map<String, dynamic>?> getMinhaFrequencia(int idEnsaio) async {
    final token = await SessionManager.getToken();
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/api/Ensaio/DetalhesSolicitacao/$idEnsaio'),
        headers: {
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      }
      return null;
    } catch (e) {
      debugPrint('Erro ao buscar frequência/justificativa do ensaio: $e');
      return null;
    }
  }

  // POST: Enviar justificativa de ausência no ensaio
  Future<String?> justificarAusencia(int idEnsaio, String justificativa) async {
    final token = await SessionManager.getToken();
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/api/Ensaio/JustificarAusencia'),
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
        body: jsonEncode({
          'idEnsaio': idEnsaio,
          'justificativa': justificativa,
        }),
      );

      if (response.statusCode == 200) return null;
      return _mensagemErro(response);
    } catch (e) {
      debugPrint('Erro ao enviar justificativa de ensaio: $e');
      return 'Não foi possível conectar à API para enviar a justificativa.';
    }
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

  Future<void> limparCacheListagem() async {
    final userId = (await SessionManager.getIdPessoa())?.toString();

    try {
      await CacheManager.clearCache(_cacheKey, userId: userId);
    } catch (e) {
      debugPrint('Erro ao limpar cache: $e');
    }
  }
}
