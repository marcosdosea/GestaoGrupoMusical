class InformativoModel {
  final int id;
  final DateTime dataInicio;
  final String mensagem;

  InformativoModel({
    required this.id,
    required this.dataInicio,
    required this.mensagem,
  });

  factory InformativoModel.fromJson(Map<String, dynamic> json) {
  return InformativoModel(
    id: json['id'] ?? 0,
    dataInicio: json['dataHoraInicio'] != null 
        ? DateTime.parse(json['dataHoraInicio']) 
        : DateTime.now(), 
    mensagem: json['mensagem']?? 'Não informado'
    
  );
}
}
