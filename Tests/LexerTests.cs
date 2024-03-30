using NUnit.Framework;
using DialogosEngine;
using System;

namespace DialogosEngine.Tests
{
    [TestFixture]
    public static class LexerTests
    {
        [Test]
        public static void Vectorize_GivenEmptyString_ReturnsEmptyFloatArray()
        {
            // Arrange
            var input = string.Empty;
            TestContext.WriteLine($"Testing with empty input string.");

            // Act
            var result = Lexer.Vectorize(input);
            TestContext.WriteLine($"Resulting float array: {Utility.FormatFloatArray(result)}");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            TestContext.WriteLine($"Test passed: Empty input string returns an empty float array.");
        }

        [Test]
        public static void Vectorize_GivenString_ConvertsToFloatArray()
        {
            // Arrange
            var input = "Test";
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Act
            var result = Lexer.Vectorize(input);
            TestContext.WriteLine($"Resulting float array: {Utility.FormatFloatArray(result)}");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Length.EqualTo(1));
            TestContext.WriteLine($"Test passed: Input string 'Test' converts to a float array with length 1.");
        }

        [Test]
        public static void Vectorize_GivenStringWithNonAsciiCharacters_DoesNotThrowException()
        {
            // Arrange
            var input = "Tést"; // String with a non-ASCII character
            TestContext.WriteLine($"Testing with non-ASCII input string: '{input}'.");

            // Act & Assert
            Assert.DoesNotThrow(() => Lexer.Vectorize(input));
            TestContext.WriteLine($"Test passed: Non-ASCII input string 'Tést' does not throw an exception.");
        }

        [Test]
        public static void Vectorize_GivenNullInput_ThrowsLexerException()
        {
            // Arrange
            string? input = null;
            TestContext.WriteLine($"Testing with null input string.");

            // Act & Assert
            var ex = Assert.Throws<LexerException>(() => Lexer.Vectorize(input));
            Assert.That(ex.InnerException, Is.TypeOf<ArgumentNullException>());
            TestContext.WriteLine($"Test passed: Null input string throws LexerException with inner ArgumentNullException.");
        }

        [Test]
        public static void Vectorize_GivenStringExceedingMaxChars_ThrowsLexerException()
        {
            // Arrange
            var input = new string('a', Lexer.k_MaxChars + 1);
            TestContext.WriteLine($"Testing with input string exceeding max characters: Length = {input.Length}.");

            // Act & Assert
            var ex = Assert.Throws<LexerException>(() => Lexer.Vectorize(input));
            Assert.That(ex.Message, Is.EqualTo($"Input exceeds the maximum length of {Lexer.k_MaxChars} characters."));
            TestContext.WriteLine($"Test passed: Input string exceeding max characters throws LexerException.");
        }

        [Test]
        public static void Vectorize_GivenSingleCharacter_ConvertsToFloat()
        {
            // Arrange
            var input = "T";
            TestContext.WriteLine($"Testing with single character input: '{input}'.");

            // Act
            var result = Lexer.Vectorize(input);
            TestContext.WriteLine($"Resulting float array: {Utility.FormatFloatArray(result)}");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result[0], Is.EqualTo((float)input[0]));
            TestContext.WriteLine($"Test passed: Single character input 'T' converts to a float array with correct value.");
        }
    }
}
