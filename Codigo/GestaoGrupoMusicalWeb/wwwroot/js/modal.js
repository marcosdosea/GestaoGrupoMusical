function showModal(formId, modalId) {
    form = $(`#${formId}`);
    modal = $(`#${modalId}`);

    console.log(form.html());
    console.log(modal.html());
}