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
          const SizedBox(height: 100),
        ],
      ),
    );
  }

  Widget _buildSection<T>(String title, Future<List<T>> future, Widget Function(T) builder) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start, 
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(16, 20, 16, 8), 
          child: Text(title, style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold))
        ),
        SizedBox(
          height: 185, 
          child: FutureBuilder<List<T>>(
            future: future,
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.waiting) {
                return const Center(child: CircularProgressIndicator());
              }
              if (snapshot.hasError) {
                return Center(child: Text("Erro ao carregar $title"));
              }
              final list = snapshot.data ?? [];
              if (list.isEmpty) {
                return const Padding(
                  padding: EdgeInsets.only(left: 16.0),
                  child: Text("Nada agendado por enquanto."),
                );
              }
              return ListView.builder(
                scrollDirection: Axis.horizontal, 
                itemCount: list.length, 
                itemBuilder: (context, i) => builder(list[i])
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
      width: 270,
      margin: const EdgeInsets.only(left: 16, bottom: 5),
      child: Card(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(15)),
        elevation: 4,
        child: Padding(
          padding: const EdgeInsets.all(14.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Text(title, 
                      style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16), 
                      maxLines: 1, overflow: TextOverflow.ellipsis
                    ),
                  ),
                  if (trailing != null) trailing,
                ],
              ),
              const SizedBox(height: 8),
              Text(
                "📅 ${date.day.toString().padLeft(2, '0')}/${date.month.toString().padLeft(2, '0')} às ${date.hour}:${date.minute.toString().padLeft(2, '0')}",
                style: TextStyle(color: Colors.grey[700], fontSize: 13),
              ),
              const Spacer(),
              if (actionWidget != null) actionWidget, 
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

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: () => setState(() => _isAccepted = !_isAccepted),
      style: ElevatedButton.styleFrom(
        backgroundColor: _isAccepted ? Colors.grey[200] : AppColors.primary,
        foregroundColor: _isAccepted ? Colors.black87 : Colors.white,
        elevation: _isAccepted ? 0 : 2,
        padding: const EdgeInsets.symmetric(horizontal: 16),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      ),
      child: Text(
        _isAccepted ? "RECUSAR" : "ACEITAR",
        style: const TextStyle(fontSize: 11, fontWeight: FontWeight.bold),
      ),
    );
  }
}