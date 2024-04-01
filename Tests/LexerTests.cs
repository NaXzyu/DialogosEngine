namespace DialogosEngine.Tests
{
    [TestFixture]
    public static class LexerTests
    {
        [Test]
        public static void Vectorize_GivenString_ConvertsToAsciiFloatArray()
        {
            // Arrange
            string input = "Test";
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Expected packed values need to be calculated based on the packing logic used in VectorizeNew
            // For the sake of this example, let's assume the packing logic results in these values
            // ASCII values for 'T', 'e', 's', 't' { 84, 101, 115, 116 }
            var expected = new float[] { 0.084101f, 0.115116f };

            // Act
            float[] result = Lexer.Vectorize(input);
            TestContext.WriteLine($"Resulting float array: {Utility.FormatFloatArray(result)}");

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Length, Is.EqualTo(expected.Length), "The length of the result array should match the expected array.");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.That(result[i], Is.EqualTo(expected[i]), $"The packed value at index {i} should match the expected value.");
            }
            TestContext.WriteLine($"Test passed: Input string '{input}' converts to expected packed float array.");
        }


        [Test]
        public static void Vectorize_GivenEmptyString_ReturnsEmptyFloatArray()
        {
            // Arrange
            var input = string.Empty;
            var expected = new float[] { };

            // Act
            var result = Lexer.Vectorize(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Length, Is.EqualTo(expected.Length), "The result array should be empty for an empty input string.");
        }

        [Test]
        public static void Vectorize_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            string input = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => Lexer.Vectorize(input));
            Assert.That(ex.ParamName, Is.EqualTo("line"), "The exception should be thrown for a null input string.");
        }

        [Test]
        public static void Vectorize_GivenStringExceedingMaxChars_ThrowsLexerException()
        {
            // Arrange
            var input = new string('a', Lexer.k_MaxChars + 1);

            // Act & Assert
            var ex = Assert.Throws<LexerException>(() => Lexer.Vectorize(input));
            Assert.That(ex.Message, Is.EqualTo($"Input exceeds the maximum length of {Lexer.k_MaxChars} characters."), "The exception should be thrown for input exceeding the maximum character limit.");
        }

        [Test]
        public static void Vectorize_GivenStringWithSpecialChars_ConvertsToAsciiFloatArray()
        {
            // Arrange
            string input = "T@st!";
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Calculate expected values based on the packing logic used in Vectorize
            // ASCII values for 'T', '@', 's', 't', '!' are 84, 64, 115, 116, 33
            // Assuming charsPerFloat is 2, and considering the multiplier and precision
            var expected = new float[] { 0.084064f, 0.115116f, 0.000033f };

            // Act
            float[] result = Lexer.Vectorize(input);
            TestContext.WriteLine($"Resulting float array: {Utility.FormatFloatArray(result)}");

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.That(result.Length, Is.EqualTo(expected.Length), "The length of the result array should match the expected array.");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.That(result[i], Is.EqualTo(expected[i]), $"The packed value at index {i} should match the expected value.");
            }
            TestContext.WriteLine($"Test passed: Input string '{input}' converts to expected packed float array.");
        }
    }
}
