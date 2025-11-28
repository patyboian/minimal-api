using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public sealed class VeiculoTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arrange - São todas as variaveis que criaremos para validação
        var veiculo = new Veiculo();

        // Act ação que será executada
        veiculo.Id = 1;
        veiculo.nome = "Onix";
        veiculo.Marca = "Toyota";
        veiculo.Ano = 2020;

        // Assert - validação dos dados
        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual("Onix", veiculo.nome);
        Assert.AreEqual("Toyota", veiculo.Marca);
        Assert.AreEqual(2020, veiculo.Ano);
    }
}