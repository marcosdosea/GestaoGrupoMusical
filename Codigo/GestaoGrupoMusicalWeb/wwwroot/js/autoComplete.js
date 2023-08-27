function fillAutocomplete(data, inputId, listId) {
    if (data != null && inputId != null) {
        data = JSON.parse(data);
        const nomes = data.map((data) => {
            return data["Nome"];
        });

        $(`#${inputId}`).autocomplete({
            source: nomes,
            select: (event, ui) => {
                var lista = document.getElementById(listId);

                for (var i = 0; i < lista.options.length; i++) {
                    if (lista.options[i].text == ui.item.value) {
                        lista.options[i].selected = true;
                        ui.item.value = "";
                    }
                }
                
            }
        });
    }
}