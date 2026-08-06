import 'dart:math' as math;

import 'package:batala_mobile/config/app_colors.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../model/ensaio_model.dart';
import '../model/evento_model.dart';
import '../service/ensaio_service.dart';
import '../service/evento_service.dart';

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

  final ScrollController _eventoScrollController = ScrollController();
  final ScrollController _ensaioScrollController = ScrollController();

  int _eventosLimit = 3;
  int _ensaiosLimit = 3;
  final int _incrementoPaginacao = 3;

  final Map<int, Map<String, dynamic>> _detalhesCache = {};
  final Map<int, Future<Map<String, dynamic>?>> _estadoEventos = {};

  @override
  void initState() {
    super.initState();
    _eventoService = EventoService();
    _ensaioService = EnsaioService();
    _carregarDados();

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
    await _eventoService.limparCacheListagem();
    await _ensaioService.limparCacheListagem();

    if (!mounted) return;
    setState(() {
      _eventosLimit = 3;
      _ensaiosLimit = 3;
      _detalhesCache.clear();
      _estadoEventos.clear();
      _carregarDados();
    });
  }

  void _onEventoScroll() {
    if (!_eventoScrollController.hasClients) return;
    if (_eventoScrollController.offset >=
        _eventoScrollController.position.maxScrollExtent) {
      setState(() => _eventosLimit += _incrementoPaginacao);
    }
  }

  void _onEnsaioScroll() {
    if (!_ensaioScrollController.hasClients) return;
    if (_ensaioScrollController.offset >=
        _ensaioScrollController.position.maxScrollExtent) {
      setState(() => _ensaiosLimit += _incrementoPaginacao);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
          colors: [Color(0xFFFDF7F7), Color(0xFFF7F7F7)],
        ),
      ),
      child: RefreshIndicator(
        color: AppColors.primary,
        onRefresh: _onRefresh,
        child: Column(
          children: [
            Expanded(
              child: _buildSection<EventoModel>(
                title: 'Próximos Eventos',
                icon: Icons.event_note_outlined,
                future: _futureEventos,
                controller: _eventoScrollController,
                limit: _eventosLimit,
                builder: (item) => _buildEventCard(context, item, _eventoService),
              ),
            ),
            Expanded(
              child: _buildSection<EnsaioModel>(
                title: 'Próximos Ensaios',
                icon: Icons.inbox_outlined,
                future: _futureEnsaios,
                controller: _ensaioScrollController,
                limit: _ensaiosLimit,
                builder: (item) => _buildEnsaioCard(item),
              ),
            ),
            const SizedBox(height: 8),
          ],
        ),
      ),
    );
  }

  Widget _buildSection<T>({
    required String title,
    required IconData icon,
    required Future<List<T>> future,
    required ScrollController controller,
    required int limit,
    required Widget Function(T) builder,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildSectionHeader(title, icon),
        Expanded(
          child: FutureBuilder<List<T>>(
            future: future,
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.waiting) {
                return const Center(child: CircularProgressIndicator());
              }
              if (snapshot.hasError) {
                return const Center(child: Text('Erro ao carregar'));
              }

              final list = snapshot.data ?? [];
              if (list.isEmpty) {
                return const Center(child: Text('Nada agendado.'));
              }

              final itemCount = math.min(list.length, limit);
              return ListView.builder(
                controller: controller,
                physics: const BouncingScrollPhysics(
                  parent: AlwaysScrollableScrollPhysics(),
                ),
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
                itemCount: itemCount,
                itemBuilder: (context, index) => builder(list[index]),
              );
            },
          ),
        ),
      ],
    );
  }

  Widget _buildEventCard(
    BuildContext context,
    EventoModel item,
    EventoService service,
  ) {
    return GestureDetector(
      behavior: HitTestBehavior.opaque,
      onTap: () => _abrirDetalhes(item.id, service),
      child: _baseCard(
        title: item.local,
        date: item.dataInicio,
        actionWidget: FutureBuilder<Map<String, dynamic>?>(
          future: _obterEstadoEvento(item.id),
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return const SizedBox(
                width: 24,
                height: 24,
                child: CircularProgressIndicator(strokeWidth: 2),
              );
            }

            final inscricao =
                snapshot.data?['minhaInscricao'] ?? snapshot.data?['inscricao'];
            final status = (inscricao?['status'] as String? ?? '').toUpperCase();

            if (status == 'DEFERIDO') {
              return _ActionIconButton(
                icon: Icons.calendar_month_outlined,
                tooltip: 'Justificar ausência',
                onPressed: () => _abrirModalJustificativa(
                  id: item.id,
                  isEnsaio: false,
                ),
              );
            }

            if (status == 'INSCRITO') {
              return const _StatusParticipacao(
                texto: 'AGUARDANDO',
                icone: Icons.hourglass_top,
                cor: Colors.orange,
              );
            }

            if (status == 'INDEFERIDO') {
              return const _StatusParticipacao(
                texto: 'NÃO APROVADO',
                icone: Icons.cancel_outlined,
                cor: Colors.red,
              );
            }

            return EventToggleButton(
              isAccepted: false,
              onPressed: () => _abrirSelecaoInstrumento(item.id, service),
            );
          },
        ),
      ),
    );
  }

  Widget _buildEnsaioCard(EnsaioModel item) {
    return _baseCard(
      title: item.local,
      date: item.dataHora,
      subtitle: 'Ensaio Geral',
      actionWidget: _ActionIconButton(
        icon: Icons.calendar_month_outlined,
        tooltip: 'Justificar ausência',
        onPressed: () => _abrirModalJustificativa(id: item.id, isEnsaio: true),
      ),
    );
  }

  Future<Map<String, dynamic>?> _obterEstadoEvento(int idEvento) {
    return _estadoEventos.putIfAbsent(
      idEvento,
      () => _eventoService.getDetalhesEvento(idEvento),
    );
  }

  Future<void> _atualizarEstadoEvento(int idEvento) async {
    await _eventoService.limparCacheListagem();
    if (!mounted) return;

    setState(() {
      _detalhesCache.remove(idEvento);
      _estadoEventos.remove(idEvento);
    });
  }

  Future<void> _abrirModalJustificativa({
    required int id,
    required bool isEnsaio,
  }) async {
    final controller = TextEditingController();

    _showLoading();
    try {
      if (isEnsaio) {
        final dados = await _ensaioService.getMinhaFrequencia(id);
        if (dados != null) {
          controller.text = dados['justificativa'] ?? '';
        }
      } else {
        final dados = await _eventoService.getDetalhesEvento(id);
        if (dados != null) {
          final minhaInscricao = dados['minhaInscricao'] ?? dados['inscricao'];
          if (minhaInscricao != null) {
            controller.text = minhaInscricao['justificativa'] ??
                minhaInscricao['Justificativa'] ??
                '';
          }
        }
      }
    } catch (_) {}

    if (mounted) {
      Navigator.of(context, rootNavigator: true).pop();
    }
    if (!mounted) return;

    showDialog(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: Text(
          isEnsaio ? 'Justificar Ausência (Ensaio)' : 'Justificar Ausência (Evento)',
        ),
        content: TextField(
          controller: controller,
          maxLines: 4,
          decoration: const InputDecoration(
            hintText: 'Escreva o motivo da sua ausência...',
            border: OutlineInputBorder(),
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(dialogContext),
            child: const Text('Cancelar'),
          ),
          ElevatedButton(
            onPressed: () async {
              final justificativa = controller.text.trim();
              if (justificativa.isEmpty) {
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text('Informe o motivo da ausência.')),
                );
                return;
              }

              _showLoading();
              final mensagemErro = isEnsaio
                  ? await _ensaioService.justificarAusencia(id, justificativa)
                  : await _eventoService.justificarAusencia(id, justificativa);

              final sucesso = mensagemErro == null;

              if (!mounted) return;
              Navigator.of(context, rootNavigator: true).pop();
              Navigator.of(context, rootNavigator: true).pop();

              if (sucesso && !isEnsaio) {
                await _atualizarEstadoEvento(id);
              }

              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(
                  content: Text(
                    sucesso ? 'Justificativa enviada com sucesso!' : mensagemErro!,
                  ),
                  backgroundColor: sucesso ? Colors.green : Colors.red,
                ),
              );
            },
            child: const Text('Enviar'),
          ),
        ],
      ),
    );
  }

  Future<void> _abrirDetalhes(int id, EventoService service) async {
    if (_detalhesCache.containsKey(id)) {
      _mostrarModalInformativo(_detalhesCache[id]!);
      return;
    }

    _showLoading();
    try {
      final data = await service.getDetalhesEvento(id).timeout(const Duration(seconds: 8));
      if (!mounted) return;
      Navigator.of(context, rootNavigator: true).pop();

      if (data != null) {
        _detalhesCache[id] = data;
        _mostrarModalInformativo(data);
      }
    } catch (e) {
      _handleError(e);
    }
  }

  void _mostrarModalInformativo(Map<String, dynamic> data) {
    final df = DateFormat('dd/MM/yyyy HH:mm');

    DateTime? parseDate(dynamic value) {
      if (value == null) return null;
      if (value is DateTime) return value;
      if (value is String && value.isNotEmpty) {
        return DateTime.tryParse(value);
      }
      return null;
    }

    final inicio = parseDate(data['dataHoraInicio'] ?? data['dataInicio']);
    final fim = parseDate(data['dataHoraFim'] ?? data['dataFim']);

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(25)),
      ),
      builder: (context) => Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(
              child: Container(
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: Colors.grey[300],
                  borderRadius: BorderRadius.circular(10),
                ),
              ),
            ),
            const SizedBox(height: 20),
            Text(
              data['repertorio'] ?? 'Sem Repertório',
              style: const TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 15),
            _infoRow(Icons.location_on, 'Local', data['local']?.toString() ?? 'N/A'),
            _infoRow(
              Icons.calendar_today,
              'Início',
              inicio != null ? df.format(inicio) : 'N/A',
            ),
            _infoRow(
              Icons.event_busy,
              'Fim',
              fim != null ? df.format(fim) : 'N/A',
            ),
            const SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  Future<void> _abrirSelecaoInstrumento(int id, EventoService service) async {
    if (_detalhesCache.containsKey(id)) {
      _mostrarModalInscricao(_detalhesCache[id]!, service);
      return;
    }

    _showLoading();
    try {
      final data = await service.getDetalhesEvento(id);
      if (!mounted) return;
      Navigator.of(context, rootNavigator: true).pop();

      if (data != null) {
        _detalhesCache[id] = data;
        _mostrarModalInscricao(data, service);
      }
    } catch (e) {
      _handleError(e);
    }
  }

  void _mostrarModalInscricao(Map<String, dynamic> data, EventoService service) {
    final instrumentos = data['instrumentosDisponiveis'] ?? [];

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
              'Solicitar Participação',
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
                        'Ops! Não há instrumentos disponíveis para este evento no momento.',
                        textAlign: TextAlign.center,
                        style: TextStyle(color: Colors.grey[600], fontSize: 16),
                      ),
                      const SizedBox(height: 8),
                      const Text(
                        'Fique atento aos próximos informativos!',
                        style: TextStyle(color: Colors.grey, fontSize: 14),
                      ),
                    ],
                  ),
                ),
              )
            else ...[
              const Text('Selecione o instrumento que você irá tocar:'),
              const Divider(height: 30),
              ...instrumentos.map(
                (inst) => ListTile(
                  leading: const Icon(Icons.music_note, color: AppColors.primary),
                  title: Text(inst['nomeInstrumento']),
                  subtitle: Text('${inst['vagasDisponiveis']} vagas restantes'),
                  trailing: const Icon(Icons.chevron_right),
                  onTap: () async {
                    Navigator.pop(context);
                    _processarInscricao(
                      data['id'],
                      inst['idInstrumento'],
                      inst['nomeInstrumento'],
                      service,
                    );
                  },
                ),
              ),
            ],
            const SizedBox(height: 10),
          ],
        ),
      ),
    );
  }

  Future<void> _processarInscricao(
    int eventoId,
    int instId,
    String nomeInst,
    EventoService service,
  ) async {
    _showLoading();

    try {
      final erro = await service.solicitarParticipacao(eventoId, instId);

      if (!mounted) return;
      Navigator.of(context, rootNavigator: true).pop();

      if (erro == null) {
        await _atualizarEstadoEvento(eventoId);
        showDialog(
          context: context,
          builder: (context) => AlertDialog(
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
            title: const Icon(Icons.check_circle, color: Colors.green, size: 60),
            content: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                const Text(
                  'Solicitação Enviada!',
                  style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
                ),
                const SizedBox(height: 10),
                Text(
                  'Sua participação com o instrumento $nomeInst foi solicitada com sucesso. Agora é só aguardar a aprovação do regente!',
                  textAlign: TextAlign.center,
                ),
              ],
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(context),
                child: const Text(
                  'OK',
                  style: TextStyle(fontWeight: FontWeight.bold),
                ),
              ),
            ],
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(erro), backgroundColor: Colors.red),
        );
      }
    } catch (_) {
      if (mounted) Navigator.of(context, rootNavigator: true).pop();
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Erro ao enviar solicitação.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Widget _infoRow(IconData icon, String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6.0),
      child: Row(
        children: [
          Icon(icon, size: 20, color: AppColors.primary),
          const SizedBox(width: 10),
          Text('$label: ', style: const TextStyle(fontWeight: FontWeight.bold)),
          Expanded(child: Text(value, overflow: TextOverflow.ellipsis)),
        ],
      ),
    );
  }

  void _showLoading() {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => const Center(child: CircularProgressIndicator()),
    );
  }

  void _handleError(Object e) {
    if (mounted) {
      Navigator.of(context, rootNavigator: true).pop();
      ScaffoldMessenger.of(context)
          .showSnackBar(SnackBar(content: Text('Erro: $e')));
    }
  }

  Widget _baseCard({
    required String title,
    required DateTime date,
    String? subtitle,
    Widget? actionWidget,
  }) {
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      decoration: BoxDecoration(
        color: AppColors.cardBackground,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: const Color(0xFFF0E0E2)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 14,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Padding(
        padding: const EdgeInsets.fromLTRB(16, 14, 16, 14),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      fontWeight: FontWeight.w800,
                      fontSize: 18,
                      color: Color(0xFF1F1F1F),
                    ),
                  ),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      const Icon(
                        Icons.calendar_today_outlined,
                        size: 15,
                        color: Color(0xFF5A77B6),
                      ),
                      const SizedBox(width: 6),
                      Text(
                        _formatDataResumo(date),
                        style: const TextStyle(
                          fontSize: 13,
                          color: Color(0xFF5A5A5A),
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),
                  if (subtitle != null) ...[
                    const SizedBox(height: 8),
                    Text(
                      subtitle,
                      style: const TextStyle(
                        fontSize: 13,
                        color: Color(0xFF686868),
                        height: 1.25,
                      ),
                    ),
                  ],
                ],
              ),
            ),
            if (actionWidget != null) ...[
              const SizedBox(width: 12),
              actionWidget,
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildSectionHeader(String title, IconData icon) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(14, 16, 14, 10),
      child: Row(
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              color: const Color(0xFFFCE9EB),
              borderRadius: BorderRadius.circular(14),
              border: Border.all(color: const Color(0xFFF1CDD1)),
            ),
            child: Icon(icon, color: AppColors.primary, size: 22),
          ),
          const SizedBox(width: 10),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
            decoration: BoxDecoration(
              color: Colors.white.withValues(alpha: 0.88),
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: const Color(0xFFF1CDD1)),
            ),
            child: Text(
              title,
              style: const TextStyle(
                color: AppColors.primary,
                fontSize: 16,
                fontWeight: FontWeight.w800,
              ),
            ),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Container(
              height: 1,
              color: const Color(0xFFF1CDD1),
            ),
          ),
        ],
      ),
    );
  }

  String _formatDataResumo(DateTime date) {
    final dia = date.day.toString().padLeft(2, '0');
    final mes = date.month.toString().padLeft(2, '0');
    final hora = date.hour.toString().padLeft(2, '0');
    final minuto = date.minute.toString().padLeft(2, '0');
    return '$dia/$mes às $hora:$minuto';
  }
}

