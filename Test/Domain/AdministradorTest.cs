namespace Domain.Test
{
    [TestClass]
    public class AdministradorTest
    {
        [TestMethod]
        public void TestarGetSetPrioridades()
        {
            //Arrange
            var adm = new Administrador();
            var prioridade = 1;
            //Act
            adm.Id = 1;
            adm.Email = "teste@teste.com";
            adm.Senha = "123456";
            adm.Perfil = "Administrador";

            //Assert
            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("teste@teste.com", adm.Email);
            Assert.AreEqual("123456", adm.Senha);
            Assert.AreEqual("Administrador", adm.Perfil);
        }
    }
}
