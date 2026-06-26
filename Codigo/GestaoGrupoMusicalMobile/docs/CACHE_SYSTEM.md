# Sistema de Cache Local - Documentação

## Visão Geral

O aplicativo implementa um **sistema de cache local** que permite aos usuários acessar dados offline após a primeira carga. O cache é armazenado localmente no dispositivo usando `SharedPreferences`.

## Características Principais

### ✅ Cache Automático
- Todos os dados carregados são automaticamente salvos em cache
- A duração padrão do cache é **30 minutos**
- Após expiração, o app tenta fazer uma nova requisição

### ✅ Acesso Offline
- Se não há conexão com a internet, o app tenta usar dados do cache
- **Mesmo que expirados**, o cache expirado ainda será usado como fallback offline
- Garante que o usuário sempre veja os últimos dados disponíveis

### ✅ Dados Cacheados
Os seguintes dados são automaticamente sincronizados em cache:

1. **Ensaios** - Lista de ensaios do grupo musical
2. **Eventos** - Eventos agendados
3. **Instrumentos do Evento** - Instrumentos de cada evento específico
4. **Informativos** - Notícias e comunicados
5. **Financeiro (Associado)** - Dados financeiros do membro
6. **Campanhas de Pagamento** - Campanhas administrativas
7. **Associados do Pagamento** - Detalhes de pagamentos
8. **Materiais de Estudo** - Recursos de aprendizado

## Fluxo de Funcionamento

```
Usuário solicita dados
        ↓
    Há conexão?
    ↙         ↘
   SIM        NÃO
    ↓          ↓
  Cache válido?  Tenta usar
  ↙         ↘    cache expirado
 SIM        NÃO
  ↓          ↓
Usa cache  Faz requisição HTTP
            ↓
         Sucesso?
         ↙      ↘
        SIM     NÃO
         ↓       ↓
     Salva   Tenta cache
     cache   expirado
         ↓
     Retorna
      dados
```

## Uso no Aplicativo

### Acesso Automático
Os dados são automaticamente cacheados quando solicitados:

```dart
// No seu serviço
final ensaios = await EnsaioService().getAll();
// Automaticamente:
// 1. Verifica cache válido
// 2. Se não encontra, faz requisição HTTP
// 3. Salva resultado no cache
```

### Gerenciamento Manual de Cache

Use a classe `CacheService` para operações manuais:

```dart
import 'package:batala_mobile/config/cache_service.dart';

// Limpar todos os caches
await CacheService.clearAllCaches();

// Limpar cache específico
await CacheService.clearSpecificCache('ensaio_list');

// Verificar status dos caches
final status = await CacheService.getCacheStatus();
// Retorna: {
//   'ensaio_list': true,
//   'evento_list': false,
//   'informativo_list': true,
//   ...
// }

// Pré-carregar todos os caches
await CacheService.preloadAllCaches();
```

## Integração nos Serviços

Todos os serviços foram atualizados para usar cache:

### EnsaioService
```dart
Future<List<EnsaioModel>> getAll() async {
  // Tenta cache primeiro
  // Se válido, retorna dados em cache
  // Se inválido ou offline, faz requisição
  // Salva resposta em cache
}
```

### EventoService
```dart
Future<List<EventoModel>> getAll() async { ... }
Future<List<dynamic>> getInstrumentosDoEvento(int idEvento) async { ... }
```

### FinanceiroService
```dart
Future<List<FinanceiroModel>> getAll() async { ... }
Future<List<CampanhaPagamentoModel>> getCampanhasAdmin() async { ... }
Future<List<AssociadoPagamentoModel>> getAssociadosDoPagamento(int idReceita) async { ... }
```

### InformativoService
```dart
Future<List<InformativoModel>> getAll() async { ... }
```

### MaterialestudoService
```dart
Future<List<MaterialestudoModel>> getAll() async { ... }
```

## Casos de Uso

### 1. Sincronização Offline
```
Usuário abre o app → Sem internet?
→ Dados armazenados em cache aparecem normalmente
→ Usuário consegue navegar e consultar dados
→ Quando internet volta → Dados são atualizados
```

### 2. Redução de Requisições
```
Usuário acessa a mesma tela múltiplas vezes
→ Primeira vez: faz requisição HTTP
→ Próximas 30 minutos: usa cache (mais rápido)
→ Após 30 minutos: nova requisição
```

### 3. Experiência do Usuário Melhorada
```
App carrega dados mais rapidamente
→ Cache disponível = resposta instantânea
→ Melhor performance em conexões lentas
→ Funcionalidade offline completa
```

## Configuração

### Duração do Cache
Edite `lib/config/cache_manager.dart`:
```dart
static const int _defaultCacheDurationMinutes = 30; // Altere este valor
```

### Adicionar Novo Cache
1. Adicione a chave ao seu serviço:
```dart
static const String _cacheKey = 'novo_cache_key';
```

2. Use no método:
```dart
final cachedData = await CacheManager.getCache(_cacheKey);
if (cachedData != null) {
  // Usar dados em cache
}
// ... requisição HTTP
await CacheManager.saveCache(_cacheKey, data);
```

## Limpeza de Cache

### Automática
- Dados expirados (> 30 minutos) são descartados
- Novos dados fazem requisição HTTP

### Manual
- Use `CacheService.clearAllCaches()` para remover tudo
- Use `CacheService.clearSpecificCache(key)` para remover específico

### Cenários de Limpeza Recomendados
- Após logout do usuário
- Ao entrar em configurações de sincronização
- Quando usuário solicita "refresh" manual

## Estrutura de Armazenamento

Os dados são armazenados em `SharedPreferences` com a seguinte estrutura:

```
cache_ensaio_list: [JSON array com ensaios]
timestamp_ensaio_list: [milliseconds desde epoch]

cache_evento_list: [JSON array com eventos]
timestamp_evento_list: [milliseconds desde epoch]

... (similar para outros dados)
```

## Tratamento de Erros

O sistema de cache inclui tratamento robusto:

```dart
try {
  // Tenta cache
  final data = await CacheManager.getCache(key);
  if (data != null) return data;
} catch (e) {
  // Falha no cache, continua
  debugPrint('Erro ao recuperar cache: $e');
}

try {
  // Tenta requisição HTTP
  final response = await http.get(...);
  if (response.statusCode == 200) {
    await CacheManager.saveCache(key, data);
    return data;
  }
} catch (e) {
  // Falha na requisição
  try {
    // Última tentativa: usar cache expirado
    final expiredCache = await CacheManager.getCache(key);
    if (expiredCache != null) return expiredCache;
  } catch (_) {}
  // Sem dados disponíveis
  rethrow;
}
```

## Benefícios

✅ **Offline-First** - App funciona sem internet  
✅ **Melhor Performance** - Cache reduz tempo de carregamento  
✅ **Sincronização Automática** - Dados atualizados sem ação do usuário  
✅ **Fallback Inteligente** - Sempre tenta oferecer dados  
✅ **Gerenciamento Simples** - APIs fáceis de usar  
✅ **Sem Dependências Extras** - Usa SharedPreferences já incluído  

## Próximas Etapas (Opcional)

Para melhorias futuras, considere:
- Implementar sincronização em background
- Adicionar status visual de "sincronizando"
- Permitir que usuário configure duração do cache
- Adicionar estatísticas de uso de cache
- Implementar compressão de dados para economizar espaço
