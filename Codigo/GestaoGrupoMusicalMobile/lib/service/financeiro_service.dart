import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/session_manager.dart';
import '../model/financeiro_model.dart';

class FinanceiroService {
  static const String baseUrl = ApiConfig.baseUrl;

  // MÉTODO GET: Recupera a lista de pagamentos/mensalidades
  Future<List<FinanceiroModel>> getAll() async {
    final String? token = await SessionManager.getToken();

    final response = await http.get(
      Uri.parse('$baseUrl/api/Financeiro'),
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer $token', // Envia o Token JWT salvo no login
      },
    );

    if (response.statusCode == 200) {
      final List data = jsonDecode(response.body);
      return data.map((item) => FinanceiroModel.fromJson(item)).toList();
    } else {
      throw Exception('Erro ao buscar financeiro: ${response.statusCode}');
    }
  }

  Future<bool> postFinanceiro(FinanceiroModel financeiro) async {
    final String? token = await SessionManager.getToken();

    final response = await http.post(
      Uri.parse('$baseUrl/api/Financeiro'),
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