import 'package:flutter/material.dart';
import 'package:batala_mobile/config/app_colors.dart';
import 'package:intl/intl.dart';
import 'dart:math' as math;
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

  // Controladores de scroll e limites de paginação
  final ScrollController _eventoScrollController = ScrollController();
  final ScrollController _ensaioScrollController = ScrollController();
  
  int _eventosLimit = 3;
  int _ensaiosLimit = 3;
  final int _incrementoPaginacao = 3;

  // NOVO: Cache local em memória para os detalhes dos eventos
  final Map<int, Map<String, dynamic>> _detalhesCache = {};

  @override
  void initState() {
    super.initState();
    _eventoService = EventoService();
    _ensaioService = EnsaioService();
    _carregarDados();

    // Listeners para ativar a paginação ao "esticar" o final da lista
    _eventoScrollController.addListener(_onEventoScroll);
    _ensaioScrollController.addListener(_onEnsaioScroll);
  }

  @override
  void dispose() {
    _eventoScrollController.dispose();
    _ensaioScrollController.dispose();
    super.dispose();
  }

  void _carregarDados() {
    _futureEventos = _eventoService.getAll();
    _futureEnsaios = _ensaioService.getAll();
  }

  Future<void> _onRefresh() async {
    setState(() {
      _eventosLimit = 3;
      _ensaiosLimit = 3;
      _detalhesCache.clear(); // Limpa o cache ao recarregar a tela
      _carregarDados();
    });
  }

  void _onEventoScroll() {
    if (_eventoScrollController.offset >= _eventoScrollController.position.maxScrollExtent) {
      setState(() {
        _eventosLimit += _incrementoPaginacao;
      });
    }
  }

  void _onEnsaioScroll() {
    if (_ensaioScrollController.offset >= _ensaioScrollController.position.maxScrollExtent) {
      setState(() {
        _ensaiosLimit += _incrementoPaginacao;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return RefreshIndicator(
      onRefresh: _onRefresh,
      child: Column(
        children: [
          Expanded(
            child: _buildSection<EventoModel>(
              "Próximos Eventos",
              _futureEventos,
              _eventoScrollController,
              _eventosLimit,
              (item) => _buildEventCard(context, item, _eventoService),
            ),
          ),
          Expanded(
            child: _buildSection<EnsaioModel>(
              "Próximos Ensaios",
              _futureEnsaios,
              _ensaioScrollController,
              _ensaiosLimit,
              (item) => _buildEnsaioCard(item),
            ),
          ),
          const SizedBox(height: 8),
        ],
      ),
    );
  }

  Widget _buildSection<T>(
      String title, 
      Future<List<T>> future, 
      ScrollController controller, 
      int limit, 
      Widget Function(T) builder) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(20, 16, 20, 8),
          child: Text(title, style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.black87)),
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

              final int itemCount = math.min(list.length, limit);

              return ListView.builder(
                controller: controller,
                physics: const BouncingScrollPhysics(parent: AlwaysScrollableScrollPhysics()), 
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
                itemCount: itemCount,
                itemBuilder: (context, index) {
                  return builder(list[index]);
                },
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
      onTap: () => _abrirDetalhes(item.id, service),
      child: _baseCard(
        title: item.local,
        date: item.dataInicio,
        actionWidget: EventToggleButton(
          isAccepted: false,
          onPressed: () => _abrirSelecaoInstrumento(item.id, service),
        ),
      ),
    );
  }

  Future<void> _abrirDetalhes(int id, EventoService service) async {
    // 1. Verifica se os dados já estão no cache em memória
    if (_detalhesCache.containsKey(id)) {
      _mostrarModalInformativo(_detalhesCache[id]!);
      return;
    }

    // 2. Se não estiver no cache, chama a API
    _showLoading();
    try {
      final data = await service.getDetalhesEvento(id).timeout(const Duration(seconds: 8));
      if (!mounted) return;
      Navigator.of(context, rootNavigator: true).pop();

      if (data != null) {
        _detalhesCache[id] = data; // 3. Salva a resposta no cache para a próxima vez
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

  Future<void> _abrirSelecaoInstrumento(int id, EventoService service) async {
    // 1. Verifica se os dados já estão no cache em memória
    if (_detalhesCache.containsKey(id)) {
      _mostrarModalInscricao(_detalhesCache[id]!, service);
      return;
    }

    // 2. Se não estiver no cache, chama a API
    _showLoading();
    try {
      final data = await service.getDetalhesEvento(id);
      if (!mounted) return;
      Navigator.of(context, rootNavigator: true).pop();

      if (data != null) {
        _detalhesCache[id] = data; // 3. Salva a resposta no cache
        _mostrarModalInscricao(data, service);
      }
    } catch (e) {
      _handleError(e);
    }
  }

  void _mostrarModalInscricao(Map<String, dynamic> data, EventoService service) {
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
                      Navigator.pop(context);
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

  Future<void> _processarInscricao(int eventoId, int instId, String nomeInst, EventoService service) async {
    _showLoading();

    try {
      final erro = await service.solicitarParticipacao(eventoId, instId);
      
      if (!mounted) return;
      Navigator.of(context, rootNavigator: true).pop();

      if (erro == null) {
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
      margin: const EdgeInsets.only(bottom: 5),
      color: AppColors.cardBackground,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(15)),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.center, 
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                  const SizedBox(height: 4),
                  Text("📅 ${date.day}/${date.month} às ${date.hour}:${date.minute.toString().padLeft(2, '0')}"),
                  if (subtitle != null) ...[
                    const SizedBox(height: 2),
                    Text(subtitle, style: const TextStyle(fontSize: 13, color: Colors.black54)),
                  ]
                ],
              ),
            ),
            if (actionWidget != null) ...[
              const SizedBox(width: 8), 
              actionWidget,
            ]
          ],
        ),
      ),
    );
  }
} 

class EventToggleButton extends StatelessWidget {
  final bool isAccepted;
  final VoidCallback onPressed;
  
  const EventToggleButton({super.key, required this.isAccepted, required this.onPressed});

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: onPressed,
      style: ElevatedButton.styleFrom(
        backgroundColor: isAccepted ? Colors.grey : const Color(0xFFD9534F), 
        foregroundColor: Colors.white, 
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
        minimumSize: Size.zero, 
        tapTargetSize: MaterialTapTargetSize.shrinkWrap, 
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
        ),
        elevation: 0, 
      ),
      child: Text(
        isAccepted ? "CANCELAR" : "ACEITAR", 
        style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 12),
      ),
    );
  }
}