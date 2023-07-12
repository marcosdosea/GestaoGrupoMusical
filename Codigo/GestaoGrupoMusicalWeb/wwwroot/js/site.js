// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


<script>
    const toggleSenha = document.querySelector('#toggleSenha');
    const senhaInput = document.querySelector('#senha');

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
</script>

