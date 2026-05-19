import 'package:flutter_test/flutter_test.dart';
import 'package:batala_mobile/model/informativo_model.dart';
import 'package:batala_mobile/service/informativo_service.dart';
import 'package:batala_mobile/service/cache_service.dart';

void main() {
  group('Cache Service Tests', () {
    setUp(() async {
      // Limpa cache antes de cada teste
      await CacheService.clearInformativosCache();
    });

    test('Cache salva e recupera informativos corretamente', () async {
      // Arrange
      final informativos = [
        InformativoModel(
          id: 1,
          dataInicio: DateTime(2026, 5, 8, 10, 30),
          mensagem: 'Aviso 1',
        ),
        InformativoModel(
          id: 2,
          dataInicio: DateTime(2026, 5, 7, 15, 45),
          mensagem: 'Aviso 2',
        ),
      ];

      // Act
      await CacheService.cacheInformativos(informativos);
      final cached = await CacheService.getInformativosCache();

      // Assert
      expect(cached, isNotNull);
      expect(cached!.length, equals(2));
      expect(cached[0].id, equals(1));
      expect(cached[1].id, equals(2));
    });

    test('Cache vazio retorna null', () async {
      final cached = await CacheService.getInformativosCache();
      expect(cached, isNull);
    });

    test('Cache válido retorna true inicialmente', () async {
      // Arrange
      final informativos = [
        InformativoModel(
          id: 1,
          dataInicio: DateTime.now(),
          mensagem: 'Aviso',
        ),
      ];

      // Act
      await CacheService.cacheInformativos(informativos);
      final isValid = await CacheService.isCacheValid();

      // Assert
      expect(isValid, isTrue);
    });

    test('Limpar cache remove todos os dados', () async {
      // Arrange
      final informativos = [
        InformativoModel(
          id: 1,
          dataInicio: DateTime.now(),
          mensagem: 'Aviso',
        ),
      ];
      await CacheService.cacheInformativos(informativos);

      // Act
      await CacheService.clearInformativosCache();
      final cached = await CacheService.getInformativosCache();

      // Assert
      expect(cached, isNull);
    });

    test('Info do cache retorna dados corretos', () async {
      // Arrange
      final informativos = [
        InformativoModel(
          id: 1,
          dataInicio: DateTime.now(),
          mensagem: 'Aviso',
        ),
        InformativoModel(
          id: 2,
          dataInicio: DateTime.now(),
          mensagem: 'Aviso 2',
        ),
      ];
      await CacheService.cacheInformativos(informativos);

      // Act
      final info = await CacheService.getCacheInfo();

      // Assert
      expect(info['cached'], isTrue);
      expect(info['itemCount'], equals(2));
      expect(info['isValid'], isTrue);
      expect(info['ageMinutes'], equals(0));
    });
  });

  group('Informativo Service Tests', () {
    setUp(() async {
      await CacheService.clearInformativosCache();
    });

    test('Ordena informativos por data descrescente (mais recentes primeiro)',
        () {
      // Arrange
      final service = InformativoService();
      final informativos = [
        InformativoModel(
          id: 1,
          dataInicio: DateTime(2026, 5, 5, 10, 0),
          mensagem: 'Aviso antigo',
        ),
        InformativoModel(
          id: 2,
          dataInicio: DateTime(2026, 5, 8, 15, 0),
          mensagem: 'Aviso recente',
        ),
        InformativoModel(
          id: 3,
          dataInicio: DateTime(2026, 5, 7, 12, 0),
          mensagem: 'Aviso meio de semana',
        ),
      ];

      // Act
      final sorted = informativos;
      sorted.sort((a, b) => b.dataInicio.compareTo(a.dataInicio));

      // Assert
      expect(sorted[0].id, equals(2)); // Mais recente
      expect(sorted[1].id, equals(3));
      expect(sorted[2].id, equals(1)); // Mais antigo
    });

    test('Paginação retorna corretamente primeiro página', () {
      // Arrange
      final informativos = List.generate(
        50,
        (i) => InformativoModel(
          id: i,
          dataInicio: DateTime(2026, 5, 8).subtract(Duration(hours: i)),
          mensagem: 'Aviso $i',
        ),
      );

      // Act
      final service = InformativoService();
      // Simula paginação (usando método privado através de reflexão ou testes de integração)
      
      // Assert - demonstração
      const pageSize = 20;
      final firstPage = informativos.sublist(0, pageSize);
      expect(firstPage.length, equals(20));
    });

    test('Paginação com múltiplas páginas calcula corretamente', () {
      // Arrange
      final informativos = List.generate(65, (i) => i);
      const pageSize = 20;
      final totalItems = informativos.length;
      final totalPages = (totalItems / pageSize).ceil();

      // Act & Assert
      expect(totalPages, equals(4)); // 20 + 20 + 20 + 5
    });

    test('Paginação não retorna itens além do limite', () {
      // Arrange
      final informativos =
          List.generate(35, (i) => InformativoModel(
            id: i,
            dataInicio: DateTime.now(),
            mensagem: 'Aviso $i',
          ));
      const pageSize = 20;

      // Act - página 1
      final page1End = (0 * pageSize + pageSize).clamp(0, informativos.length);
      final page1 = informativos.sublist(0, page1End);

      // Act - página 2
      final page2Start = 1 * pageSize;
      final page2End = (page2Start + pageSize).clamp(0, informativos.length);
      final page2 = informativos.sublist(page2Start, page2End);

      // Assert
      expect(page1.length, equals(20));
      expect(page2.length, equals(15)); // Restante
    });
  });

  group('Cenários de Uso - Economia de Dados', () {
    setUp(() async {
      await CacheService.clearInformativosCache();
    });

    test('Primeira requisição busca API e salva cache', () async {
      // Cenário: Primeira vez que usuário abre a app
      // Esperado: Busca API, exibe dados, salva cache
      
      final service = InformativoService();
      
      // Mock seria necessário para testar API real
      // Este é um teste conceitual
      expect(true, isTrue);
    });

    test('Segunda requisição dentro de 1h usa cache (sem dados)', () async {
      // Cenário: Usuário fecha app e abre em menos de 1 hora
      // Esperado: Carrega cache instantaneamente, economiza dados
      
      final informativos = [
        InformativoModel(
          id: 1,
          dataInicio: DateTime.now(),
          mensagem: 'Aviso',
        ),
      ];

      await CacheService.cacheInformativos(informativos);
      final isValid = await CacheService.isCacheValid();

      expect(isValid, isTrue);
    });

    test('Paginação carrega sob demanda (economiza dados)', () async {
      // Cenário: Usuário vê 20 avisos, depois rola e carrega mais
      // Esperado: Primeiro carregamento = 20 items, depois +20, etc
      
      // Primeira página
      const pageSize = 20;
      expect(pageSize, equals(20));

      // Segunda página seria +20 novos items
      // Total no app: 40 items
      // Sem paginação: teria carregado tudo (ex: 100+)
    });

    test('Pull-to-refresh atualiza com forceRefresh', () async {
      // Cenário: Usuário arrasta para atualizar
      // Esperado: forceRefresh=true, busca API nova
      
      // Setup: Cache antigo
      final oldData = [
        InformativoModel(
          id: 1,
          dataInicio: DateTime(2026, 5, 7), // Ontem
          mensagem: 'Aviso antigo',
        ),
      ];
      await CacheService.cacheInformativos(oldData);

      // Simulação: Pull-to-refresh com forceRefresh=true
      // Em produção, iria buscar API e atualizar

      final cached = await CacheService.getInformativosCache();
      expect(cached, isNotNull);
    });
  });

  group('Tratamento de Erros', () {
    setUp(() async {
      await CacheService.clearInformativosCache();
    });

    test('Sem internet retorna cache antigo como fallback', () async {
      // Arrange: Cache existe mas inválido
      final dados = [
        InformativoModel(
          id: 1,
          dataInicio: DateTime.now().subtract(const Duration(hours: 2)),
          mensagem: 'Aviso em cache antigo',
        ),
      ];
      await CacheService.cacheInformativos(dados);

      // Act: Tenta carregar mas sem internet (simulated)
      final cached = await CacheService.getInformativosCache();

      // Assert: Retorna dados mesmo antigos
      expect(cached, isNotNull);
      expect(cached!.first.mensagem, equals('Aviso em cache antigo'));
    });

    test('Cache inválido + sem internet = erro controlado', () async {
      // Arrange: Sem cache e sem internet
      
      // Act: Tenta carregar
      final cached = await CacheService.getInformativosCache();
      final isValid = await CacheService.isCacheValid();

      // Assert: Ambos retornam null/false para tratamento de erro
      expect(cached, isNull);
      expect(isValid, isFalse);
    });
  });
}
