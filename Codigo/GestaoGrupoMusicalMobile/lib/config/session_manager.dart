import 'package:shared_preferences/shared_preferences.dart';

class SessionManager {
  static const String _keyToken = "jwt_token";
  static const String _keyIdGrupo = "id_grupo";
  static const String _keyIdPessoa = "id_pessoa";

  static Future<void> saveSession(String token, int idGrupo, int idPessoa) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_keyToken, token);
    await prefs.setInt(_keyIdGrupo, idGrupo);
    await prefs.setInt(_keyIdPessoa, idPessoa);
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
}