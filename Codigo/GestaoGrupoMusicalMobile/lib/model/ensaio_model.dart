class EnsaioModel {
  final int id;
  final DateTime dataHora;
  final String tipo;
  final String local;
  final bool presencaObrigatoria;


  EnsaioModel({
    required this.id,
    required this.dataHora,
    required this.tipo,
    required this.local,
    required this.presencaObrigatoria,
  });

  factory EnsaioModel.fromJson(Map<String, dynamic> json) {
  return EnsaioModel(
    id: json['id'] ?? 0,
    dataHora: json['dataHoraInicio'] != null 
        ? DateTime.parse(json['dataHoraInicio']) 
        : DateTime.now(), 
    tipo: json['tipo'] ?? 'Sem tipo',
    local: json['local'] ?? 'Local não informado',
    presencaObrigatoria: json['presencaObrigatoria'] == 'Sim',
  );
}
}
