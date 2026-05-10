# Troubleshooting - Sistema de Cache Local

## ❌ Problemas Comuns e Soluções

---

## Problema 1: Cache Não Está Sendo Salvado

### Sintomas
- Dados não aparecem offline
- Sempre faz requisição HTTP mesmo após carregar antes

### Possíveis Causas

#### Causa 1: SharedPreferences Não Está Inicializado
```dart
// ❌ ERRADO: Usar antes de inicializar
final cache = await CacheManager.getCache('key');

// ✅ CORRETO: Garantir que app está pronto
void main() {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(const MyApp());
}
```

#### Causa 2: Permissões Não Concedidas
```
Android (AndroidManifest.xml):
✓ Verificar permissões de escrita
✓ Verificar se usando API 30+

iOS (Info.plist):
✓ Verificar permissões
```

#### Causa 3: Espaço em Disco Insuficiente
```dart
// Dispositivo sem espaço livre
// Solução: Implementar limpeza de cache antigo

// Verificar espaço:
final prefs = await SharedPreferences.getInstance();
final allKeys = prefs.getKeys();
print('Chaves armazenadas: ${allKeys.length}');
```

### Solução

```dart
// 1. Verificar se CacheManager está sendo chamado
debugPrint('Salvando cache...');
await CacheManager.saveCache(_cacheKey, data);
debugPrint('Cache salvo!');

// 2. Verificar se SharedPreferences está funcionando
final prefs = await SharedPreferences.getInstance();
await prefs.setString('test', 'value');
final value = prefs.getString('test');
assert(value == 'value'); // Deve passar

// 3. Limpar tudo e reiniciar
await CacheService.clearAllCaches();
// Reabrir app
```

---

## Problema 2: Cache Expirado Muito Rápido

### Sintomas
- Cache expira em minutos, não 30 minutos
- Sempre fazendo requisições HTTP

### Possíveis Causas

#### Causa 1: Duração Configurada Errada
```dart
// Verificar em lib/config/cache_manager.dart
static const int _defaultCacheDurationMinutes = 30;
// Se estiver muito baixo (ex: 1), está errado!
```

#### Causa 2: Timestamp Incorreto
```dart
// Pode estar usando fuso horário diferente
// Solução: Usar UTC em vez de local

// ❌ ERRADO:
DateTime.now().millisecondsSinceEpoch

// ✅ CORRETO (já está no código):
DateTime.now().millisecondsSinceEpoch // Sempre UTC
```

### Solução

```dart
// 1. Verificar configuração
const int _defaultCacheDurationMinutes = 30; // Deve ser 30 ou maior

// 2. Verificar timestamp
final timestamp = DateTime.now().millisecondsSinceEpoch;
print('Timestamp atual: $timestamp');

// 3. Testar cache
final cached = await CacheManager.getCache('test_key');
print('Cache válido: ${cached != null}');
```

---

## Problema 3: Offline Não Funciona

### Sintomas
- App crashea quando sem internet
- Mensagem "Sem dados disponíveis" quando deveria usar cache
- Sem fallback para dados antigos

### Possíveis Causas

#### Causa 1: Cache Não Está Sendo Carregado Offline
```dart
// Código nos serviços deve ter tratamento de erro
// Se falta, não há fallback

// ✅ CORRETO (já implementado):
try {
  final cachedData = await CacheManager.getCache(_cacheKey);
  if (cachedData != null) return cachedData;
} catch (e) {
  debugPrint('Cache erro: $e');
}
```

#### Causa 2: Exceção Não Está Sendo Capturada
```dart
// ❌ ERRADO: Lançar erro sem tentar fallback
throw Exception('HTTP falhou');

// ✅ CORRETO (já implementado):
try {
  final cachedData = await CacheManager.getCache(_cacheKey);
  if (cachedData != null) return cachedData;
} catch (_) {}
rethrow;
```

#### Causa 3: Cache Expirou Completamente
```dart
// Se cache expirou E foi removido, não há fallback
// Solução: Não remover cache expirado, apenas marcar como expirado

// ✅ Já implementado:
// Cache expirado é mantido em SharedPreferences
// Apenas marcado como expirado
```

