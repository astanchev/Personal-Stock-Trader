namespace PersonalStockTrader.Web.PDFHelpers
{
    public interface IHtmlToPdfConverter
    {
        byte[] Convert(string basePath, string htmlCode, FormatType formatType, OrientationType orientationType);
    }
}