import 'dart:convert';
import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/config/session_manager.dart';
import 'package:batala_mobile/model/material_estudo_model.dart';
import 'package:http/http.dart' as http;

class MaterialestudoService {

   static const String baseUrl = ApiConfig.baseUrl;

Future<List<MaterialestudoModel>> getAll() async {
  final String? token = await SessionManager.getToken();

  // A rota agora é apenas /api/MaterialEstudo
  final response = await http.get(
    Uri.parse('${ApiConfig.baseUrl}/api/MaterialEstudo'),
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Authorization': 'Bearer $token', 
    },
  );

  if (response.statusCode == 200) {
    final List data = jsonDecode(response.body);
    return data.map((e) => MaterialestudoModel.fromJson(e)).toList();
  } else {
    // Agora você verá 401 (Token inválido) ou 404 (Rota errada)
    throw Exception('Erro: ${response.statusCode}');
  }
}
}