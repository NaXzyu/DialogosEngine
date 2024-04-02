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

        [Test]
        public static void VectorizeUTF8_ChineseCharacters_ConvertsToUtf8FloatArray()
        {
            // Arrange
            string input = "你好，世界！"; // "Hello, World!" in Chinese
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Expected values based on UTF-8 encoding for the Chinese characters
            // UTF-8 encoding for '你' is E4 BD A0, for '好' is E5 A5 BD, for '，' is EF BC 8C,
            // for '世' is E4 B8 96, for '界' is E7 95 8C, and for '！' is EF BC 81
            var expected = new float[] {
                228.0f / (1 << 23), 189.0f / (1 << 23), 160.0f / (1 << 23), // '你'
                229.0f / (1 << 23), 165.0f / (1 << 23), 189.0f / (1 << 23), // '好'
                239.0f / (1 << 23), 188.0f / (1 << 23), 140.0f / (1 << 23), // '，'
                228.0f / (1 << 23), 184.0f / (1 << 23), 150.0f / (1 << 23), // '世'
                231.0f / (1 << 23), 149.0f / (1 << 23), 140.0f / (1 << 23), // '界'
                239.0f / (1 << 23), 188.0f / (1 << 23), 129.0f / (1 << 23)  // '！'
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
        public static void VectorizeUTF8_ChineseHeading_ConvertsToUtf8FloatArray()
        {
            // Arrange
            string input = "Dialogos: 在Unity中利用增强的AI开创互动叙事和语言能力";
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Expected values based on UTF-8 encoding for the Chinese heading
            var expected = new float[] {
                // UTF-8 values for "Dialogos: "
                68.0f / (1 << 23), 105.0f / (1 << 23), 97.0f / (1 << 23), 108.0f / (1 << 23), 111.0f / (1 << 23),
                103.0f / (1 << 23), 111.0f / (1 << 23), 115.0f / (1 << 23), 58.0f / (1 << 23),
                // Space is represented by 32 in UTF-8
                32.0f / (1 << 23),
                // UTF-8 values for the Chinese part of the heading "在Unity中利用增强的AI开创互动叙事和语言能力"
                // '在' is E5 9C A8
                229.0f / (1 << 23), 156.0f / (1 << 23), 168.0f / (1 << 23),
                // 'Unity' in ASCII
                85.0f / (1 << 23), 110.0f / (1 << 23), 105.0f / (1 << 23), 116.0f / (1 << 23), 121.0f / (1 << 23),
                // '中' is E4 B8 AD
                228.0f / (1 << 23), 184.0f / (1 << 23), 173.0f / (1 << 23),
                // '利' is E5 88 A9
                229.0f / (1 << 23), 136.0f / (1 << 23), 169.0f / (1 << 23),
                // '用' is E7 94 A8
                231.0f / (1 << 23), 149.0f / (1 << 23), 168.0f / (1 << 23),
                // '增' is E5 A2 9E
                229.0f / (1 << 23), 162.0f / (1 << 23), 158.0f / (1 << 23),
                // '强' is E5 BC BA
                229.0f / (1 << 23), 188.0f / (1 << 23), 186.0f / (1 << 23),
                // '的' is E7 9A 84
                231.0f / (1 << 23), 154.0f / (1 << 23), 132.0f / (1 << 23),
                // 'AI' in ASCII
                65.0f / (1 << 23), 73.0f / (1 << 23),
                // '开' is E5 BC 80
                229.0f / (1 << 23), 188.0f / (1 << 23), 128.0f / (1 << 23),
                // '创' is E5 88 9B
                229.0f / (1 << 23), 136.0f / (1 << 23), 155.0f / (1 << 23),
                // '互' is E4 BA 92
                228.0f / (1 << 23), 186.0f / (1 << 23), 146.0f / (1 << 23),
                // '动' is E5 8A A8
                229.0f / (1 << 23), 138.0f / (1 << 23), 168.0f / (1 << 23),
                // '叙' is E5 8F 99
                229.0f / (1 << 23), 143.0f / (1 << 23), 153.0f / (1 << 23),
                // '事' is E4 BA 8B
                228.0f / (1 << 23), 186.0f / (1 << 23), 139.0f / (1 << 23),
                // '和' is E5 92 8C
                229.0f / (1 << 23), 146.0f / (1 << 23), 140.0f / (1 << 23),
                // '语' is E8 AF AD
                232.0f / (1 << 23), 175.0f / (1 << 23), 173.0f / (1 << 23),
                // '言' is E8 A8 80
                232.0f / (1 << 23), 168.0f / (1 << 23), 128.0f / (1 << 23),
                // '能' is E8 83 BD
                232.0f / (1 << 23), 131.0f / (1 << 23), 189.0f / (1 << 23),
                // '力' is E5 8A 9B
                229.0f / (1 << 23), 138.0f / (1 << 23), 155.0f / (1 << 23),
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
