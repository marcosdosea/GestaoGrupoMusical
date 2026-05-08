# Exemplo de Integração de Cache em Telas

## Exemplo 1: Adicionar Botão de Refresh em HomeView

```dart
import 'package:flutter/material.dart';
import 'package:batala_mobile/config/session_manager.dart';
import 'package:batala_mobile/config/app_colors.dart';
import 'package:batala_mobile/widgets/cache_management_widget.dart'; // ← NOVO
import '../model/ensaio_model.dart';
import '../model/evento_model.dart';
import '../service/evento_service.dart';
import '../service/ensaio_service.dart';

class HomeView extends StatefulWidget {  // ← Mude para StatefulWidget
  const HomeView({super.key});

  @override
  State<HomeView> createState() => _HomeViewState();
}

class _HomeViewState extends State<HomeView> {
  late Future<List<EventoModel>> _eventosFuture;
  late Future<List<EnsaioModel>> _ensaiosFuture;
  
  final eventoService = EventoService();
  final ensaioService = EnsaioService();

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  void _loadData() {
    setState(() {
      _eventosFuture = eventoService.getAll();
      _ensaiosFuture = ensaioService.getAll();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // ← NOVO: Botão de refresh na AppBar
        // (Adicione isto em main_screen.dart no AppBar)
        
        // SEÇÃO DE EVENTOS
        Expanded(
          child: _buildSection<EventoModel>(
            "Próximos Eventos", 
            _eventosFuture,  // ← Use a variável ao invés de chamar direto
            (item) => _buildEventCard(item)
          ),
        ),

        // SEÇÃO DE ENSAIOS
        Expanded(
          child: _buildSection<EnsaioModel>(
            "Próximos Ensaios", 
            _ensaiosFuture,  // ← Use a variável ao invés de chamar direto
            (item) => _buildEnsaioCard(item)
          ),
        ),
        const SizedBox(height: 16), 
      ],
    );
  }

  // ... resto do código
}
```

## Exemplo 2: Adicionar Botão de Refresh em main_screen.dart

```dart
import 'package:flutter/material.dart';
import 'package:batala_mobile/widgets/cache_management_widget.dart'; // ← NOVO
import '../config/cache_service.dart'; // ← NOVO

class MainScreen extends StatefulWidget {
  // ... resto do código

  @override
  State<MainScreen> createState() => _MainScreenState();
}

class _MainScreenState extends State<MainScreen> {
  int _selectedIndex = 0;
  bool _isRefreshing = false;

  Future<void> _refreshAllData() async {
    setState(() => _isRefreshing = true);
    try {
      // Força limpeza de cache para recarregar tudo
      await CacheService.clearAllCaches();
      // Recarrega dados (as telas farão isso automaticamente)
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Dados atualizados!')),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Erro ao atualizar: $e')),
        );
      }
    } finally {
      if (mounted) {
        setState(() => _isRefreshing = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Gestão do Grupo Musical'),
        actions: [
          // ← NOVO: Botão de refresh com indicador
          CacheRefreshButton(
            onRefresh: _refreshAllData,
            isLoading: _isRefreshing,
          ),
        ],
      ),
      body: _getViewByIndex(_selectedIndex),
      bottomNavigationBar: BottomNavigationBar(
        // ... resto do código
      ),
    );
  }
}
```

## Exemplo 3: Adicionar Tela de Configurações com Gerenciamento de Cache

