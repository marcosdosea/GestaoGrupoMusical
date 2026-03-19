import 'package:batala_mobile/model/informativo_model.dart';
import 'package:batala_mobile/service/informativo_service.dart';
import 'package:flutter/material.dart';

class InformativoView extends StatelessWidget {
  const InformativoView({super.key});

  String _formatarData(String dataString) {
    try {
      final data = DateTime.parse(dataString);
      final dia = data.day.toString().padLeft(2, '0');
      final mes = data.month.toString().padLeft(2, '0');
      final ano = data.year;
      final hora = data.hour.toString().padLeft(2, '0');
      final minuto = data.minute.toString().padLeft(2, '0');
      return "$dia/$mes/$ano às $hora:$minuto";
    } catch (e) {
      return dataString; 
    }
  }

  @override
  Widget build(BuildContext context) {
    final service = InformativoService();

    return FutureBuilder<List<InformativoModel>>(
      future: service.getAll(),
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        } else if (snapshot.hasError) {
          return Center(child: Text("Erro: ${snapshot.error}"));
        } else if (snapshot.hasData) {
          final informativos = snapshot.data!;
          return ListView.builder(
            padding: const EdgeInsets.only(top: 10, bottom: 100), 
            itemCount: informativos.length,
            itemBuilder: (context, index) => ListTile(
              leading: const Icon(Icons.info_outline, color: Color(0xFFD64550)),
              title: Text(
                informativos[index].mensagem,
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
              subtitle: Text("Postado em: ${_formatarData(informativos[index].dataInicio.toString())}"),
            ),
          );
        }
        return const Center(child: Text("Nenhum informativo encontrado"));
      },
    );
  }
}