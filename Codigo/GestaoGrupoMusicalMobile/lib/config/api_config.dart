import 'dart:io';

class ApiConfig {
  static String get baseUrl{
    if (Platform.isAndroid) {
      return "https://192.168.0.103:7125"; 
    }
    return "https://localhost:7125";
  }
  static const Duration timeout = Duration(seconds: 30);
}