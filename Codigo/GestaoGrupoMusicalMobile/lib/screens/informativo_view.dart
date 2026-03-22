import 'package:batala_mobile/model/informativo_model.dart';
import 'package:batala_mobile/service/informativo_service.dart';
import 'package:flutter/material.dart';

class InformativoView extends StatelessWidget {
  const InformativoView({super.key});

  @override
  Widget build(BuildContext context) {
    final service = InformativoService();

    // Retornamos apenas o FutureBuilder, sem Scaffold ou AppBar
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
            // Adicionamos um padding para o conteúdo não colar no topo
            padding: const EdgeInsets.only(top: 10, bottom: 100), 
            itemCount: informativos.length,
            itemBuilder: (context, index) => ListTile(
              leading: const Icon(Icons.info_outline, color: Color(0xFFD64550)),
              title: Text(
                informativos[index].mensagem,
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
            subtitle: Text(
              "Postado em: ${informativos[index].dataInicio.day.toString().padLeft
              (2, '0')}/${informativos[index].dataInicio.month.toString().padLeft
              (2, '0')}/${informativos[index].dataInicio.year} às ${informativos[index].dataInicio.hour.toString().padLeft
              (2, '0')}:${informativos[index].dataInicio.minute.toString().padLeft(2, '0')}",
            ),
            ),
          );
        }
        return const Center(child: Text("Nenhum informativo encontrado"));
      },
    );
  }
}