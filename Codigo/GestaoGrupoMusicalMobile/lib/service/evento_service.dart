import 'dart:convert';

import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/config/session_manager.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../model/evento_model.dart';

class EventoService {

  final String baseUrl = ApiConfig.baseUrl;

  Future<List<EventoModel>> getAll() async {
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
    return data.map((e) => EventoModel.fromJson(e)).toList();
  } 
 Future<List<dynamic>> getInstrumentosDoEvento(int idEvento) async {
    try {
      final token = await SessionManager.getToken();
      final response = await http.get(
        Uri.parse('$baseUrl/api/Evento/$idEvento/Instrumentos'),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body); 
      }
      return [];
    } catch (e) {
      debugPrint("Erro ao buscar instrumentos: $e");
      return [];
    }
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