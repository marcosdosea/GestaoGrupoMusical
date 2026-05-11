import 'package:flutter/material.dart';
import 'package:batala_mobile/config/app_colors.dart';
import 'package:intl/intl.dart'; // Certifique-se de ter o intl no pubspec.yaml
import '../model/ensaio_model.dart';
import '../model/evento_model.dart';
import '../service/evento_service.dart';
import '../service/ensaio_service.dart';

class HomeView extends StatefulWidget {
  const HomeView({super.key});

  @override
  State<HomeView> createState() => _HomeViewState();
}

class _HomeViewState extends State<HomeView> {
  late final EventoService _eventoService;
  late final EnsaioService _ensaioService;
  late Future<List<EventoModel>> _futureEventos;
  late Future<List<EnsaioModel>> _futureEnsaios;

  @override
  void initState() {
    super.initState();
    _eventoService = EventoService();
    _ensaioService = EnsaioService();
    _futureEventos = _eventoService.getAll();
    _futureEnsaios = _ensaioService.getAll();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Expanded(
          child: _buildSection<EventoModel>(
            "Próximos Eventos",
            _futureEventos,
            (item) => _buildEventCard(context, item, _eventoService),
          ),
        ),
        Expanded(
          child: _buildSection<EnsaioModel>(
            "Próximos Ensaios",
            _futureEnsaios,
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
      behavior: HitTestBehavior.opaque,
      onTap: () => _abrirDetalhes(item.id, service), // Apenas visualização
      child: _baseCard(
        title: item.local,
        date: item.dataInicio,
        actionWidget: EventToggleButton(
          isAccepted: false,
          onPressed: () => _abrirSelecaoInstrumento(item.id, service), // Ação de se inscrever
        ),
      ),
    );
  }

  // --- LÓGICA DE VISUALIZAÇÃO (APENAS INFO) ---
  Future<void> _abrirDetalhes(int id, EventoService service) async {
    _showLoading();
    try {
      final data = await service.getDetalhesEvento(id).timeout(const Duration(seconds: 8));
      if (!mounted) return;
      Navigator.of(context, rootNavigator: true).pop(); // Fecha loading

      if (data != null) {
        _mostrarModalInformativo(data);
      }
    } catch (e) {
      _handleError(e);
    }
  }

  void _mostrarModalInformativo(Map<String, dynamic> data) {
    final df = DateFormat('dd/MM/yyyy HH:mm');
    showModalBottomSheet(
      context: context,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(25))),
      builder: (context) => Padding(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(child: Container(width: 40, height: 4, decoration: BoxDecoration(color: Colors.grey[300], borderRadius: BorderRadius.circular(10)))),
            const SizedBox(height: 20),
            Text(data['repertorio'] ?? "Sem Repertório", style: const TextStyle(fontSize: 22, fontWeight: FontWeight.bold)),
            const SizedBox(height: 15),
            _infoRow(Icons.location_on, "Local", data['local']),
            _infoRow(Icons.calendar_today, "Início", data['dataHoraInicio'] != null ? df.format(DateTime.parse(data['dataHoraInicio'])) : "N/A"),
            _infoRow(Icons.event_busy, "Fim", data['dataHoraFim'] != null ? df.format(DateTime.parse(data['dataHoraFim'])) : "N/A"),
            const SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  // --- LÓGICA DE INSCRIÇÃO (ESCOLHER INSTRUMENTO) ---
  Future<void> _abrirSelecaoInstrumento(int id, EventoService service) async {
    _showLoading();
    try {
      final data = await service.getDetalhesEvento(id);
      if (!mounted) return;
      Navigator.of(context, rootNavigator: true).pop();

      if (data != null) {
        _mostrarModalInscricao(data, service);
      }
    } catch (e) {
      _handleError(e);
    }
  }

  void _mostrarModalInscricao(Map<String, dynamic> data, EventoService service) {
  // Extrai a lista com segurança
  final List instrumentos = data['instrumentosDisponiveis'] ?? [];

  showModalBottomSheet(
    context: context,
    isScrollControlled: true,
    shape: const RoundedRectangleBorder(
      borderRadius: BorderRadius.vertical(top: Radius.circular(25)),
    ),
    builder: (context) => Container(
      padding: const EdgeInsets.all(24),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            "Solicitar Participação",
            style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 10),
          
          // --- LÓGICA DE LISTA VAZIA ---
          if (instrumentos.isEmpty)
            Padding(
              padding: const EdgeInsets.symmetric(vertical: 30),
              child: Center(
                child: Column(
                  children: [
                    Icon(Icons.info_outline, size: 50, color: Colors.grey[400]),
                    const SizedBox(height: 16),
                    Text(
                      "Ops! Não há instrumentos disponíveis para este evento no momento.",
                      textAlign: TextAlign.center,
                      style: TextStyle(color: Colors.grey[600], fontSize: 16),
                    ),
                    const SizedBox(height: 8),
                    const Text(
                      "Fique atento aos próximos informativos!",
                      style: TextStyle(color: Colors.grey, fontSize: 14),
                    ),
                  ],
                ),
              ),
            )
          else ...[
            const Text("Selecione o instrumento que você irá tocar:"),
            const Divider(height: 30),
            ...instrumentos.map((inst) => ListTile(
                  leading: const Icon(Icons.music_note, color: AppColors.primary),
                  title: Text(inst['nomeInstrumento']),
                  subtitle: Text("${inst['vagasDisponiveis']} vagas restantes"),
                  trailing: const Icon(Icons.chevron_right),
                  onTap: () async {
                    Navigator.pop(context); // Fecha o modal de escolha
                    _processarInscricao(data['id'], inst['idInstrumento'], inst['nomeInstrumento'], service);
                  },
                )),
          ],
          const SizedBox(height: 10),
        ],
      ),
    ),
  );
}

