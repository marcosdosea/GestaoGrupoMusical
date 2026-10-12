namespace Email
{
    public class EmailModel
    {
        public string From { get; set; } = string.Empty;
        public string Assunto { get; set; } = string.Empty;
        public List<string> To { get; set; } = new();
        public string Subject { get; set; } = "Gestão Grupo Musical";
        public string AddresseeName { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
    }
}