```dart
import 'package:flutter/material.dart';
import 'package:batala_mobile/widgets/cache_management_widget.dart';
import 'package:batala_mobile/config/cache_service.dart';
import 'package:batala_mobile/config/session_manager.dart';

class SettingsView extends StatefulWidget {
  const SettingsView({Key? key}) : super(key: key);

  @override
  State<SettingsView> createState() => _SettingsViewState();
}

class _SettingsViewState extends State<SettingsView> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Configurações'),
        elevation: 0,
      ),
      body: SingleChildScrollView(
        child: Column(
          children: [
            // Seção de Conta
            _buildSection(
              'Conta',
              [
                _buildSettingItem(
                  icon: Icons.person,
                  title: 'Perfil',
                  subtitle: 'Ver e editar dados pessoais',
                  onTap: () {
                    // Navegar para perfil
                  },
                ),
                _buildSettingItem(
                  icon: Icons.lock,
                  title: 'Alterar Senha',
                  subtitle: 'Mudar sua senha de acesso',
                  onTap: () {
                    // Navegar para alterar senha
                  },
                ),
              ],
            ),
            const Divider(),
            
            // Seção de Dados e Sincronização
            _buildSection(
              'Dados e Sincronização',
              [
                _buildSettingItem(
                  icon: Icons.cloud_download,
                  title: 'Sincronizar Agora',
                  subtitle: 'Baixar dados atualizados do servidor',
                  onTap: _syncData,
                ),
                _buildSettingItem(
                  icon: Icons.info,
                  title: 'Status do Cache',
                  subtitle: 'Ver status dos dados em cache',
                  onTap: () {
                    _showCacheStatusDialog();
                  },
                ),
              ],
            ),
            const Divider(),
            
            // Seção de Cache
            Padding(
              padding: const EdgeInsets.all(16),
              child: CacheManagementWidget(),
            ),
            const Divider(),
            
            // Seção de Segurança
            _buildSection(
              'Segurança',
              [
                _buildSettingItem(
                  icon: Icons.logout,
                  title: 'Sair',
                  subtitle: 'Fazer logout da conta',
                  onTap: _logout,
                  isDestructive: true,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSection(String title, List<Widget> items) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
          child: Text(
            title,
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.bold,
              color: Colors.grey,
            ),
          ),
        ),
        ...items,
      ],
    );
  }

  Widget _buildSettingItem({
    required IconData icon,
    required String title,
    required String subtitle,
    required VoidCallback onTap,
    bool isDestructive = false,
  }) {
    return ListTile(
      leading: Icon(
        icon,
        color: isDestructive ? Colors.red : Colors.blue,
      ),
      title: Text(
        title,
        style: TextStyle(
          color: isDestructive ? Colors.red : Colors.black,
          fontWeight: FontWeight.w500,
        ),
      ),
      subtitle: Text(subtitle),
      onTap: onTap,
      trailing: const Icon(Icons.chevron_right, color: Colors.grey),
    );
  }

  Future<void> _syncData() async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Sincronizar Dados'),
        content: const Text(
          'Isso vai atualizar todos os dados do servidor.\n\n'
          'Esta ação requer conexão com a internet.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancelar'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Sincronizar'),
          ),
        ],
      ),
    );

    if (confirm == true) {
      if (!mounted) return;
      
      showDialog(
        context: context,
        barrierDismissible: false,
        builder: (context) => AlertDialog(
          title: const Text('Sincronizando...'),
          content: const SizedBox(
            height: 100,
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                CircularProgressIndicator(),
                SizedBox(height: 16),
                Text('Atualizando dados...'),
              ],
            ),
          ),
        ),
      );

      try {
        // Limpa cache para forçar recarregamento
        await CacheService.clearAllCaches();
        
        if (mounted) {
          Navigator.pop(context); // Fecha dialog de loading
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Dados sincronizados com sucesso!')),
          );
        }
      } catch (e) {
        if (mounted) {
          Navigator.pop(context); // Fecha dialog de loading
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Erro ao sincronizar: $e')),
          );
        }
      }
    }
  }

  Future<void> _showCacheStatusDialog() async {
    final status = await CacheService.getCacheStatus();
    
    if (!mounted) return;
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Status do Cache'),
        content: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text('Dados armazenados em cache:'),
              const SizedBox(height: 12),
              ...status.entries.map((entry) {
                return Padding(
                  padding: const EdgeInsets.symmetric(vertical: 4),
                  child: Row(
                    children: [
                      Icon(
                        entry.value ? Icons.check_circle : Icons.schedule,
                        color: entry.value ? Colors.green : Colors.orange,
                        size: 16,
                      ),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          entry.key.replaceAll('_', ' '),
                          style: const TextStyle(fontSize: 12),
                        ),
                      ),
                      Text(
                        entry.value ? 'Válido' : 'Expirado',
                        style: TextStyle(
                          fontSize: 12,
                          color: entry.value ? Colors.green : Colors.orange,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                );
              }).toList(),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Fechar'),
          ),
        ],
      ),
    );
  }

  Future<void> _logout() async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Sair'),
        content: const Text('Você tem certeza que deseja sair?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancelar'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Sair', style: TextStyle(color: Colors.red)),
          ),
        ],
      ),
    );

    if (confirm == true) {
      // Limpa cache ao fazer logout
      await CacheService.clearAllCaches();
      
      // Remove dados de sessão
      await SessionManager.clearSession();
      
      if (mounted) {
        Navigator.of(context).pushReplacementNamed('/login');
      }
    }
  }
}
```

