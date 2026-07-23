import 'package:batala_mobile/model/material_estudo_model.dart';
import 'package:batala_mobile/service/material_estudo_service.dart';
import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';

class MaterialEstudoView extends StatefulWidget {
  const MaterialEstudoView({super.key});

  @override
  State<MaterialEstudoView> createState() => _MaterialEstudoViewState();
}

class _MaterialEstudoViewState extends State<MaterialEstudoView> {
  final MaterialestudoService service = MaterialestudoService();
  late Future<List<MaterialestudoModel>> _futureMateriais;

  @override
  void initState() {
    super.initState();
    _carregarDados(forceRefresh: false);
  }

  void _carregarDados({bool forceRefresh = false}) {
    _futureMateriais = service.getAll(forceRefresh: forceRefresh);
  }

  Future<void> _refresh() async {
    setState(() {
      _carregarDados(forceRefresh: true); // Força buscar direto da API ignorando o cache
    });
    try {
      await _futureMateriais;
    } catch (_) {}
  }

  // Lógica completa para tratar e abrir o link
  Future<void> _abrirLink(String urlString, BuildContext context) async {
    if (!urlString.startsWith('http://') && !urlString.startsWith('https://')) {
      urlString = 'https://$urlString';
    }

    final Uri url = Uri.parse(urlString);

    try {
      if (await canLaunchUrl(url)) {
        await launchUrl(
          url,
          mode: LaunchMode.externalApplication,
        );
      } else {
        throw 'Não foi possível abrir: $urlString';
      }
    } catch (e) {
      if (context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Link inválido ou não suportado pelo dispositivo.')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<MaterialestudoModel>>(
      future: _futureMateriais,
      builder: (context, snapshot) {
        // 1. Tratamento de Carregamento
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        } 
        
        // 2. Tratamento de Erro
        else if (snapshot.hasError) {
          return RefreshIndicator(
            onRefresh: _refresh,
            child: CustomScrollView(
              physics: const AlwaysScrollableScrollPhysics(),
              slivers: [
                SliverFillRemaining(
                  child: Center(child: Text("Erro ao carregar materiais: ${snapshot.error}")),
                ),
              ],
            ),
          );
        } 
        
        // 3. Verificação de Dados
        else if (snapshot.hasData && snapshot.data!.isNotEmpty) {
          final materiais = snapshot.data!;

          return RefreshIndicator(
            onRefresh: _refresh,
            child: ListView.builder(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.only(top: 10, bottom: 100, left: 10, right: 10),
              itemCount: materiais.length,
              itemBuilder: (context, index) {
                final item = materiais[index];

                return Card(
                  elevation: 2,
                  margin: const EdgeInsets.symmetric(vertical: 8),
                  child: ListTile(
                    leading: const Icon(Icons.menu_book, color: Color(0xFFD64550)),
                    title: Text(
                      item.nome,
                      style: const TextStyle(fontWeight: FontWeight.bold),
                    ),
                    subtitle: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const SizedBox(height: 4),
                        Text(
                          item.link,
                          style: const TextStyle(color: Colors.blue, decoration: TextDecoration.underline),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          "Postado em: ${item.dataInicio.day.toString().padLeft(2, '0')}/${item.dataInicio.month.toString().padLeft(2, '0')}/${item.dataInicio.year} às ${item.dataInicio.hour.toString().padLeft(2, '0')}:${item.dataInicio.minute.toString().padLeft(2, '0')}",
                          style: TextStyle(color: Colors.grey[600], fontSize: 12),
                        ),
                      ],
                    ),
                    isThreeLine: true,
                    onTap: () => _abrirLink(item.link, context), 
                  ),
                );
              },
            ),
          );
        }

        // 4. Caso a lista esteja vazia
        return RefreshIndicator(
          onRefresh: _refresh,
          child: CustomScrollView(
            physics: const AlwaysScrollableScrollPhysics(),
            slivers: [
              SliverFillRemaining(
                child: Center(child: const Text("Nenhum material de estudo disponível.")),
              ),
            ],
          ),
        );
      },
    );
  }
}