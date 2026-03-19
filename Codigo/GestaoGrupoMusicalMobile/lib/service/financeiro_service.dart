import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/session_manager.dart';
import '../model/financeiro_model.dart';

class FinanceiroService {

  Future<List<FinanceiroModel>> getAll() async {
    try {
      final String? token = await SessionManager.getToken();
      final String url  = ApiConfig.baseUrl;

      final response = await http.get(
        // CORREÇÃO: Adicionado o /15 no final para a API não recusar (Erro 400)
        Uri.parse('$url/api/Financeiro/associado/15'),
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
          'Authorization': 'Bearer $token', 
        },
      );

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = json.decode(response.body);
        return jsonList.map((json) => FinanceiroModel.fromJson(json)).toList();
      } else {
        throw Exception("Falha ao carregar pagamentos. Status: ${response.statusCode}");
      }
    } catch (e) {
      throw Exception("Erro de conexão: $e");
    }
  }

  Future<bool> postFinanceiro(FinanceiroModel financeiro) async {
    final String? token = await SessionManager.getToken();
    final String url = ApiConfig.baseUrl; // Trazemos a variável de URL para cá também

    final response = await http.post(
      // CORREÇÃO: Adicionado o $ na frente da variável url
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
}