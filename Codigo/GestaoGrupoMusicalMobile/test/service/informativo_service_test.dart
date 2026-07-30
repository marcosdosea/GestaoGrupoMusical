import 'dart:convert';
import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/model/informativo_model.dart';
import 'package:batala_mobile/service/informativo_service.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:shared_preferences/shared_preferences.dart';

void main() {
  setUpAll(() {
    ApiConfig.baseUrl = 'http://localhost:5153';
  });

  setUp(() {
    SharedPreferences.setMockInitialValues({});
  });

  // Dados de teste: intencionalmente fora de ordem para testar ordenação
  final informativosJson = [
    {
      'id': 1,
      'data': '2024-01-10T00:00:00.000',
      'mensagem': 'Mensagem mais antiga',
    },
    {
      'id': 2,
      'data': '2024-06-15T00:00:00.000',
      'mensagem': 'Mensagem mais recente',
    },
    {
      'id': 3,
      'data': '2024-03-20T00:00:00.000',
      'mensagem': 'Mensagem intermediária',
    },
  ];

  group('InformativoService.getAll()', () {
    test('deve retornar informativos ordenados por data decrescente quando API retorna 200', () async {
      final client = MockClient((request) async {
        expect(request.url.path, contains('/api/Informativo/Grupo'));
        return http.Response(jsonEncode(informativosJson), 200);
      });

      final service = InformativoService(client: client);
      final result = await service.getAll();

      expect(result, hasLength(3));
      // Mais recente primeiro (id=2: 2024-06-15)
      expect(result[0].id, equals(2));
      // Intermediário (id=3: 2024-03-20)
      expect(result[1].id, equals(3));
      // Mais antigo por último (id=1: 2024-01-10)
      expect(result[2].id, equals(1));
    });

    test('deve retornar lista do cache quando cache válido existe (sem chamada HTTP)', () async {
      SharedPreferences.setMockInitialValues({
        'cache_informativo_list': jsonEncode(informativosJson),
        'timestamp_informativo_list': DateTime.now().millisecondsSinceEpoch,
      });

      var httpCalled = false;
      final client = MockClient((_) async {
        httpCalled = true;
        return http.Response('', 200);
      });

      final service = InformativoService(client: client);
      final result = await service.getAll();

      expect(httpCalled, isFalse);
      expect(result, hasLength(3));
    });

    test('deve lançar Exception quando API falha e não há cache', () async {
      final client = MockClient((_) async {
        return http.Response('Error', 500);
      });

      final service = InformativoService(client: client);

      expect(() => service.getAll(), throwsException);
    });

    test('deve retornar objetos InformativoModel válidos', () async {
      final client = MockClient((_) async {
        return http.Response(jsonEncode(informativosJson), 200);
      });

      final service = InformativoService(client: client);
      final result = await service.getAll();

      for (final item in result) {
        expect(item, isA<InformativoModel>());
        expect(item.dataInicio, isA<DateTime>());
        expect(item.mensagem, isNotEmpty);
      }
    });

    test('deve retornar lista vazia quando API retorna array vazio', () async {
      final client = MockClient((_) async {
        return http.Response('[]', 200);
      });

      final service = InformativoService(client: client);
      final result = await service.getAll();

      expect(result, isEmpty);
    });
  });

  group('InformativoService.getPaginated()', () {
    test('deve retornar primeira página com hasMorePages=true quando há mais itens', () async {
      final client = MockClient((_) async {
        return http.Response(jsonEncode(informativosJson), 200);
      });

      final service = InformativoService(client: client);
      final result = await service.getPaginated(pageNumber: 1, pageSize: 2);

      expect(result.items, hasLength(2));
      expect(result.pageNumber, equals(1));
      expect(result.pageSize, equals(2));
      expect(result.totalItems, equals(3));
      expect(result.hasMorePages, isTrue);
    });

    test('deve retornar última página com hasMorePages=false', () async {
      final client = MockClient((_) async {
        return http.Response(jsonEncode(informativosJson), 200);
      });

      final service = InformativoService(client: client);
      final result = await service.getPaginated(pageNumber: 2, pageSize: 2);

      // Página 2 com pageSize=2 → só 1 item (o 3º)
      expect(result.items, hasLength(1));
      expect(result.hasMorePages, isFalse);
      expect(result.totalItems, equals(3));
    });

    test('deve retornar lista vazia quando pageNumber excede o total de páginas', () async {
      final client = MockClient((_) async {
        return http.Response(jsonEncode(informativosJson), 200);
      });

      final service = InformativoService(client: client);
      final result = await service.getPaginated(pageNumber: 10, pageSize: 5);

      expect(result.items, isEmpty);
      expect(result.hasMorePages, isFalse);
      expect(result.totalItems, equals(3));
    });

    test('deve respeitar pageSize configurado', () async {
      final client = MockClient((_) async {
        return http.Response(jsonEncode(informativosJson), 200);
      });

      final service = InformativoService(client: client);

      final pagina1 = await service.getPaginated(pageNumber: 1, pageSize: 1);
      expect(pagina1.items, hasLength(1));
      expect(pagina1.hasMorePages, isTrue);

      final pagina3 = await service.getPaginated(pageNumber: 3, pageSize: 1);
      expect(pagina3.items, hasLength(1));
      expect(pagina3.hasMorePages, isFalse);
    });

    test('deve ordenar itens paginados por data decrescente', () async {
      final client = MockClient((_) async {
        return http.Response(jsonEncode(informativosJson), 200);
      });

      final service = InformativoService(client: client);
      final result = await service.getPaginated(pageNumber: 1, pageSize: 3);

      // Verifica ordenação: mais recente primeiro
      expect(result.items[0].id, equals(2)); // 2024-06-15
      expect(result.items[1].id, equals(3)); // 2024-03-20
      expect(result.items[2].id, equals(1)); // 2024-01-10
    });

    test('deve lançar Exception quando getAll falha', () async {
      final client = MockClient((_) async {
        return http.Response('Server Error', 500);
      });

      final service = InformativoService(client: client);

      expect(
        () => service.getPaginated(pageNumber: 1, pageSize: 10),
        throwsException,
      );
    });
  });
}
