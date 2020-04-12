namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MockQueryable.Moq;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Data.Repositories;
    using PersonalStockTrader.Web.ViewModels.Contact;

    [TestFixture]
    public class ContactFormServiceTests
    {
        private Mock<IRepository<ContactFormEntry>> mockContactFormRepository;
        private Mock<IQueryable<ContactFormEntry>> mock;
        private IContactFormService mockContactFormService;

        [SetUp]
        public void Setup()
        {
            this.mock = this.GetTestData().AsQueryable().BuildMock();

            this.mockContactFormRepository = new Mock<IRepository<ContactFormEntry>>();
            this.mockContactFormRepository
                .Setup(a => a.All())
                .Returns(this.mock.Object);

            this.mockContactFormService = new ContactFormService(this.mockContactFormRepository.Object);
        }

        [Test]
        public async Task AddAsyncShouldAddContactFormEntryCorrectly()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            var contactFormRepository = new EfRepository<ContactFormEntry>(context);
            var contactFormService = new ContactFormService(contactFormRepository);

            var input = new ContactFormViewModel();

            var result = await contactFormService.AddAsync(input);

            Assert.True(result);
        }

        [Test]
        public async Task AddAsyncShouldIncreaseContactFormEntries()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            var contactFormRepository = new EfRepository<ContactFormEntry>(context);
            var contactFormService = new ContactFormService(contactFormRepository);

            var input = new ContactFormViewModel();
            var countPreAdd = contactFormRepository.All().Count();
            var result = await contactFormService.AddAsync(input);
            var countAfterAdd = contactFormRepository.All().Count();

            Assert.AreEqual(countPreAdd + 1, countAfterAdd);
        }

        [Test]
        public async Task MarkAsAnsweredShouldMarkCorrectlyNotAnsweredEmailId()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            var contactFormRepository = new EfRepository<ContactFormEntry>(context);
            var contactFormService = new ContactFormService(contactFormRepository);

            var newEmailId = await this.GetNewEmailId(contactFormService, contactFormRepository);

            var result = await contactFormService.MarkAsAnsweredAsync(newEmailId);

            Assert.True(result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public async Task MarkAsAnsweredShouldNotMarkNotExistingEmailId(int emailId)
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            var contactFormRepository = new EfRepository<ContactFormEntry>(context);
            var contactFormService = new ContactFormService(contactFormRepository);

            await contactFormService.MarkAsAnsweredAsync(emailId);

            var result = await contactFormService.MarkAsAnsweredAsync(emailId);

            Assert.False(result);
        }

        [Test]
        [TestCase(2)]
        [TestCase(1)]
        public async Task GetByIdAsyncWithCorrectEmailIdShouldReturnCorrectly(int emailId)
        {
            var actualEmail = await this.mockContactFormService.GetByIdAsync(emailId);

            Assert.NotNull(actualEmail);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(30)]
        public async Task GetByIdAsyncWithInCorrectEmailIdShouldReturnNull(int emailId)
        {
            var actualEmail = await this.mockContactFormService.GetByIdAsync(emailId);

            Assert.Null(actualEmail);
        }

        [Test]
        public void GetAllNotAnsweredShouldReturnCorrectly()
        {
            var result = this.mockContactFormService.GetAllNotAnswered();

            Assert.AreEqual(10, result.Count());
        }

        [Test]
        public void GetAllCountAsyncShouldReturnCorrectly()
        {
            var result = this.mockContactFormService.GetAllCountAsync();

            Assert.AreEqual(10, result.Result.CountAnswered);
            Assert.AreEqual(10, result.Result.CountNotAnswered);
        }

        [Test]
        public void GetNotAnsweredLast10DaysShouldReturn10days()
        {
            var result = this.mockContactFormService.GetNotAnsweredLast10Days();

            Assert.AreEqual(10, result.Keys.Count);
        }

        [Test]
        public void GetNotAnsweredLast10DaysShouldReturnDictionaryResult()
        {
            var result = this.mockContactFormService.GetNotAnsweredLast10Days();

            Assert.AreEqual(typeof(Dictionary<DateTime, int>), result.GetType());
        }

        private List<ContactFormEntry> GetTestData()
        {
            var result = new List<ContactFormEntry>();
            int id = 1;

            for (DateTime i = DateTime.UtcNow.Date.AddDays(-19); i <= DateTime.UtcNow.Date; i = i.AddDays(1))
            {
                bool answered = id % 2 != 0;

                var entry = new ContactFormEntry
                {
                    Id = id,
                    CreatedOn = i,
                    Answered = answered,
                };

                result.Add(entry);

                id++;
            }

            return result;
        }

        private async Task<int> GetNewEmailId(IContactFormService contactFormService, EfRepository<ContactFormEntry> contactFormRepository)
        {
            var newEmail = new ContactFormViewModel()
            {
                Name = "Test",
                Email = "Test@test.test",
            };

            var result = await contactFormService.AddAsync(newEmail);

            var emailId = contactFormRepository
                .All()
                .Where(e => e.Email == "Test@test.test")
                .Select(e => e.Id)
                .FirstOrDefault();

            return emailId;
        }
    }
}
