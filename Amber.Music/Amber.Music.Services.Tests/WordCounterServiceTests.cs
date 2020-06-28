using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amber.Music.Services.Tests
{
    [TestClass]
    public class WordCounterServiceTests
    {
        [DataTestMethod]
        [DataRow("Null text should be ignored", null, 0)]
        [DataRow("Empty string should be ignored", "", 0)]
        [DataRow("Whitespace should be ignored", "    ", 0)]
        [DataRow("Whitespace at start should be ignored", "  word1, word2", 2)]
        [DataRow("Whitespace at end should be ignored", "word1, word2, word3   ", 3)]
        [DataRow("Whitespace at start and end should be ignored", "   word1, word2, word3   ", 3)]
        [DataRow("Multiple whitespace between words should be ignored", "word1,     word2!!    word3", 3)]
        [DataRow("Carriage return characters should be ignored", "word1!\r\n word2,\n word3 word4   ", 4)]
        [DataRow("Tab character should be ignored", "word1!\t word2, word3\t", 3)]
        [DataRow("Single quote character should be ignored", "word's saying there are six words", 6)]
        [DataRow("Numbers should count as words", "there are 4 words", 4)]
        public void Count_ReturnsExpectedWordCount(string testDescription, string text, int expectedWordCount)
        {
            // Arrange
            var sut = new WordCounterService();

            // Act
            var actualCount = sut.Count(text);

            // Assert
            Assert.AreEqual(expectedWordCount, actualCount, testDescription);
        }
    }
}
