function fillAutocomplete(data, inputId) {
    if (data != null && inputId != null) {
        data = JSON.parse(data);
        const nomes = data.map((data) => {
            return data["Nome"];
        });

        $(`#${inputId}`).autocomplete({
            source: nomes
        });
    }
}