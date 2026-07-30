import 'package:batala_mobile/model/material_estudo_model.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('MaterialestudoModel.fromJson', () {
    test('deve fazer parse correto de todos os campos', () {
      final json = {
        'id': 7,
        'nome': 'Apostila de Ritmos',
        'link': 'https://drive.google.com/apostila',
        'data': '2024-03-01T00:00:00.000',
      };

      final model = MaterialestudoModel.fromJson(json);

      expect(model.id, equals(7));
      expect(model.nome, equals('Apostila de Ritmos'));
      expect(model.link, equals('https://drive.google.com/apostila'));
      expect(model.dataInicio, equals(DateTime.parse('2024-03-01T00:00:00.000')));
    });

    test('deve usar "Não informado" quando nome é null', () {
      final json = <String, dynamic>{
        'id': 1,
        'nome': null,
        'link': 'https://exemplo.com',
        'data': '2024-01-01T00:00:00',
      };

      final model = MaterialestudoModel.fromJson(json);

      expect(model.nome, equals('Não informado'));
    });

    test('deve usar "Não informado" quando link é null', () {
      final json = <String, dynamic>{
        'id': 2,
        'nome': 'Material Sem Link',
        'link': null,
        'data': '2024-01-01T00:00:00',
      };

      final model = MaterialestudoModel.fromJson(json);

      expect(model.link, equals('Não informado'));
    });

    test('deve usar DateTime.now() como fallback quando data é null', () {
      final antes = DateTime.now();

      final json = <String, dynamic>{
        'id': 3,
        'nome': 'Material',
        'link': 'http://link.com',
        'data': null,
      };

      final model = MaterialestudoModel.fromJson(json);
      final depois = DateTime.now();

      expect(
        model.dataInicio.isAfter(antes.subtract(const Duration(seconds: 1))),
        isTrue,
      );
      expect(
        model.dataInicio.isBefore(depois.add(const Duration(seconds: 1))),
        isTrue,
      );
    });

    test('deve usar id 0 quando id é null', () {
      final json = <String, dynamic>{
        'id': null,
        'nome': 'Material',
        'link': 'http://link.com',
        'data': '2024-01-01T00:00:00',
      };

      final model = MaterialestudoModel.fromJson(json);

      expect(model.id, equals(0));
    });
  });
}
