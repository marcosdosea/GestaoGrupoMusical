import 'package:batala_mobile/config/app_colors.dart';
import 'package:flutter/material.dart';
import 'package:mask_text_input_formatter/mask_text_input_formatter.dart';
import '../service/login_service.dart'; 

class LoginView extends StatefulWidget {
  const LoginView({super.key});

  @override
  State<LoginView> createState() => _LoginViewState();
}

class _LoginViewState extends State<LoginView> {
  final LoginService _loginService = LoginService();
  

  final _formKey = GlobalKey<FormState>();
  final TextEditingController _cpfController = TextEditingController();
  final TextEditingController _senhaController = TextEditingController();
  

  bool _isLoading = false; 
  bool _obscureSenha = true;
  bool _permanecerConectado = false;

  final cpfFormatter = MaskTextInputFormatter(
    mask: '###.###.###-##', 
    filter: { "#": RegExp(r'[0-9]') },
    type: MaskAutoCompletionType.lazy
  );

  // Lógica de Login
  Future<void> _fazerLogin() async {
    // Valida os campos do formulário antes de enviar
    if (!_formKey.currentState!.validate()) {
      return;
    }

    // Pega apenas os números puros do CPF
    String cpfLimpo = cpfFormatter.getUnmaskedText();
    String senha = _senhaController.text;

    setState(() => _isLoading = true);

    try {
      bool sucesso = await _loginService.login(cpfLimpo, senha);

      if (sucesso) {
        if (!mounted) return;
        Navigator.pushReplacementNamed(context, '/home'); 
      } else {
        if (!mounted) return;
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('CPF ou senha inválidos.'),
            backgroundColor: Colors.red,
          ),
        );
      }
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(e.toString().replaceAll('Exception: ', '')),
          backgroundColor: Colors.orange,
        ),
      );
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  void dispose() {
    _cpfController.dispose();
    _senhaController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth > 600) {
            return Row(
              children: [
                Expanded(flex: 3, child: _buildLoginFormPanel()),
                Expanded(flex: 2, child: _buildWelcomePanel()),  
              ],
            );
          } else {
            return Column(
              children: [
                Expanded(flex: 1, child: _buildWelcomePanelMobile()), 
                Expanded(flex: 10, child: _buildLoginFormPanel()),       
              ],
            );
          }
        },
      ),
    );
  }

  // --- PAINEL DO FORMULÁRIO DE LOGIN (Branco) ---
  Widget _buildLoginFormPanel() {
    return Container(
      color: Colors.white,
      padding: const EdgeInsets.all(40.0),
      child: Center(
        child: SingleChildScrollView( 
          child: Form(
            key: _formKey, 
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Center(
                  child: Image.asset(
                    'assets/img/batala.png',
                    height: 180, 
                  ),
                ),
                const SizedBox(height: 30),

                // --- Campo CPF ---
                const Text("CPF", style: TextStyle(fontWeight: FontWeight.w600, color: Colors.black87)),
                const SizedBox(height: 8),
                TextFormField(
                  controller: _cpfController,
                  inputFormatters: [cpfFormatter],
                  decoration: const InputDecoration(
                    prefixIcon: Icon(Icons.person_outline),
                    hintText: '000.000.000-00',
                    border: OutlineInputBorder(),
                    filled: true,
                    fillColor: Color(0xFFF5F5F5), 
                  ),
                  keyboardType: TextInputType.number,
                  // VALIDAÇÃO DO CPF
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Por favor, informe o seu CPF';
                    }
                    if (cpfFormatter.getUnmaskedText().length != 11) {
                      return 'CPF inválido';
                    }
                    return null; 
                  },
                ),
                const SizedBox(height: 20),

                // --- Campo Senha ---
                const Text("Senha", style: TextStyle(fontWeight: FontWeight.w600, color: Colors.black87)),
                const SizedBox(height: 8),
                TextFormField(
                  controller: _senhaController,
                  obscureText: _obscureSenha,
                  decoration: InputDecoration(
                    prefixIcon: const Icon(Icons.lock_outline),
                    hintText: '******',
                    border: const OutlineInputBorder(),
                    filled: true,
                    fillColor: const Color(0xFFF5F5F5),
                    suffixIcon: IconButton(
                      icon: Icon(_obscureSenha ? Icons.visibility_off : Icons.visibility),
                      onPressed: () => setState(() => _obscureSenha = !_obscureSenha),
                    ),
                  ),
                  // VALIDAÇÃO DA SENHA
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Por favor, informe a sua senha';
                    }
                    return null; 
                  },
                ),
                const SizedBox(height: 15),

                // Permanecer Conectado
                Row(
                  children: [
                    Checkbox(
                      value: _permanecerConectado,
                      onChanged: (value) => setState(() => _permanecerConectado = value!),
                    ),
                    const Text("Permanecer Conectado", style: TextStyle(color: Colors.black54)),
                  ],
                ),
                const SizedBox(height: 30),

                // --- Botão Entrar ---
                SizedBox(
                  width: double.infinity,
                  height: 55,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.primary, 
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                      elevation: 3,
                    ),
                    onPressed: _isLoading ? null : _fazerLogin, 
                    child: _isLoading 
                        ? const CircularProgressIndicator(color: Colors.white)
                        : const Text("Entrar", style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
                  ),
                ),
                const SizedBox(height: 20),

                // Esqueceu a senha
                Center(
                  child: TextButton(
                    onPressed: () { /* Lógica esqueci senha */ },
                    child: const Text(
                      "Esqueceu a senha? Clique aqui",
                      style: TextStyle(color: Colors.blue, decoration: TextDecoration.underline),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildWelcomePanel() {
    return Container(
      color: AppColors.primary, 
      child: Center(
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              padding: const EdgeInsets.all(12),
              decoration: const BoxDecoration(color: Colors.white30, shape: BoxShape.circle),
              child: const Icon(Icons.arrow_back, color: Colors.white, size: 30),
            ),
            const SizedBox(width: 20),
            const Text(
              "Bem-vindo!",
              style: TextStyle(
                color: Colors.white,
                fontSize: 35,
                fontWeight: FontWeight.bold,
                decoration: TextDecoration.underline, 
                decorationColor: Colors.white,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildWelcomePanelMobile() {
    return Container(
      width: double.infinity,
      decoration: const BoxDecoration(
        color: AppColors.primary, 
        borderRadius: BorderRadius.only(
          bottomLeft: Radius.circular(10.0),  
          bottomRight: Radius.circular(10.0), 
        ),
      ),
      child: const Center(
        child: Text(
          "Bem-vindo!",
          style: TextStyle(color: Colors.white, fontSize: 26, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}