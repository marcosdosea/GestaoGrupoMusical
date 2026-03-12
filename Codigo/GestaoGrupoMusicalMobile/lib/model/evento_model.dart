class EventoModel {
  final int id;
  final DateTime dataInicio;
  final DateTime dataFim;
  final String local;
  final String repertorio;

  EventoModel({
    required this.id,
    required this.dataInicio,
    required this.dataFim,
    required this.local,
    required this.repertorio,
  });

  factory EventoModel.fromJson(Map<String, dynamic> json) {
  return EventoModel(
    id: json['id'] ?? 0,
    dataInicio: json['dataHoraInicio'] != null 
        ? DateTime.parse(json['dataHoraInicio']) 
        : DateTime.now(), 
    dataFim: json['dataHoraFim'] != null 
        ? DateTime.parse(json['dataHoraFim']) 
        : DateTime.now(),
    local: json['local'] ?? 'Local não informado',
    repertorio: json['repertorio'] ?? 'Sem repertório',
  );
}
}
