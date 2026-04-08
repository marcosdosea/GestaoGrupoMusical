class FinanceiroModel {
  final int id;
  final String descricao;
  final DateTime dataInicio;
  final DateTime dataFim;
  final double valor;
  final String statusPagamento; // Recebe o status da API

  FinanceiroModel({
    required this.id,
    required this.descricao,
    required this.dataInicio,
    required this.dataFim,
    required this.valor,
    required this.statusPagamento,
  });

  factory FinanceiroModel.fromJson(Map<String, dynamic> json) {
    return FinanceiroModel(
      id: json['id'] ?? 0,
      descricao: json['descricao'] ?? '',
      dataInicio: DateTime.parse(json['dataInicio']),
      dataFim: DateTime.parse(json['dataFim']),
      valor: (json['valor'] ?? 0.0).toDouble(),
      statusPagamento: json['statusPagamento'] ?? 'Pendente', 
    );
  }
}

//(Resumo das Campanhas do Admin)-> tela de pagamento para role 'ADMINISTRADOR GRUPO'
class CampanhaPagamentoModel {
  final int id;
  final String descricao;
  final DateTime inicio;
  final DateTime fim;
  final int pagos;
  final int isentos;
  final int atrasos;
  final double recebido;

  CampanhaPagamentoModel({
    required this.id, required this.descricao, required this.inicio, 
    required this.fim, required this.pagos, required this.isentos, 
    required this.atrasos, required this.recebido
  });

  factory CampanhaPagamentoModel.fromJson(Map<String, dynamic> json) {
    return CampanhaPagamentoModel(
      id: json['id'] ?? 0,
      descricao: json['descricao'] ?? '',
      inicio: DateTime.parse(json['dataInicio']),
      fim: DateTime.parse(json['dataFim']),
      pagos: json['pagos'] ?? 0,
      isentos: json['isentos'] ?? 0,
      atrasos: json['atrasos'] ?? 0,
      recebido: (json['recebido'] ?? 0).toDouble(),
    );
  }
}

// Modelo para a Tela 2 (Detalhes dos Associados)
class AssociadoPagamentoModel {
  final int idAssociado;
  final String nomeAssociado;
  final String cpf;
  final DateTime? dataPagamento;
  final double valorPago;
  final String status;

  AssociadoPagamentoModel({
    required this.idAssociado, required this.nomeAssociado, required this.cpf, 
    this.dataPagamento, required this.valorPago, required this.status
  });

  factory AssociadoPagamentoModel.fromJson(Map<String, dynamic> json) {
    return AssociadoPagamentoModel(
      idAssociado: json['idAssociado'] ?? 0,
      nomeAssociado: json['nomeAssociado'] ?? '',
      cpf: json['cpf'] ?? '',
      dataPagamento: json['dataPagamento'] != null ? DateTime.parse(json['dataPagamento']) : null,
      valorPago: (json['valorPago'] ?? 0).toDouble(),
      status: json['status'] ?? 'NAO_PAGOU',
    );
  }
}