class _ActionIconButton extends StatelessWidget {
  final IconData icon;
  final String tooltip;
  final VoidCallback onPressed;

  const _ActionIconButton({
    required this.icon,
    required this.tooltip,
    required this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 56,
      height: 38,
      decoration: BoxDecoration(
        color: const Color(0xFFFDECEE),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: const Color(0xFFF1CDD1)),
      ),
      child: IconButton(
        padding: EdgeInsets.zero,
        iconSize: 20,
        tooltip: tooltip,
        onPressed: onPressed,
        icon: Icon(icon, color: AppColors.primary),
      ),
    );
  }
}

class _StatusParticipacao extends StatelessWidget {
  final String texto;
  final IconData icone;
  final Color cor;

  const _StatusParticipacao({
    required this.texto,
    required this.icone,
    required this.cor,
  });

  @override
  Widget build(BuildContext context) {
    return Tooltip(
      message: texto == 'AGUARDANDO'
          ? 'Aguardando a aprovação do administrador.'
          : 'Participação não aprovada.',
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icone, color: cor, size: 20),
          Text(
            texto,
            style: TextStyle(
              color: cor,
              fontSize: 9,
              fontWeight: FontWeight.bold,
            ),
          ),
        ],
      ),
    );
  }
}

class EventToggleButton extends StatelessWidget {
  final bool isAccepted;
  final VoidCallback onPressed;

  const EventToggleButton({
    super.key,
    required this.isAccepted,
    required this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: onPressed,
      style: ElevatedButton.styleFrom(
        backgroundColor:
            isAccepted ? const Color(0xFFB9B9B9) : AppColors.primary,
        foregroundColor: Colors.white,
        padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 10),
        minimumSize: const Size(88, 38),
        tapTargetSize: MaterialTapTargetSize.shrinkWrap,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(18),
        ),
        elevation: 0,
      ),
      child: Text(
        isAccepted ? 'CANCELAR' : 'ACEITAR',
        style: const TextStyle(fontWeight: FontWeight.w800, fontSize: 12),
      ),
    );
  }
}