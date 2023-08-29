function fillAutocomplete(data, inputId, listId, form, message) {
    if (data != null && inputId != null) {
        data = JSON.parse(data);
        const nomes = data.map((data) => {
            return data["Nome"];
        });

        if (form != null) {
            $(`#${form}`).on("submit", function (event) {
                if (!$(`#${listId}`).val().length) {
                    $(`#${inputId}`).addClass("input-validation-error");
                    document.querySelector("span[for='" + inputId + "']").textContent = message;

                    event.preventDefault();
                }
                else {
                    $(`#${inputId}`).removeClass("input-validation-error");
                    document.querySelector("span[for='" + inputId + "']").textContent = "";
                }
            });
        }

        $(`#${inputId}`).autocomplete({
            source: nomes,
            select: (event, ui) => {
                var lista = document.getElementById(listId);

                for (var i = 0; i < lista.options.length; i++) {
                    if (lista.options[i].text == ui.item.value) {
                        lista.options[i].selected = true;
                        $("#blockNames").addClass("my-4");
                        $("#blockNames").prepend(`
                            <h6 id="${ui.item.value.replaceAll(' ','')}">${ui.item.value}
                                <button class="btn btn-secondary badge" title="Excluir Regente" type="button" onclick="removeRegente('${ui.item.value}', '${listId}')">
                                    <i class="fa-solid fa-xmark"> </i>
                                </button>
                            </h6>
                        `);
                        ui.item.value = "";

                        $(`#${inputId}`).removeClass("input-validation-error");
                        document.querySelector("span[for='" + inputId + "']").textContent = "";
                    }
                }
                
            }
        });
    }
}

function removeRegente(regente, listId) {
    var lista = document.getElementById(listId);

    for (var i = 0; i < lista.options.length; i++) {
        if (lista.options[i].text == regente) {
            lista.options[i].selected = false;
            $(`#${regente.replaceAll(' ', '')}`).remove();
            if ($('#blockNames').children().length == 0) {
                $('#blockNames').removeClass("my-4");
            }
        }
    }
}