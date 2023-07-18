// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const toggleSenha = document.querySelector('#toggleSenha');
const toggleConfirmSenha = document.querySelector('#toggleSenha2');
const senhaInput = document.querySelector('#InputSenha');

toggleSenha.addEventListener('click', function () {
if (senhaInput.type === 'password') {
    senhaInput.type = 'text';
    toggleSenha.classList.remove('fa-eye');
    toggleSenha.classList.add('fa-eye-slash');
} else {
    senhaInput.type = 'password';
    toggleSenha.classList.remove('fa-eye-slash');
    toggleSenha.classList.add('fa-eye');
}
});

toggleConfirmSenha.addEventListener('click', function () {
    if (InputConfirmarSenha.type === 'password') {
        InputConfirmarSenha.type = 'text';
        toggleSenha2.classList.remove('fa-eye');
        toggleSenha2.classList.add('fa-eye-slash');
    } else {
        InputConfirmarSenha.type = 'password';
        toggleSenha2.classList.remove('fa-eye-slash');
        toggleSenha2.classList.add('fa-eye');
    }
});


