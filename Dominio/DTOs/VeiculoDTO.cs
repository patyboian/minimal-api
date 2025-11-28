namespace MinimalApi.DTOs;

public record VeiculoDTO{ //record é uma instancia menor do que classe, pq vamos trabalhar de forma simples
    
    public string Nome { get; set; } = default!;

    public string Marca { get; set; } = default!;

      public int Ano { get; set; } = default!;
}