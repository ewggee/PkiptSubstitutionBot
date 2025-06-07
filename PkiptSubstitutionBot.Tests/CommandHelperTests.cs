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

        var result = CommandsHelper.TryParseSubstitutionCommand(text, out DateOnly date);

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
        var result = CommandsHelper.TryParseSubstitutionCommand(text, out DateOnly date);

        Assert.That(result, Is.EqualTo(false));
    }

    [TestCase("/ban 123456789", 123456789)]
    [TestCase("/ban 987654321", 987654321)]
    [TestCase("/ban 111222333", 111222333)]
    [TestCase("/ban 999888777", 999888777)]
    [TestCase("/ban 555000123", 555000123)]
    [TestCase("/ban 100200300", 100200300)]
    public void TryParseBanCommand_WhenValidCommand_ReturnsTrue(string text, long expectedChatId)
    {
        var result = CommandsHelper.TryParseBanCommand(text, out long chatId);

        Assert.That(result, Is.EqualTo(true));
        Assert.That(chatId, Is.EqualTo(expectedChatId));
    }

    [TestCase("/an 123456789", 0)]
    [TestCase("/bn 987654321", 0)]
    [TestCase("/ba 111222333", 0)]
    [TestCase("/bn 999888777", 0)]
    [TestCase("/bn 555000123", 0)]
    [TestCase("/ba 100200300", 0)]
    public void TryParseBanCommand_WhenInvalidCommand_ReturnsFalseAndZero(string text, long expectedChatId)
    {
        var result = CommandsHelper.TryParseBanCommand(text, out long chatId);

        Assert.That(result, Is.EqualTo(false));
        Assert.That(chatId, Is.EqualTo(expectedChatId));
    }

    [TestCase("/an 123456789")]
    [TestCase("/bn 987654321")]
    [TestCase("/ba 111222333")]
    [TestCase("/bn 999888777")]
    [TestCase("/bn 555000123")]
    [TestCase("/ba 100200300")]
    public void TryParseBanCommand_WhenInvalidCommand_ReturnsFalse(string text)
    {
        var result = CommandsHelper.TryParseBanCommand(text, out long chatId);

        Assert.That(result, Is.EqualTo(false));
    }
}