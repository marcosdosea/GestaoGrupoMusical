# 🎯 Referência Rápida - Comandos de Cache

## Importações

```dart
import 'package:batala_mobile/config/cache_manager.dart';
import 'package:batala_mobile/config/cache_service.dart';
import 'package:batala_mobile/widgets/cache_management_widget.dart';
```

---

## 📌 Operações Básicas

### Limpar Tudo
```dart
await CacheService.clearAllCaches();
```

### Limpar Específico
```dart
await CacheService.clearSpecificCache('ensaio_list');
```

### Ver Status
```dart
final status = await CacheService.getCacheStatus();
// Retorna: {'ensaio_list': true, 'evento_list': false, ...}
```

### Verificar Se Cache Existe
```dart
final isValid = await CacheManager.isCacheValid('ensaio_list');
if (isValid) {
  // Cache é válido
} else {
  // Cache expirou ou não existe
}
```

### Recuperar Cache Manualmente
```dart
final data = await CacheManager.getCache('ensaio_list');
if (data != null) {
  // Usar dados
}
```

### Salvar Dados Manualmente
```dart
final dados = [1, 2, 3];
await CacheManager.saveCache('meu_cache', dados);
```

### Forçar Atualização
```dart
await CacheManager.refreshCache('ensaio_list');
// Remove cache para forçar nova requisição
```

---

## 🎨 Componentes UI

### Botão de Refresh
```dart
CacheRefreshButton(
  onRefresh: () async {
    // Carregue os dados aqui
    final dados = await meuServico.getAll();
  },
)
```

### Gerenciador Completo
```dart
CacheManagementWidget()
// Mostra status e permite limpar cache
```

### Dialog de Info
```dart
showCacheInfoDialog(context, 'Ensaios');
```

---

## 🔄 Padrão de Implementação em Serviço

```dart
import 'package:batala_mobile/config/cache_manager.dart';

class MeuService {
  static const String _cacheKey = 'meu_cache_key';
  
  Future<List<Modelo>> getAll() async {
    // 1. Tenta cache
    try {
      final cached = await CacheManager.getCache(_cacheKey);
      if (cached != null) {
        final List data = cached is List ? cached : jsonDecode(cached);
        return data.map((e) => Modelo.fromJson(e)).toList();
      }
    } catch (e) {
      debugPrint('Cache erro: $e');
    }
    
    // 2. Tenta HTTP
    try {
      final response = await http.get(...);
      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        await CacheManager.saveCache(_cacheKey, data);
        return data.map((e) => Modelo.fromJson(e)).toList();
      }
    } catch (e) {
      // 3. Tenta cache expirado
      try {
        final cached = await CacheManager.getCache(_cacheKey);
        if (cached != null) {
          final List data = cached is List ? cached : jsonDecode(cached);
          return data.map((e) => Modelo.fromJson(e)).toList();
        }
      } catch (_) {}
    }
    
    rethrow;
  }
}
```

---

## 📊 Chaves de Cache Disponíveis

```dart
'ensaio_list'                           // Ensaios
'evento_list'                           // Eventos
'instrumentos_{idEvento}'               // Instrumentos de evento
'informativo_list'                      // Informativos
'financeiro_associado'                  // Financeiro associado
'financeiro_campanhas'                  // Campanhas admin
'financeiro_associados_{idReceita}'     // Associados de pagamento
'material_estudo_list'                  // Materiais de estudo
```

---

## ⏱️ Configuração de Duração

```dart
// Em lib/config/cache_manager.dart

static const int _defaultCacheDurationMinutes = 30;

// Valores comuns:
// 1 = 1 minuto (debug)
// 5 = 5 minutos
// 30 = 30 minutos (padrão)
// 60 = 1 hora
// 1440 = 24 horas
```

---

## 🧪 Testes

### Teste Offline
```dart
// 1. Carregar dados com internet
final dados = await meuService.getAll();

// 2. Ativar airplane mode

// 3. Tentar carregar novamente
final cached = await meuService.getAll();
// Deve funcionar sem erro

// 4. Desativar airplane mode
```

