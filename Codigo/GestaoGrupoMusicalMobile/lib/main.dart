import 'dart:io';

import 'package:batala_mobile/config/http_overrides.dart';
import 'package:batala_mobile/screens/pagamentos_solicitados_view.dart';
import 'package:flutter/material.dart';
import 'config/session_manager.dart'; 
import 'screens/main_screen.dart';
import 'screens/login_view.dart';

void main() async { 
  WidgetsFlutterBinding.ensureInitialized(); 

  HttpOverrides.global = MyHttpOverrides();
  
  // LEMBRETE: Comente essa linha após testar, senão ele nunca salvará o login.
  await SessionManager.clear(); 
  
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'Batalá Mobile',
      theme: ThemeData(
        colorSchemeSeed: const Color(0xFFD64550),
        useMaterial3: true,
      ),
      // MANTENHA o FutureBuilder para decidir a tela inicial
      home: FutureBuilder<int?>(
        future: SessionManager.getIdGrupo(), 
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Scaffold(body: Center(child: CircularProgressIndicator()));
          }
          if (snapshot.hasData && snapshot.data != null) {
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
