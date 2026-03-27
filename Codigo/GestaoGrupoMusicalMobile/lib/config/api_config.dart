import 'dart:io';

class ApiConfig {
  static String get baseUrl{
    if (Platform.isAndroid) {
      return "http://192.168.0.108:5153"; 
    }
    if((Platform.isWindows)){}
    return "https://localhost:7125";
  }
  static const Duration timeout = Duration(seconds: 30);
}