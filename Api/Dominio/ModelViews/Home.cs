namespace MinimalApi.Dominio.ModelViewes;

public struct Home{

    public string Mensagem {get => "Bem vindo à API de veículos - Minimal API ";} 

    public string Doc { get => "/swagger";} // vai ser um link na home onde teremos a documentação

}