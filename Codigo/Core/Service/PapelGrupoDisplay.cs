namespace Core.Service
{
    /// <summary>
    /// Converte o nome interno de um papel (armazenado no banco e usado em autorizações)
    /// para o rótulo exibido ao usuário. O valor interno NÃO muda, apenas o texto mostrado.
    /// </summary>
    public static class PapelGrupoDisplay
    {
        public const string ChefeDeNaipe = "Chefe de Naipe";

        public static string? Nome(string? nomeInterno)
        {
            if (string.IsNullOrWhiteSpace(nomeInterno))
            {
                return nomeInterno;
            }

            return nomeInterno.Trim().ToUpperInvariant() == "COLABORADOR"
                ? ChefeDeNaipe
                : nomeInterno;
        }
    }
}
