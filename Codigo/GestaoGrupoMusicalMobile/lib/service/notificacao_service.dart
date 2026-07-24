import 'dart:convert';

import 'package:batala_mobile/config/session_manager.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:http/http.dart' as http;

import '../config/api_config.dart';

class NotificacaoService {
  final FirebaseMessaging _firebaseMessaging = FirebaseMessaging.instance;

  static final FlutterLocalNotificationsPlugin _localNotifications =
      FlutterLocalNotificationsPlugin();

  static const AndroidNotificationChannel _notificationChannel =
      AndroidNotificationChannel(
    'notificacoes_gerais',
    'Notificações gerais',
    description: 'Notificações do Grupo Musical',
    importance: Importance.max,
  );

  Future<void> initNotifications() async {
    await _initLocalNotifications();

    final settings = await _firebaseMessaging.requestPermission(
      alert: true,
      badge: true,
      sound: true,
    );

    if (settings.authorizationStatus == AuthorizationStatus.authorized) {
      debugPrint('Permissão concedida pelo usuário.');

      await _firebaseMessaging.setForegroundNotificationPresentationOptions(
        alert: true,
        badge: true,
        sound: true,
      );

      _configureForegroundNotifications();
    } else {
      debugPrint('Permissão negada pelo usuário.');
    }
  }

  Future<void> _initLocalNotifications() async {
    const initializationSettings = InitializationSettings(
      android: AndroidInitializationSettings('@mipmap/ic_launcher'),
      iOS: DarwinInitializationSettings(),
    );

    await _localNotifications.initialize(initializationSettings);

    await _localNotifications
        .resolvePlatformSpecificImplementation<
            AndroidFlutterLocalNotificationsPlugin>()
        ?.createNotificationChannel(_notificationChannel);
  }

  Future<void> registrarDispositivoNoBackend(int idPessoa) async {
    try {
      final token = await _firebaseMessaging.getToken();

      if (token != null) {
        debugPrint('FCM Token gerado com sucesso: $token');
        await _enviarTokenParaAPI(idPessoa, token);
      }
    } catch (e) {
      debugPrint('Erro ao obter ou registrar o FCM Token: $e');
    }
  }

  Future<void> _enviarTokenParaAPI(int idPessoa, String fcmToken) async {
    final url =
        Uri.parse('${ApiConfig.baseUrl}/api/DispositivoPessoa/Registrar');
    final jwtToken = await SessionManager.getToken();

    final response = await http.post(
      url,
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $jwtToken',
      },
      body: jsonEncode({
        'IdPessoa': idPessoa,
        'FcmToken': fcmToken,
        'dataAtualizacao': DateTime.now().toIso8601String(),
      }),
    );

    if (response.statusCode == 200 || response.statusCode == 204) {
      debugPrint('Sucesso total: dispositivo registrado no banco!');
    } else {
      debugPrint('Erro na API (${response.statusCode}): ${response.body}');
    }
  }

  void _configureForegroundNotifications() {
    FirebaseMessaging.onMessage.listen((RemoteMessage message) async {
      debugPrint('Nova notificação recebida com o app aberto!');

      final notification = message.notification;
      if (notification == null) {
        return;
      }

      debugPrint('Título: ${notification.title}');
      debugPrint('Corpo: ${notification.body}');

      if (defaultTargetPlatform == TargetPlatform.android) {
        final notificationId = message.messageId?.hashCode ??
            DateTime.now().millisecondsSinceEpoch.remainder(2147483647);

        await _localNotifications.show(
          notificationId & 0x7fffffff,
          notification.title ?? 'Batalá',
          notification.body ?? '',
          const NotificationDetails(
            android: AndroidNotificationDetails(
              'notificacoes_gerais',
              'Notificações gerais',
              channelDescription: 'Notificações do Grupo Musical',
              importance: Importance.max,
              priority: Priority.high,
            ),
          ),
        );
      }
    });
  }
}
