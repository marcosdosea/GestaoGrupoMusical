import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:batala_mobile/config/session_manager.dart';
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
        // SEÇÃO DE EVENTOS - Ocupa metade da tela ou o espaço necessário
        Expanded(
          child: _buildSection<EventoModel>(
            "Próximos Eventos", 
            eventoService.getAll(), 
            (item) => _buildEventCard(item)
          ),
        ),

        // SEÇÃO DE ENSAIOS - Ocupa a outra metade
        Expanded(
          child: _buildSection<EnsaioModel>(
            "Próximos Ensaios", 
            ensaioService.getAll(), 
            (item) => _buildEnsaioCard(item)
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
          child: Text(
            title, 
            style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold, color: Colors.black87)
          )
        ),
        Expanded(
          child: FutureBuilder<List<T>>(
            future: future,
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.waiting) {
                return const Center(child: CircularProgressIndicator());
              }
              if (snapshot.hasError) {
                return const Center(child: Text("Erro ao carregar dados"));
              }
              final list = snapshot.data ?? [];
              if (list.isEmpty) {
                return const Padding(
                  padding: EdgeInsets.symmetric(horizontal: 20.0),
                  child: Text("Nada agendado."),
                );
              }

              return ListView.builder(
                // O padding interno garante que o card não cole nas bordas
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
                itemCount: list.length,
                // BouncingScrollPhysics dá um efeito mais fluido ao chegar no fim
                physics: const BouncingScrollPhysics(),
                itemBuilder: (context, index) {
                  return builder(list[index]);
                },
              );
            },
          ),
        )
      ]
    );
  }

  Widget _buildEventCard(EventoModel item) {
    return _baseCard(
      title: item.local, 
      date: item.dataInicio,
      // Exemplo de como você pode passar o endereço futuramente:
      subtitle: "Endereço: Rua Exemplo, 123 - Centro", 
      actionWidget: EventToggleButton(eventId: item.id),
    );
  }

  Widget _buildEnsaioCard(EnsaioModel item) {
    return _baseCard(
      title: "[${item.tipo}] ${item.local}",
      date: item.dataHora,
      subtitle: "Levar baquetas e protetor auricular",
    );
  }

  Widget _baseCard({
    required String title, 
    required DateTime date, 
    String? subtitle,
    Widget? actionWidget,
  }) {
    return Container(
      margin: const EdgeInsets.only(bottom: 16), // Espaçamento entre os cards
      child: Card(
        // Cor de fundo levemente rosada/clara como na imagem
        color:  AppColors.cardBackground, 
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
        elevation: 2,
        child: Padding(
          padding: const EdgeInsets.all(20), // Padding maior para o card ficar "grande"
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                title, 
                style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 18, color: Color(0xFF333333)), 
                maxLines: 2, 
                overflow: TextOverflow.ellipsis
              ),
              const SizedBox(height: 10),
              Row(
                children: [
                  //const Icon(Icons.calendar_month, size: 18, color: Colors.grey),
                  const SizedBox(width: 8),
                   const SizedBox(height: 6),
                    Text(
                      "📅 ${date.day.toString().padLeft(2, '0')}/${date.month.toString().padLeft(2, '0')} às ${date.hour}:${date.minute.toString().padLeft(2, '0')}",
                      style: TextStyle(color: Colors.grey[700], fontSize: 13),
                    ),
                ],
              ),
              
              // Espaço para os dados extras (Endereço, etc)
              if (subtitle != null) ...[
                const SizedBox(height: 10),
                Text(
                  subtitle,
                  style: TextStyle(color: const Color.fromARGB(255, 48, 48, 48), fontSize: 14),
                ),
              ],

              if (actionWidget != null) ...[
                const SizedBox(height: 15),
                Align(
                  alignment: Alignment.centerRight,
                  child: actionWidget,
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}

// O EventToggleButton permanece o mesmo, apenas ajuste o estilo se necessário
class EventToggleButton extends StatefulWidget {
  final int eventId;
  const EventToggleButton({super.key, required this.eventId});

  @override
  State<EventToggleButton> createState() => _EventToggleButtonState();
}

class _EventToggleButtonState extends State<EventToggleButton> {
  bool _isAccepted = false;
  bool _isLoading = false;
  bool _isAssociado = false;
  final _eventoService = EventoService();

  @override
  void initState() {
    super.initState();
    _verificarRole();
  }

  Future<void> _verificarRole() async {
    final token = await SessionManager.getToken();
    if (token != null && token.isNotEmpty) {
      final parts = token.split('.');
      if (parts.length == 3) {
        String payload = parts[1];
        while (payload.length % 4 != 0) { payload += '='; }
        final decoded = utf8.decode(base64Url.decode(payload));
        final Map<String, dynamic> data = jsonDecode(decoded);
        String role = data['role'] ?? 
                      data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? 
                      "";
        if (role.toUpperCase() == "ASSOCIADO") {
          if (mounted) setState(() { _isAssociado = true; });
        }
      }
    }
  }

  void _abrirModalInstrumentos() {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          backgroundColor: AppColors.background, // Cor combinando com o app
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
          title: const Text(
            "Escolha seu instrumento",
            textAlign: TextAlign.center,
            style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
          ),
          content: Container(
            // Definimos uma largura mínima para ele não ficar "esmagado"
            width: MediaQuery.of(context).size.width * 0.8,
            // Definimos um limite de altura para não ocupar a tela toda se tiver muitos itens
            constraints: BoxConstraints(
              maxHeight: MediaQuery.of(context).size.height * 0.4,
            ),
            child: FutureBuilder<List<dynamic>>(
              future: _eventoService.getInstrumentosDoEvento(widget.eventId),
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const SizedBox(
                    height: 100,
                    child: Center(child: CircularProgressIndicator(color: AppColors.primary)),
                  );
                }

                final lista = snapshot.data ?? [];

                if (lista.isEmpty) {
                  return const Padding(
                    padding: EdgeInsets.symmetric(vertical: 20),
                    child: Text(
                      "Nenhuma vaga disponível no momento. 🥁",
                      textAlign: TextAlign.center,
                    ),
                  );
                }

                return ListView.separated(
                  shrinkWrap: true, // Importante para o modal se ajustar ao conteúdo
                  itemCount: lista.length,
                  separatorBuilder: (context, index) => const SizedBox(height: 8),
                  itemBuilder: (context, index) {
                    final instrumento = lista[index];
                    return Card(
                      elevation: 0,
                      color: AppColors.cardBackground,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(10),
                        side: BorderSide(color: Colors.grey.shade200),
                      ),
                      child: ListTile(
                        leading: const Icon(Icons.music_note, color: AppColors.primary),
                        title: Text(
                          instrumento['listaInstrumentos'] ?? "Instrumento",
                          style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w500),
                        ),
                        onTap: () {
                          Navigator.of(context).pop();
                          _confirmarInscricao(instrumento['idInstrumento']);
                        },
                      ),
                    );
                  },
                );
              },
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text("VOLTAR", style: TextStyle(color: Colors.black)),
            ),
          ],
        );
      },
    );
  }

  Future<void> _confirmarInscricao(int idInstrumento) async {
    setState(() => _isLoading = true);
    String? erro = await _eventoService.solicitarParticipacao(widget.eventId, idInstrumento);
    if (mounted) {
      setState(() {
        _isLoading = false;
        if (erro == null) _isAccepted = true;
      });
    }
  }

  Future<void> _cancelarInscricao() async {
    setState(() => _isLoading = true);
    bool sucesso = await _eventoService.cancelarSolicitacao(widget.eventId);
    if (mounted) {
      setState(() {
        _isLoading = false;
        if (sucesso) _isAccepted = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    if (!_isAssociado) return const SizedBox.shrink();

    return ElevatedButton(
      onPressed: _isLoading ? null : (_isAccepted ? _cancelarInscricao : _abrirModalInstrumentos),
      style: ElevatedButton.styleFrom(
        backgroundColor: _isAccepted ? Colors.grey[400] : const Color(0xFFD9534F),
        foregroundColor: Colors.white,
        padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
      child: _isLoading 
          ? const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2))
          : Text(_isAccepted ? "CANCELAR" : "ACEITAR", style: const TextStyle(fontWeight: FontWeight.bold)),
    );
  }
}