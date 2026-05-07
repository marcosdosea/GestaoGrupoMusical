import 'dart:convert';

import 'package:batala_mobile/config/session_manager.dart';
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

    return SingleChildScrollView(
      child: Column(
        children: [
          _buildSection<EventoModel>(
            "Próximos Eventos", 
            eventoService.getAll(), 
            (item) => _buildEventCard(item)
          ),
          _buildSection<EnsaioModel>(
            "Próximos Ensaios", 
            ensaioService.getAll(), 
            (item) => _buildEnsaioCard(item)
          ),
          const SizedBox(height: 60),
        ],
      ),
    );
  }

  Widget _buildSection<T>(String title, Future<List<T>> future, Widget Function(T) builder) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start, 
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(16, 12, 16, 4), 
          child: Text(title, style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold))
        ),
        FutureBuilder<List<T>>(
          future: future,
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return const Padding(
                padding: EdgeInsets.only(left: 16.0),
                child: SizedBox(height: 30, child: CircularProgressIndicator()),
              );
            }
            if (snapshot.hasError) {
              return Padding(
                padding: const EdgeInsets.only(left: 16.0),
                child: Text("Erro ao carregar $title"),
              );
            }
            final list = snapshot.data ?? [];
            if (list.isEmpty) {
              return const Padding(
                padding: EdgeInsets.only(left: 16.0),
                child: Text("Nada agendado por enquanto."),
              );
            }
            return SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              physics: const BouncingScrollPhysics(),
              child: Padding(
                padding: const EdgeInsets.only(right: 16),
                child: Row(
                  children: List.generate(list.length, (i) => builder(list[i])),
                ),
              ),
            );
          },
        )
      ]
    );
  }
  Widget _buildEventCard(EventoModel item) {
    return _baseCard(
      title: item.local, 
      date: item.dataInicio,
      actionWidget: Align(
        alignment: Alignment.centerRight,
        child: EventToggleButton(eventId: item.id),
      ),
    );
  }

  Widget _buildEnsaioCard(EnsaioModel item) {
    return _baseCard(
      title: "[${item.tipo}] ${item.local}",
      date: item.dataHora,
      trailing: null, 
      actionWidget: null, 
    );
  }

  Widget _baseCard({
    required String title, 
    required DateTime date, 
    Widget? trailing, 
    Widget? actionWidget,
  }) {
    return Container(
      width: 232,
      margin: const EdgeInsets.only(left: 16, bottom: 2),
      child: Card(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(15)),
        elevation: 4,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Text(title, 
                      style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14), 
                      maxLines: 1, overflow: TextOverflow.ellipsis
                    ),
                  ),
                  if (trailing != null) trailing,
                ],
              ),
              const SizedBox(height: 1),
              Text(
                "📅 ${date.day.toString().padLeft(2, '0')}/${date.month.toString().padLeft(2, '0')} às ${date.hour}:${date.minute.toString().padLeft(2, '0')}",
                style: TextStyle(color: Colors.grey[700], fontSize: 11),
              ),
              if (actionWidget != null) ...[
                const SizedBox(height: 2),
                actionWidget,
              ],
            ],
          ),
        ),
      ),
    );
  }
}

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

  // Verifica o cargo no token para saber se mostra ou não o botão
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
          setState(() { _isAssociado = true; });
        }
      }
    }
  }

  // Abre o Pop-up para o usuário escolher o instrumento
  void _abrirModalInstrumentos() {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(15)),
          title: const Text("Escolha seu instrumento", style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18)),
          content: SizedBox(
            width: double.maxFinite,
            child: FutureBuilder<List<dynamic>>(
              future: _eventoService.getInstrumentosDoEvento(widget.eventId),
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const SizedBox(
                    height: 100, 
                    child: Center(child: CircularProgressIndicator())
                  );
                }

                if (snapshot.hasError || !snapshot.hasData) {
                  return const Text("Erro ao carregar a lista. Tente novamente.");
                }

                final lista = snapshot.data!;

                if (lista.isEmpty) {
                  return const Padding(
                    padding: EdgeInsets.symmetric(vertical: 10.0),
                    child: Text(
                      "Poxa, a coordenação ainda não liberou as vagas dos instrumentos para este evento. 🥁\n\nAguarde mais um pouquinho e tente novamente!",
                      style: TextStyle(fontSize: 15, color: Colors.black87),
                      textAlign: TextAlign.center,
                    ),
                  );
                }

                return ListView.builder(
                  shrinkWrap: true, 
                  itemCount: lista.length,
                  itemBuilder: (context, index) {
                    final instrumento = lista[index];
                    
                    // Lendo os dados com base no DTO do C#
                    final idInstrumento = instrumento['idInstrumento'];
                    final nomeInstrumento = instrumento['listaInstrumentos'] ?? "Instrumento";
                    
                    return Card(
                      elevation: 3,
                      margin: const EdgeInsets.symmetric(vertical: 7),
                      child: ListTile(
                        leading: const Icon(Icons.music_note, color: AppColors.primary),
                        title: Text(nomeInstrumento, style: const TextStyle(fontSize: 12)),
                        trailing: const Icon(Icons.arrow_forward_ios, size: 9),
                        onTap: () {
                          Navigator.of(context).pop(); 
                          _confirmarInscricao(idInstrumento); 
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
              child: const Text("VOLTAR", style: TextStyle(color: AppColors.secondary)),
            ),
          ],
        );
      },
    );
  }

  // Chama a API de solicitar presença após a escolha do instrumento
  Future<void> _confirmarInscricao(int idInstrumento) async {
    setState(() => _isLoading = true);
    
    // Agora ele recebe a mensagem de erro (ou null se deu certo)
    String? erroMensagem = await _eventoService.solicitarParticipacao(widget.eventId, idInstrumento);

    setState(() {
      _isLoading = false;
      
      if (erroMensagem == null) {
        // Sucesso!
        _isAccepted = true; 
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Inscrição confirmada com sucesso!'), 
            backgroundColor: Colors.green
          ),
        );
      } else {
        // Falha! Mostra exatamente o que o C# reclamou
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(erroMensagem, style: const TextStyle(fontWeight: FontWeight.bold)), 
            backgroundColor: Colors.red,
            duration: const Duration(seconds: 4), // Deixa um tempinho a mais pra ele ler
          ),
        );
      }
    });
  }

  // Chama a API de cancelar presença
  Future<void> _cancelarInscricao() async {
    setState(() => _isLoading = true);
    
    bool sucesso = await _eventoService.cancelarSolicitacao(widget.eventId);

    setState(() {
      _isLoading = false;
      if (sucesso) {
        _isAccepted = false; 
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Erro ao cancelar inscrição.')),
        );
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    if (!_isAssociado) return const SizedBox.shrink();

    return ElevatedButton(
      onPressed: _isLoading ? null : (_isAccepted ? _cancelarInscricao : _abrirModalInstrumentos),
      style: ElevatedButton.styleFrom(
        backgroundColor: _isAccepted ? Colors.grey[200] : AppColors.primary,
        foregroundColor: _isAccepted ? Colors.black87 : Colors.white,
        elevation: _isAccepted ? 0 : 2,
        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      ),
      child: _isLoading 
          ? const SizedBox(width: 15, height: 15, child: CircularProgressIndicator(strokeWidth: 2))
          : Text(
              _isAccepted ? "RECUSAR" : "ACEITAR",
              style: const TextStyle(fontSize: 11, fontWeight: FontWeight.bold),
            ),
    );
  }
}