using MinimalApi.Dominio.Enuns;

namespace MinimalApi.Dominio.ModelViewes;

public record AdministradorModelView{

        public int Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public String Perfil { get; set; } = default!; // foi criado o enum Perfil

    }