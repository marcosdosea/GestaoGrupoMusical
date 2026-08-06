import 'package:batala_mobile/model/login_model.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('LoginModel.toJson', () {
    test('deve serializar cpf e senha corretamente', () {
      final model = LoginModel(cpf: '123.456.789-00', senha: 'senha123');

      final json = model.toJson();

      expect(json['cpf'], equals('123.456.789-00'));
      expect(json['senha'], equals('senha123'));
    });

    test('deve conter exatamente as chaves cpf e senha', () {
      final model = LoginModel(cpf: '000.000.000-00', senha: 'abc');

      final json = model.toJson();

      expect(json.keys.toList(), containsAll(['cpf', 'senha']));
      expect(json.length, equals(2));
    });

    test('deve preservar cpf com formatação completa', () {
      final model = LoginModel(cpf: '987.654.321-09', senha: 'pass');

      final json = model.toJson();

      expect(json['cpf'], equals('987.654.321-09'));
    });

    test('deve suportar senha com caracteres especiais', () {
      final model = LoginModel(cpf: '111.222.333-44', senha: r'P@ss!123#');

      final json = model.toJson();

      expect(json['senha'], equals(r'P@ss!123#'));
    });

    test('deve suportar valores vazios sem lançar exceção', () {
      final model = LoginModel(cpf: '', senha: '');

      expect(() => model.toJson(), returnsNormally);

      final json = model.toJson();
      expect(json['cpf'], equals(''));
      expect(json['senha'], equals(''));
    });
  });
}
