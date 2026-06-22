# Guia de Implementação - Sistema de Cache Local

## 🚀 Início Rápido

O sistema de cache foi automaticamente integrado em todos os serviços. **Não é necessário fazer nada especial** - o cache funciona automaticamente!

### Como Funciona Automaticamente

```dart
// Quando você chama qualquer serviço:
final ensaios = await EnsaioService().getAll();

// O sistema automaticamente:
// 1. ✓ Verifica se há dados válidos no cache
// 2. ✓ Se sim, retorna do cache (super rápido)
// 3. ✓ Se não, faz requisição HTTP
// 4. ✓ Salva resultado no cache
// 5. ✓ Se offline, tenta cache expirado
```

---

## 📱 Adicionar Gerenciamento Visual de Cache

### Opção 1: Adicionar Botão de Refresh

Em qualquer tela, adicione um botão para atualizar dados:

```dart
import 'package:batala_mobile/widgets/cache_management_widget.dart';

// Na AppBar:
AppBar(
  title: const Text('Ensaios'),
  actions: [
    CacheRefreshButton(
      onRefresh: () => loadEnsaios(),
    ),
  ],
)

// Método de carregamento:
Future<void> loadEnsaios() async {
  final ensaios = await EnsaioService().getAll();
  setState(() => this.ensaios = ensaios);
}
```

### Opção 2: Adicionar Tela de Gerenciamento de Cache

Em uma tela de configurações ou menu:

```dart
import 'package:batala_mobile/widgets/cache_management_widget.dart';

class SettingsScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Configurações')),
      body: CacheManagementWidget(),
    );
  }
}
```

### Opção 3: Mostrar Informações em Dialogs

```dart
import 'package:batala_mobile/widgets/cache_management_widget.dart';

// Quando usuário toca em um item:
showCacheInfoDialog(context, 'Ensaios');
```

---

## 🔧 Operações Manuais de Cache

### Limpar Todo o Cache

```dart
import 'package:batala_mobile/config/cache_service.dart';

// Quando usuário faz logout:
await CacheService.clearAllCaches();
```

### Limpar Cache Específico

```dart
// Ao sair de uma tela ou quando necessário:
await CacheService.clearSpecificCache('ensaio_list');
```

### Verificar Status do Cache

```dart
final status = await CacheService.getCacheStatus();
print(status);
// Output: {
//   'ensaio_list': true,
//   'evento_list': false,
//   ...
// }
```

---

## 📡 Adicionar Cache a um Novo Serviço

Se criar um novo serviço, siga este padrão:

```dart
import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/cache_manager.dart';
import '../config/session_manager.dart';

class MeuNovoService {
  static const String _cacheKey = 'meu_novo_dados'; // Chave única
  
  Future<List<MeuModel>> getAll() async {
    try {
      // 1. Tenta recuperar do cache
      final cachedData = await CacheManager.getCache(_cacheKey);
      if (cachedData != null) {
        debugPrint('Usando dados em cache');
        final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
        return data.map((e) => MeuModel.fromJson(e)).toList();
      }
    } catch (e) {
      debugPrint('Erro ao recuperar cache: $e');
    }

    try {
      // 2. Faz requisição HTTP
      final token = await SessionManager.getToken();
      final response = await http.get(
        Uri.parse('${ApiConfig.baseUrl}/api/MeuEndpoint'),
        headers: {
          'Accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final List data = jsonDecode(response.body);
        final resultado = data.map((e) => MeuModel.fromJson(e)).toList();
        
        // 3. Salva no cache
        await CacheManager.saveCache(_cacheKey, data);
        
        return resultado;
      } else {
        // Se HTTP falhar, tenta cache expirado
        try {
          final cachedData = await CacheManager.getCache(_cacheKey);
          if (cachedData != null) {
            final List data = cachedData is List ? cachedData : jsonDecode(cachedData);
            return data.map((e) => MeuModel.fromJson(e)).toList();
          }
        } catch (_) {}
        
        throw Exception('Erro: ${response.statusCode}');
      }
    } catch (e) {
      debugPrint('Erro: $e');
      rethrow;
    }
  }
}
```

---

## 🔄 Ciclo de Vida do Cache

