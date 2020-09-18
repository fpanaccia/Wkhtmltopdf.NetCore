using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Wkhtmltopdf.NetCore.Test
{
    /// <summary>
    /// Some smoke tests including legacy API.
    /// </summary>
    public class SmokeTest
    {
        [SetUp]
        public void SetRotativaPath()
        {
#pragma warning disable 618
            WkhtmltopdfConfiguration.RotativaPath = AppDomain.CurrentDomain.BaseDirectory + "Rotativa";
#pragma warning restore 618
        }

        [TearDown]
        public void ClearRotativaPath()
        {
#pragma warning disable 618
            WkhtmltopdfConfiguration.RotativaPath = null;
#pragma warning restore 618
        }

        [Test]
        public void CanConvert()
        {
            var generatePdf = new GeneratePdf(null);
            generatePdf.GetPDF("<p><h1>Hello World</h1>This is html rendered text</p>");
        }

        [Test]
        public void CanConvertWithLegacyProvider()
        {
            var generatePdf = new GeneratePdf(null, new WkhtmlDriver(new LegacyPathProvider()));
            generatePdf.GetPDF("<p><h1>Hello World</h1>This is html rendered text</p>");
        }

        [Test]
        public void CanConvertWithAbsoluteProvider()
        {
            var path = new LegacyPathProvider().GetPath();
            var generatePdf = new GeneratePdf(null, new WkhtmlDriver(new ExactPathProvider(path)));
            generatePdf.GetPDF("<p><h1>Hello World</h1>This is html rendered text</p>");
        }

        [Test]
        public void LegacyResolutionWorks()
        {
            ClearRotativaPath();
            using var host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
#pragma warning disable 618
                    webBuilder.ConfigureServices(services => services.AddWkhtmltopdf());
#pragma warning restore 618
                })
                .Build();

            using var serviceScope = host.Services.CreateScope();
            var generatePdf = serviceScope.ServiceProvider.GetService<IGeneratePdf>();
            generatePdf.GetPDF("<p><h1>Hello World</h1>This is html rendered text</p>");
        }

        [Test]
        public void ResolutionWorks()
        {
            ClearRotativaPath();
            using var host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder => { })
                .ConfigureServices(services => services
                    .AddMvc()
                    .AddWkhtmltopdf<LegacyPathProvider>())
                .Build();

            using var serviceScope = host.Services.CreateScope();
            var generatePdf = serviceScope.ServiceProvider.GetService<IGeneratePdf>();
            generatePdf.GetPDF("<p><h1>Hello World</h1>This is html rendered text</p>");
        }

        [Test]
        public void ThrowsForMissingExecutable()
        {
            var path = "not_valid_path";
            var generatePdf = new GeneratePdf(null, new WkhtmlDriver(new ExactPathProvider(path)));

            var ex = Assert.Throws<WkhtmlDriverException>(() => generatePdf.GetPDF(""));
            StringAssert.Contains(path, ex.Message);
        }
    }
}
