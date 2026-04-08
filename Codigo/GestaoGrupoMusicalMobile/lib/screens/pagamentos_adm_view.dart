import 'package:flutter/material.dart';
import 'package:batala_mobile/config/app_colors.dart';
import 'package:intl/intl.dart';
import '../model/financeiro_model.dart';
import '../service/financeiro_service.dart';

// ============================================================================
// 1. TELA DE CAMPANHAS DE PAGAMENTO (VISÃO GERAL DO ADMIN)
// ============================================================================
class PagamentosAdminView extends StatefulWidget {
  const PagamentosAdminView({super.key});

  @override
  State<PagamentosAdminView> createState() => _PagamentosAdminViewState();
}

class _PagamentosAdminViewState extends State<PagamentosAdminView> {
  final FinanceiroService _financeiroService = FinanceiroService();
  late Future<List<CampanhaPagamentoModel>> _futureCampanhas;

  @override
  void initState() {
    super.initState();
    _carregarCampanhas();
  }

  void _carregarCampanhas() {
    setState(() {
      _futureCampanhas = _financeiroService.getCampanhasAdmin();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Padding(
          padding: EdgeInsets.symmetric(horizontal: 20.0, vertical: 10.0),
          child: Text(
            "Gerenciar Pagamentos",
            style: TextStyle(fontSize: 22, fontWeight: FontWeight.normal, color: Color.fromARGB(204, 27, 27, 27)),
          ),
        ),
        Expanded(
          child: FutureBuilder<List<CampanhaPagamentoModel>>(
            future: _futureCampanhas,
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.waiting) {
                return const Center(child: CircularProgressIndicator(color: AppColors.primary));
              } else if (snapshot.hasError) {
                return Center(
                  child: Text("Erro ao carregar dados: ${snapshot.error}", textAlign: TextAlign.center),
                );
              } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
                return const Center(child: Text("Nenhuma campanha financeira encontrada."));
              }

              final campanhas = snapshot.data!;

              return ListView.builder(
                padding: const EdgeInsets.symmetric(horizontal: 16.0),
                itemCount: campanhas.length,
                itemBuilder: (context, index) {
                  final camp = campanhas[index];
                  return Card(
                    elevation: 3,
                    margin: const EdgeInsets.only(bottom: 16.0),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(15)),
                    child: Padding(
                      padding: const EdgeInsets.all(16.0),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(camp.descricao, style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
                          const SizedBox(height: 8),
                          Text(
                            "Período: ${DateFormat('dd/MM/yyyy').format(camp.inicio)} até ${DateFormat('dd/MM/yyyy').format(camp.fim)}",
                            style: const TextStyle(color: Colors.black54, fontSize: 13),
                          ),
                          const Divider(height: 24),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              _buildStatColumn("Pagos", camp.pagos.toString(), Colors.green),
                              _buildStatColumn("Atrasos", camp.atrasos.toString(), Colors.red),
                              _buildStatColumn("Isentos", camp.isentos.toString(), Colors.grey),
                              _buildStatColumn("Recebido", "R\$ ${camp.recebido.toStringAsFixed(2)}", AppColors.primary),
                            ],
                          ),
                          const SizedBox(height: 16),
                          SizedBox(
                            width: double.infinity,
                            child: ElevatedButton(
                              style: ElevatedButton.styleFrom(
                                backgroundColor: AppColors.secondary,
                                foregroundColor: Colors.white,
                                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                              ),
                              onPressed: () {
                                Navigator.push(
                                  context,
                                  MaterialPageRoute(
                                    builder: (context) => DetalhesPagamentoAdminView(campanha: camp),
                                  ),
                                );
                              },
                              child: const Text("Detalhes do Pagamento"),
                            ),
                          )
                        ],
                      ),
                    ),
                  );
                },
              );
            },
          ),
        ),
      ],
    );
  }

  Widget _buildStatColumn(String label, String value, Color color) {
    return Column(
      children: [
        Text(label, style: const TextStyle(fontSize: 12, color: Colors.black54)),
        const SizedBox(height: 4),
        Text(value, style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: color)),
      ],
    );
  }
}

// ============================================================================
// 2. TELA DE DETALHES (LISTA DE ASSOCIADOS DO PAGAMENTO)
// ============================================================================
class DetalhesPagamentoAdminView extends StatefulWidget {
  final CampanhaPagamentoModel campanha;

  const DetalhesPagamentoAdminView({super.key, required this.campanha});

  @override
  State<DetalhesPagamentoAdminView> createState() => _DetalhesPagamentoAdminViewState();
}

class _DetalhesPagamentoAdminViewState extends State<DetalhesPagamentoAdminView> {
  final FinanceiroService _financeiroService = FinanceiroService();
  late Future<List<AssociadoPagamentoModel>> _futureAssociados;

  @override
  void initState() {
    super.initState();
    // Inicia a requisição passando o ID da campanha escolhida
    _futureAssociados = _financeiroService.getAssociadosDoPagamento(widget.campanha.id);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.campanha.descricao, style: const TextStyle(color: Colors.white, fontSize: 18)),
        backgroundColor: AppColors.secondary,
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: FutureBuilder<List<AssociadoPagamentoModel>>(
        future: _futureAssociados,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator(color: AppColors.primary));
          } else if (snapshot.hasError) {
            return Center(child: Text("Erro: ${snapshot.error}"));
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(child: Text("Nenhum associado encontrado para este pagamento."));
          }

          final associados = snapshot.data!;

          return ListView.builder(
            padding: const EdgeInsets.all(16.0),
            itemCount: associados.length,
            itemBuilder: (context, index) {
              final assoc = associados[index];
              return Card(
                elevation: 2,
                margin: const EdgeInsets.only(bottom: 12.0),
                child: Padding(
                  padding: const EdgeInsets.all(12.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Expanded(
                            child: Text(
                              assoc.nomeAssociado,
                              style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                            ),
                          ),
                          _buildStatusChip(assoc.status),
                        ],
                      ),
                      const SizedBox(height: 8),
                      Text("CPF: ${assoc.cpf}", style: const TextStyle(color: Colors.black87, fontSize: 13)),
                      const SizedBox(height: 4),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Text(
                            "Data: ${assoc.dataPagamento != null ? DateFormat('dd/MM/yyyy').format(assoc.dataPagamento!) : 'Pendente'}",
                            style: const TextStyle(color: Colors.black54, fontSize: 13),
                          ),
                          Text(
                            "R\$ ${assoc.valorPago.toStringAsFixed(2)}",
                            style: const TextStyle(fontWeight: FontWeight.bold, color: Colors.black87),
                          ),
                        ],
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

  Widget _buildStatusChip(String status) {
    Color bgColor;
    Color textColor = Colors.white;

    switch (status.toUpperCase()) {
      case 'ABERTO':
      case 'NAO_PAGOU':
        bgColor = Colors.orange;
        status = "Pendente";
        break;
      case 'ENVIADO':
        bgColor = Colors.blue;
        break;
      case 'ISENTO':
        bgColor = Colors.grey;
        break;
      case 'PAGO':
      case 'PAGO_COMPROVANTE':
        bgColor = Colors.green;
        status = "Pago";
        break;
      default:
        bgColor = Colors.black54;
    }

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(color: bgColor, borderRadius: BorderRadius.circular(12)),
      child: Text(status, style: TextStyle(color: textColor, fontSize: 12, fontWeight: FontWeight.bold)),
    );
  }
}