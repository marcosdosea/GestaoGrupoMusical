import 'dart:convert'; 
import 'package:batala_mobile/config/app_colors.dart';
import 'package:batala_mobile/screens/login_view.dart';
import 'package:batala_mobile/screens/pagamentos_solicitados_view.dart';
import 'package:flutter/material.dart';
import '../config/session_manager.dart'; 
import 'home_view.dart';
import 'informativo_view.dart';
import 'material_estudo_view.dart';

class MainScreen extends StatefulWidget {
  const MainScreen({super.key});

  @override
  State<MainScreen> createState() => _MainScreenState();
}

class _MainScreenState extends State<MainScreen> {
  int _selectedIndex = 0;
  int countAvisos = 4;
  int countEstudo = 2;
  
  String _primeiroNome = "Carregando...";
  String _tipoConta = "";
  String _emailConta = ""; 
  bool _isMenuExpanded = false; 

  bool get _podeVerPagamentos {
    final role = _tipoConta.toUpperCase();
    return role == 'ASSOCIADO' || role == 'ADMINISTRADOR GRUPO';
  }

  final List<Widget> _screens = [
    const HomeView(),
    const InformativoView(),
    const MaterialEstudoView(),
    const PagamentosSolicitadosView(),
    const Center(child: Text("Área de Pagamento")),
  ];

  @override
  void initState() {
    super.initState();
    _carregarNomeDoToken(); 
  }

  Future<void> _carregarNomeDoToken() async {
    final token = await SessionManager.getToken();
    
    if (token != null && token.isNotEmpty) {
      try {
        final parts = token.split('.');
        if (parts.length == 3) {
          String payload = parts[1];

          while (payload.length % 4 != 0) {
            payload += '=';
          }

          final String decoded = utf8.decode(base64Url.decode(payload));
          final Map<String, dynamic> data = jsonDecode(decoded);

          String nomeCompleto = data['Nome'] ??                              
                                data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ?? 
                                "Bataleiro";

          String role = data['role'] ?? 
                        data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? 
                        "Músico";
          
          String email = data['Email'] ?? "";

          setState(() {
            _primeiroNome = nomeCompleto.split(' ')[0];
            _tipoConta = role;
            _emailConta = email; 
          });
          return;
        }
      } catch (e) {
        debugPrint("Erro ao decodificar token: $e");
      }
    }
    setState(() {
      _primeiroNome = "Bataleiro"; 
    });
  }

   @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          Padding(
            padding: const EdgeInsets.only(top: 50.0, bottom: 60.0),
            child: (!_podeVerPagamentos && _selectedIndex == 3) 
                ? const HomeView() 
                : _screens[_selectedIndex],
          ),
          
          if (_isMenuExpanded)
            GestureDetector(
              onTap: () => setState(() => _isMenuExpanded = false),
              child: Container(
                color: Colors.black.withValues(alpha: 0.5),
                width: double.infinity,
                height: double.infinity,
              ),
            ),

