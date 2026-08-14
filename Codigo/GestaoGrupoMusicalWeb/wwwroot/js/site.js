// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const toggleSenha = document.querySelector('#toggleSenha');
const toggleConfirmSenha = document.querySelector('#toggleSenha2');
const toggleSenhaAtual = document.querySelector('#toggleSenha3');
const senhaInput = document.querySelector('#InputSenha');

if(toggleSenha != null){
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
}

if(toggleConfirmSenha != null){
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
}

if(toggleSenhaAtual != null){
    toggleSenhaAtual.addEventListener('click', function () {
        if (InputSenhaAtual.type === 'password') {
            InputSenhaAtual.type = 'text';
            toggleSenha3.classList.remove('fa-eye');
            toggleSenha3.classList.add('fa-eye-slash');
        } else {
            InputSenhaAtual.type = 'password';
            toggleSenha3.classList.remove('fa-eye-slash');
            toggleSenha3.classList.add('fa-eye');
        }
    });
}

// Sidebar: overlay fechado por padrão, aberto somente por interação do usuário.
// O estado não é persistido (sem localStorage) para garantir que toda página
// carregue sempre com a sidebar fechada, conforme exigido.
document.addEventListener('DOMContentLoaded', function () {
    const body = document.body;
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebarCloseBtn = document.getElementById('sidebarCloseBtn');
    const sidebarHeaderCloseBtn = document.getElementById('sidebarHeaderCloseBtn');
    const sidebarBackdrop = document.getElementById('sidebarBackdrop');

    function openSidebar() {
        body.classList.add('sb-sidenav-toggled');
    }

    function closeSidebar() {
        body.classList.remove('sb-sidenav-toggled');
    }

    function toggleSidebar(event) {
        event.preventDefault();
        body.classList.toggle('sb-sidenav-toggled');
    }

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', toggleSidebar);
    }

    [sidebarCloseBtn, sidebarHeaderCloseBtn].forEach(function (btn) {
        if (btn) {
            btn.addEventListener('click', function (e) {
                e.preventDefault();
                closeSidebar();
            });
        }
    });

    if (sidebarBackdrop) {
        sidebarBackdrop.addEventListener('click', closeSidebar);
    }

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape' && body.classList.contains('sb-sidenav-toggled')) {
            closeSidebar();
        }
    });
});





