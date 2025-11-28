using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entidades;

public class Administrador
{
    [Key] // Informa que é chave primária
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Informa que é incremental
    public int Id { get; set; } = default!;

    [Required] // Informa que o e-mail será obrigatório
    [StringLength(255)] // tamanho de 255 caracteres
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(50)] 
    public string Senha { get; set; } = default!;

    [Required]
    [StringLength(10)] 
    public string Perfil { get; set; } = default!;

}