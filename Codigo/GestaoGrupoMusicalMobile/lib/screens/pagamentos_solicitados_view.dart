import 'package:flutter/material.dart';
import '../config/app_colors.dart';
import '../model/financeiro_model.dart';
import '../service/financeiro_service.dart';
import 'financeiro_view.dart'; 

class PagamentosSolicitadosView extends StatelessWidget {
  const PagamentosSolicitadosView({super.key});

  @override
  Widget build(BuildContext context) {
    final service = FinanceiroService();

    return Scaffold(
      appBar: AppBar(
        title: const Text("Pagamentos Solicitados"),
        backgroundColor: AppColors.secondary,
        foregroundColor: Colors.white,
      ),
      body: FutureBuilder<List<FinanceiroModel>>(
        future: service.getAll(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError) {
            return Center(child: Text("Erro ao carregar pagamentos: ${snapshot.error}"));
          }

          final lista = snapshot.data ?? [];

          if (lista.isEmpty) {
            return const Center(child: Text("Nenhum pagamento pendente no momento."));
          }

          return ListView.builder(
            padding: const EdgeInsets.only(top: 10, bottom: 20),
            itemCount: lista.length,
            itemBuilder: (context, index) {
              final item = lista[index];
              
              return Card(
                margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                child: ListTile(
                  isThreeLine: true,
                  title: Text(
                    item.descricao, 
                    style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
                    softWrap: true,
                    maxLines: 2,
                    overflow: TextOverflow.visible,
                  ),
                  subtitle: Padding(
                    padding: const EdgeInsets.only(top: 4.0),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text("Vence em: ${item.dataFim.day.toString().padLeft(2, '0')}/${item.dataFim.month.toString().padLeft(2, '0')}"),
                        const SizedBox(height: 2),
                        // Removida a verificação de null, imprimindo o int diretamente
                        Text(
                          "Status: ${item.statusPagamento}", 
                          style: TextStyle(
                            fontWeight: FontWeight.w600,
                            color: item.statusPagamento == 'Pago' 
                                ? Colors.green 
                                : item.statusPagamento == 'Atrasado' 
                                    ? Colors.red 
                                    : Colors.orange[800], // Para 'Pendente'
                          ),
                        ),
                      ],
                    ),
                  ),
                  trailing: ElevatedButton(
                    style: ElevatedButton.styleFrom(backgroundColor: AppColors.primary),
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => FinanceiroView(solicitacao: item)
                        ),
                      );
                    },
                    child: const Text("PAGAR", style: TextStyle(color: Colors.white)),
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }
}