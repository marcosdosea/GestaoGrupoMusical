import 'dart:convert'; 
import 'package:batala_mobile/config/app_colors.dart';
import 'package:batala_mobile/screens/login_view.dart';
import 'package:batala_mobile/screens/pagamentos_adm_view.dart';
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
    final statusBarHeight = MediaQuery.of(context).padding.top;
    final double headerHeight = 75.0 + statusBarHeight;
    return Scaffold(
      body: Stack(
        children: [
          Padding(
            padding: EdgeInsets.only(top: headerHeight + 10.0, bottom: 90.0),
            child: _selectedIndex == 3 
                ? (_tipoConta.toUpperCase() == 'ADMINISTRADOR GRUPO' 
                    ? const PagamentosAdminView() 
                    : const PagamentosSolicitadosView()) 
                : _screens[_selectedIndex],
          ),
          // 3. FUNDO ESCURO (Aparece quando o menu abre para focar na tela vermelha)
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
  final mediaQuery = MediaQuery.of(context);
  final statusBarHeight = mediaQuery.padding.top;
  
  final double headerClosedHeight = 75.0 + statusBarHeight;
  final double headerExpandedHeight = mediaQuery.size.height * 0.25;

  return AnimatedContainer(
    duration: const Duration(milliseconds: 450),
    curve: Curves.fastOutSlowIn,
    height: _isMenuExpanded ? headerExpandedHeight : headerClosedHeight,
    width: double.infinity,
    clipBehavior: Clip.hardEdge, 
    decoration: const BoxDecoration(
      color: AppColors.secondary,
      borderRadius: BorderRadius.vertical(bottom: Radius.circular(25.0)),
    ),
    child: SafeArea(
      bottom: false,
      child: Column(
        children: [
          // 1. BLOCO SUPERIOR (Logo + Textos + Perfil)
          Container(
            height: 75, // Altura fixa para o topo nunca "pular"
            padding: const EdgeInsets.symmetric(horizontal: 20.0),
            child: Row(
              children: [
                Image.asset('assets/img/batala.png', height: _isMenuExpanded ? 55 : 45),
                const SizedBox(width: 10),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text("Batalá Mobile", style: TextStyle(color: AppColors.textLight, fontSize: _isMenuExpanded ? 13 : 10)),
                      Text("Olá, $_primeiroNome", style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: _isMenuExpanded ? 17 : 14)),
                      
                      // Container flexível para Tipo e Email
                      Stack(
                        children: [
                          // Tipo de conta (Sempre visível ou movendo-se suavemente)
                          Text(_tipoConta, style: TextStyle(color: Colors.white70, fontSize: _isMenuExpanded ? 14 : 11, fontStyle: FontStyle.italic)),
                          
                          // Aparece com fade somente ao expandir
                          AnimatedOpacity(
                            duration: const Duration(milliseconds: 500),
                            opacity: _isMenuExpanded ? 1.0 : 0.0,
                            child: Padding(
                              padding: const EdgeInsets.only(top: 15.0), // Ajuste para ficar logo abaixo do tipo
                              child: _isMenuExpanded 
                                ? Text(_emailConta, style: const TextStyle(color: Colors.white54, fontSize: 11))
                                : const SizedBox.shrink(),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
                IconButton(
                  icon: Icon(_isMenuExpanded ? Icons.close : Icons.person, color: Colors.white),
                  onPressed: () => setState(() => _isMenuExpanded = !_isMenuExpanded),
                ),
              ],
            ),
          ),

          // 2. CONTEÚDO EXPANSÍVEL (Botão de Sair)
          if (_isMenuExpanded)
            Expanded(
              child: AnimatedOpacity(
                duration: const Duration(milliseconds: 400),
                opacity: 1.0,
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    ElevatedButton.icon(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.white,
                        foregroundColor: AppColors.secondary,
                        padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
                      ),
                      icon: const Icon(Icons.logout),
                      label: const Text("Sair ou mudar de conta", style: TextStyle(fontWeight: FontWeight.bold)),
                      onPressed: () async {
                         await SessionManager.clear();
                         if (!mounted) return;
                         Navigator.pushReplacement(context, MaterialPageRoute(builder: (context) => const LoginView()));
                      },
                    ),
                  ],
                ),
              ),
            ),
        ],
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