import 'package:batala_mobile/model/ensaio_model.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('EnsaioModel.fromJson', () {
    test('deve fazer parse correto de todos os campos', () {
      final json = {
        'id': 42,
        'dataHoraInicio': '2024-06-15T20:00:00.000',
        'tipo': 'Geral',
        'local': 'Quadra Central',
        'presencaObrigatoria': 'Sim',
      };

      final model = EnsaioModel.fromJson(json);

      expect(model.id, equals(42));
      expect(model.dataHora, equals(DateTime.parse('2024-06-15T20:00:00.000')));
      expect(model.tipo, equals('Geral'));
      expect(model.local, equals('Quadra Central'));
      expect(model.presencaObrigatoria, isTrue);
    });

    test('presencaObrigatoria deve ser true somente quando valor é "Sim"', () {
      final json = {
        'id': 1,
        'dataHoraInicio': '2024-01-01T10:00:00',
        'tipo': 'Naipe',
        'local': 'Sala A',
        'presencaObrigatoria': 'Sim',
      };

      final model = EnsaioModel.fromJson(json);

      expect(model.presencaObrigatoria, isTrue);
    });

    test('presencaObrigatoria deve ser false para valor diferente de "Sim"', () {
      final jsonNao = {
        'id': 2,
        'dataHoraInicio': '2024-01-01T10:00:00',
        'tipo': 'Naipe',
        'local': 'Sala A',
        'presencaObrigatoria': 'Não',
      };
      final jsonFalse = {
        'id': 3,
        'dataHoraInicio': '2024-01-01T10:00:00',
        'tipo': 'Naipe',
        'local': 'Sala A',
        'presencaObrigatoria': false,
      };

      expect(EnsaioModel.fromJson(jsonNao).presencaObrigatoria, isFalse);
      expect(EnsaioModel.fromJson(jsonFalse).presencaObrigatoria, isFalse);
    });

    test('deve usar valores default quando campos são null', () {
      final json = <String, dynamic>{
        'id': null,
        'dataHoraInicio': null,
        'tipo': null,
        'local': null,
        'presencaObrigatoria': null,
      };

      final model = EnsaioModel.fromJson(json);

      expect(model.id, equals(0));
      expect(model.tipo, equals('Sem tipo'));
      expect(model.local, equals('Local não informado'));
      expect(model.presencaObrigatoria, isFalse);
    });

    test('deve usar valores default quando campos estão ausentes do json', () {
      final json = <String, dynamic>{};

      final model = EnsaioModel.fromJson(json);

      expect(model.id, equals(0));
      expect(model.tipo, equals('Sem tipo'));
      expect(model.local, equals('Local não informado'));
      expect(model.presencaObrigatoria, isFalse);
    });
  });
}
