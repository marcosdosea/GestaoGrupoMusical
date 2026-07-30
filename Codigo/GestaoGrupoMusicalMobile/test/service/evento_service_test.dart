import 'dart:convert';
import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/model/evento_model.dart';
import 'package:batala_mobile/service/evento_service.dart';
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

  final eventoJson = [
    {
      'id': 1,
      'dataHoraInicio': '2024-07-20T18:00:00.000',
      'dataHoraFim': '2024-07-20T22:00:00.000',
      'local': 'Teatro Municipal',
      'repertorio': 'Samba Reggae',
    },
    {
      'id': 2,
      'dataHoraInicio': '2024-08-10T09:00:00.000',
      'dataHoraFim': '2024-08-10T12:00:00.000',
      'local': 'Praça Central',
      'repertorio': 'Afro',
    },
  ];

  group('EventoService.getAll()', () {
    test('deve retornar lista de eventos quando API retorna 200 e salvar no cache', () async {
      final client = MockClient((request) async {
        expect(request.url.path, equals('/api/Evento'));
        return http.Response(jsonEncode(eventoJson), 200);
      });

      final service = EventoService(client: client);
      final result = await service.getAll();

      expect(result, hasLength(2));
      expect(result[0].id, equals(1));
      expect(result[0].local, equals('Teatro Municipal'));
      expect(result[1].repertorio, equals('Afro'));
    });

    test('deve retornar lista do cache quando cache válido existe', () async {
      SharedPreferences.setMockInitialValues({
        'cache_evento_list': jsonEncode(eventoJson),
        'timestamp_evento_list': DateTime.now().millisecondsSinceEpoch,
      });

      var httpCalled = false;
      final client = MockClient((request) async {
        httpCalled = true;
        return http.Response('', 200);
      });

      final service = EventoService(client: client);
      final result = await service.getAll();

      expect(httpCalled, isFalse);
      expect(result, hasLength(2));
    });

    test('deve lançar Exception quando API falha e não há cache', () async {
      final client = MockClient((_) async {
        return http.Response('Error', 503);
      });

      final service = EventoService(client: client);

      expect(() => service.getAll(), throwsException);
    });

    test('deve retornar objetos EventoModel válidos', () async {
      final client = MockClient((_) async {
        return http.Response(jsonEncode(eventoJson), 200);
      });

      final service = EventoService(client: client);
      final result = await service.getAll();

      expect(result.first, isA<EventoModel>());
      expect(result.first.dataInicio, isA<DateTime>());
      expect(result.first.dataFim, isA<DateTime>());
    });
  });

  group('EventoService.getDetalhesEvento()', () {
    test('deve retornar Map quando API retorna 200 com campo "id"', () async {
      final detalhesData = {
        'id': 1,
        'local': 'Teatro Municipal',
        'minhaInscricao': {'status': 'CONFIRMADO'},
      };

      final client = MockClient((request) async {
        expect(request.url.path, contains('/api/Evento/Detalhes/1'));
        return http.Response(jsonEncode(detalhesData), 200);
      });

      final service = EventoService(client: client);
      final result = await service.getDetalhesEvento(1);

      expect(result, isNotNull);
      expect(result!['id'], equals(1));
    });

    test('deve retornar null quando JSON não tem campo "id" nem "Id"', () async {
      final dadosSemId = {'local': 'Algum lugar', 'repertorio': 'X'};

      final client = MockClient((_) async {
        return http.Response(jsonEncode(dadosSemId), 200);
      });

      final service = EventoService(client: client);
      final result = await service.getDetalhesEvento(1);

      expect(result, isNull);
    });

    test('deve retornar null quando API retorna erro HTTP', () async {
      final client = MockClient((_) async {
        return http.Response('Not found', 404);
      });

      final service = EventoService(client: client);
      final result = await service.getDetalhesEvento(999);

      expect(result, isNull);
    });

    test('deve aceitar campo "Id" maiúsculo também', () async {
      final detalhesData = {
        'Id': 5,
        'Local': 'Praça',
      };

      final client = MockClient((_) async {
        return http.Response(jsonEncode(detalhesData), 200);
      });

      final service = EventoService(client: client);
      final result = await service.getDetalhesEvento(5);

      expect(result, isNotNull);
    });
  });

  group('EventoService.solicitarParticipacao()', () {
    test('deve retornar null (sucesso) quando API retorna 200', () async {
      final client = MockClient((request) async {
        expect(request.url.path, contains('/api/Evento/ResponderPresenca'));
        final body = jsonDecode(request.body);
        expect(body['idEvento'], equals(1));
        expect(body['idTipoInstrumento'], equals(3));
        return http.Response('', 200);
      });

      final service = EventoService(client: client);
      final result = await service.solicitarParticipacao(1, 3);

      expect(result, isNull);
    });

    test('deve retornar mensagem de erro da API quando falha', () async {
      final client = MockClient((_) async {
        return http.Response(
          jsonEncode({'mensagem': 'Vagas esgotadas para este instrumento.'}),
          400,
        );
      });

      final service = EventoService(client: client);
      final result = await service.solicitarParticipacao(1, 3);

      expect(result, equals('Vagas esgotadas para este instrumento.'));
    });

    test('deve retornar "Erro de conexão." quando há exceção', () async {
      final client = MockClient((_) async {
        throw Exception('Offline');
      });

      final service = EventoService(client: client);
      final result = await service.solicitarParticipacao(1, 3);

      expect(result, equals('Erro de conexão.'));
    });
  });

  group('EventoService.cancelarSolicitacao()', () {
    test('deve retornar true quando API retorna 200', () async {
      final client = MockClient((request) async {
        expect(request.url.path, contains('/api/Evento/CancelarPresenca/1'));
        return http.Response('', 200);
      });

      final service = EventoService(client: client);
      final result = await service.cancelarSolicitacao(1);

      expect(result, isTrue);
    });

    test('deve retornar false quando API retorna erro', () async {
      final client = MockClient((_) async {
        return http.Response('', 400);
      });

      final service = EventoService(client: client);
      final result = await service.cancelarSolicitacao(1);

      expect(result, isFalse);
    });

    test('deve retornar false quando há exceção de rede', () async {
      final client = MockClient((_) async {
        throw Exception('Timeout');
      });

      final service = EventoService(client: client);
      final result = await service.cancelarSolicitacao(1);

      expect(result, isFalse);
    });
  });

  group('EventoService.justificarAusencia()', () {
    test('deve retornar null quando API retorna 200', () async {
      final client = MockClient((request) async {
        expect(request.url.path, equals('/api/Evento/JustificarAusencia'));
        final body = jsonDecode(request.body);
        expect(body['idEvento'], equals(2));
        expect(body['justificativa'], equals('Viagem de trabalho'));
        return http.Response('', 200);
      });

      final service = EventoService(client: client);
      final result = await service.justificarAusencia(2, 'Viagem de trabalho');

      expect(result, isNull);
    });

    test('deve retornar mensagem "sessão expirada" para 401', () async {
      final client = MockClient((_) async => http.Response('', 401));

      final service = EventoService(client: client);
      final result = await service.justificarAusencia(1, 'Justificativa');

      expect(result, equals('Sua sessão expirou. Entre novamente.'));
    });

    test('deve retornar mensagem "sem permissão" para 403', () async {
      final client = MockClient((_) async => http.Response('', 403));

      final service = EventoService(client: client);
      final result = await service.justificarAusencia(1, 'Justificativa');

      expect(result, equals('Você não tem permissão para justificar esta ausência.'));
    });

    test('deve retornar mensagem customizada da API para outros status de erro', () async {
      final client = MockClient((_) async {
        return http.Response(
          jsonEncode({'mensagem': 'Prazo expirado.'}),
          422,
        );
      });

      final service = EventoService(client: client);
      final result = await service.justificarAusencia(1, 'Tarde');

      expect(result, equals('Prazo expirado.'));
    });

    test('deve retornar mensagem de conexão quando há exceção', () async {
      final client = MockClient((_) async => throw Exception('Offline'));

      final service = EventoService(client: client);
      final result = await service.justificarAusencia(1, 'Justificativa');

      expect(result, contains('Não foi possível conectar'));
    });
  });

  group('EventoService.getMinhaInscricao()', () {
    test('deve delegar para getDetalhesEvento e retornar mesmo resultado', () async {
      final detalhes = {'id': 3, 'local': 'Quadra', 'minhaInscricao': {}};
      final client = MockClient((_) async {
        return http.Response(jsonEncode(detalhes), 200);
      });

      final service = EventoService(client: client);
      final result = await service.getMinhaInscricao(3);

      expect(result, isNotNull);
      expect(result!['id'], equals(3));
    });
  });
}
