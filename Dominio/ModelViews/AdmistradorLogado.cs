
namespace MinimalApi.Dominio.ModelViewes;
public record AdmistradorLogado{

        public string Email { get; set; } = default!;
        public string Perfil { get; set; } = default!;
        public String Token { get; set; } = default!; 

    }