```
┌─────────────────────────────────────────────────────────┐
│ Usuário Solicita Dados                                  │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
         ┌───────────────────────────────┐
         │ Existe cache válido?          │
         │ (< 30 minutos)                │
         └───────┬───────────────────┬───┘
                 │ SIM               │ NÃO
                 │                   │
        ┌────────▼─────────┐   ┌─────▼──────────────┐
        │ Retorna do Cache │   │ Faz Requisição HTTP│
        │ (rápido!)        │   │ (mais lento)       │
        └────────┬─────────┘   └─────┬──────────────┘
                 │                   │
                 │                   ▼
                 │         ┌─────────────────────────┐
                 │         │ Requisição bem-sucedida?│
                 │         └─────┬─────────────┬─────┘
                 │               │ SIM         │ NÃO
                 │               │             │
                 │        ┌──────▼──────┐     │
                 │        │ Salva Cache │     │
                 │        └──────┬──────┘     │
                 │               │             │
                 │               │    ┌────────▼───────────────┐
                 │               │    │ Tenta Cache Expirado   │
                 │               │    │ (último recurso offline)
                 │               │    └────────┬───────────────┘
                 │               │             │
                 └───────┬───────┴─────────────┘
                         │
                         ▼
              ┌──────────────────────┐
              │ Retorna Dados ao App │
              └──────────────────────┘
```

---

## ⚙️ Configuração

### Alterar Duração do Cache

Edite `lib/config/cache_manager.dart`:

```dart
class CacheManager {
  static const int _defaultCacheDurationMinutes = 30; // ← Altere aqui
  
  // 60 = 1 hora
  // 120 = 2 horas
  // 5 = 5 minutos (para testes)
}
```

### Desabilitar Cache para Um Serviço (debugging)

Remova as linhas de cache do serviço ou comente:

```dart
// Comentado para debug:
// final cachedData = await CacheManager.getCache(_cacheKey);
// if (cachedData != null) {
//   return cachedData;
// }
```

---

## 🧪 Testando o Cache

### Teste 1: Verificar Funcionamento Offline

```dart
// 1. Abra a tela e carregue dados (com internet)
// 2. Desative a internet (airplane mode)
// 3. Feche e reabra o app
// 4. Dados devem aparecer (do cache)
```

### Teste 2: Verificar Atualização de Cache

```dart
// 1. Carregue dados normalmente
// 2. Aguarde mais de 30 minutos OU
// 3. Use CacheRefreshButton para forçar atualização
// 4. Novos dados devem ser carregados
```

### Teste 3: Verificar Fallback Expirado

```dart
// 1. Ative modo offline
// 2. Limpe o cache (CacheService.clearAllCaches())
// 3. Tente carregar dados
// 4. Deve exibir dados antigos (se houver expirados)
```

---

## 📊 Monitoramento e Debugging

### Ver Logs de Cache

```
Com print() no CacheManager:

I/flutter: Usando dados em cache para ensaios
I/flutter: Cache para ensaio_list foi atualizado
I/flutter: Todos os caches foram removidos
```

### Status do SharedPreferences

```dart
import 'package:shared_preferences/shared_preferences.dart';

final prefs = await SharedPreferences.getInstance();
final keys = prefs.getKeys();
keys.where((k) => k.startsWith('cache_')).forEach(print);
```

---

## 🚨 Tratamento de Erros

O sistema já trata erros automaticamente:

```
┌─ Falha ao ler cache? → Continua para HTTP
├─ Falha na HTTP? → Tenta cache expirado
└─ Tudo falha? → Lança exceção
```

Tratamento transparente ao desenvolvedor!

---

## 📋 Checklist de Implementação

- ✅ `CacheManager` criado e funcionando
- ✅ `CacheService` criado para operações manuais
- ✅ `CacheManagementWidget` disponível para UI
- ✅ Todos os 5 serviços atualizados com cache
- ✅ SharedPreferences já é dependência
- ✅ Documentação completa (CACHE_SYSTEM.md)
- ✅ Exemplos de uso e padrões definidos

---

## 🎯 Próximos Passos Recomendados

1. **Integrar Gerenciamento Visual**
   - Adicionar `CacheManagementWidget` em Settings
   - Adicionar `CacheRefreshButton` em telas principais

2. **Adicionar Indicador Visual**
   - Mostrar badge com status do cache
   - Exibir "Offline" quando usando cache expirado

3. **Sincronização em Background**
   - Usar `WorkManager` para sincronizar periodicamente

4. **Analytics**
   - Rastrear hits/misses do cache
   - Medir economia de dados

---

## ❓ Perguntas Frequentes

**P: O cache funciona offline?**  
R: Sim! Mesmo sem internet, dados do cache funcionam por 30 minutos. Depois, tenta cache expirado.

**P: Como limpar cache?**  
R: `CacheService.clearAllCaches()` ou use o widget de gerenciamento.

**P: Posso mudar a duração?**  
R: Sim, edite `_defaultCacheDurationMinutes` em `CacheManager`.

**P: E se os dados mudarem no servidor?**  
R: Após 30 minutos, novo cache é criado. Ou limpe manualmente.

**P: Funciona com logout?**  
R: Sim, limpe cache no logout: `CacheService.clearAllCaches()`.

**P: Usa muita memória?**  
R: Não, usa `SharedPreferences` que é eficiente. Dados antigos são removidos.

---

**Sistema pronto para usar! Não requer configuração adicional.** ✅
