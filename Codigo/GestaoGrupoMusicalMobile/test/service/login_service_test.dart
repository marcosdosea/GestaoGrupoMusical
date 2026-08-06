import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'package:batala_mobile/config/api_config.dart';
import 'package:batala_mobile/service/login_service.dart';
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

  group('LoginService.login()', () {
    test('deve retornar true e salvar sessão quando API retorna 200', () async {
      final responseBody = {
        'token': 'jwt-token-valido-aqui',
        'idGrupoMusical': 3,
        'idPessoa': 42,
      };

      final client = MockClient((request) async {
        expect(request.url.path, contains('/api/Identity/login'));
        expect(request.headers['Content-Type'], equals('application/json'));

        final body = jsonDecode(request.body);
        expect(body['cpf'], equals('123.456.789-00'));
        expect(body['senha'], equals('minha_senha'));

        return http.Response(jsonEncode(responseBody), 200);
      });

      final service = LoginService(client: client);
      final result = await service.login('123.456.789-00', 'minha_senha');

      expect(result, isTrue);

      // Verifica que a sessão foi salva corretamente
      final prefs = await SharedPreferences.getInstance();
      expect(prefs.getString('jwt_token'), equals('jwt-token-valido-aqui'));
      expect(prefs.getInt('id_grupo'), equals(3));
      expect(prefs.getInt('id_pessoa'), equals(42));
    });

    test('deve retornar false quando API retorna 401 (credenciais inválidas)', () async {
      final client = MockClient((_) async {
        return http.Response('Unauthorized', 401);
      });

      final service = LoginService(client: client);
      final result = await service.login('111.222.333-44', 'senha_errada');

      expect(result, isFalse);
    });

    test('deve retornar false quando API retorna qualquer status diferente de 200', () async {
      for (final statusCode in [400, 403, 404, 500, 503]) {
        SharedPreferences.setMockInitialValues({});

        final client = MockClient((_) async {
          return http.Response('Error', statusCode);
        });

        final service = LoginService(client: client);
        final result = await service.login('000.000.000-00', 'senha');

        expect(result, isFalse,
            reason: 'Status $statusCode deveria retornar false');
      }
    });

    test('deve lançar Exception com mensagem de timeout quando TimeoutException', () async {
      // MockClient que lança TimeoutException diretamente
      final client = MockClient((_) async {
        throw TimeoutException('Timeout simulado', const Duration(seconds: 10));
      });

      final service = LoginService(client: client);

      expect(
        () => service.login('cpf', 'senha'),
        throwsA(
          predicate((e) =>
              e is Exception &&
              e.toString().contains('servidor demorou a responder')),
        ),
      );
    });

    test('deve lançar Exception com mensagem de conexão quando SocketException', () async {
      final client = MockClient((_) async {
        throw const SocketException('Connection refused');
      });

      final service = LoginService(client: client);

      expect(
        () => service.login('cpf', 'senha'),
        throwsA(
          predicate((e) =>
              e is Exception &&
              e.toString().contains('Falha de conexão')),
        ),
      );
    });

    test('deve lançar Exception genérica para outros erros inesperados', () async {
      // Simula um erro que não é TimeoutException nem SocketException
      final client = MockClient((_) async {
        throw FormatException('Resposta malformada da API');
      });

      final service = LoginService(client: client);

      expect(
        () => service.login('cpf', 'senha'),
        throwsA(
          predicate((e) =>
              e is Exception &&
              e.toString().contains('erro inesperado')),
        ),
      );
    });

    test('deve enviar JSON com cpf e senha no body da requisição', () async {
      String? capturedBody;

      final client = MockClient((request) async {
        capturedBody = request.body;
        return http.Response(
          jsonEncode({'token': 'tk', 'idGrupoMusical': 1, 'idPessoa': 1}),
          200,
        );
      });

      final service = LoginService(client: client);
      await service.login('999.888.777-66', 'secreto');

      expect(capturedBody, isNotNull);
      final body = jsonDecode(capturedBody!);
      expect(body['cpf'], equals('999.888.777-66'));
      expect(body['senha'], equals('secreto'));
    });

    test('deve converter idGrupoMusical e idPessoa de string para int quando API retorna strings', () async {
      final responseBody = {
        'token': 'token-abc',
        'idGrupoMusical': '5', // String ao invés de int
        'idPessoa': '10',      // String ao invés de int
      };

      final client = MockClient((_) async {
        return http.Response(jsonEncode(responseBody), 200);
      });

      final service = LoginService(client: client);
      final result = await service.login('cpf', 'senha');

      expect(result, isTrue);

      final prefs = await SharedPreferences.getInstance();
      // Verifica que int.tryParse funcionou corretamente para converter string -> int
      expect(prefs.getInt('id_grupo'), equals(5));
      expect(prefs.getInt('id_pessoa'), equals(10));
    });
  });
}
