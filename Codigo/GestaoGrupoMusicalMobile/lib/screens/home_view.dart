import 'package:flutter/material.dart';
import 'package:batala_mobile/config/app_colors.dart';
import '../model/ensaio_model.dart';
import '../model/evento_model.dart';
import '../service/evento_service.dart';
import '../service/ensaio_service.dart';

class HomeView extends StatelessWidget {
  const HomeView({super.key});

  @override
  Widget build(BuildContext context) {
    final eventoService = EventoService();
    final ensaioService = EnsaioService();

    return Column(
      children: [
        Expanded(
          child: _buildSection<EventoModel>(
            "Próximos Eventos",
            eventoService.getAll(),
            (item) => _buildEventCard(context, item, eventoService),
          ),
        ),
        Expanded(
          child: _buildSection<EnsaioModel>(
            "Próximos Ensaios",
            ensaioService.getAll(),
            (item) => _buildEnsaioCard(item),
          ),
        ),
        const SizedBox(height: 16),
      ],
    );
  }

  Widget _buildSection<T>(String title, Future<List<T>> future, Widget Function(T) builder) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(20, 16, 20, 8),
          child: Text(title, style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold, color: Colors.black87)),
        ),
        Expanded(
          child: FutureBuilder<List<T>>(
            future: future,
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.waiting) {
                return const Center(child: CircularProgressIndicator());
              }
              if (snapshot.hasError) return const Center(child: Text("Erro ao carregar"));
              final list = snapshot.data ?? [];
              if (list.isEmpty) return const Center(child: Text("Nada agendado."));
              
              return ListView.builder(
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
                itemCount: list.length,
                itemBuilder: (context, index) => builder(list[index]),
              );
            },
          ),
        )
      ],
    );
  }

  Widget _buildEventCard(BuildContext context, EventoModel item, EventoService service) {
    return GestureDetector(
      onTap: () => _abrirDetalhes(context, item.id, service),
      child: _baseCard(
        title: item.local,
        date: item.dataInicio,
        subtitle: "Toque para ver detalhes",
        actionWidget: EventToggleButton(isAccepted: false), // Estado simplificado
      ),
    );
  }
Future<void> _abrirDetalhes(BuildContext context, int id, EventoService service) async {
    // 1. Abre o loading
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => const Center(child: CircularProgressIndicator()),
    );

    try {
    final detalhes = await service.getDetalhesEvento(id);

    // Verificação de segurança crucial após o await: o usuário ainda está na tela?
    if (!context.mounted) return;

    // 2. Fecha o loading (o showDialog acima)
    // Usamos o Navigator do contexto atual para garantir que fechamos o Dialog
    Navigator.pop(context);

    if (detalhes != null) {
      // 3. Pequeno delay para evitar conflito de animações entre o Dialog fechando e o Modal subindo
      Future.delayed(const Duration(milliseconds: 100), () {
        if (context.mounted) {
          _mostrarModalDetalhes(context, detalhes, service);
        }
      });
    }
  } catch (e) {
    // Caso ocorra erro, fechamos o loading se ele ainda estiver aberto
    if (context.mounted) {
      Navigator.of(context).pop();
    }
    debugPrint("Erro ao abrir detalhes: $e");
  }
}

  void _mostrarModalDetalhes(BuildContext context, Map<String, dynamic> data, EventoService service) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent, // Permite cantos arredondados personalizados
      builder: (context) => Container(
        decoration: const BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
        ),
        padding: EdgeInsets.only(
          bottom: MediaQuery.of(context).viewInsets.bottom + 24, // Ajuste para teclado
          left: 24,
          right: 24,
          top: 24,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(
              child: Container(
                width: 40,
                height: 4,
                // Correto
                margin: const EdgeInsets.only(bottom: 20),
                decoration: BoxDecoration(color: Colors.grey[300], borderRadius: BorderRadius.circular(10)),
              ),
            ),
            Text(
              data['repertorio'] ?? "Detalhes do Evento",
              style: const TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            Text("📍 Local: ${data['local']}", style: const TextStyle(fontSize: 16)),
            const Divider(height: 32),
            const Text("Escolha seu instrumento:", style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
            const SizedBox(height: 12),
            // Tratamento seguro da lista de instrumentos
            if (data['instrumentosDisponiveis'] != null)
              ...((data['instrumentosDisponiveis'] as List).map((inst) => ListTile(
                    contentPadding: EdgeInsets.zero,
                    leading: const Icon(Icons.music_note, color: AppColors.primary),
                    title: Text(inst['nomeInstrumento'] ?? "Instrumento"),
                    subtitle: Text("${inst['vagasDisponiveis'] ?? 0} vagas"),
                    onTap: () {
                      Navigator.pop(context);
                      // Aqui você chama o service.solicitarParticipacao
                    },
                  ))),
            const SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  Widget _buildEnsaioCard(EnsaioModel item) {
    return _baseCard(title: item.local, date: item.dataHora, subtitle: "Ensaio Geral");
  }

  Widget _baseCard({required String title, required DateTime date, String? subtitle, Widget? actionWidget}) {
    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      color: AppColors.cardBackground,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(title, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 18)),
            const SizedBox(height: 8),
            Text("📅 ${date.day}/${date.month} às ${date.hour}:${date.minute.toString().padLeft(2, '0')}"),
            if (subtitle != null) Text("\n$subtitle", style: const TextStyle(fontSize: 14, color: Colors.black54)),
            if (actionWidget != null) Align(alignment: Alignment.centerRight, child: actionWidget),
          ],
        ),
      ),
    );
  }
}

class EventToggleButton extends StatelessWidget {
  final bool isAccepted;
  const EventToggleButton({super.key, required this.isAccepted});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      decoration: BoxDecoration(
        color: isAccepted ? Colors.grey : const Color(0xFFD9534F),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Text(isAccepted ? "CANCELAR" : "ACEITAR", style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
    );
  }
}