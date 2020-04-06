namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using PDFHelpers;
    using PersonalStockTrader.Services.Data;
    using Web.Controllers;

    public class TradeHistoryController : UserController
    {
        private readonly IAccountService accountService;
        private readonly IViewRenderService viewRenderService;
        private readonly IHtmlToPdfConverter htmlToPdfConverter;
        private readonly IWebHostEnvironment environment;

        public TradeHistoryController(IAccountService accountService, IViewRenderService viewRenderService, IHtmlToPdfConverter htmlToPdfConverter, IWebHostEnvironment environment)
        {
            this.accountService = accountService;
            this.viewRenderService = viewRenderService;
            this.htmlToPdfConverter = htmlToPdfConverter;
            this.environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await this.accountService.GetAllClosedPositionsByUserIdAsync(userId);

            return this.View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string startDate, string endDate)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await this.accountService.GetAllClosedPositionsIntervalByUserIdAsync(userId, startDate, endDate);

            return this.View(result);
        }

        public async Task<IActionResult> GenerateReport(string startDate, string endDate)
        {
            var start = DateTime.Parse(startDate, CultureInfo.InvariantCulture).ToShortDateString();
            var end = DateTime.Parse(endDate, CultureInfo.InvariantCulture).ToShortDateString();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await this.accountService.GetAllClosedPositionsIntervalByUserIdAsync(userId, start, end);

            var htmlData = await this.RenderViewAsync("GeneratedReport", result);

            var fileContents = this.htmlToPdfConverter.Convert(this.environment.ContentRootPath, htmlData, FormatType.A4, OrientationType.Portrait);

            return this.File(fileContents, "application/pdf");
        }
    }
}