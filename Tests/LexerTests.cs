using System.Diagnostics;

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

        [Test]
        public static void Vectorize_GivenLongStringWithPunctuation_ConvertsToAsciiFloatArray()
        {
            // Arrange
            string input = "The quick brown fox jumps over the lazy dog! Why? Because it can...";
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Calculate expected values based on the packing logic used in Vectorize
            // ASCII values for the input characters will be packed into float values
            var expected = new float[] {
                0.084104f, // 'Th'
                0.101032f, // 'e '
                0.113117f, // ' q'
                0.117105f, // 'ui'
                0.099107f, // 'ck'
                0.032098f, // ' b'
                0.114111f, // 'ro'
                0.119110f, // 'wn'
                0.032102f, // ' f'
                0.111120f, // 'ox'
                0.032106f, // ' j'
                0.117109f, // 'um'
                0.112115f, // 'ps'
                0.032111f, // ' o'
                0.118101f, // 've'
                0.114032f, // 'r '
                0.116104f, // 'th'
                0.101032f, // 'e '
                0.108097f, // 'la'
                0.122121f, // 'zy'
                0.032100f, // ' d'
                0.111103f, // 'og'
                0.033032f, // '! '
                0.087104f, // 'Wh'
                0.121063f, // 'y?'
                0.032066f, // ' B'
                0.101099f, // 'ec'
                0.097117f, // 'au'
                0.115101f, // 'se'
                0.032105f, // ' i'
                0.116032f, // 't '
                0.099097f, // 'ca'
                0.110046f, // 'n.'
                0.046046f, // '..'
                0.000046f  // '. '
            };

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
    }
}
