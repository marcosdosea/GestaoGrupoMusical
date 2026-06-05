import 'dart:async';
import 'dart:convert';
import 'dart:io';    
import 'package:flutter/widgets.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/session_manager.dart';
// import 'package:jwt_decoder/jwt_decoder.dart';

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

        // DEBUG: Imprime o que chegou da API para garantir que as chaves estão corretas
        debugPrint("BODY COMPLETO:");
        debugPrint(response.body);

        debugPrint("CHAVES:");
        debugPrint(data.keys.toList().toString());

        // Salvando os dados de sessão no aparelho
        // Ajustado para as chaves com 'I' maiúsculo conforme o seu log
        // final token = data['token'];

      // final claims = JwtDecoder.decode(token);

        await SessionManager.saveSession(
        data['token'] ?? '',
        int.tryParse(data['idGrupoMusical'].toString()) ?? 0,
        int.tryParse(data['idPessoa'].toString()) ?? 0,
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