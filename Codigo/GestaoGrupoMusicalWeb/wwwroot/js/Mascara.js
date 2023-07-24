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
            limparCampo("#chavePix");
            $("#spanChavePix").text("");
            $("#chavePix").attr("placeholder", "000.000.000-00");
            $("#chavePix").inputmask("999.999.999-99");
        } else if (valorSelecionado == "celular") {
            limparCampo("#chavePix");
            $("#spanChavePix").text("");
            $("#chavePix").attr("placeholder", "(99)99999-9999");
            $("#chavePix").inputmask("(99)99999-9999");
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

});
