import 'dart:convert';
import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/model/financeiro_model.dart';
import 'package:batala_mobile/service/financeiro_service.dart';
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

  final financeiroJson = [
    {
      'id': 1,
      'descricao': 'Mensalidade Janeiro',
      'dataInicio': '2024-01-01T00:00:00',
      'dataFim': '2024-01-31T00:00:00',
      'valor': 50.0,
      'statusPagamento': 'PAGO',
    },
    {
      'id': 2,
      'descricao': 'Mensalidade Fevereiro',
      'dataInicio': '2024-02-01T00:00:00',
      'dataFim': '2024-02-29T00:00:00',
      'valor': 50.0,
      'statusPagamento': 'PENDENTE',
    },
  ];

  final campanhasJson = [
    {
      'id': 10,
      'descricao': 'Campanha 2024',
      'dataInicio': '2024-01-01T00:00:00',
      'dataFim': '2024-12-31T00:00:00',
      'pagos': 40,
      'isentos': 5,
      'atrasos': 10,
      'recebido': 2000.0,
    },
  ];

  final associadosJson = [
    {
      'idAssociado': 1,
      'nomeAssociado': 'João Silva',
      'cpf': '123.456.789-00',
      'dataPagamento': '2024-01-15T10:00:00',
      'valorPago': 50.0,
      'status': 'PAGOU',
    },
    {
      'idAssociado': 2,
      'nomeAssociado': 'Maria Souza',
      'cpf': '987.654.321-00',
      'dataPagamento': null,
      'valorPago': 0.0,
      'status': 'NAO_PAGOU',
    },
  ];

  group('FinanceiroService.getAll()', () {
    test('deve retornar lista de itens financeiros quando API retorna 200', () async {
      final client = MockClient((request) async {
        expect(request.url.path, contains('/api/Financeiro/associado'));
        return http.Response(jsonEncode(financeiroJson), 200);
      });

      final service = FinanceiroService(client: client);
      final result = await service.getAll();

      expect(result, isNotEmpty);
      expect(result.first, isA<FinanceiroModel>());
      expect(result.first.id, equals(1));
      expect(result.first.descricao, equals('Mensalidade Janeiro'));
      expect(result.first.statusPagamento, equals('PAGO'));
    });

    test('deve usar cache quando cache válido existe', () async {
      SharedPreferences.setMockInitialValues({
        'cache_financeiro_associado': jsonEncode(financeiroJson),
        'timestamp_financeiro_associado': DateTime.now().millisecondsSinceEpoch,
      });

      var httpCalled = false;
      final client = MockClient((_) async {
        httpCalled = true;
        return http.Response('', 200);
      });

      final service = FinanceiroService(client: client);
      final result = await service.getAll();

      expect(httpCalled, isFalse);
      expect(result, isNotEmpty);
    });

    test('deve lançar Exception quando API retorna erro e não há cache', () async {
      final client = MockClient((_) async {
        return http.Response('Unauthorized', 401);
      });

      final service = FinanceiroService(client: client);

      expect(() => service.getAll(), throwsException);
    });

    test('deve incluir token Bearer no header', () async {
      SharedPreferences.setMockInitialValues({'jwt_token': 'token-financeiro'});

      final client = MockClient((request) async {
        expect(request.headers['Authorization'], equals('Bearer token-financeiro'));
        return http.Response(jsonEncode(financeiroJson), 200);
      });

      final service = FinanceiroService(client: client);
      await service.getAll();
    });

    test('deve retornar valores double para campo valor', () async {
      final client = MockClient((_) async {
        return http.Response(jsonEncode(financeiroJson), 200);
      });

      final service = FinanceiroService(client: client);
      final result = await service.getAll();

      for (final item in result) {
        expect(item.valor, isA<double>());
      }
    });
  });

  group('FinanceiroService.postFinanceiro()', () {
    test('deve retornar true quando API retorna 200', () async {
      final financeiro = FinanceiroModel(
        id: 1,
        descricao: 'Nova campanha',
        dataInicio: DateTime(2024, 1, 1),
        dataFim: DateTime(2024, 12, 31),
        valor: 100.0,
        statusPagamento: 'PENDENTE',
      );

      final client = MockClient((request) async {
        expect(request.url.path, equals('/api/Financeiro'));
        final body = jsonDecode(request.body);
        expect(body['id'], equals(1));
        expect(body['descricao'], equals('Nova campanha'));
        return http.Response('', 200);
      });

      final service = FinanceiroService(client: client);
      final result = await service.postFinanceiro(financeiro);

      expect(result, isTrue);
    });

    test('deve retornar true quando API retorna 201 (created)', () async {
      final financeiro = FinanceiroModel(
        id: 2,
        descricao: 'Campanha criada',
        dataInicio: DateTime(2024, 1, 1),
        dataFim: DateTime(2024, 12, 31),
        valor: 50.0,
        statusPagamento: 'PENDENTE',
      );

      final client = MockClient((_) async => http.Response('', 201));

      final service = FinanceiroService(client: client);
      final result = await service.postFinanceiro(financeiro);

      expect(result, isTrue);
    });

    test('deve retornar false quando API retorna erro', () async {
      final financeiro = FinanceiroModel(
        id: 3,
        descricao: 'Campanha inválida',
        dataInicio: DateTime(2024, 1, 1),
        dataFim: DateTime(2024, 12, 31),
        valor: 0.0,
        statusPagamento: 'PENDENTE',
      );

      final client = MockClient((_) async => http.Response('Bad Request', 400));

      final service = FinanceiroService(client: client);
      final result = await service.postFinanceiro(financeiro);

      expect(result, isFalse);
    });
  });

  group('FinanceiroService.getCampanhasAdmin()', () {
    test('deve retornar lista de campanhas quando API retorna 200', () async {
      final client = MockClient((request) async {
        expect(request.url.path, equals('/api/Financeiro'));
        return http.Response(jsonEncode(campanhasJson), 200);
      });

      final service = FinanceiroService(client: client);
      final result = await service.getCampanhasAdmin();

      expect(result, hasLength(1));
      expect(result.first, isA<CampanhaPagamentoModel>());
      expect(result.first.descricao, equals('Campanha 2024'));
      expect(result.first.pagos, equals(40));
      expect(result.first.recebido, equals(2000.0));
    });

    test('deve usar cache quando cache válido existe', () async {
      SharedPreferences.setMockInitialValues({
        'cache_financeiro_campanhas': jsonEncode(campanhasJson),
        'timestamp_financeiro_campanhas': DateTime.now().millisecondsSinceEpoch,
      });

      var httpCalled = false;
      final client = MockClient((_) async {
        httpCalled = true;
        return http.Response('', 200);
      });

      final service = FinanceiroService(client: client);
      final result = await service.getCampanhasAdmin();

      expect(httpCalled, isFalse);
      expect(result, hasLength(1));
    });

    test('deve lançar Exception quando API falha e não há cache', () async {
      final client = MockClient((_) async {
        return http.Response('Forbidden', 403);
      });

      final service = FinanceiroService(client: client);

      expect(() => service.getCampanhasAdmin(), throwsException);
    });
  });

  group('FinanceiroService.getAssociadosDoPagamento()', () {
    test('deve retornar lista de associados quando API retorna 200', () async {
      final client = MockClient((request) async {
        expect(request.url.path, equals('/api/Financeiro/10/associados'));
        return http.Response(jsonEncode(associadosJson), 200);
      });

      final service = FinanceiroService(client: client);
      final result = await service.getAssociadosDoPagamento(10);

      expect(result, hasLength(2));
      expect(result.first, isA<AssociadoPagamentoModel>());
      expect(result.first.nomeAssociado, equals('João Silva'));
      expect(result.first.status, equals('PAGOU'));
      expect(result.last.dataPagamento, isNull);
      expect(result.last.status, equals('NAO_PAGOU'));
    });

    test('deve usar cache específico por idReceita', () async {
      SharedPreferences.setMockInitialValues({
        'cache_financeiro_associados_10': jsonEncode(associadosJson),
        'timestamp_financeiro_associados_10': DateTime.now().millisecondsSinceEpoch,
      });

      var httpCalled = false;
      final client = MockClient((_) async {
        httpCalled = true;
        return http.Response('', 200);
      });

      final service = FinanceiroService(client: client);
      final result = await service.getAssociadosDoPagamento(10);

      expect(httpCalled, isFalse);
      expect(result, hasLength(2));
    });

    test('deve lançar Exception quando API falha e não há cache', () async {
      final client = MockClient((_) async {
        return http.Response('Not found', 404);
      });

      final service = FinanceiroService(client: client);

      expect(() => service.getAssociadosDoPagamento(999), throwsException);
    });
  });
}
