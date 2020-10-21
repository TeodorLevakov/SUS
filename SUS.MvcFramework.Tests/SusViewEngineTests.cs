using SUS.MvcFramework.ViewEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace SUS.MvcFramework.Tests
{
    public class SusViewEngineTests
    {
        [Theory]
        [InlineData("CleanHtml")]
        [InlineData("Foreach")]
        [InlineData("IfElseFor")]
        [InlineData("ViewModel")]
        public void TestGetHtml(string fileName)
        {
            var viewModel = new TestViewModel
            {
                Name = "Doggo",
                Price = 1234.56M,
                DateOfBirth = new DateTime(2019, 6, 1)
            };

            IViewEngine viewEngine = new SusViewEngine();

            var view = File.ReadAllText($"ViewTests/{fileName}.html");
            var result = viewEngine.GetHtml(view, viewModel, null);

            var expectedResult = File.ReadAllText($"ViewTests/{fileName}.Result.html");

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void TestTemplateViewModel() 
        {
            IViewEngine viewEngine = new SusViewEngine();
            var actualResult = viewEngine.GetHtml(@"@foreach(var num in Model)
{
<span>@num</span>
}", new List<int> { 1, 2, 3}, null);

            var expectedResult = @"<span>1</span>
<span>2</span>
<span>3</span>
";

            Assert.Equal(expectedResult, actualResult);

        }

    }
}
