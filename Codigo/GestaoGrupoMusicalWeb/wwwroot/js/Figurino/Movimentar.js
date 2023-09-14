function bloquearSelecaoAssociado() {
    document.getElementById("campoAssociado").hidden = false;
    document.getElementById("campoAssociado_Colaborador").hidden = true;
}

function permitirSelecaoAssociado() {
    document.getElementById("campoAssociado").hidden = true;
    document.getElementById("campoAssociado_Colaborador").hidden = false;
}