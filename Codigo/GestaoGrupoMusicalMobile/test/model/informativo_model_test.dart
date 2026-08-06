import 'package:batala_mobile/model/informativo_model.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('InformativoModel.fromJson', () {
    test('deve fazer parse correto de todos os campos', () {
      final json = {
        'id': 5,
        'data': '2024-05-10T08:00:00.000',
        'mensagem': 'Ensaio cancelado esta semana.',
      };

      final model = InformativoModel.fromJson(json);

      expect(model.id, equals(5));
      expect(model.dataInicio, equals(DateTime.parse('2024-05-10T08:00:00.000')));
      expect(model.mensagem, equals('Ensaio cancelado esta semana.'));
    });

    test('deve usar DateTime.now() como default quando data é null', () {
      final antes = DateTime.now();

      final json = <String, dynamic>{
        'id': 1,
        'data': null,
        'mensagem': 'Mensagem sem data',
      };

      final model = InformativoModel.fromJson(json);
      final depois = DateTime.now();

      expect(model.dataInicio.isAfter(antes.subtract(const Duration(seconds: 1))), isTrue);
      expect(model.dataInicio.isBefore(depois.add(const Duration(seconds: 1))), isTrue);
    });

    test('deve usar "Não informado" quando mensagem é null', () {
      final json = <String, dynamic>{
        'id': 2,
        'data': '2024-01-01T00:00:00',
        'mensagem': null,
      };

      final model = InformativoModel.fromJson(json);

      expect(model.mensagem, equals('Não informado'));
    });

    test('deve usar id 0 quando id é null', () {
      final json = <String, dynamic>{
        'id': null,
        'data': '2024-01-01T00:00:00',
        'mensagem': 'Teste',
      };

      final model = InformativoModel.fromJson(json);

      expect(model.id, equals(0));
    });

    test('deve funcionar corretamente com json sem campos', () {
      final json = <String, dynamic>{};

      expect(
        () => InformativoModel.fromJson(json),
        returnsNormally,
      );

      final model = InformativoModel.fromJson(json);
      expect(model.id, equals(0));
      expect(model.mensagem, equals('Não informado'));
    });
  });
}
