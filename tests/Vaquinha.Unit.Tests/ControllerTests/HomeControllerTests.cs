using Microsoft.Extensions.Logging;
using Moq;
using Vaquinha.MVC.Controllers;
using Vaquinha.Domain;
using NToastNotify;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Vaquinha.Service;
using AutoMapper;
using Vaquinha.Tests.Common.Fixtures;
using Vaquinha.Domain.Entities;
using Vaquinha.Domain.ViewModels;
using Vaquinha.Repository.Context;
using Vaquinha.Repository;
using System;
using Microsoft.EntityFrameworkCore;
using Vaquinha.Unit.Tests.Fixture;

namespace Vaquinha.Unit.Tests.ControllerTests
{
    [Collection("ActorProjectCollection")]
    public class HomeControllerTests : IClassFixture<DoacaoFixture>,
                                    IClassFixture<EnderecoFixture>,
                                    IClassFixture<CartaoCreditoFixture>,
                                    IClassFixture<DependencyInjectionFixture>
    {
        private readonly Mock<IDoacaoRepository> _doacaoRepository = new Mock<IDoacaoRepository>();
        
        private IDomainNotificationService _domainNotificationService = new DomainNotificationService();

        private readonly IHomeInfoService _homeInfoService;
        
        private readonly DoacaoFixture _doacaoFixture;
        private readonly EnderecoFixture _enderecoFixture;
        private readonly CartaoCreditoFixture _cartaoCreditoFixture;
        private readonly DependencyInjectionFixture _dependencyFixture;
        
        private Mock<IMapper> _mapper;
        private readonly IDoacaoService _doacaoService;
        private readonly Mock<GloballAppConfig> _globallAppConfig = new Mock<GloballAppConfig>();
        private Mock<IHomeInfoRepository> _homeRepository;
        private Mock<ICausaRepository> _causaRepository = new Mock<ICausaRepository>();
        
        private readonly Mock<ILogger<HomeController>> _logger = new Mock<ILogger<HomeController>>();
        private Mock<IToastNotification> _toastNotification = new Mock<IToastNotification>();

        private readonly Doacao _doacaoValida;
        private readonly DoacaoViewModel _doacaoModelValida;
        
        public HomeControllerTests(DoacaoFixture doacaoFixture, EnderecoFixture enderecoFixture, CartaoCreditoFixture cartaoCreditoFixture, DependencyInjectionFixture dependencyFixture)
        {
            _doacaoFixture = doacaoFixture;
            _enderecoFixture = enderecoFixture;
            _cartaoCreditoFixture = cartaoCreditoFixture;
            _dependencyFixture = dependencyFixture;

            _mapper = new Mock<IMapper>();

            _doacaoValida = doacaoFixture.DoacaoValida();
            _doacaoValida.AdicionarEnderecoCobranca(enderecoFixture.EnderecoValido());
            _doacaoValida.AdicionarFormaPagamento(cartaoCreditoFixture.CartaoCreditoValido());

            _doacaoModelValida = doacaoFixture.DoacaoModelValida();
            _doacaoModelValida.EnderecoCobranca = enderecoFixture.EnderecoModelValido();
            _doacaoModelValida.FormaPagamento = cartaoCreditoFixture.CartaoCreditoModelValido();

            _mapper.Setup(a => a.Map<DoacaoViewModel, Doacao>(_doacaoModelValida)).Returns(_doacaoValida);

            _doacaoService = new DoacaoService(_mapper.Object, _doacaoRepository.Object, _domainNotificationService);

            // var _doacoeSet = new Mock<DbSet<Doacao>>();
            // var _causaSet = new Mock<DbSet<Causa>>();
            // var _pessoaSet = new Mock<DbSet<Pessoa>>();
            // var _enderecoSet = new Mock<DbSet<Endereco>>();
            var _context = new Mock<VaquinhaOnlineDBContext>();
            // _context.Setup(m => m.Doacoes);
            // _context.Setup(m => m.Causas).Returns(_causaSet.Object);
            // _context.Setup(m => m.Enderecos).Returns(_enderecoSet.Object);
            // _context.Setup(m => m.Causas).Returns(_causaSet.Object);

            _homeRepository = new Mock<IHomeInfoRepository>(dependencyFixture.services);
            _causaRepository = new Mock<ICausaRepository>(_context.Object);

            _homeInfoService = new HomeInfoService(_mapper.Object, _doacaoService, _globallAppConfig.Object, _homeRepository.Object, _causaRepository.Object);
        }

        [Trait("HomeController", "HomeControllerIndex_RetornaViewResult")]
        [Fact]
        public async void HomeControllerIndex_RetornaViewResult()
        {
            //Arrange
            var _HomeController = new HomeController(_logger.Object, _homeInfoService, _toastNotification.Object);

            //Act
            var retorno = await _HomeController.Index();

            //Assert
            retorno.Should().BeOfType<ViewResult>();
        }
    }
}
