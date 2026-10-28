// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Sidebar no celular: abre por cima do conteúdo, fecha ao tocar no fundo
// e não altera a preferência (aberta/fechada) salva para o desktop
const sidebarToggle = document.querySelector('#sidebarToggle');
const sidebarBackdrop = document.querySelector('#sidebarBackdrop');
const telaCelular = window.matchMedia('(max-width: 767.98px)');
const chaveSidebar = 'sb|sidebar-toggle';

if (sidebarToggle != null) {
    let preferenciaDesktop = localStorage.getItem(chaveSidebar);

    const aplicarEstadoSidebar = function () {
        if (telaCelular.matches) {
            document.body.classList.remove('sb-sidenav-toggled');
        } else {
            document.body.classList.toggle('sb-sidenav-toggled', preferenciaDesktop === 'true');
        }
    };

    window.addEventListener('DOMContentLoaded', function () {
        aplicarEstadoSidebar();
        sidebarToggle.addEventListener('click', function () {
            if (telaCelular.matches) {
                if (preferenciaDesktop === null) {
                    localStorage.removeItem(chaveSidebar);
                } else {
                    localStorage.setItem(chaveSidebar, preferenciaDesktop);
                }
            } else {
                preferenciaDesktop = localStorage.getItem(chaveSidebar);
            }
        });
    });

    telaCelular.addEventListener('change', aplicarEstadoSidebar);

    if (sidebarBackdrop != null) {
        sidebarBackdrop.addEventListener('click', function () {
            document.body.classList.remove('sb-sidenav-toggled');
        });
    }
}

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
