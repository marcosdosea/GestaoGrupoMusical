using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Controllers;
using GestaoGrupoMusicalWeb.Mapper;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Security.Claims;
using Core.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace GestaoGrupoMusicalWeb.Controllers.Tests
{
    [TestClass()]
    public class FigurinoControllerTests
    {
        private static FigurinoController _controller;
        private static Mock<IMapper> _mockMapper;
        private static Mock<IGrupoMusicalService> _mockGrupoMusicalService;
        private static Mock<IManequimService> _mockManequimService;
        private static Mock<IFigurinoService> _mockFigurinoService;
        private static Mock<IPessoaService> _mockPessoaService;
        private static Mock<IMovimentacaoFigurinoService> _mockMovimentacaoService;
        private readonly Mock<ILogger<FigurinoController>> _mockLogger = new Mock<ILogger<FigurinoController>>();
        private static Mock<UserManager<UsuarioIdentity>> _mockUserManager;

        [TestInitialize]
        public void Initialize()
        {
            _mockMapper = new Mock<IMapper>();
            _mockGrupoMusicalService = new Mock<IGrupoMusicalService>();
            _mockManequimService = new Mock<IManequimService>();
            _mockFigurinoService = new Mock<IFigurinoService>();
            _mockPessoaService = new Mock<IPessoaService>();
            _mockMovimentacaoService = new Mock<IMovimentacaoFigurinoService>();
            _mockUserManager = new Mock<UserManager<UsuarioIdentity>>(
                Mock.Of<IUserStore<UsuarioIdentity>>(), null, null, null, null, null, null, null, null
            );

            // Configuração dos mocks
            SetupMocks();

            // Mocking User property
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("IdGrupoMusical", "1"),
                new Claim(ClaimTypes.Name, "teste@test.com")
            }));

            _controller = new FigurinoController(
                _mockMapper.Object,
                _mockGrupoMusicalService.Object,
                _mockManequimService.Object,
                _mockFigurinoService.Object,
                _mockUserManager.Object,
                _mockPessoaService.Object,
                _mockMovimentacaoService.Object,
                _mockLogger.Object
            )
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                },
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            // Limpar o ModelState antes de cada teste
            _controller.ModelState.Clear();
        }

        private void SetupMocks()
        {
            _mockMapper.Setup(m => m.Map<IEnumerable<FigurinoViewModel>>(It.IsAny<IEnumerable<Figurino>>()))
                .Returns(GetTestFigurinosViewModel());
            _mockMapper.Setup(m => m.Map<FigurinoViewModel>(It.IsAny<Figurino>()))
                .Returns(GetTargetFigurinoViewModel());
            _mockMapper.Setup(m => m.Map<Figurino>(It.IsAny<FigurinoViewModel>()))
                .Returns(GetTargetFigurino());

            _mockFigurinoService.Setup(service => service.GetAll(It.IsAny<int>()))
                .ReturnsAsync(GetTestFigurinos());
            _mockFigurinoService.Setup(service => service.Create(It.IsAny<Figurino>()))
                .ReturnsAsync(HttpStatusCode.Created);
            _mockFigurinoService.Setup(service => service.Edit(It.IsAny<Figurino>()))
                .ReturnsAsync(HttpStatusCode.OK);
            _mockFigurinoService.Setup(service => service.Get(It.IsAny<int>()))
                .ReturnsAsync(GetTargetFigurino());
            _mockFigurinoService.Setup(service => service.Delete(It.IsAny<int>()))
                .ReturnsAsync(HttpStatusCode.OK);
            _mockFigurinoService.Setup(service => service.CreateEstoque(It.IsAny<Figurinomanequim>()))
                .ReturnsAsync(HttpStatusCode.Created);
            _mockFigurinoService.Setup(service => service.GetAllEstoqueDTO(It.IsAny<int>()))
                .ReturnsAsync(GetTestEstoqueDTO());

            _mockManequimService.Setup(service => service.GetAll())
                .Returns(GetTestManequins());

            _mockGrupoMusicalService.Setup(service => service.GetIdGrupo(It.IsAny<string>()))
                .ReturnsAsync(1);

            var associados = GetTestPessoas().ToList();
            _mockPessoaService.Setup(service => service.GetAllPessoasOrder(It.IsAny<int>()))
                .Returns(associados);
            _mockPessoaService.Setup(service => service.GetByCpf(It.IsAny<string>()))
                .ReturnsAsync(GetTargetUserDTO());

            _mockMovimentacaoService.Setup(service => service.GetAllByIdFigurino(It.IsAny<int>()))
                .ReturnsAsync(GetTestMovimentacaoFigurinoDTO());
            _mockMovimentacaoService.Setup(service => service.CreateAsync(It.IsAny<Movimentacaofigurino>()))
                .ReturnsAsync(HttpStatusCode.OK);
        }

        private void SetupInvalidModelState(string propertyName, string errorMessage)
        {
            _controller.ModelState.Clear();
            _controller.ModelState.AddModelError(propertyName, errorMessage);
            Assert.IsFalse(_controller.ModelState.IsValid, "ModelState deve ser inválido");
        }

        [TestMethod()]
        public async Task IndexTest()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<FigurinoViewModel>));
            IEnumerable<FigurinoViewModel> listFigurinos = (IEnumerable<FigurinoViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(2, listFigurinos.Count());
        }

        [TestMethod()]
        public void CreateTest_Get()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public async Task CreateTest_Post_Valid()
        {
            // Act
            var result = await _controller.Create(GetTargetFigurinoViewModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public async Task CreateTest_Post_Invalid()
        {
            // Arrange
            var figurinoViewModel = GetTargetFigurinoViewModel();
            SetupInvalidModelState("Nome", "Nome é obrigatório");

            // Act
            var result = await _controller.Create(figurinoViewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "O resultado deve ser um ViewResult quando o modelo é inválido");
            ViewResult viewResult = (ViewResult)result;
            Assert.IsNull(viewResult.ViewData.Model, "O modelo deve ser nulo quando o ModelState é inválido");
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod()]
        public async Task EditTest_Get()
        {
            // Arrange
            var figurinoId = 1;
            var expectedFigurino = GetTargetFigurino();

            _mockFigurinoService.Setup(service => service.Get(figurinoId))
                .ReturnsAsync(expectedFigurino);

            // Act
            var result = await _controller.Edit(figurinoId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(FigurinoViewModel));
            var model = (FigurinoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(expectedFigurino.Id, model.Id);
            Assert.AreEqual(expectedFigurino.Nome, model.Nome);
        }

        [TestMethod()]
        public async Task EditTest_Post_Valid()
        {
            // Arrange
            var figurinoViewModel = GetTargetFigurinoViewModel();

            // Act
            var result = await _controller.Edit(figurinoViewModel.Id, figurinoViewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(FigurinoController.Index), redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public async Task EditTest_Post_Invalid()
        {
            // Arrange
            var figurinoViewModel = GetTargetFigurinoViewModel();
            SetupInvalidModelState("Nome", "Nome é obrigatório");

            // Act
            var result = await _controller.Edit(figurinoViewModel.Id, figurinoViewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "O resultado deve ser um ViewResult quando o modelo é inválido");
            ViewResult viewResult = (ViewResult)result;
            Assert.IsNull(viewResult.ViewData.Model, "O modelo deve ser nulo quando o ModelState é inválido");
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod()]
        public async Task DeleteTest_Get()
        {
            // Arrange
            var figurinoId = 1;
            var expectedFigurino = GetTargetFigurino();

            _mockFigurinoService.Setup(service => service.Get(figurinoId))
                .ReturnsAsync(expectedFigurino);

            // Act
            var result = await _controller.Delete(figurinoId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(FigurinoViewModel));
            var model = (FigurinoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(expectedFigurino.Id, model.Id);
            Assert.AreEqual(expectedFigurino.Nome, model.Nome);
        }

        [TestMethod()]
        public async Task DeleteTest_Post()
        {
            // Arrange
            var figurinoId = 1;
            var figurinoViewModel = GetTargetFigurinoViewModel();

            // Act
            var result = await _controller.Delete(figurinoId, figurinoViewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(FigurinoController.Index), redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public async Task CreateEstoqueTest_Get()
        {
            // Arrange
            var figurinoId = 1;
            var expectedFigurino = GetTargetFigurino();
            var expectedManequins = GetTestManequins();

            _mockFigurinoService.Setup(service => service.Get(figurinoId))
                .ReturnsAsync(expectedFigurino);
            _mockManequimService.Setup(service => service.GetAll())
                .Returns(expectedManequins);

            // Act
            var result = await _controller.CreateEstoque(figurinoId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(CreateEstoqueViewModel));
            var model = (CreateEstoqueViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(expectedFigurino.Id, model.IdFigurino);
            Assert.AreEqual(expectedFigurino.Nome, model.Nome);
            Assert.IsNotNull(model.listManequim);
            Assert.AreEqual(expectedManequins.Count(), model.listManequim.Count());
        }

        [TestMethod()]
        public async Task CreateEstoqueTest_Post_Valid()
        {
            // Arrange
            var estoqueViewModel = GetTargetCreateEstoqueViewModel();

            // Act
            var result = await _controller.CreateEstoque(estoqueViewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(FigurinoController.Estoque), redirectToActionResult.ActionName);
            Assert.AreEqual(estoqueViewModel.IdFigurino, redirectToActionResult.RouteValues["id"]);
        }

        [TestMethod()]
        public async Task CreateEstoqueTest_Post_Invalid()
        {
            // Arrange
            var estoqueViewModel = GetTargetCreateEstoqueViewModel();
            SetupInvalidModelState("QuantidadeDisponivel", "Quantidade é obrigatória");

            // Act
            var result = await _controller.CreateEstoque(estoqueViewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "O resultado deve ser um RedirectToActionResult");
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(FigurinoController.Estoque), redirectToActionResult.ActionName);
            Assert.AreEqual(estoqueViewModel.IdFigurino, redirectToActionResult.RouteValues["id"]);
        }

        [TestMethod()]
        public async Task MovimentarTest_Get()
        {
            // Arrange
            var figurinoId = 1;
            var expectedFigurino = GetTargetFigurino();
            var expectedPessoas = GetTestPessoas().ToList();
            var expectedEstoques = GetTestEstoqueDTO();
            var expectedMovimentacoes = GetTestMovimentacaoFigurinoDTO();

            _mockFigurinoService.Setup(service => service.Get(figurinoId))
                .ReturnsAsync(expectedFigurino);
            
            _mockPessoaService.Setup(service => service.GetAllPessoasOrder(It.IsAny<int>()))
                .Returns(expectedPessoas);
            
            _mockFigurinoService.Setup(service => service.GetAllEstoqueDTO(figurinoId))
                .ReturnsAsync(expectedEstoques);
            
            _mockMovimentacaoService.Setup(service => service.GetAllByIdFigurino(figurinoId))
                .ReturnsAsync(expectedMovimentacoes);

            _mockMovimentacaoService.Setup(service => service.GetEstoque(figurinoId))
                .ReturnsAsync(expectedEstoques);

            // Act
            var result = await _controller.Movimentar(figurinoId, null, "", "");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(MovimentacaoFigurinoViewModel));
            
            var model = (MovimentacaoFigurinoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(expectedFigurino.Id, model.IdFigurino);
            Assert.AreEqual(expectedFigurino.Nome, model.NomeFigurino);
            
            Assert.IsNotNull(model.ListaAssociado, "ListaAssociado não deve ser nula");
            Assert.AreEqual(expectedPessoas.Count, model.ListaAssociado.Count(), 
                $"Esperado {expectedPessoas.Count} associados, mas recebeu {model.ListaAssociado.Count()}");
            
            Assert.IsNotNull(model.ListaManequim);
            Assert.AreEqual(expectedEstoques.Count(), model.ListaManequim.Count());
            
            Assert.IsNotNull(model.Movimentacoes);
            Assert.AreEqual(expectedMovimentacoes.Count(), model.Movimentacoes.Count());

            // Verify mocks
            _mockPessoaService.Verify(service => service.GetAllPessoasOrder(It.IsAny<int>()), Times.Once);
            _mockGrupoMusicalService.Verify(service => service.GetIdGrupo(It.IsAny<string>()), Times.Once);
            _mockMovimentacaoService.Verify(service => service.GetEstoque(figurinoId), Times.Once);
        }

        [TestMethod()]
        public async Task MovimentarTest_Post_Valid()
        {
            // Arrange
            var movimentacaoViewModel = GetTargetMovimentacaoFigurinoViewModel();

            // Act
            var result = await _controller.Movimentar(movimentacaoViewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(FigurinoController.Movimentar), redirectToActionResult.ActionName);
            Assert.AreEqual(movimentacaoViewModel.IdFigurino, redirectToActionResult.RouteValues["id"]);
        }

        [TestMethod()]
        public async Task MovimentarTest_Post_Invalid()
        {
            // Arrange
            var movimentacaoViewModel = GetTargetMovimentacaoFigurinoViewModel();
            SetupInvalidModelState("Quantidade", "Quantidade é obrigatória");

            // Act
            var result = await _controller.Movimentar(movimentacaoViewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "O resultado deve ser um RedirectToActionResult");
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(FigurinoController.Movimentar), redirectToActionResult.ActionName);
            Assert.AreEqual(movimentacaoViewModel.IdFigurino, redirectToActionResult.RouteValues["id"]);
        }

        private IEnumerable<Figurino> GetTestFigurinos()
        {
            return new List<Figurino>
            {
                new Figurino
                {
                    Id = 1,
                    Nome = "Figurino Teste 1",
                    Data = new DateTime(2023, 10, 26),
                    IdGrupoMusical = 1
                },
                new Figurino
                {
                    Id = 2,
                    Nome = "Figurino Teste 2",
                    Data = new DateTime(2023, 11, 15),
                    IdGrupoMusical = 1
                }
            };
        }

        private Figurino GetTargetFigurino()
        {
            return new Figurino
            {
                Id = 1,
                Nome = "Figurino Teste 1",
                Data = new DateTime(2023, 10, 26),
                IdGrupoMusical = 1
            };
        }

        private IEnumerable<FigurinoViewModel> GetTestFigurinosViewModel()
        {
            return new List<FigurinoViewModel>
            {
                new FigurinoViewModel
                {
                    Id = 1,
                    Nome = "Figurino Teste 1",
                    Data = new DateTime(2023, 10, 26),
                    IdGrupoMusical = 1
                },
                new FigurinoViewModel
                {
                    Id = 2,
                    Nome = "Figurino Teste 2",
                    Data = new DateTime(2023, 11, 15),
                    IdGrupoMusical = 1
                }
            };
        }

        private FigurinoViewModel GetTargetFigurinoViewModel()
        {
            return new FigurinoViewModel
            {
                Id = 1,
                Nome = "Figurino Teste 1",
                Data = new DateTime(2023, 10, 26),
                IdGrupoMusical = 1
            };
        }

        private IEnumerable<Manequim> GetTestManequins()
        {
            return new List<Manequim>
            {
                new Manequim
                {
                    Id = 1,
                    Tamanho = "P"
                },
                new Manequim
                {
                    Id = 2,
                    Tamanho = "M"
                }
            };
        }

        private CreateEstoqueViewModel GetTargetCreateEstoqueViewModel()
        {
            return new CreateEstoqueViewModel
            {
                IdFigurino = 1,
                Nome = "Figurino Teste 1",
                Data = "26/10/2023",
                QuantidadeDisponivel = 10,
                IdManequim = 1
            };
        }

        private IEnumerable<AssociadoDTO> GetTestAssociadosDTO()
        {
            return new List<AssociadoDTO>
            {
                new AssociadoDTO
                {
                    Id = 1,
                    Nome = "Associado Teste 1",
                    Cpf = "111.111.111-11"
                },
                new AssociadoDTO
                {
                    Id = 2,
                    Nome = "Associado Teste 2",
                    Cpf = "222.222.222-22"
                }
            };
        }

        private IEnumerable<EstoqueDTO> GetTestEstoqueDTO()
        {
            return new List<EstoqueDTO>
            {
                new EstoqueDTO
                {
                    IdFigurino = 1,
                    IdManequim = 1,
                    Tamanho = "P",
                    Disponivel = 5,
                    Entregues = 2
                },
                new EstoqueDTO
                {
                    IdFigurino = 1,
                    IdManequim = 2,
                    Tamanho = "M",
                    Disponivel = 3,
                    Entregues = 1
                }
            };
        }

        private IEnumerable<MovimentacaoFigurinoDTO> GetTestMovimentacaoFigurinoDTO()
        {
            return new List<MovimentacaoFigurinoDTO>
            {
                new MovimentacaoFigurinoDTO
                {
                    Id = 1,
                    IdFigurino = 1,
                    IdManequim = 1,
                    Cpf = "111.111.111-11",
                    NomeAssociado = "Associado Teste 1",
                    Data = new DateTime(2023, 10, 20),
                    Tamanho = "P",
                    Movimentacao = "ENTREGUE",
                    QuantidadeEntregue = 1,
                    Status = "ATIVO"
                },
                new MovimentacaoFigurinoDTO
                {
                    Id = 2,
                    IdFigurino = 1,
                    IdManequim = 2,
                    Cpf = "222.222.222-22",
                    NomeAssociado = "Associado Teste 2",
                    Data = new DateTime(2023, 10, 25),
                    Tamanho = "M",
                    Movimentacao = "DEVOLVIDO",
                    QuantidadeEntregue = 1,
                    Status = "FINALIZADO"
                }
            };
        }

        private MovimentacaoFigurinoViewModel GetTargetMovimentacaoFigurinoViewModel()
        {
            return new MovimentacaoFigurinoViewModel
            {
                IdFigurino = 1,
                NomeFigurino = "Figurino Teste 1",
                Data = new DateTime(2023, 10, 26),
                IdManequim = 1,
                IdAssociado = 1,
                Quantidade = 1,
                Movimentacao = "ENTREGUE"
            };
        }

        private IEnumerable<Pessoa> GetTestPessoas()
        {
            return new List<Pessoa>
            {
                new Pessoa
                {
                    Id = 1,
                    Nome = "Associado Teste 1",
                    Cpf = "111.111.111-11",
                    IdGrupoMusical = 1
                },
                new Pessoa
                {
                    Id = 2,
                    Nome = "Associado Teste 2",
                    Cpf = "222.222.222-22",
                    IdGrupoMusical = 1
                }
            };
        }

        private UserDTO GetTargetUserDTO()
        {
            return new UserDTO
            {
                Id = 1,
                Nome = "Teste Pessoa",
                IdGrupoMusical = 1,
                Papel = "COLABORADOR"
            };
        }
    }
} 