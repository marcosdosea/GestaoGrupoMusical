import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/session_manager.dart';
import '../model/informativo_model.dart';

class InformativoService {
  Future<List<InformativoModel>> getAll() async {
  final String? token = await SessionManager.getToken();
  final response = await http.get(
    Uri.parse('${ApiConfig.baseUrl}/api/Informativo/Grupo'),
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Authorization': 'Bearer $token', 
    },
  );

  if (response.statusCode == 200) {
    final List data = jsonDecode(response.body);
    return data.map((e) => InformativoModel.fromJson(e)).toList();
  } else {
    // Agora você verá 401 (Token inválido) ou 404 (Rota errada)
    throw Exception('Erro: ${response.statusCode}');
  }
}
}