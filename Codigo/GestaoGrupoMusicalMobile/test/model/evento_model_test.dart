import 'package:batala_mobile/model/evento_model.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('EventoModel.fromJson', () {
    test('deve fazer parse correto de todos os campos com chaves minúsculas', () {
      final json = {
        'id': 10,
        'dataHoraInicio': '2024-07-20T18:00:00.000',
        'dataHoraFim': '2024-07-20T22:00:00.000',
        'local': 'Teatro Municipal',
        'repertorio': 'Samba Reggae',
      };

      final model = EventoModel.fromJson(json);

      expect(model.id, equals(10));
      expect(model.dataInicio, equals(DateTime.parse('2024-07-20T18:00:00.000')));
      expect(model.dataFim, equals(DateTime.parse('2024-07-20T22:00:00.000')));
      expect(model.local, equals('Teatro Municipal'));
      expect(model.repertorio, equals('Samba Reggae'));
    });

    test('deve fazer parse correto de todos os campos com chaves maiúsculas (Id, DataHoraInicio)', () {
      final json = {
        'Id': 20,
        'DataHoraInicio': '2024-08-10T09:00:00.000',
        'DataHoraFim': '2024-08-10T12:00:00.000',
        'Local': 'Praça da Liberdade',
        'Repertorio': 'Afro',
      };

      final model = EventoModel.fromJson(json);

      expect(model.id, equals(20));
      expect(model.dataInicio, equals(DateTime.parse('2024-08-10T09:00:00.000')));
      expect(model.dataFim, equals(DateTime.parse('2024-08-10T12:00:00.000')));
      expect(model.local, equals('Praça da Liberdade'));
      expect(model.repertorio, equals('Afro'));
    });

    test('deve usar valores default quando campos são null (preferência por minúsculo)', () {
      final json = <String, dynamic>{
        'id': null,
        'dataHoraInicio': null,
        'local': null,
        'repertorio': null,
      };

      // dataHoraFim também nulo — o fromJson usa DateTime.now() como fallback
      // Apenas verificamos que não lança exceção e que os defaults são aplicados
      expect(
        () => EventoModel.fromJson(json),
        returnsNormally,
      );

      final model = EventoModel.fromJson(json);
      expect(model.id, equals(0));
      expect(model.local, equals('Local não informado'));
      expect(model.repertorio, equals('Sem repertório'));
    });

    test('deve preferir chaves minúsculas sobre maiúsculas quando ambas presentes', () {
      final json = {
        'id': 5,
        'Id': 99,
        'dataHoraInicio': '2024-01-01T00:00:00',
        'DataHoraInicio': '2025-12-31T00:00:00',
        'dataHoraFim': '2024-01-01T06:00:00',
        'DataHoraFim': '2025-12-31T06:00:00',
        'local': 'Local Minúsculo',
        'Local': 'Local Maiúsculo',
        'repertorio': 'Rep Minúsculo',
        'Repertorio': 'Rep Maiúsculo',
      };

      final model = EventoModel.fromJson(json);

      expect(model.id, equals(5));
      expect(model.local, equals('Local Minúsculo'));
      expect(model.repertorio, equals('Rep Minúsculo'));
    });
  });
}
