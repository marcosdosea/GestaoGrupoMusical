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
      // Tenta minúsculo, se for nulo tenta maiúsculo, se não 0
      id: json['id'] ?? json['Id'] ?? 0,
      dataInicio: DateTime.parse(json['dataHoraInicio'] ?? json['DataHoraInicio'] ?? DateTime.now().toIso8601String()),
      dataFim: DateTime.parse(json['dataHoraFim'] ?? json['DataHoraFim'] ?? DateTime.now().toIso8601String()),
      local: json['local'] ?? json['Local'] ?? 'Local não informado',
      repertorio: json['repertorio'] ?? json['Repertorio'] ?? 'Sem repertório',
    );
  }
}
