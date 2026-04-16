import 'dart:io';
import 'package:device_info_plus/device_info_plus.dart';

class ApiConfig {

  static late String baseUrl;
  
  static const Duration timeout = Duration(seconds: 10);

  static Future<void> inicializarConfiguracoes() async {
    if (Platform.isWindows) {
      baseUrl = "http://localhost:5153";
      return;
    }

    if (Platform.isAndroid) {
      DeviceInfoPlugin deviceInfo = DeviceInfoPlugin();
      AndroidDeviceInfo androidInfo = await deviceInfo.androidInfo;

      if (androidInfo.isPhysicalDevice) {
        // Se for um aparelho físico (Celular na mão via Wi-Fi ou Cabo)
        baseUrl = "http://192.168.0.109:5153";
      } else {
        // Se for o Emulador do Android Studio
        baseUrl = "http://10.0.2.2:5153";
      }
      return;
    }
    
    // Fallback padrão de segurança
    baseUrl = "http://localhost:5153";
  }
}