class LoginModel {
  final String cpf;
  final String senha;

  LoginModel({
    required this.cpf,
    required this.senha,
  });

  Map<String, dynamic> toJson() {
    return {
      "cpf": cpf,
      "senha": senha,
    };
  }
}