          _buildCustomHeader(),
          _buildBottomNav(),
        ],
      ),
    );
  }

  Widget _buildCustomHeader() {
    final screenHeight = MediaQuery.of(context).size.height;
    
    // 🔥 A CORREÇÃO: Aumentei a altura fechada de 55.0 para 65.0 para evitar estouro em alguns dispositivos.
    final double headerClosedHeight = 65.0;
    final double headerExpandedHeight = screenHeight * 0.25;
    
    final double currentHeaderHeight = _isMenuExpanded ? headerExpandedHeight : headerClosedHeight;

    return AnimatedContainer(
      duration: const Duration(milliseconds: 450),
      curve: Curves.fastOutSlowIn, 
      height: currentHeaderHeight,
      width: double.infinity,
      clipBehavior: Clip.hardEdge, 
      decoration: const BoxDecoration(
        color: AppColors.secondary,
        borderRadius: BorderRadius.vertical(
          bottom: Radius.circular(25.0),
        ),
      ),
      child: SafeArea( // SafeArea apenas no topo para evitar o notch.
        bottom: false,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20.0), // Padding apenas nas laterais.
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            // Centraliza verticalmente todo o conteúdo dentro do header quando fechado.
            mainAxisAlignment: MainAxisAlignment.center, 
            children: [
              // Linha principal com logo, textos e ícone de perfil.
              Row(
                crossAxisAlignment: CrossAxisAlignment.center, // Centraliza verticalmente o logo e o bloco de textos.
                children: [
                  AnimatedContainer(
                    duration: const Duration(milliseconds: 400),
                    curve: Curves.fastOutSlowIn,
                    // Logo redondo.
                    height: _isMenuExpanded ? 55 : 45, 
                    child: Image.asset('assets/img/batala.png'),
                  ),
                  const SizedBox(width: 10),
                  
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      // Centraliza verticalmente os três textos entre si.
                      mainAxisAlignment: MainAxisAlignment.center, 
                      children: [
                        AnimatedDefaultTextStyle(
                          duration: const Duration(milliseconds: 400),
                          style: TextStyle(
                            color: AppColors.textLight, 
                            fontSize: _isMenuExpanded ? 13 : 10 
                          ),
                          child: const Text("Batalá Mobile"),
                        ),
                        AnimatedDefaultTextStyle(
                          duration: const Duration(milliseconds: 400),
                          style: TextStyle(
                            color: Colors.white, 
                            fontWeight: FontWeight.bold, 
                            fontSize: _isMenuExpanded ? 17 : 15 
                          ),
                          child: Text("Olá, $_primeiroNome"),
                        ),
                        AnimatedDefaultTextStyle(
                          duration: const Duration(milliseconds: 400),
                          style: TextStyle(
                            color: Colors.white70, 
                            fontSize: _isMenuExpanded ? 14 : 11, 
                            fontStyle: FontStyle.italic, 
                          ),
                          child: Text(_tipoConta),
                        ),
                        
                        // 🔥 O E-mail só é renderizado quando o menu expande.
                        if (_isMenuExpanded && _emailConta.isNotEmpty)
                          Padding(
                            padding: const EdgeInsets.only(top: 2.0),
                            child: AnimatedOpacity(
                              duration: const Duration(milliseconds: 300),
                              opacity: 1.0,
                              child: Text(
                                _emailConta,
                                style: const TextStyle(
                                  color: Colors.white54,
                                  fontSize: 12, 
                                ),
                              ),
                            ),
                          ),
                      ],
                    ),
                  ),
                  
                  // Ícone de perfil à direita, centralizado com o logo.
                  IconButton(
                    icon: Icon(
                      _isMenuExpanded ? Icons.close : Icons.person,
                      color: Colors.white,
                      size: 25,
                    ),
                    onPressed: () {
                      setState(() {
                        _isMenuExpanded = !_isMenuExpanded;
                      });
                    },
                  ),
                ],
              ),
              
              // Flexible para o botão de sair, visível apenas quando expandido.
              Flexible(
                child: AnimatedOpacity(
                  duration: const Duration(milliseconds: 300),
                  opacity: _isMenuExpanded ? 1.0 : 0.0, 
                  child: _isMenuExpanded 
                    ? Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const SizedBox(width: double.infinity), 
                          
                          ElevatedButton.icon(
                            style: ElevatedButton.styleFrom(
                              backgroundColor: Colors.white,
                              foregroundColor: AppColors.secondary,
                              padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
                              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
                              elevation: 5,
                            ),
                            icon: const Icon(Icons.logout),
                            label: const Text("Sair ou mudar de conta", style: TextStyle(fontSize: 15.6, fontWeight: FontWeight.bold)),
                            onPressed: () async {
                              await SessionManager.clear(); 
                              if (!mounted) return;
                              
                               Navigator.pushReplacement(context, MaterialPageRoute(builder: (context) => const LoginView()));
                            },
                          ),
                        ],
                      )
                    : const SizedBox.shrink(), 
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildBottomNav() {
    return Align(
      alignment: Alignment.bottomCenter,
      child: Container(
        margin: const EdgeInsets.only(bottom: 20.0),
        height: 55,
        width: MediaQuery.of(context).size.width * 0.92,
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(40.0),
          boxShadow: [
            BoxShadow(color: Colors.black.withValues(alpha: 0.15), blurRadius: 15, offset: const Offset(0, 8)),
          ],
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          children: [
            _buildNavItem(0, Icons.home, "Início", 0),
            _buildNavItem(1, Icons.notifications, "Avisos", countAvisos),
            _buildNavItem(2, Icons.library_books, "Estudo", countEstudo),
            if (_podeVerPagamentos) 
              _buildNavItem(3, Icons.payments, "Pagar", 0),
          ],
        ),
      ),
    );
  }

  Widget _buildNavItem(int index, IconData icon, String label, int badge) {
    final isSelected = _selectedIndex == index;
    return GestureDetector(
      onTap: () => setState(() {
        _selectedIndex = index;
        if (index == 1) countAvisos = 0;
        if (index == 2) countEstudo = 0;
      }),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Stack(
            clipBehavior: Clip.none,
            children: [
              Icon(icon, color: isSelected ? AppColors.primary : Colors.black54, size: 28),
              if (badge > 0)
                Positioned(
                  right: -5, top: -5,
                  child: Container(
                    padding: const EdgeInsets.all(4),
                    decoration: const BoxDecoration(color: AppColors.primary, shape: BoxShape.circle),
                    child: Text('$badge', style: const TextStyle(color: Colors.white, fontSize: 10, fontWeight: FontWeight.bold)),
                  ),
                ),
            ],
          ),
          Text(label, style: TextStyle(fontSize: 10, color: isSelected ? AppColors.primary : Colors.black54)),
        ],
      ),
    );
  }
}