// Nova função para gerenciar o feedback da inscrição
Future<void> _processarInscricao(int eventoId, int instId, String nomeInst, EventoService service) async {
  _showLoading(); // Mostra um loading rápido enquanto envia para a API

  try {
    final erro = await service.solicitarParticipacao(eventoId, instId);
    
    if (!mounted) return;
    Navigator.of(context, rootNavigator: true).pop(); // Fecha o loading

    if (erro == null) {
      // --- FEEDBACK DE SUCESSO ---
      showDialog(
        context: context,
        builder: (context) => AlertDialog(
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
          title: const Icon(Icons.check_circle, color: Colors.green, size: 60),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Text(
                "Solicitação Enviada!",
                style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
              ),
              const SizedBox(height: 10),
              Text(
                "Sua participação com o instrumento $nomeInst foi solicitada com sucesso. Agora é só aguardar a aprovação do regente!",
                textAlign: TextAlign.center,
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text("OK", style: TextStyle(fontWeight: FontWeight.bold)),
            ),
          ],
        ),
      );
    } else {
      // Exibe erro retornado pela API
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(erro), backgroundColor: Colors.red),
      );
    }
  } catch (e) {
    if (mounted) Navigator.of(context, rootNavigator: true).pop();
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text("Erro ao enviar solicitação."), backgroundColor: Colors.red),
    );
  }
}
  // --- WIDGETS AUXILIARES ---
  Widget _infoRow(IconData icon, String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6.0),
      child: Row(children: [
        Icon(icon, size: 20, color: AppColors.primary),
        const SizedBox(width: 10),
        Text("$label: ", style: const TextStyle(fontWeight: FontWeight.bold)),
        Expanded(child: Text(value, overflow: TextOverflow.ellipsis)),
      ]),
    );
  }

  void _showLoading() {
    showDialog(context: context, barrierDismissible: false, builder: (_) => const Center(child: CircularProgressIndicator()));
  }

  void _handleError(Object e) {
    if (mounted) {
      Navigator.of(context, rootNavigator: true).pop();
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text("Erro: $e")));
    }
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
            Text("📅 ${date.day}/${date.month} às ${date.hour}:${date.minute.toString().padLeft(2, '0')}") ,
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
  final VoidCallback onPressed; // Adicionado callback
  const EventToggleButton({super.key, required this.isAccepted, required this.onPressed});

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onPressed,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        decoration: BoxDecoration(
          color: isAccepted ? Colors.grey : const Color(0xFFD9534F),
          borderRadius: BorderRadius.circular(10),
        ),
        child: Text(isAccepted ? "CANCELAR" : "ACEITAR", style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
      ),
    );
  }
}