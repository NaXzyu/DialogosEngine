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
        public static void VectorizeUTF8_GivenString_ConvertsToUtf8FloatArray()
        {
            // Arrange
            string input = "Test 🚀";
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Expected values based on UTF-8 encoding
            // ASCII values for 'T', 'e', 's', 't', ' ' { 84, 101, 115, 116, 32 }
            // UTF-8 encoding for '🚀' is F0 9F 9A 80
            var expected = new float[] {
                 84.0f / (1 << 23),
                101.0f / (1 << 23),
                115.0f / (1 << 23),
                116.0f / (1 << 23),
                 32.0f / (1 << 23),
                240.0f / (1 << 23),
                159.0f / (1 << 23),
                154.0f / (1 << 23),
                128.0f / (1 << 23)
            };

            // Act
            float[] result = Lexer.VectorizeUTF8(input);
            TestContext.WriteLine($"Resulting float array: {Utility.FormatFloatArray(result)}");

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Length, Is.EqualTo(expected.Length), "The length of the result array should match the expected array.");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.That(result[i], Is.EqualTo(expected[i]).Within(0.000001f), $"The UTF-8 value at index {i} should match the expected value within precision.");
            }
            TestContext.WriteLine($"Test passed: Input string '{input}' converts to expected UTF-8 float array.");
        }

        [Test]
        public static void VectorizeUTF8_EmptyString_ReturnsEmptyArray()
        {
            // Arrange
            string input = "";
            TestContext.WriteLine($"Testing with empty input string.");

            // Expected value for an empty string is an empty float array
            var expected = new float[] { };

            // Act
            float[] result = Lexer.VectorizeUTF8(input);
            TestContext.WriteLine($"Resulting float array: {Utility.FormatFloatArray(result)}");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result, "The result array should be empty for an empty input string.");
            TestContext.WriteLine($"Test passed: Empty input string converts to expected empty UTF-8 float array.");
        }

        [Test]
        public static void VectorizeUTF8_PangramWithEmojisAndPunctuation_ConvertsToUtf8FloatArray()
        {
            // Arrange
            string input = "The quick brown fox jumps over the lazy dog! 🦊🐶";
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Expected values based on UTF-8 encoding
            // ASCII values for the sentence and punctuation
            // UTF-8 encoding for '🦊' is F0 9F A6 8A and for '🐶' is F0 9F 90 B6
            var expected = new float[] {
                // ASCII values for "The quick brown fox jumps over the lazy dog! "
                84.0f / (1 << 23), 104.0f / (1 << 23), 101.0f / (1 << 23),
                32.0f / (1 << 23), // space
                113.0f / (1 << 23), 117.0f / (1 << 23), 105.0f / (1 << 23), 99.0f / (1 << 23), 107.0f / (1 << 23),
                32.0f / (1 << 23), // space
                98.0f / (1 << 23), 114.0f / (1 << 23), 111.0f / (1 << 23), 119.0f / (1 << 23), 110.0f / (1 << 23),
                32.0f / (1 << 23), // space
                102.0f / (1 << 23), 111.0f / (1 << 23), 120.0f / (1 << 23),
                32.0f / (1 << 23), // space
                106.0f / (1 << 23), 117.0f / (1 << 23), 109.0f / (1 << 23), 112.0f / (1 << 23), 115.0f / (1 << 23),
                32.0f / (1 << 23), // space
                111.0f / (1 << 23), 118.0f / (1 << 23), 101.0f / (1 << 23), 114.0f / (1 << 23),
                32.0f / (1 << 23), // space
                116.0f / (1 << 23), 104.0f / (1 << 23), 101.0f / (1 << 23),
                32.0f / (1 << 23), // space
                108.0f / (1 << 23), 97.0f / (1 << 23), 122.0f / (1 << 23), 121.0f / (1 << 23),
                32.0f / (1 << 23), // space
                100.0f / (1 << 23), 111.0f / (1 << 23), 103.0f / (1 << 23),
                33.0f / (1 << 23), // exclamation mark
                32.0f / (1 << 23), // space
                // UTF-8 values for the emojis '🦊' and '🐶'
                240.0f / (1 << 23), 159.0f / (1 << 23), 166.0f / (1 << 23), 138.0f / (1 << 23),
                240.0f / (1 << 23), 159.0f / (1 << 23), 144.0f / (1 << 23), 182.0f / (1 << 23)
            };


            // Act
            float[] result = Lexer.VectorizeUTF8(input);
            TestContext.WriteLine($"Resulting float array: {Utility.FormatFloatArray(result)}");

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Length, Is.EqualTo(expected.Length), "The length of the result array should match the expected array.");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.That(result[i], Is.EqualTo(expected[i]).Within(0.000001f), $"The UTF-8 value at index {i} should match the expected value within precision.");
            }
            TestContext.WriteLine($"Test passed: Input string '{input}' converts to expected UTF-8 float array.");
        }

    }
}
