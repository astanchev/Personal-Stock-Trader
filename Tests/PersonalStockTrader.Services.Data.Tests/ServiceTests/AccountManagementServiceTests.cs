namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using MockQueryable.Moq;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Data.Repositories;
    using PersonalStockTrader.Services.Data.Tests.ServiceTests.Helpers;

    [TestFixture]
    public class AccountManagementServiceTests
    {
        private Mock<IDeletableEntityRepository<ApplicationUser>> userRepository;
        private Mock<UserManager<ApplicationUser>> userManager;
        private Mock<IDeletableEntityRepository<Account>> accountRepository;
        private Mock<IPositionsService> positionService;
        private Mock<IRepository<FeePayment>> feePaymentsRepository;
        private IAccountManagementService accountManagementService;
        private Mock<IQueryable<Account>> mock;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            this.userManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            this.mock = TestDataHelpers.GetTestData().AsQueryable().BuildMock();
            this.accountRepository = new Mock<IDeletableEntityRepository<Account>>();
            this.accountRepository
                .Setup(a => a.All())
                .Returns(this.mock.Object);

            this.userRepository = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            this.positionService = new Mock<IPositionsService>();
            this.feePaymentsRepository = new Mock<IRepository<FeePayment>>();

            this.accountManagementService = new AccountManagementService(this.userManager.Object, this.userRepository.Object, this.accountRepository.Object, this.positionService.Object, this.feePaymentsRepository.Object);
        }
    }
}