### Solução

```dart
// 1. Verificar logs
// Procurar por: "Usando dados em cache para..."
// Se não aparecer, cache não foi salvo

// 2. Testar offline manualmente
// - Carregar dados com internet
// - Ativar airplane mode
// - Fechar app
// - Reabrir app
// - Dados devem aparecer

// 3. Verificar se serviço está usando try-catch
// Todos os serviços devem ter:
try {
  // Tentar cache
  // Tentar HTTP
} catch (e) {
  // Tentar cache expirado
}
```

---

## Problema 4: Cache Muito Grande

### Sintomas
- App fica lento
- SharedPreferences usando muita memória
- Avisos sobre tamanho de dados

### Possíveis Causas

#### Causa 1: Cache Nunca É Limpo
```dart
// Cache vai crescendo indefinidamente
// Solução: Implementar limpeza

// Limpar ao fazer logout:
await SessionManager.clearSession(); // Deve incluir cache
```

#### Causa 2: Dados Muito Grandes Sendo Cacheados
```dart
// Listas muito grandes consomem espaço
// Solução: Limitar tamanho ou não cachear tudo

// Exemplo: Cachear apenas primeiros 100 itens
final limitedData = data.take(100).toList();
await CacheManager.saveCache(_cacheKey, limitedData);
```

### Solução

```dart
// 1. Ver tamanho total
final prefs = await SharedPreferences.getInstance();
final keys = prefs.getKeys().where((k) => k.startsWith('cache_'));
print('Chaves de cache: ${keys.length}');

// 2. Limpar caches específicos que não usa mais
await CacheManager.clearCache('chave_nao_usada');

// 3. Implementar limpeza automática
// Limpar ao logout:
await CacheService.clearAllCaches();

// 4. Considerar compressão (futuro):
// - Comprimir dados antes de salvar
// - Descomprimir ao recuperar
```

---

## Problema 5: Dados Inconsistentes Entre Dispositivos

### Sintomas
- Diferentes usuários veem dados diferentes
- Cache de usuário A aparece para usuário B após logout/login

### Possíveis Causas

#### Causa 1: Cache Não Está Sendo Limpo no Logout
```dart
// ❌ ERRADO:
await SessionManager.clearSession();
// (se não limpar cache)

// ✅ CORRETO:
await CacheService.clearAllCaches();
await SessionManager.clearSession();
```

#### Causa 2: Usar Mesma Chave de Cache Para Diferentes Usuários
```dart
// ❌ ERRADO:
static const String _cacheKey = 'evento_list';
// Mesmo usuário diferente, mesma chave

// ✅ CORRETO:
final userId = await SessionManager.getUserId();
final cacheKey = 'evento_list_$userId';
```

### Solução

```dart
// 1. Sempre limpar cache ao fazer logout
@override
void dispose() {
  CacheService.clearAllCaches(); // Garantir limpeza
}

// 2. Chamar no logout:
Future<void> logout() async {
  await CacheService.clearAllCaches();
  await SessionManager.clearSession();
  navigateToLogin();
}

// 3. Para ter dados por usuário (se necessário):
static String _getCacheKey(String baseKey) {
  final userId = SessionManager.getUserId();
  return '${baseKey}_$userId';
}
```

---

## Problema 6: Dados Desatualizados Por Muito Tempo

### Sintomas
- Usuário vê dados de dias atrás
- Dados não são atualizados mesmo após reopening

### Possível Causa

#### Cache Muito Longo
```dart
// Duração configurada para muito tempo
static const int _defaultCacheDurationMinutes = 1440; // 24 horas!
// Deve ser 30 minutos (padrão)
```

### Solução

```dart
// 1. Reduzir duração do cache
static const int _defaultCacheDurationMinutes = 30; // 30 min padrão

// 2. Permitir refresh manual
// Adicionar CacheRefreshButton em telas principais

// 3. Sincronização periódica (futuro)
// Usar WorkManager para sincronizar a cada X minutos
```

