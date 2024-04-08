namespace DialogosEngine.Tests
{
    [TestFixture]
    public static class LexerTests
    {
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

        [Test]
        public static void QuantizeUTF8_GivenFloatArray_ConvertsToUtf8String()
        {
            // Arrange
            string expected = "Test"; // The expected UTF-8 string output
            float[] input = Lexer.VectorizeUTF8(expected); // Use VectorizeUTF8 to get the correct float array

            TestContext.WriteLine($"Testing with input float array: '{Utility.FormatFloatArray(input)}'.");

            // Act
            string result = Lexer.QuantizeUTF8(input);
            TestContext.WriteLine($"Resulting UTF-8 string: '{result}'");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expected), "The resulting string should match the expected UTF-8 string.");
            TestContext.WriteLine($"Test passed: Input float array converts to expected UTF-8 string '{expected}'.");
        }

        [Test]
        public static void QuantizeUTF8_ComplexStringWithEmojis_ConvertsToUtf8String()
        {
            // Arrange
            string expected = "The quick brown fox jumps over the lazy dog 🚀🦊🐶";
            float[] input = Lexer.VectorizeUTF8(expected); // Use VectorizeUTF8 to get the correct float array

            TestContext.WriteLine($"Testing with input float array: '{Utility.FormatFloatArray(input)}'.");

            // Act
            string result = Lexer.QuantizeUTF8(input);
            TestContext.WriteLine($"Resulting UTF-8 string: '{result}'");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expected), "The resulting string should match the expected complex UTF-8 string with emojis.");
            TestContext.WriteLine($"Test passed: Input float array converts to expected UTF-8 string with emojis '{expected}'.");
        }

        [Test]
        public static void QuantizeUTF8_JapaneseText_ConvertsToUtf8String()
        {
            // Arrange
            string expected = "これは日本語の段落テストです。このテストは、UTF-8関数が日本語の文字を含むテキストを正しく処理できることを確認するためのものです。"; // A sample Japanese paragraph
                                                                                                     // Ensure the string is not longer than 1000 characters
            expected = expected.Substring(0, Math.Min(1000, expected.Length));
            float[] input = Lexer.VectorizeUTF8(expected); // Use VectorizeUTF8 to get the correct float array

            TestContext.WriteLine($"Testing with input float array: '{Utility.FormatFloatArray(input)}'.");

            // Act
            string result = Lexer.QuantizeUTF8(input);
            TestContext.WriteLine($"Resulting UTF-8 string: '{result}'");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expected), "The resulting string should match the expected Japanese UTF-8 string.");
            TestContext.WriteLine($"Test passed: Input float array converts to expected UTF-8 Japanese text '{expected}'.");
        }

        [Test]
        public static void QuantizeUTF8_ChineseText_ConvertsToUtf8String()
        {
            // Arrange
            string expected = "这是一个中文段落测试。这个测试将验证UTF-8函数是否能够正确处理包含中文字符的文本。"; // A sample Chinese paragraph
                                                                             // Ensure the string is not longer than 1000 characters
            expected = expected.Substring(0, Math.Min(1000, expected.Length));
            float[] input = Lexer.VectorizeUTF8(expected); // Use VectorizeUTF8 to get the correct float array

            TestContext.WriteLine($"Testing with input float array: '{Utility.FormatFloatArray(input)}'.");

            // Act
            string result = Lexer.QuantizeUTF8(input);
            TestContext.WriteLine($"Resulting UTF-8 string: '{result}'");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expected), "The resulting string should match the expected Chinese UTF-8 string.");
            TestContext.WriteLine($"Test passed: Input float array converts to expected UTF-8 Chinese text '{expected}'.");
        }

        [Test]
        public static void QuantizeUTF8_EmojiString_ConvertsToUtf8String()
        {
            // Arrange
            string expected = "😀😃😄😁😆😅😂🤣😊😇🙂🙃😉😌😍🥰😘😗😙😚😋😛😝😜🤪🤨🧐🤓😎🤩🥳😏😒😞😔😟😕🙁☹️😣😖😫😩🥺😢😭😤😠😡🤬😱😨😰😥😓🤗🤔🤭🤫🤥😶😐😑😬🙄😯😦😧😮😲🥱😴🤤😪😵🤐🥴🤢🤮🤧😷🤒🤕😈👿👹👺💀☠️👻👽👾🤖💩😺😸😹😻😼😽🙀😿😾";
            // Ensure the string is not longer than 1000 characters
            expected = expected.Substring(0, Math.Min(1000, expected.Length));
            float[] input = Lexer.VectorizeUTF8(expected); // Use VectorizeUTF8 to get the correct float array

            TestContext.WriteLine($"Testing with input float array: '{Utility.FormatFloatArray(input)}'.");

            // Act
            string result = Lexer.QuantizeUTF8(input);
            TestContext.WriteLine($"Resulting UTF-8 string: '{result}'");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expected), "The resulting string should match the expected emoji-only UTF-8 string.");
            TestContext.WriteLine($"Test passed: Input float array converts to expected UTF-8 emoji-only string '{expected}'.");
        }

        [Test]
        public static void LevenshteinDistance_GivenStrings_CalculatesCorrectDistance()
        {
            // Arrange
            string stringA = "kitten";
            string stringB = "sitting";
            int expectedDistance = 3; // 'k' to 's', 'e' to 'i', and append 'g' at the end

            TestContext.WriteLine($"Testing Levenshtein distance between '{stringA}' and '{stringB}'.");

            // Act
            int result = Lexer.LevenshteinDistance(stringA, stringB);
            TestContext.WriteLine($"Resulting Levenshtein distance: {result}");

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.That(result, Is.EqualTo(expectedDistance), $"The Levenshtein distance between '{stringA}' and '{stringB}' should be {expectedDistance}.");

            TestContext.WriteLine($"Test passed: The Levenshtein distance between '{stringA}' and '{stringB}' is correctly calculated as {expectedDistance}.");
        }

        [Test]
        public static void LevenshteinDistance_ComplexScenarios_CalculatesCorrectDistances()
        {
            // Arrange
            var testCases = new[]
            {
                new { StringA = "book", StringB = "back", ExpectedDistance = 2 },
                new { StringA = "historical", StringB = "hysterical", ExpectedDistance = 2 },
                new { StringA = "intention", StringB = "execution", ExpectedDistance = 5 },
                new { StringA = "example", StringB = "samples", ExpectedDistance = 3 },
                new { StringA = "sturgeon", StringB = "urgently", ExpectedDistance = 6 },
                new { StringA = "levenshtein", StringB = "frankenstein", ExpectedDistance = 6 },
                new { StringA = "distance", StringB = "difference", ExpectedDistance = 5 },
                new { StringA = "a", StringB = "ab", ExpectedDistance = 1 },
                new { StringA = "", StringB = "nonempty", ExpectedDistance = 8 },
                new { StringA = "uppercase", StringB = "UPPERCASE", ExpectedDistance = 9 }
            };

            foreach (var testCase in testCases)
            {
                TestContext.WriteLine($"Testing Levenshtein distance between '{testCase.StringA}' and '{testCase.StringB}'.");

                // Act
                int result = Lexer.LevenshteinDistance(testCase.StringA, testCase.StringB);
                TestContext.WriteLine($"Resulting Levenshtein distance: {result}");

                // Assert
                Assert.IsNotNull(result, "The result should not be null.");
                Assert.That(result, Is.EqualTo(testCase.ExpectedDistance), $"The Levenshtein distance between '{testCase.StringA}' and '{testCase.StringB}' should be {testCase.ExpectedDistance}.");
                TestContext.WriteLine($"Test passed: The Levenshtein distance between '{testCase.StringA}' and '{testCase.StringB}' is correctly calculated as {testCase.ExpectedDistance}.");
            }
        }

        [Test]
        public static void LevenshteinDistance_WithUTF8Characters_CalculatesCorrectDistance()
        {
            // Arrange
            string stringA = "café"; // 'é' is a UTF-8 character
            string stringB = "cafe";
            int expectedDistance = 1; // Only one substitution required ('é' to 'e')

            TestContext.WriteLine($"Testing Levenshtein distance between '{stringA}' and '{stringB}'.");

            // Act
            int result = Lexer.LevenshteinDistance(stringA, stringB);
            TestContext.WriteLine($"Resulting Levenshtein distance: {result}");

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.That(result, Is.EqualTo(expectedDistance), $"The Levenshtein distance between '{stringA}' and '{stringB}' should be {expectedDistance}.");

            TestContext.WriteLine($"Test passed: The Levenshtein distance between '{stringA}' and '{stringB}' is correctly calculated as {expectedDistance}.");
        }

        [Test]
        public static void LevenshteinDistance_MultipleUTF8Characters_CalculatesCorrectDistances()
        {
            // Arrange
            var testCases = new[]
            {
                new { StringA = "schön", StringB = "schon", ExpectedDistance = 1 },
                new { StringA = "naïve", StringB = "naive", ExpectedDistance = 1 },
                new { StringA = "façade", StringB = "facade", ExpectedDistance = 1 },
                new { StringA = "résumé", StringB = "resume", ExpectedDistance = 2 },
                new { StringA = "niño", StringB = "nino", ExpectedDistance = 1 },
                new { StringA = "jalapeño", StringB = "jalapeno", ExpectedDistance = 1 },
                new { StringA = "touché", StringB = "touche", ExpectedDistance = 1 },
                new { StringA = "über", StringB = "uber", ExpectedDistance = 1 },
                new { StringA = "crème brûlée", StringB = "creme brulee", ExpectedDistance = 3 },
                new { StringA = "coöperate", StringB = "cooperate", ExpectedDistance = 1 }
            };

            foreach (var testCase in testCases)
            {
                TestContext.WriteLine($"Testing Levenshtein distance between '{testCase.StringA}' and '{testCase.StringB}'.");

                // Act
                int result = Lexer.LevenshteinDistance(testCase.StringA, testCase.StringB);
                TestContext.WriteLine($"Resulting Levenshtein distance: {result}");

                // Assert
                Assert.IsNotNull(result, "The result should not be null.");
                Assert.That(result, Is.EqualTo(testCase.ExpectedDistance), $"The Levenshtein distance between '{testCase.StringA}' and '{testCase.StringB}' should be {testCase.ExpectedDistance}.");
                TestContext.WriteLine($"Test passed: The Levenshtein distance between '{testCase.StringA}' and '{testCase.StringB}' is correctly calculated as {testCase.ExpectedDistance}.");
            }
        }

        [Test]
        public static void LevenshteinDistance_ChineseCharacters_CalculatesCorrectDistance()
        {
            // Arrange
            string stringA = "中文"; // Chinese for "Chinese language"
            string stringB = "汉语"; // Chinese for "Chinese language" in another dialect
            int expectedDistance = 2; // Two substitutions required

            TestContext.WriteLine($"Testing Levenshtein distance between '{stringA}' and '{stringB}'.");

            // Act
            int result = Lexer.LevenshteinDistance(stringA, stringB);
            TestContext.WriteLine($"Resulting Levenshtein distance: {result}");

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.That(result, Is.EqualTo(expectedDistance), $"The Levenshtein distance between '{stringA}' and '{stringB}' should be {expectedDistance}.");

            TestContext.WriteLine($"Test passed: The Levenshtein distance between '{stringA}' and '{stringB}' is correctly calculated as {expectedDistance}.");
        }

        [Test]
        public static void LevenshteinDistance_ComplexChineseCharacters_CalculatesCorrectDistance()
        {
            // Arrange
            string stringA = "提案"; // Chinese for "proposal"
            string stringB = "题案"; // Chinese for "test case" or "exam question"
            int expectedDistance = 1; // Two substitutions required

            TestContext.WriteLine($"Testing Levenshtein distance between '{stringA}' and '{stringB}'.");

            // Act
            int result = Lexer.LevenshteinDistance(stringA, stringB);
            TestContext.WriteLine($"Resulting Levenshtein distance: {result}");

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.That(result, Is.EqualTo(expectedDistance), $"The Levenshtein distance between '{stringA}' and '{stringB}' should be {expectedDistance}.");

            TestContext.WriteLine($"Test passed: The Levenshtein distance between '{stringA}' and '{stringB}' is correctly calculated as {expectedDistance}.");
        }

        [Test]
        public static void LevenshteinDistance_LongChineseStrings_CalculatesCorrectDistance()
        {
            // Arrange
            string stringA = "我喜欢在公园散步"; // Chinese for "I like walking in the park"
            string stringB = "我喜歡在公園散步和閱讀"; // Chinese for "I like walking and reading in the park"
            int expectedDistance = 5;

            TestContext.WriteLine($"Testing Levenshtein distance between '{stringA}' and '{stringB}'.");

            // Act
            int result = Lexer.LevenshteinDistance(stringA, stringB);
            TestContext.WriteLine($"Resulting Levenshtein distance: {result}");

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.That(result, Is.EqualTo(expectedDistance), $"The Levenshtein distance between '{stringA}' and '{stringB}' should be {expectedDistance}.");

            TestContext.WriteLine($"Test passed: The Levenshtein distance between '{stringA}' and '{stringB}' is correctly calculated as {expectedDistance}.");
        }

        [Test]
        public static void VectorizeUTF8_ExceedsMaxBufferLength_ThrowsLexerException()
        {
            // Arrange
            string input = new string('a', Lexer.k_MaxBufferLength + 1); // Input string that exceeds max buffer length
            TestContext.WriteLine($"Testing VectorizeUTF8 with input exceeding max buffer length: '{input.Length}' characters.");

            // Act & Assert
            var ex = Assert.Throws<LexerException>(() => Lexer.VectorizeUTF8(input));
            TestContext.WriteLine($"Expected LexerException was thrown: {ex.Message}");
        }

        [Test]
        public static void VectorizeUTF8_WithinMaxBufferLength_DoesNotThrowException()
        {
            // Arrange
            string input = new string('a', Lexer.k_MaxBufferLength); // Input string within max buffer length
            TestContext.WriteLine($"Testing VectorizeUTF8 with input within max buffer length: '{input.Length}' characters.");

            // Act
            float[] result = Lexer.VectorizeUTF8(input);
            TestContext.WriteLine($"Vectorized buffer length: {result.Length}");

            // Assert
            Assert.That(result.Length, Is.EqualTo(Lexer.k_MaxBufferLength), "The vectorized buffer length should match the max buffer length.");
            TestContext.WriteLine("Test passed: The vectorized buffer length matches the max buffer length.");
        }

        [Test]
        public static void QuantizeUTF8_ExceedsMaxBufferLength_ThrowsLexerException()
        {
            // Arrange
            float[] vector = Enumerable.Repeat(1f, Lexer.k_MaxBufferLength + 1).ToArray(); // Vector that exceeds max buffer length
            TestContext.WriteLine($"Testing QuantizeUTF8 with vector exceeding max buffer length: '{vector.Length}' floats.");

            // Act & Assert
            var ex = Assert.Throws<LexerException>(() => Lexer.QuantizeUTF8(vector));
            TestContext.WriteLine($"Expected LexerException was thrown: {ex.Message}");
        }

        [Test]
        public static void QuantizeUTF8_WithinMaxBufferLength_DoesNotThrowException()
        {
            // Arrange
            float[] vector = Enumerable.Repeat(1f, Lexer.k_MaxBufferLength).ToArray(); // Vector within max buffer length
            TestContext.WriteLine($"Testing QuantizeUTF8 with vector within max buffer length: '{vector.Length}' floats.");

            // Act
            string result = Lexer.QuantizeUTF8(vector);
            TestContext.WriteLine($"Quantized string length: {result.Length}");

            // Assert
            Assert.That(result.Length, Is.LessThanOrEqualTo(Lexer.k_MaxBufferLength), "The quantized string length should be less than or equal to the max buffer length.");
            TestContext.WriteLine("Test passed: The quantized string length is within the max buffer length.");
        }

        [Test]
        public static void VectorizeUTF8_AtMaxBufferLengthBoundary_DoesNotThrowException()
        {
            // Arrange
            string input = new string('a', Lexer.k_MaxBufferLength); // Input string at max buffer length boundary
            TestContext.WriteLine($"Testing VectorizeUTF8 with input at max buffer length boundary: '{input.Length}' characters.");

            // Act
            float[] result = Lexer.VectorizeUTF8(input);
            TestContext.WriteLine($"Vectorized buffer length: {result.Length}");

            // Assert
            Assert.That(result.Length, Is.EqualTo(Lexer.k_MaxBufferLength), "The vectorized buffer length should match the max buffer length.");
            TestContext.WriteLine("Test passed: The vectorized buffer length matches the max buffer length.");
        }

        [Test]
        public static void VectorizeUTF8_JustBelowMaxBufferLengthBoundary_DoesNotThrowException()
        {
            // Arrange
            string input = new string('a', Lexer.k_MaxBufferLength - 1); // Input string just below max buffer length boundary
            TestContext.WriteLine($"Testing VectorizeUTF8 with input just below max buffer length boundary: '{input.Length}' characters.");

            // Act
            float[] result = Lexer.VectorizeUTF8(input);
            TestContext.WriteLine($"Vectorized buffer length: {result.Length}");

            // Assert
            Assert.That(result.Length, Is.EqualTo(Lexer.k_MaxBufferLength - 1), "The vectorized buffer length should be just below the max buffer length.");
            TestContext.WriteLine("Test passed: The vectorized buffer length is just below the max buffer length.");
        }

        [Test]
        public static void QuantizeUTF8_AtMaxBufferLengthBoundary_DoesNotThrowException()
        {
            // Arrange
            float[] vector = Enumerable.Repeat(1f, Lexer.k_MaxBufferLength).ToArray(); // Vector at max buffer length boundary
            TestContext.WriteLine($"Testing QuantizeUTF8 with vector at max buffer length boundary: '{vector.Length}' floats.");

            // Act
            string result = Lexer.QuantizeUTF8(vector);
            TestContext.WriteLine($"Quantized string length: {result.Length}");

            // Assert
            Assert.That(result.Length, Is.LessThanOrEqualTo(Lexer.k_MaxBufferLength), "The quantized string length should be less than or equal to the max buffer length.");
            TestContext.WriteLine("Test passed: The quantized string length is within the max buffer length.");
        }

        [Test]
        public static void QuantizeUTF8_JustBelowMaxBufferLengthBoundary_DoesNotThrowException()
        {
            // Arrange
            float[] vector = Enumerable.Repeat(1f, Lexer.k_MaxBufferLength - 1).ToArray(); // Vector just below max buffer length boundary
            TestContext.WriteLine($"Testing QuantizeUTF8 with vector just below max buffer length boundary: '{vector.Length}' floats.");

            // Act
            string result = Lexer.QuantizeUTF8(vector);
            TestContext.WriteLine($"Quantized string length: {result.Length}");

            // Assert
            Assert.That(result.Length, Is.LessThanOrEqualTo(Lexer.k_MaxBufferLength - 1), "The quantized string length should be less than or equal to one less than the max buffer length.");
            TestContext.WriteLine("Test passed: The quantized string length is just below the max buffer length.");
        }

    }
}
