class FinanceiroModel {
  final int id;
  final String descricao;
  final DateTime dataInicio;
  final DateTime dataFim;
  final int pagos;
  final int isentos;
  final int atrasos;
  final double recebido;

  FinanceiroModel({
    required this.id,
    required this.descricao,
    required this.dataInicio,
    required this.dataFim,
    this.pagos = 0,
    this.isentos = 0,
    this.atrasos = 0,
    this.recebido = 0.0,
  });

  factory FinanceiroModel.fromJson(Map<String, dynamic> json) {
    return FinanceiroModel(
      id: json['id'] ?? 0,
      descricao: json['descricao'] ?? '',
      dataInicio: DateTime.parse(json['dataInicio']),
      dataFim: DateTime.parse(json['dataFim']),
      pagos: json['pagos'] ?? 0,
      isentos: json['isentos'] ?? 0,
      atrasos: json['atrasos'] ?? 0,
      recebido: (json['recebido'] as num).toDouble(),
    );
  }
}