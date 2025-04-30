using Microsoft.AspNetCore.Identity;

namespace APIDevSteam.Models
{
    public class Usuario : IdentityUser
    {
        public string? NomeCompleto { get; set; }
        public DateOnly DataNascimento { get; set; }
        public string? Cpf { get; set; }
        public string? ChavePix { get; set; }
        public string? NumeroCartao { get; set; }

        public Usuario() : base()
        {
        }
    }
}
