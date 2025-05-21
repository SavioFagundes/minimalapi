namespace Domain.Servicos
{
    [TestClass]
    public class AdministradorServico
    {
        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            //Arrange
            var adm = new Administrador();
            var prioridade = 1;
            //Act
            
            var context = new DbContext();

            //Assert
            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("teste@teste.com", adm.Email);
            Assert.AreEqual("123456", adm.Senha);
            Assert.AreEqual("Administrador", adm.Perfil);
        }
    }
}

