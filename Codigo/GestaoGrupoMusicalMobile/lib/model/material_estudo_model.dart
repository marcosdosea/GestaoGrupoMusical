class MaterialestudoModel {
  final int id;
  final String nome;
  final String link; 
  final DateTime dataInicio;


  MaterialestudoModel({
    required this.id,
    required this.nome,
    required this.link,
    required this.dataInicio,

  });

  factory MaterialestudoModel.fromJson(Map<String, dynamic> json) {
  return MaterialestudoModel(
    id: json['id'] ?? 0,
    nome: json['nome']?? 'Não informado',
    link: json['link']?? 'Não informado',
    dataInicio: json['dataHoraInicio'] != null 
        ? DateTime.parse(json['dataHoraInicio']) 
        : DateTime.now(), 
  );
}
}
