import 'dart:convert';

import 'package:batala_mobile/config/api_config.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../model/evento_model.dart';

class EventoService {

  final String baseUrl = ApiConfig.baseUrl;

  Future<List<EventoModel>> getAll() async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/Evento'),
      headers: {
        'Accept': 'application/json',
      },
    );

    if (response.statusCode != 200) {
      throw Exception('Erro ao buscar eventos');
    }

    final List data = jsonDecode(response.body);
    return data.map((e) => EventoModel.fromJson(e)).toList();
  } 

  Future<bool> responderPresenca(int eventoId, bool vaiComparecer) async {
  try {
    final response = await http.post(
      Uri.parse('$baseUrl/api/ResponderPresenca'),
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json', 
      },
   
      body: jsonEncode({
        'eventoId': eventoId,
        'usuarioNome': 'Talysson', // Por enquanto fixo, depois virá do JWT
        'confirmado': vaiComparecer,
      }),
    );

   if (response.statusCode == 200 || response.statusCode == 201) {
  return true;
} else {
  // Use debugPrint em vez de print
  debugPrint("Erro na API Batalá: ${response.body}"); 
  return false;
}
  } catch (e) {
    debugPrint("Erro de conexão: $e");
    return false;
  }
}
}
