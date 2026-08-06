import 'dart:convert';
import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/model/ensaio_model.dart';
import 'package:batala_mobile/service/ensaio_service.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:shared_preferences/shared_preferences.dart';

void main() {
  // Inicializa o ApiConfig.baseUrl para os testes (campo late static)
  setUpAll(() {
    ApiConfig.baseUrl = 'http://localhost:5153';
  });

  // Isola o SharedPreferences para cada teste
  setUp(() {
    SharedPreferences.setMockInitialValues({});
  });

  // Dados de teste reutilizáveis
  final ensaioJson = [
    {
      'id': 1,
      'dataHoraInicio': '2024-06-15T20:00:00.000',
      'tipo': 'Geral',
      'local': 'Quadra Central',
      'presencaObrigatoria': 'Sim',
    },
    {
      'id': 2,
      'dataHoraInicio': '2024-06-22T20:00:00.000',
      'tipo': 'Naipe',
      'local': 'Sala de Ensaio',
      'presencaObrigatoria': 'Não',
    },
  ];

  group('EnsaioService.getAll()', () {
    test('deve retornar lista de ensaios quando API retorna 200', () async {
      final client = MockClient((request) async {
        expect(request.url.path, equals('/api/Ensaio'));
        expect(request.headers['Accept'], equals('application/json'));
        return http.Response(jsonEncode(ensaioJson), 200);
      });

      final service = EnsaioService(client: client);
      final result = await service.getAll();

      expect(result, hasLength(2));
      expect(result[0].id, equals(1));
      expect(result[0].tipo, equals('Geral'));
      expect(result[0].local, equals('Quadra Central'));
      expect(result[0].presencaObrigatoria, isTrue);
      expect(result[1].id, equals(2));
      expect(result[1].presencaObrigatoria, isFalse);
    });

    test('deve retornar lista do cache quando cache válido existe (sem chamada HTTP)', () async {
      // Pré-carrega o cache
      SharedPreferences.setMockInitialValues({
        'cache_ensaio_list': jsonEncode(ensaioJson),
        'timestamp_ensaio_list':
            DateTime.now().millisecondsSinceEpoch,
      });

      var httpCalled = false;
      final client = MockClient((request) async {
        httpCalled = true;
        return http.Response('', 200);
      });

      final service = EnsaioService(client: client);
      final result = await service.getAll();

      expect(httpCalled, isFalse, reason: 'HTTP não deveria ser chamado quando cache é válido');
      expect(result, hasLength(2));
    });

    test('deve lançar Exception quando API falha e não há cache', () async {
      final client = MockClient((request) async {
        return http.Response('Internal Server Error', 500);
      });

      final service = EnsaioService(client: client);

      expect(() => service.getAll(), throwsException);
    });

    test('deve retornar lista de ensaios com dados corretos do modelo', () async {
      final client = MockClient((_) async {
        return http.Response(jsonEncode(ensaioJson), 200);
      });

      final service = EnsaioService(client: client);
      final result = await service.getAll();

      expect(result.first, isA<EnsaioModel>());
      expect(result.first.dataHora, isA<DateTime>());
    });
  });

  group('EnsaioService.getMinhaFrequencia()', () {
    test('deve retornar Map quando API retorna 200', () async {
      final frequenciaData = {
        'idEnsaio': 1,
        'frequencia': 'PRESENTE',
        'justificativa': null,
      };

      final client = MockClient((request) async {
        expect(request.url.path, contains('/api/Ensaio/DetalhesSolicitacao/1'));
        return http.Response(jsonEncode(frequenciaData), 200);
      });

      final service = EnsaioService(client: client);
      final result = await service.getMinhaFrequencia(1);

      expect(result, isNotNull);
      expect(result!['frequencia'], equals('PRESENTE'));
    });

    test('deve retornar null quando API retorna 404', () async {
      final client = MockClient((_) async {
        return http.Response('Not Found', 404);
      });

      final service = EnsaioService(client: client);
      final result = await service.getMinhaFrequencia(999);

      expect(result, isNull);
    });

    test('deve retornar null quando há exceção de rede', () async {
      final client = MockClient((_) async {
        throw Exception('Sem conexão');
      });

      final service = EnsaioService(client: client);
      final result = await service.getMinhaFrequencia(1);

      expect(result, isNull);
    });

    test('deve incluir token no header de autorização', () async {
      SharedPreferences.setMockInitialValues({'jwt_token': 'meu-token-jwt'});

      final client = MockClient((request) async {
        expect(request.headers['Authorization'], equals('Bearer meu-token-jwt'));
        return http.Response(jsonEncode({'idEnsaio': 1}), 200);
      });

      final service = EnsaioService(client: client);
      await service.getMinhaFrequencia(1);
    });
  });

  group('EnsaioService.justificarAusencia()', () {
    test('deve retornar null (sucesso) quando API retorna 200', () async {
      final client = MockClient((request) async {
        expect(request.url.path, equals('/api/Ensaio/JustificarAusencia'));
        final body = jsonDecode(request.body);
        expect(body['idEnsaio'], equals(5));
        expect(body['justificativa'], equals('Estava doente'));
        return http.Response('', 200);
      });

      final service = EnsaioService(client: client);
      final result = await service.justificarAusencia(5, 'Estava doente');

      expect(result, isNull);
    });

    test('deve retornar mensagem de sessão expirada para status 401', () async {
      final client = MockClient((_) async {
        return http.Response('', 401);
      });

      final service = EnsaioService(client: client);
      final result = await service.justificarAusencia(1, 'Justificativa');

      expect(result, equals('Sua sessão expirou. Entre novamente.'));
    });

    test('deve retornar mensagem de permissão negada para status 403', () async {
      final client = MockClient((_) async {
        return http.Response('', 403);
      });

      final service = EnsaioService(client: client);
      final result = await service.justificarAusencia(1, 'Justificativa');

      expect(result, equals('Você não tem permissão para justificar esta ausência.'));
    });

    test('deve retornar mensagem customizada da API quando disponível', () async {
      final client = MockClient((_) async {
        return http.Response(
          jsonEncode({'mensagem': 'Prazo de justificativa encerrado.'}),
          422,
        );
      });

      final service = EnsaioService(client: client);
      final result = await service.justificarAusencia(1, 'Tarde demais');

      expect(result, equals('Prazo de justificativa encerrado.'));
    });

    test('deve retornar mensagem genérica com código HTTP para outros erros', () async {
      final client = MockClient((_) async {
        return http.Response('{}', 500);
      });

      final service = EnsaioService(client: client);
      final result = await service.justificarAusencia(1, 'Justificativa');

      expect(result, contains('500'));
    });

    test('deve retornar mensagem de conexão quando há exceção de rede', () async {
      final client = MockClient((_) async {
        throw Exception('Timeout');
      });

      final service = EnsaioService(client: client);
      final result = await service.justificarAusencia(1, 'Justificativa');

      expect(result, contains('Não foi possível conectar'));
    });
  });

  group('EnsaioService.limparCacheListagem()', () {
    test('deve limpar cache sem lançar exceção', () async {
      SharedPreferences.setMockInitialValues({
        'cache_ensaio_list': jsonEncode(ensaioJson),
        'timestamp_ensaio_list': DateTime.now().millisecondsSinceEpoch,
      });

      final client = MockClient((_) async => http.Response('[]', 200));
      final service = EnsaioService(client: client);

      expect(
        () => service.limparCacheListagem(),
        returnsNormally,
      );
    });
  });
}
