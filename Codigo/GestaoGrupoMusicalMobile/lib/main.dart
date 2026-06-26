import 'dart:io';
import 'package:batala_mobile/config/http_overrides.dart';
import 'package:batala_mobile/screens/pagamentos_solicitados_view.dart';
import 'package:batala_mobile/service/notificacao_service.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'config/session_manager.dart'; 
import 'screens/main_screen.dart';
import 'screens/login_view.dart';
import 'config/api_config.dart'; 

void main() async { 
  WidgetsFlutterBinding.ensureInitialized(); 
  await ApiConfig.inicializarConfiguracoes();
  

  if (!kIsWeb && (Platform.isAndroid || Platform.isIOS)) {
    await Firebase.initializeApp();
    await NotificacaoService().initNotifications();
  }

  HttpOverrides.global = MyHttpOverrides();
  
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  // 1. CRIAMOS ESSA FUNÇÃO PARA ENCAPSULAR A LÓGICA DE START
  Future<bool> _verificarSessaoEInicializar() async {
    try {
      int? idGrupo = await SessionManager.getIdGrupo();
      
      int? idPessoa = await SessionManager.getIdPessoa(); 

      // Se o usuário tem sessão válida
      if (idGrupo != null && idGrupo > 0 && idPessoa != null && idPessoa > 0) {
        
        // 2. DISPARA A ATUALIZAÇÃO DO TOKEN EM SEGUNDO PLAN
        NotificacaoService().registrarDispositivoNoBackend(idPessoa).catchError((e) {
          debugPrint('Aviso: Falha ao renovar token no auto-login: $e');
        });
        
        return true; // Vai para a MainScreen
      }
    } catch (e) {
      debugPrint('Erro ao verificar sessão inicial: $e');
    }
    
    return false; // Vai para a LoginView
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'Batalá Mobile',
      theme: ThemeData(
        colorSchemeSeed: const Color(0xFFD64550),
        useMaterial3: true,
      ),
      // 3. ALTERAMOS O FUTUREBUILDER PARA USAR NOSSA NOVA FUNÇÃO
      home: FutureBuilder<bool>(
        future: _verificarSessaoEInicializar(), 
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Scaffold(body: Center(child: CircularProgressIndicator()));
          }
          // Se retornou true, joga para a Home. Se não, joga para o Login.
          if (snapshot.hasData && snapshot.data == true) {
            return const MainScreen();
          }
          return const LoginView();
        },
      ),
      routes: {
        '/login': (context) => const LoginView(),
        '/home': (context) => const MainScreen(),
        '/Financeiro': (context) => const PagamentosSolicitadosView(),
      },
    );
  }
}