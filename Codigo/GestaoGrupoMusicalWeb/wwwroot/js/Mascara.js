$(document).ready(function () {
    $("#campoCpf").inputmask("999.999.999-99"); //static mask
    $("#campoCep").inputmask("99999-999");
    $("#campoTelefone1").inputmask("(99)99999-9999");
    $("#campoTelefone2").inputmask("(99)99999-9999");
    $("#campoCnpj").inputmask("99.999.999/9999-99");

    function limparCampo(idCampo) {
        $(idCampo).inputmask('remove');
        $(idCampo).val("");
    }
    $("#selectPix").change(function () {
        // Código para aplicar a máscara de acordo com a opção selecionada em #selectPix
        var valorSelecionado = $(this).val();
        if (valorSelecionado == "cpf") {
            $('#chavePix').on('change', function () {
                const cpfInput = $('#chavePix').val();
                if (!validarCPF(cpfInput)) {
                    $('#spanChavePix').text("CPF inválido.");
                } else {
                    $('#spanChavePix').text("");
                }
            });
            limparCampo("#chavePix");
            $("#spanChavePix").text("");
            $("#chavePix").attr("placeholder", "000.000.000-00");
            $("#chavePix").inputmask("999.999.999-99");
        } else if (valorSelecionado == "celular") {
            $('#chavePix').off('change');
            limparCampo("#chavePix");
            $("#chavePix").attr("placeholder", "(99)99999-9999");
            $("#chavePix").inputmask("(99)99999-9999");
            $("#spanChavePix").text("");
        } else if (valorSelecionado == "email") {
            limparCampo("#chavePix");
            $("#spanChavePix").text("");
            $("#chavePix").attr("placeholder", "email@exemplo.com");
            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            var inputEmail = $("#chavePix").val();
            if (!emailRegex.test(inputEmail)) {
                $("#spanChavePix").text("email não está no formato válido")
            } else {
                $("#spanChavePix").text("");
            }
        } else if (valorSelecionado == "chave aleatoria") {
            limparCampo("#chavePix");
            $("#spanChavePix").text("");
            $("#chavePix").attr("placeholder", "Chave aleatória");
        } else {
            limparCampo("#chavePix");
            $("#chavePix").attr("placeholder", "Sem chave pix definida");
            $("#spanChavePix").text("");
        }
    });
    function validarCPF(cpf) {
        cpf = cpf.replace(/\D/g, ''); // Remover caracteres não numéricos

        if (cpf.length !== 11 || /^(\d)\1{10}$/.test(cpf)) return false; // Verificar se tem todos os dígitos iguais

        // Calcular o primeiro dígito verificador
        let soma = 0;
        for (let i = 0; i < 9; i++) {
            soma += parseInt(cpf.charAt(i)) * (10 - i);
        }
        let resto = (soma * 10) % 11;
        let digitoVerificador1 = (resto === 10 || resto === 11) ? 0 : resto;

        // Calcular o segundo dígito verificador
        soma = 0;
        for (let i = 0; i < 10; i++) {
            soma += parseInt(cpf.charAt(i)) * (11 - i);
        }
        resto = (soma * 10) % 11;
        let digitoVerificador2 = (resto === 10 || resto === 11) ? 0 : resto;

        // Verificar se os dígitos verificadores são iguais aos do CPF informado
        return (digitoVerificador1 === parseInt(cpf.charAt(9)) && digitoVerificador2 === parseInt(cpf.charAt(10)));
    }
});