## Exemplo 4: Pull-to-Refresh em Lista

```dart
import 'package:flutter/material.dart';
import 'package:batala_mobile/config/cache_manager.dart';

class EventosComRefresh extends StatefulWidget {
  const EventosComRefresh({Key? key}) : super(key: key);

  @override
  State<EventosComRefresh> createState() => _EventosComRefreshState();
}

class _EventosComRefreshState extends State<EventosComRefresh> {
  late Future<List<EventoModel>> _eventosFuture;
  final _service = EventoService();

  @override
  void initState() {
    super.initState();
    _loadEventos();
  }

  void _loadEventos() {
    setState(() {
      _eventosFuture = _service.getAll();
    });
  }

  Future<void> _onRefresh() async {
    // Limpa cache para forçar recarregamento
    await CacheManager.clearCache('evento_list');
    _loadEventos();
    return Future.delayed(const Duration(seconds: 1));
  }

  @override
  Widget build(BuildContext context) {
    return RefreshIndicator(
      onRefresh: _onRefresh,
      child: FutureBuilder<List<EventoModel>>(
        future: _eventosFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError) {
            return Center(
              child: Text('Erro: ${snapshot.error}'),
            );
          }

          final eventos = snapshot.data ?? [];

          return ListView.builder(
            itemCount: eventos.length,
            itemBuilder: (context, index) {
              return _buildEventoTile(eventos[index]);
            },
          );
        },
      ),
    );
  }

  Widget _buildEventoTile(EventoModel evento) {
    // Widget customizado para cada evento
    return ListTile(
      title: Text(evento.nome),
      subtitle: Text(evento.data),
    );
  }
}
```

## Exemplo 5: Badge de Cache Offline

```dart
import 'package:flutter/material.dart';
import 'package:batala_mobile/config/cache_manager.dart';

class CacheOfflineBadge extends StatefulWidget {
  final String cacheKey;
  final Widget child;

  const CacheOfflineBadge({
    Key? key,
    required this.cacheKey,
    required this.child,
  }) : super(key: key);

  @override
  State<CacheOfflineBadge> createState() => _CacheOfflineBadgeState();
}

class _CacheOfflineBadgeState extends State<CacheOfflineBadge> {
  bool _isCacheValid = false;

  @override
  void initState() {
    super.initState();
    _checkCacheValidity();
  }

  Future<void> _checkCacheValidity() async {
    final isValid = await CacheManager.isCacheValid(widget.cacheKey);
    setState(() => _isCacheValid = isValid);
  }

  @override
  Widget build(BuildContext context) {
    if (!_isCacheValid) {
      return Stack(
        children: [
          widget.child,
          Positioned(
            top: 0,
            right: 0,
            child: Container(
              padding: const EdgeInsets.all(4),
              decoration: BoxDecoration(
                color: Colors.orange,
                borderRadius: BorderRadius.circular(4),
              ),
              child: const Tooltip(
                message: 'Dados em cache (offline)',
                child: Icon(
                  Icons.cloud_off,
                  color: Colors.white,
                  size: 12,
                ),
              ),
            ),
          ),
        ],
      );
    }

    return widget.child;
  }
}

// Uso:
// CacheOfflineBadge(
//   cacheKey: 'evento_list',
//   child: EventoCard(evento: evento),
// )
```

---

**Copie e adapte estes exemplos para suas telas!** 🎯
