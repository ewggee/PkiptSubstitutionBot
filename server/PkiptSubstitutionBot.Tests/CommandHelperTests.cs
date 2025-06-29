using PkiptSubstitutionBot.Application.Helpers;

namespace PkiptSubstitutionBot.Tests;

public class CommandHelperTests
{
    //[SetUp]
    //public void Setup()
    //{

    //}

    [TestCase("/substitution 20.05.2025", "2025/05/20")]
    [TestCase("/substitution 12.02.2045", "2045/02/12")]
    [TestCase("/substitution 01.01.2023", "2023/01/01")]
    [TestCase("/substitution 31.12.2030", "2030/12/31")]
    [TestCase("/substitution 29.02.2024", "2024/02/29")]
    [TestCase("/substitution 15.08.2026", "2026/08/15")]
    public void TryParseSubstitutionCommand_WhenValidDate_ReturnsTrueWithDate(string text, string expectedDateString)
    {
        var expectedDate = DateOnly.Parse(expectedDateString);

        var result = CommandHelper.TryParseSubstitutionCommand(text, out DateOnly date);

        Assert.That(result, Is.EqualTo(true));
        Assert.That(date, Is.EqualTo(expectedDate));
    }

    [TestCase("/substitut 20.05.2025")]
    [TestCase("/substittion 12.02.2045")]
    [TestCase("/sutution 01.01.2023")]
    [TestCase("/subston 31.12.2030")]
    [TestCase("/substion 29.02.2024")]
    [TestCase("/ddsadas 15.08.2026")]
    public void TryParseSubstitutionCommand_WhenInvalidCommand_ReturnsFalse(string text)
    {
        var result = CommandHelper.TryParseSubstitutionCommand(text, out DateOnly date);

        Assert.That(result, Is.EqualTo(false));
    }
}