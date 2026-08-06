import 'package:batala_mobile/model/financeiro_model.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('FinanceiroModel.fromJson', () {
    test('deve fazer parse correto de todos os campos', () {
      final json = {
        'id': 1,
        'descricao': 'Mensalidade Janeiro',
        'dataInicio': '2024-01-01T00:00:00',
        'dataFim': '2024-01-31T00:00:00',
        'valor': 50.0,
        'statusPagamento': 'PAGO',
      };

      final model = FinanceiroModel.fromJson(json);

      expect(model.id, equals(1));
      expect(model.descricao, equals('Mensalidade Janeiro'));
      expect(model.dataInicio, equals(DateTime.parse('2024-01-01T00:00:00')));
      expect(model.dataFim, equals(DateTime.parse('2024-01-31T00:00:00')));
      expect(model.valor, equals(50.0));
      expect(model.statusPagamento, equals('PAGO'));
    });

    test('deve converter valor inteiro para double', () {
      final json = {
        'id': 2,
        'descricao': 'Taxa',
        'dataInicio': '2024-02-01T00:00:00',
        'dataFim': '2024-02-28T00:00:00',
        'valor': 100,
        'statusPagamento': 'PENDENTE',
      };

      final model = FinanceiroModel.fromJson(json);

      expect(model.valor, isA<double>());
      expect(model.valor, equals(100.0));
    });

    test('deve usar valores default quando campos são null', () {
      final json = <String, dynamic>{
        'id': null,
        'descricao': null,
        'dataInicio': '2024-01-01T00:00:00',
        'dataFim': '2024-01-31T00:00:00',
        'valor': null,
        'statusPagamento': null,
      };

      final model = FinanceiroModel.fromJson(json);

      expect(model.id, equals(0));
      expect(model.descricao, equals(''));
      expect(model.valor, equals(0.0));
      expect(model.statusPagamento, equals('Pendente'));
    });
  });

  group('CampanhaPagamentoModel.fromJson', () {
    test('deve fazer parse correto de todos os campos', () {
      final json = {
        'id': 10,
        'descricao': 'Campanha Anual 2024',
        'dataInicio': '2024-01-01T00:00:00',
        'dataFim': '2024-12-31T00:00:00',
        'pagos': 30,
        'isentos': 5,
        'atrasos': 10,
        'recebido': 1500.0,
      };

      final model = CampanhaPagamentoModel.fromJson(json);

      expect(model.id, equals(10));
      expect(model.descricao, equals('Campanha Anual 2024'));
      expect(model.inicio, equals(DateTime.parse('2024-01-01T00:00:00')));
      expect(model.fim, equals(DateTime.parse('2024-12-31T00:00:00')));
      expect(model.pagos, equals(30));
      expect(model.isentos, equals(5));
      expect(model.atrasos, equals(10));
      expect(model.recebido, equals(1500.0));
    });

    test('deve usar valores default quando campos numéricos são null', () {
      final json = <String, dynamic>{
        'id': null,
        'descricao': null,
        'dataInicio': '2024-01-01T00:00:00',
        'dataFim': '2024-12-31T00:00:00',
        'pagos': null,
        'isentos': null,
        'atrasos': null,
        'recebido': null,
      };

      final model = CampanhaPagamentoModel.fromJson(json);

      expect(model.id, equals(0));
      expect(model.descricao, equals(''));
      expect(model.pagos, equals(0));
      expect(model.isentos, equals(0));
      expect(model.atrasos, equals(0));
      expect(model.recebido, equals(0.0));
    });
  });

  group('AssociadoPagamentoModel.fromJson', () {
    test('deve fazer parse correto de todos os campos incluindo dataPagamento', () {
      final json = {
        'idAssociado': 100,
        'nomeAssociado': 'João Silva',
        'cpf': '123.456.789-00',
        'dataPagamento': '2024-03-15T14:30:00',
        'valorPago': 50.0,
        'status': 'PAGOU',
      };

      final model = AssociadoPagamentoModel.fromJson(json);

      expect(model.idAssociado, equals(100));
      expect(model.nomeAssociado, equals('João Silva'));
      expect(model.cpf, equals('123.456.789-00'));
      expect(model.dataPagamento, equals(DateTime.parse('2024-03-15T14:30:00')));
      expect(model.valorPago, equals(50.0));
      expect(model.status, equals('PAGOU'));
    });

    test('dataPagamento deve ser null quando não informado', () {
      final json = <String, dynamic>{
        'idAssociado': 101,
        'nomeAssociado': 'Maria Souza',
        'cpf': '987.654.321-00',
        'dataPagamento': null,
        'valorPago': 0,
        'status': 'NAO_PAGOU',
      };

      final model = AssociadoPagamentoModel.fromJson(json);

      expect(model.dataPagamento, isNull);
      expect(model.status, equals('NAO_PAGOU'));
    });

    test('deve usar valor default de status quando null', () {
      final json = <String, dynamic>{
        'idAssociado': 102,
        'nomeAssociado': 'Pedro Costa',
        'cpf': '111.222.333-44',
        'valorPago': null,
        'status': null,
      };

      final model = AssociadoPagamentoModel.fromJson(json);

      expect(model.valorPago, equals(0.0));
      expect(model.status, equals('NAO_PAGOU'));
    });
  });
}
