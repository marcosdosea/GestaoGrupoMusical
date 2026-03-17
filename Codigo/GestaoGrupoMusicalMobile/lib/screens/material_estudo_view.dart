import 'package:batala_mobile/model/material_estudo_model.dart';
import 'package:batala_mobile/service/material_estudo_service.dart';
import 'package:flutter/material.dart';

class MaterialEstudoView extends StatelessWidget {
  const MaterialEstudoView({super.key});

  @override
  Widget build(BuildContext context) {
    final service = MaterialestudoService();

    return FutureBuilder<List<MaterialestudoModel>>(
      future: service.getAll(),
      builder: (context, snapshot) {
        // 1. Tratamento de Carregamento
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        } 
        
        // 2. Tratamento de Erro
        else if (snapshot.hasError) {
          return Center(child: Text("Erro ao carregar materiais: ${snapshot.error}"));
        } 
        
        // 3. Verificação de Dados
        else if (snapshot.hasData && snapshot.data!.isNotEmpty) {
          final materiais = snapshot.data!;

          return ListView.builder(
            // Padding para não ficar colado na barra flutuante
            padding: const EdgeInsets.only(top: 10, bottom: 100, left: 10, right: 10),
            itemCount: materiais.length,
            itemBuilder: (context, index) {
              final item = materiais[index];

              return Card(
                elevation: 2,
                margin: const EdgeInsets.symmetric(vertical: 8),
                child: ListTile(
                  leading: const Icon(Icons.menu_book, color: Color(0xFFD64550)),
                  title: Text(
                    item.nome,
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                  subtitle: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const SizedBox(height: 4),
                      Text(
                        item.link,
                        style: const TextStyle(color: Colors.blue, decoration: TextDecoration.underline),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        "Adicionado em: ${item.dataInicio.day}/${item.dataInicio.month}/${item.dataInicio.year}",
                        style: TextStyle(color: Colors.grey[600], fontSize: 12),
                      ),
                    ],
                  ),
                  isThreeLine: true,
                  onTap: () {
                    // Aqui você pode implementar a lógica para abrir o link
                  },
                ),
              );
            },
          );
        }

        // 4. Caso a lista esteja vazia
        return const Center(child: Text("Nenhum material de estudo disponível."));
      },
    );
  }
}