import 'dart:convert';
import 'package:flutter/widgets.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/session_manager.dart';

class LoginService {
  Future<bool> login(String cpf, String senha) async {
    final response = await http.post(
      Uri.parse('${ApiConfig.baseUrl}/api/Identity/login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'cpf': cpf, 'senha': senha}),
    );

    // ... dentro do seu método login
if (response.statusCode == 200) {
  final Map<String, dynamic> data = jsonDecode(response.body);

  // LOG PARA VOCÊ CONFERIR OS NOMES NO CONSOLE:
  debugPrint("Campos da API: ${data.keys.toList()}");

  // AJUSTE AQUI: Passe os 3 argumentos exigidos
  await SessionManager.saveSession(
    data['token'] ?? '',              // 1º: Token (String)
    data['idGrupoMusical'] ?? 0,      // 2º: ID do Grupo (int)
    data['id'] ?? 0,                  // 3º: ID da Pessoa (int)
  );

  return true;
}
    return false;
  }
}