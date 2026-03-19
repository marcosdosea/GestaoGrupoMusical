import 'package:flutter/material.dart';
import '../config/app_colors.dart';
import '../model/financeiro_model.dart';

class FinanceiroView extends StatefulWidget {
  final FinanceiroModel solicitacao;
  const FinanceiroView({super.key, required this.solicitacao});

  @override
  State<FinanceiroView> createState() => _FinanceiroFormViewState();
}

class _FinanceiroFormViewState extends State<FinanceiroView> {
  final _descController = TextEditingController();
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    _descController.text = "Pagamento: ${widget.solicitacao.descricao}";
  }

  void _confirmarPagamento() async {
    setState(() => _isSaving = true);
    
    setState(() => _isSaving = false);
    Navigator.pop(context);
    ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text("Pagamento enviado com sucesso!")));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Confirmar Pagamento")),
      body: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text("Resumo: ${widget.solicitacao.descricao}", style: const TextStyle(fontSize: 16)),
            const SizedBox(height: 25),
            TextField(
              maxLines: 4, 
              keyboardType: TextInputType.multiline,
              controller: _descController,
              decoration: const InputDecoration(
                labelText: "Descrição/Observação",
                border: OutlineInputBorder(),
              ),
            ),
            const Spacer(),
            SizedBox(
              width: double.infinity,
              height: 50,
              child: ElevatedButton(
                style: ElevatedButton.styleFrom(backgroundColor: AppColors.primary),
                onPressed: _isSaving ? null : _confirmarPagamento,
                child: _isSaving 
                  ? const CircularProgressIndicator(color: Colors.white) 
                  : const Text("ENVIAR PAGAMENTO", style: TextStyle(color: Colors.white)),
              ),
            )
          ],
        ),
      ),
    );
  }
}