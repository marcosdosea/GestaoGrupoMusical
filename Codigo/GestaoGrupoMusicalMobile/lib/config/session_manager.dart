import 'package:shared_preferences/shared_preferences.dart';

class SessionManager {
  static const String _keyToken = "jwt_token";
  static const String _keyIdGrupo = "id_grupo";
  static const String _keyIdPessoa = "id_pessoa";

 static Future<void> saveSession(
  String token,
  int idGrupo,
  int idPessoa,
) async {
  final prefs = await SharedPreferences.getInstance();

  print("SALVANDO:");
  print("token=$token");
  print("idGrupo=$idGrupo");
  print("idPessoa=$idPessoa");

  await prefs.setString(_keyToken, token);
  await prefs.setInt(_keyIdGrupo, idGrupo);
  await prefs.setInt(_keyIdPessoa, idPessoa);

  print("SALVO COM SUCESSO");
}

  static Future<String?> getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_keyToken);
  }

  static Future<int?> getIdGrupo() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getInt(_keyIdGrupo);
  }

  static Future<void> clear() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.clear();
  }

  static Future<int?> getIdPessoa() async {
  final prefs = await SharedPreferences.getInstance();

  final valor = prefs.getInt(_keyIdPessoa);

  print("LENDO ID PESSOA: $valor");

  return valor;
}
}