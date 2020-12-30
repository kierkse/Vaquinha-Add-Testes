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
        
        private readonly Mock<ILogger<HomeController>> _logger = new Mock<ILogger<HomeController>>();
        private Mock<IToastNotification> _toastNotification = new Mock<IToastNotification>();

        private readonly Doacao _doacaoValida;
        private readonly DoacaoViewModel _doacaoModelValida;
        
        public HomeControllerTests(DoacaoFixture doacaoFixture, EnderecoFixture enderecoFixture, CartaoCreditoFixture cartaoCreditoFixture)
        {
            _doacaoFixture = doacaoFixture;
            _enderecoFixture = enderecoFixture;
            _cartaoCreditoFixture = cartaoCreditoFixture;

            _mapper = new Mock<IMapper>();

            _doacaoValida = doacaoFixture.DoacaoValida();
            _doacaoValida.AdicionarEnderecoCobranca(enderecoFixture.EnderecoValido());
            _doacaoValida.AdicionarFormaPagamento(cartaoCreditoFixture.CartaoCreditoValido());

            _doacaoModelValida = doacaoFixture.DoacaoModelValida();
            _doacaoModelValida.EnderecoCobranca = enderecoFixture.EnderecoModelValido();
            _doacaoModelValida.FormaPagamento = cartaoCreditoFixture.CartaoCreditoModelValido();

            _mapper.Setup(a => a.Map<DoacaoViewModel, Doacao>(_doacaoModelValida)).Returns(_doacaoValida);

            _doacaoService = new DoacaoService(_mapper.Object, _doacaoRepository.Object, _domainNotificationService);

            DbContextOptions<VaquinhaOnlineDBContext> _options = new DbContextOptionsBuilder<VaquinhaOnlineDBContext>()
                .UseInMemoryDatabase(databaseName: "VaquinhaOnlineDIOTests")
                .Options;
            var _context = new VaquinhaOnlineDBContext(_options);

            var _homeRepository = new HomeInfoRepository(_context);
            var _causaRepository = new CausaRepository(_context);

            _homeInfoService = new HomeInfoService(_mapper.Object, _doacaoService, _globallAppConfig.Object, _homeRepository, _causaRepository);
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

        [Trait("HomeController", "HomeControllerPrivacy_RetornaViewResult")]
        [Fact]
        public void HomeControllerPrivacy_RetornaViewResult()
        {
            //Arrange
            var _HomeController = new HomeController(_logger.Object, _homeInfoService, _toastNotification.Object);

            //Act
            var retorno = _HomeController.Privacy();

            //Assert
            retorno.Should().BeOfType<ViewResult>();
        }
    }
}
