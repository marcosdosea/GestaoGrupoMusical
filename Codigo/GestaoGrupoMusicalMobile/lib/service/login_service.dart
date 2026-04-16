import 'dart:async';
import 'dart:convert';
import 'dart:io';    
import 'package:flutter/widgets.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/session_manager.dart';

class LoginService {
  Future<bool> login(String cpf, String senha) async {
    try {
      final response = await http.post(
        Uri.parse('${ApiConfig.baseUrl}/api/Identity/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'cpf': cpf, 'senha': senha}),
      ).timeout(
        const Duration(seconds: 10),
      );

      if (response.statusCode == 200) {
        final Map<String, dynamic> data = jsonDecode(response.body);

        // Salvando os dados de sessão no aparelho
        await SessionManager.saveSession(
          data['token'] ?? '',              // 1º: Token (String)
          data['idGrupoMusical'] ?? 0,      // 2º: ID do Grupo (int)
          data['id'] ?? 0,                  // 3º: ID da Pessoa (int)
        );

        return true;
      }
      
     
      return false;

    } on TimeoutException catch (_) {
      
      debugPrint('Timeout: A API demorou mais de 10 segundos para responder.');
      
      throw Exception('O servidor demorou a responder. Tente novamente mais tarde.');
      
    } on SocketException catch (_) {
      
      debugPrint('SocketException: Sem internet ou servidor desligado.');
      throw Exception('Falha de conexão. Verifique sua internet.');
      
    } catch (e) {
      
      debugPrint('Erro no LoginService: $e');
      throw Exception('Ocorreu um erro inesperado.');
    }
  }
}