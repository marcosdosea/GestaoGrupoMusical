import 'dart:io';
import 'package:flutter/foundation.dart';
import 'package:device_info_plus/device_info_plus.dart';

class ApiConfig {

  static late String baseUrl;
  static late String webBaseUrl;
  
  static const Duration timeout = Duration(seconds: 10);

  static Future<void> inicializarConfiguracoes() async {
    
    if (kIsWeb) {
      baseUrl = "http://localhost:5153";
      return;
    }

    if (Platform.isWindows) {
      baseUrl = "http://localhost:5153";
      webBaseUrl = "https://localhost:7242";
      return;
    }

    if (Platform.isAndroid) {
      DeviceInfoPlugin deviceInfo = DeviceInfoPlugin();
      AndroidDeviceInfo androidInfo = await deviceInfo.androidInfo;

      if (androidInfo.isPhysicalDevice) {
        // Se for um aparelho físico (Celular na mão via Wi-Fi ou Cabo)
        baseUrl = "http://192.168.0.109:5153";
        webBaseUrl = "http://192.168.0.109:5051";
      } else {
        // Se for o Emulador do Android Studio
        baseUrl = "http://10.0.2.2:5153";
        webBaseUrl = "http://10.0.2.2:5051";
      }
      return;
    }
    
    // Fallback padrão de segurança
    baseUrl = "http://localhost:5153";
    webBaseUrl = "https://localhost:7242";
  }
}