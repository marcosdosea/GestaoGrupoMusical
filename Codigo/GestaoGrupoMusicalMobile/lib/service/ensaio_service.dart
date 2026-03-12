import 'dart:convert';

import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/model/ensaio_model.dart';
import 'package:http/http.dart' as http;

class EnsaioService {

   static const String baseUrl = ApiConfig.baseUrl;

  Future<List<EnsaioModel>> getAll() async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/Ensaio'),
      headers: {
        'Accept': 'application/json',
      },
    );

    if (response.statusCode != 200) {
      throw Exception('Erro ao buscar eventos');
    }

    final List data = jsonDecode(response.body);
    return data.map((e) => EnsaioModel.fromJson(e)).toList();
  }
}
