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
              
              // 🔥 1. Normaliza a string para caixa alta (ignora maiúsculas/minúsculas)
              String statusRaw = item.statusPagamento.toUpperCase().trim();
              
              Color corStatus;
              bool isPendente = false; // Flag para controlar se é pendente/aberto

              // 🔥 2. Analisa o status com segurança
              if (statusRaw.contains('ISENTO')) {
                corStatus = Colors.blue;
                isPendente = false;
              } else if (statusRaw.contains('ENVIADO')) {
                // Enviado (comprovante em análise) - ainda é pendente
                corStatus = Colors.orange; 
                isPendente = true; // Mostra botão pois está aguardando
              } else if (statusRaw == 'PAGO' || statusRaw.endsWith('PAGO')) {
                // Apenas PAGO, não NAO_PAGOU
                corStatus = Colors.green;
                isPendente = false;
              } else {
                // ABERTO, NAO_PAGOU ou qualquer outro status não previsto
                corStatus = Colors.red[700]!;
                isPendente = true; // Mostra botão
              }

              // Deixa a primeira letra maiúscula para ficar bonito na tela (Ex: "Aberto", "Enviado")
              String textoExibicao = item.statusPagamento.isNotEmpty 
                  ? item.statusPagamento[0].toUpperCase() + item.statusPagamento.substring(1).toLowerCase()
                  : "Desconhecido";

              return Card(
                margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                child: Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Row(
                    children: [
                      Expanded(
                        child: ListTile(
                          isThreeLine: true,
                          title: Text(
                            item.descricao, 
                            style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
                            softWrap: true,
                            maxLines: null,
                            overflow: TextOverflow.visible,
                          ),
                          subtitle: Padding(
                            padding: const EdgeInsets.only(top: 4.0),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text("Vence em: ${item.dataFim.day.toString().padLeft(2, '0')}/${item.dataFim.month.toString().padLeft(2, '0')}"),
                                const SizedBox(height: 2),
                                // 🔥 3. Aplica a cor dinâmica e o texto formatado
                                Text(
                                  "Status: $textoExibicao", 
                                  style: TextStyle(
                                    fontWeight: FontWeight.w600,
                                    color: corStatus, 
                                  ),
                                ),
                              ],
                            ),
                          ),
                          contentPadding: EdgeInsets.zero,
                        ),
                      ),
                      // 🔥 4. Botão visível apenas para ABERTO (isPendente)
                      if (isPendente)
                        ElevatedButton(
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppColors.primary,
                            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                          ),
                          onPressed: () {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) => FinanceiroView(solicitacao: item)
                              ),
                            );
                          },
                          child: const Text(
                            "VER", 
                            style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)
                          ),
                        ),
                    ],
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