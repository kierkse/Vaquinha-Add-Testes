using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Vaquinha.Tests.Common.Fixtures;
using Xunit;

namespace Vaquinha.AutomatedUITests
{
	public class DoacaoTests : IDisposable, IClassFixture<DoacaoFixture>, 
                                               IClassFixture<EnderecoFixture>, 
                                               IClassFixture<CartaoCreditoFixture>
	{
		private DriverFactory _driverFactory = new DriverFactory();
		private IWebDriver _driver;

		private readonly DoacaoFixture _doacaoFixture;
		private readonly EnderecoFixture _enderecoFixture;
		private readonly CartaoCreditoFixture _cartaoCreditoFixture;

		public DoacaoTests(DoacaoFixture doacaoFixture, EnderecoFixture enderecoFixture, CartaoCreditoFixture cartaoCreditoFixture)
        {
            _doacaoFixture = doacaoFixture;
            _enderecoFixture = enderecoFixture;
            _cartaoCreditoFixture = cartaoCreditoFixture;
        }
		public void Dispose()
		{
			_driverFactory.Close();
		}

		[Fact]
		public void DoacaoUI_AcessoTelaHome()
		{
			// Arrange
			_driverFactory.NavigateToUrl("https://vaquinha.azurewebsites.net/");
			_driver = _driverFactory.GetWebDriver();

			// Act
			IWebElement webElement = null;
			webElement = _driver.FindElement(By.ClassName("vaquinha-logo"));

			// Assert
			webElement.Displayed.Should().BeTrue(because:"logo exibido");
		}
		[Fact]
		public void DoacaoUI_CriacaoDoacao()
		{
			//Arrange
			var doacao = _doacaoFixture.DoacaoValida();
            doacao.AdicionarEnderecoCobranca(_enderecoFixture.EnderecoValido());
            doacao.AdicionarFormaPagamento(_cartaoCreditoFixture.CartaoCreditoValido());
			_driverFactory.NavigateToUrl("https://vaquinha.azurewebsites.net/");
			_driver = _driverFactory.GetWebDriver();

			//Act
			IWebElement webElement = null;
			webElement = _driver.FindElement(By.ClassName("btn-yellow"));
			webElement.Click();

			//Dados Pessoais
			IWebElement campoNome = null;
			campoNome = _driver.FindElement(By.Id("DadosPessoais_Nome"));
			campoNome.SendKeys(doacao.DadosPessoais.Nome);
			IWebElement campoEmail = null;
			campoEmail = _driver.FindElement(By.Id("DadosPessoais_Email"));
			campoEmail.SendKeys(doacao.DadosPessoais.Email);
			IWebElement campoMensagemApoio = null;
			campoMensagemApoio = _driver.FindElement(By.Id("DadosPessoais_MensagemApoio"));
			campoMensagemApoio.SendKeys(doacao.DadosPessoais.MensagemApoio);

			//Dados Endereço
			IWebElement enderecoTexto = null;
			enderecoTexto = _driver.FindElement(By.Id("EnderecoCobranca_TextoEndereco"));
			enderecoTexto.SendKeys(doacao.EnderecoCobranca.TextoEndereco);
			IWebElement enderecoNumero = null;
			enderecoNumero = _driver.FindElement(By.Id("EnderecoCobranca_Numero"));
			enderecoNumero.SendKeys(doacao.EnderecoCobranca.Numero);
			IWebElement enderecoCidade = null;
			enderecoCidade = _driver.FindElement(By.Id("EnderecoCobranca_Cidade"));
			enderecoCidade.SendKeys(doacao.EnderecoCobranca.Cidade);
			IWebElement enderecoEstado = null;
			enderecoEstado = _driver.FindElement(By.Id("estado"));
			enderecoEstado.SendKeys(doacao.EnderecoCobranca.Estado);
			IWebElement enderecoCEP = null;
			enderecoCEP = _driver.FindElement(By.Id("cep"));
			enderecoCEP.SendKeys(doacao.EnderecoCobranca.CEP);
			IWebElement enderecoComplemento = null;
			enderecoComplemento = _driver.FindElement(By.Id("EnderecoCobranca_Complemento"));
			enderecoComplemento.SendKeys(doacao.EnderecoCobranca.Complemento);
			IWebElement enderecoTelefone = null;
			enderecoTelefone = _driver.FindElement(By.Id("telefone"));
			enderecoTelefone.SendKeys(doacao.EnderecoCobranca.Telefone);

			//Dados Pagamento
			IWebElement pagamentoTitular = null;
			pagamentoTitular = _driver.FindElement(By.Id("FormaPagamento_NomeTitular"));
			pagamentoTitular.SendKeys(doacao.FormaPagamento.NomeTitular);
			IWebElement pagamentoCartao = null;
			pagamentoCartao = _driver.FindElement(By.Id("cardNumber"));
			pagamentoCartao.SendKeys(doacao.FormaPagamento.NumeroCartaoCredito);
			IWebElement pagamentoValidade = null;
			pagamentoValidade = _driver.FindElement(By.Id("validade"));
			pagamentoValidade.SendKeys(doacao.FormaPagamento.Validade);
			IWebElement pagamentoCvv = null;
			pagamentoCvv = _driver.FindElement(By.Id("cvv"));
			pagamentoCvv.SendKeys(doacao.FormaPagamento.CVV);

			//Clicar em Doar
			IWebElement btnDoar = null;
			btnDoar = _driver.FindElement(By.ClassName("btn-yellow"));
			btnDoar.Click();

			//Assert
			_driver.Url.Should().Contain("/Doacoes/Index");
		}
	}
}