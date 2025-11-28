using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entidades;

public class Veiculo
{
    [Key] // Informa que é chave primária
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Informa que é incremental
    public int Id { get; set; } = default!;

    [Required] // Informa que o e-mail será obrigatório
    [StringLength(150)] // tamanho de 255 caracteres
    public string nome { get; set; } = default!;

    [Required]
    [StringLength(100)] 
    public string Marca { get; set; } = default!;

    [Required]
    public int Ano { get; set; } = default!;

}