### Teste Cache Expire
```dart
// 1. Carregar dados
await meuService.getAll();

// 2. Aguardar 30+ minutos

// 3. Carregar novamente
// Deve fazer nova requisição HTTP
```

### Teste Limpeza
```dart
// 1. Limpar cache
await CacheService.clearAllCaches();

// 2. Ativar airplane mode

// 3. Tentar carregar
// Deve falhar (sem cache)
```

---

## 🔍 Debugging

### Ver Logs
```
I/flutter: Usando dados em cache para ensaios
I/flutter: Cache para ensaio_list foi atualizado
I/flutter: Todos os caches foram removidos
```

### Ver Chaves Armazenadas
```dart
import 'package:shared_preferences/shared_preferences.dart';

final prefs = await SharedPreferences.getInstance();
final keys = prefs.getKeys();
keys.where((k) => k.startsWith('cache_')).forEach(print);
```

### Ver Tamanho Total
```dart
final prefs = await SharedPreferences.getInstance();
final cacheKeys = prefs.getKeys().where((k) => k.startsWith('cache_'));
print('Total: ${cacheKeys.length} chaves de cache');
```

---

## 🚨 Tratamento de Erros

```dart
// Capturar erro com fallback offline
try {
  final dados = await meuService.getAll();
  mostrarDados(dados);
} on Exception catch (e) {
  mostrarErro('Erro: $e - Tente novamente');
  // Cache expirado já foi tentado automaticamente
}
```

---

## 📱 Integração em Tela

### StatefulWidget com Refresh
```dart
class MinhaTelaState extends State<MinhatTela> {
  late Future<List<Modelo>> _dataFuture;
  
  @override
  void initState() {
    _loadData();
  }
  
  void _loadData() {
    setState(() {
      _dataFuture = meuService.getAll();
    });
  }
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        actions: [
          CacheRefreshButton(onRefresh: () async {
            _loadData();
          }),
        ],
      ),
      body: FutureBuilder(...),
    );
  }
}
```

### Pull-to-Refresh
```dart
RefreshIndicator(
  onRefresh: () async {
    await CacheManager.clearCache('ensaio_list');
    _loadData();
  },
  child: ListView(...),
)
```

---

## 🎓 Casos de Uso Comuns

### Logout com Limpeza
```dart
Future<void> logout() async {
  await CacheService.clearAllCaches();
  await SessionManager.clearSession();
  navigateToLogin();
}
```

### Sincronização Manual
```dart
Future<void> syncData() async {
  await CacheService.clearAllCaches();
  // Services farão requisição nova automaticamente
  await meuService.getAll(); // Recarrega tudo
}
```

### Badge de Offline
```dart
if (!await CacheManager.isCacheValid('ensaio_list')) {
  showOfflineBadge();
}
```

### Verificar Antes de Usar
```dart
final isOnline = await CacheManager.isCacheValid('ensaio_list');
if (!isOnline) {
  mostrarMensagem('Você está usando dados em cache');
}
```

---

## ✅ Checklist Antes de Deploy

- [ ] App compila sem erros
- [ ] Dados carregam com internet
- [ ] Dados aparecem offline
- [ ] Cache limpa no logout
- [ ] UI de refresh funciona (se implementada)
- [ ] Offline mode testado
- [ ] Cache expira corretamente
- [ ] Sem crashes

---

## 🔗 Links Úteis

- [QUICK_START.md](QUICK_START.md) - Início rápido
- [CACHE_SYSTEM.md](CACHE_SYSTEM.md) - Sistema completo
- [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md) - Guia prático
- [INTEGRATION_EXAMPLES.md](INTEGRATION_EXAMPLES.md) - Exemplos
- [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Resolução de problemas

---

**Última atualização:** 8 de maio de 2026
**Versão:** 1.0
**Status:** Completo ✅