---

## Problema 7: Erro "Sem Dados Disponíveis" Mesmo Com Dados em Cache

### Sintomas
- Dados foram carregados antes
- App mostra erro mesmo estando offline
- Cache existe mas não é acessado

### Possível Causa

#### Cache Não Está Sendo Recuperado Corretamente
```dart
// ❌ ERRADO:
final data = await CacheManager.getCache(_cacheKey);
if (data == null) throw Exception('Sem dados');

// ✅ CORRETO:
try {
  final data = await CacheManager.getCache(_cacheKey);
  if (data != null) return data;
} catch (e) {
  debugPrint('Cache erro: $e');
}
// Continuar tentando HTTP ou fallback
```

### Solução

```dart
// 1. Verificar se cache foi salvo
final cached = await CacheManager.getCache(_cacheKey);
print('Cache: $cached'); // Deve ter valor

// 2. Verificar estrutura do dados
// Deve ser List, não String JSON
final data = cached is List ? cached : jsonDecode(cached);

// 3. Usar o padrão correto em todos os serviços
// Todos os 5 serviços já estão corretos
// Verificar se estão importando CacheManager corretamente
```

---

## Problema 8: Cache Compila Mas Não Funciona

### Sintomas
- App compila sem erros
- Não há mensagens de erro
- Dados não são cacheados silenciosamente

### Possíveis Causas

#### Causa 1: Import Ausente
```dart
// ❌ ERRADO:
// import 'package:batala_mobile/config/cache_manager.dart'; // Falta

// ✅ CORRETO:
import 'package:batala_mobile/config/cache_manager.dart';
```

#### Causa 2: Chave Estática Ausente
```dart
// ❌ ERRADO:
class MeuService {
  // Falta a chave
  Future<List> getAll() {
    await CacheManager.getCache('string'); // Hardcoded!
  }
}

// ✅ CORRETO:
class MeuService {
  static const String _cacheKey = 'meu_cache';
  Future<List> getAll() {
    await CacheManager.getCache(_cacheKey);
  }
}
```

#### Causa 3: Try-Catch Ausente
```dart
// ❌ ERRADO:
final cached = await CacheManager.getCache(_cacheKey);
return cached; // Pode ser null!

// ✅ CORRETO:
try {
  final cached = await CacheManager.getCache(_cacheKey);
  if (cached != null) return cached;
} catch (e) {
  debugPrint('Cache erro: $e');
}
```

### Solução

```dart
// 1. Verificar imports
// Ctrl+Shift+O no VS Code para organizar imports

// 2. Usar template correto
// Copiar de um serviço já implementado

// 3. Ativar todos os debugPrints temporariamente
// Descomente no CacheManager para ver logs
```

---

## Checklist de Debugging

```
☐ SharedPreferences está funcionando?
  → flutter pub get
  → Rebuild app completamente

☐ CacheManager está compilando?
  → Sem erros de import
  → Chaves estáticas definidas

☐ Serviços estão atualizados?
  → Todos 5 serviços têm imports
  → Todos têm try-catch correto

☐ Cache está sendo salvo?
  → Ver logs: "Usando dados em cache..."
  → Verificar SharedPreferences manualmente

☐ Cache está expirando corretamente?
  → Aguardar 30+ minutos
  → Verificar nova requisição HTTP

☐ Offline funciona?
  → Carregar dados com internet
  → Ativar airplane mode
  → Reabrir app
  → Dados devem aparecer

☐ Logout limpa cache?
  → Após logout, sem dados offline
  → Novo login com dados limpem
```

---

## 📞 Contato e Suporte

Se nenhuma solução funcionar:

1. Verifique os logs: `flutter logs`
2. Limpe build: `flutter clean && flutter pub get && flutter run`
3. Verifique se todos os 5 serviços foram atualizados
4. Consulte `CACHE_SYSTEM.md` para referência
5. Consulte `IMPLEMENTATION_GUIDE.md` para padrões

---

**Última atualização:** 8 de maio de 2026
