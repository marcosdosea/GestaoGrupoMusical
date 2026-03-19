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