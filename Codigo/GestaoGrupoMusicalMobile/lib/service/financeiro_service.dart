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
        Uri.parse('$url/api/Financeiro/associado'),
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
    final String url = ApiConfig.baseUrl;

    final response = await http.post(
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

  // 1. Busca todas as campanhas para o Administrador
  Future<List<CampanhaPagamentoModel>> getCampanhasAdmin() async {
    final String? token = await SessionManager.getToken();
    final String url = ApiConfig.baseUrl;

    final response = await http.get(
      Uri.parse('$url/api/Financeiro'), 
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer $token',
      },
    );

    if (response.statusCode == 200) {
      final List<dynamic> jsonList = json.decode(response.body);
      return jsonList.map((json) => CampanhaPagamentoModel.fromJson(json)).toList();
    } else {
      throw Exception("Falha ao carregar campanhas. Status: ${response.statusCode}");
    }
  }

  // 2. Busca a lista de pessoas detalhada para uma campanha específica(associados)
  Future<List<AssociadoPagamentoModel>> getAssociadosDoPagamento(int idReceita) async {
    final String? token = await SessionManager.getToken();
    final String url = ApiConfig.baseUrl;

    final response = await http.get(
      Uri.parse('$url/api/Financeiro/$idReceita/associados'), 
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer $token',
      },
    );

    if (response.statusCode == 200) {
      final List<dynamic> jsonList = json.decode(response.body);
      return jsonList.map((json) => AssociadoPagamentoModel.fromJson(json)).toList();
    } else {
      throw Exception("Falha ao carregar detalhes. Status: ${response.statusCode}");
    }
  }
}