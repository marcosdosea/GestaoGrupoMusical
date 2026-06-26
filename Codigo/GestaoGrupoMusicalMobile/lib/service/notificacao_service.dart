import 'package:batala_mobile/config/session_manager.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import '../config/api_config.dart'; 
import 'dart:convert';
import 'package:http/http.dart' as http;

class NotificacaoService {
  final FirebaseMessaging _firebaseMessaging = FirebaseMessaging.instance;

  Future<void> initNotifications() async {
    NotificationSettings settings = await _firebaseMessaging.requestPermission(
      alert: true,
      badge: true,
      sound: true,
    );

    if (settings.authorizationStatus == AuthorizationStatus.authorized) {
      print('Permissão concedida pelo usuário.');
      
      _configureForegroundNotifications();
    } else {
      print('Permissão negada pelo usuário.');
    }
  }


  Future<void> registrarDispositivoNoBackend(int idPessoa) async {
    try {
      String? token = await _firebaseMessaging.getToken();

      if (token != null) {
        print('FCM Token gerado com sucesso: $token');
        await _enviarTokenParaAPI(idPessoa, token);
      }
    } catch (e) {
      print('Erro ao obter ou registrar o FCM Token: $e');
    }
  }

  Future<void> _enviarTokenParaAPI(int idPessoa, String fcmToken) async {
  final url = Uri.parse('${ApiConfig.baseUrl}/api/DispositivoPessoa/Registrar'); 
  final jwtToken = await SessionManager.getToken();

  final response = await http.post(
    url,
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $jwtToken', 
    },
    // jsonEncode garante que o formato fique correto para o C#
    body: jsonEncode({
      "IdPessoa": idPessoa,        
      "FcmToken": fcmToken,
      "dataAtualizacao": DateTime.now().toIso8601String()
    }),
  );

  if (response.statusCode == 200 || response.statusCode == 204) {
    print('Sucesso total: Dispositivo registrado no banco!');
  } else {
    // Isso vai te mostrar exatamente o que o C# não gostou
    print('Erro na API (${response.statusCode}): ${response.body}');
  }
}
  void _configureForegroundNotifications() {
    FirebaseMessaging.onMessage.listen((RemoteMessage message) {
      print('Nova notificação recebida com o app aberto!');
      if (message.notification != null) {
        print('Título: ${message.notification!.title}');
        print('Corpo: ${message.notification!.body}');
  
      }
    });
  }
}