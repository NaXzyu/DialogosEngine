using CommandTerminal;

namespace DialogosEngine.Tests
{
    [TestFixture]
    public static class CommandArgTests
    {
        [Test]
        public static void EatArgument_GivenString_ReturnsCorrectCommandArgAndRemainingString()
        {
            // Arrange
            string input = "echo \"Hello World\"";
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Act
            CommandArg result = CommandUtils.ParseCommand(ref input);
            TestContext.WriteLine($"Resulting CommandArg: '{result.String}' and remaining string: '{input}'");

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.String, Is.EqualTo("echo"), "The CommandArg should contain the command 'echo'.");
            Assert.That(input, Is.EqualTo("\"Hello World\""), "The remaining string should contain the quoted argument.");
            TestContext.WriteLine($"Test passed: Input string '{input}' is correctly processed into CommandArg and remaining string.");
        }

        [Test]
        public static void EatArgument_MultipleArgumentsWithAndWithoutQuotes_ReturnsCorrectCommandArgsAndRemainingString()
        {
            // Arrange
            string input = "print \"Hello World\" \"Another \\\"quoted\\\" string\" unquoted";
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Act
            CommandArg firstArg = CommandUtils.ParseCommand(ref input);
            CommandArg secondArg = CommandUtils.ParseCommand(ref input);
            CommandArg thirdArg = CommandUtils.ParseCommand(ref input);
            CommandArg fourthArg = CommandUtils.ParseCommand(ref input);

            // Assert
            Assert.IsNotNull(firstArg);
            Assert.IsNotNull(secondArg);
            Assert.IsNotNull(thirdArg);
            Assert.IsNotNull(fourthArg);

            Assert.That(firstArg.String, Is.EqualTo("print"), "The first CommandArg should contain the command 'print'.");
            Assert.That(secondArg.String, Is.EqualTo("Hello World"), "The second CommandArg should contain the quoted string 'Hello World'.");
            Assert.That(thirdArg.String, Is.EqualTo("Another \"quoted\" string"), "The third CommandArg should contain the quoted string with an escaped quote.");
            Assert.That(fourthArg.String, Is.EqualTo("unquoted"), "The fourth CommandArg should contain the unquoted string 'unquoted'.");

            Assert.That(input, Is.Empty, "The remaining string should be empty after processing all arguments.");

            TestContext.WriteLine($"Test passed: Input string '{input}' is correctly processed into CommandArgs and remaining string.");
        }

        [Test]
        public static void EatArgument_ComplexArgumentsWithNestedQuotesAndEscapedCharacters_ReturnsCorrectCommandArgsAndRemainingString()
        {
            // Arrange
            string input = "complex \"Nested \\\"quotes\\\" and 'single quotes'\" 'Escaped \\'single\\' quotes' \"Mixed \\\"quotes\\\" 'and' single\" trailing";
            TestContext.WriteLine($"Testing with input string: '{input}'.");

            // Act
            CommandArg firstArg = CommandUtils.ParseCommand(ref input);
            CommandArg secondArg = CommandUtils.ParseCommand(ref input);
            CommandArg thirdArg = CommandUtils.ParseCommand(ref input);
            CommandArg fourthArg = CommandUtils.ParseCommand(ref input);
            CommandArg fifthArg = CommandUtils.ParseCommand(ref input);

            // Assert
            Assert.IsNotNull(firstArg);
            Assert.IsNotNull(secondArg);
            Assert.IsNotNull(thirdArg);
            Assert.IsNotNull(fourthArg);
            Assert.IsNotNull(fifthArg);

            Assert.That(firstArg.String, Is.EqualTo("complex"), "The first CommandArg should contain the command 'complex'.");
            Assert.That(secondArg.String, Is.EqualTo("Nested \"quotes\" and 'single quotes'"), "The second CommandArg should contain the nested quoted string.");
            Assert.That(thirdArg.String, Is.EqualTo("Escaped 'single' quotes"), "The third CommandArg should contain the escaped single quoted string.");
            Assert.That(fourthArg.String, Is.EqualTo("Mixed \"quotes\" 'and' single"), "The fourth CommandArg should contain the mixed quoted string.");
            Assert.That(fifthArg.String, Is.EqualTo("trailing"), "The fifth CommandArg should contain the unquoted string 'trailing'.");

            Assert.That(input, Is.Empty, "The remaining string should be empty after processing all arguments.");

            TestContext.WriteLine($"Test passed: Input string '{input}' is correctly processed into CommandArgs and remaining string.");
        }

        [Test]
        public static void FindClosingQuote_WithEscapedAndUnescapedQuotes_ReturnsCorrectIndex()
        {
            // Arrange
            string input = "\"This is a \\\"test\\\" string\"";
            char quoteChar = '\"';
            int expectedIndex = input.Length - 1;

            // Act
            int result = CommandUtils.FindClosingQuote(input, quoteChar);

            // Assert
            Assert.AreEqual(expectedIndex, result, "The index of the closing quote should be at the end of the string.");
        }

        [Test]
        public static void FindClosingQuote_WithNoClosingQuote_ReturnsNegativeOne()
        {
            // Arrange
            string input = "\"No closing quote here";
            char quoteChar = '\"';

            // Act
            int result = CommandUtils.FindClosingQuote(input, quoteChar);

            // Assert
            Assert.That(result, Is.EqualTo(-1), "The result should be -1 indicating no closing quote was found.");
        }

        [Test]
        public static void UnescapedQuotes_WithEscapedQuotes_ReturnsUnescapedString()
        {
            // Arrange
            string input = "Escaped \\\"quotes\\\" here";
            char quoteChar = '\"';
            string expected = "Escaped \"quotes\" here";

            // Act
            string result = CommandUtils.UnescapedQuotes(input, quoteChar);

            // Assert
            Assert.That(result, Is.EqualTo(expected), "The escaped quotes should be unescaped in the result string.");
        }

        [Test]
        public static void UnescapedQuotes_WithNoEscapedQuotes_ReturnsOriginalString()
        {
            // Arrange
            string input = "No escaped quotes here";
            char quoteChar = '\"';

            // Act
            string result = CommandUtils.UnescapedQuotes(input, quoteChar);

            // Assert
            Assert.That(result, Is.EqualTo(input), "The result should be the same as the input string when there are no escaped quotes.");
        }

        [Test]
        public static void ParseCommand_MixedArgumentTypes_HandlesAllCorrectly()
        {
            // Arrange
            string input = "command \"quoted arg\" unquoted '' \"\"";
            string[] expected = { "command", "quoted arg", "unquoted", "", "" };

            // Act & Assert
            for (int i = 0; i < expected.Length; i++)
            {
                CommandArg result = CommandUtils.ParseCommand(ref input);
                Assert.That(result.String, Is.EqualTo(expected[i]), $"Argument {i} should be '{expected[i]}'");
            }
        }

        [Test]
        public static void ParseCommand_ExcessiveWhitespace_HandlesCorrectly()
        {
            // Arrange
            string input = "command   \t \"quoted arg\"   unquoted  ";
            string[] expected = { "command", "quoted arg", "unquoted" };

            // Act & Assert
            for (int i = 0; i < expected.Length; i++)
            {
                CommandArg result = CommandUtils.ParseCommand(ref input);
                Assert.That(result.String, Is.EqualTo(expected[i]), $"Argument {i} should be '{expected[i]}'");
            }
        }

        [Test]
        public static void ParseCommand_SpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            string input = "command $%^&*()_+!";
            string expected = "command";

            // Act
            CommandArg result = CommandUtils.ParseCommand(ref input);

            // Assert
            Assert.That(result.String, Is.EqualTo(expected), "Command with special characters should be handled correctly.");
        }

        [Test]
        public static void ParseCommand_UnicodeCharacters_HandlesCorrectly()
        {
            // Arrange
            string input = "command 你好";
            string expected = "command";

            // Act
            CommandArg result = CommandUtils.ParseCommand(ref input);

            // Assert
            Assert.That(result.String, Is.EqualTo(expected), "Command with Unicode characters should be handled correctly.");
        }

        [Test]
        public static void ParseCommand_LongString_HandlesCorrectly()
        {
            // Arrange
            string input = new string('a', 10000);
            string expected = input;

            // Act
            CommandArg result = CommandUtils.ParseCommand(ref input);

            // Assert
            Assert.That(result.String, Is.EqualTo(expected), "Long string arguments should be handled correctly.");
        }

        [Test]
        public static void ParseCommand_EmptyInput_ReturnsEmptyCommandArg()
        {
            // Arrange
            string input = "";

            // Act
            CommandArg result = CommandUtils.ParseCommand(ref input);

            // Assert
            Assert.That(result.String, Is.EqualTo(""), "Empty input should return an empty CommandArg.");
        }

        [Test]
        public static void ParseCommand_OnlyQuotes_HandlesCorrectly()
        {
            // Arrange
            string input = "\"\"";
            string expected = "";

            // Act
            CommandArg result = CommandUtils.ParseCommand(ref input);

            // Assert
            Assert.That(result.String, Is.EqualTo(expected), "Input with only quotes should return an empty CommandArg.");
        }

        [Test]
        public static void ParseCommand_EscapedBackslashes_HandlesCorrectly()
        {
            // Arrange
            string input = "command \\\\";
            string expected = "command";

            // Act
            CommandArg result = CommandUtils.ParseCommand(ref input);

            // Assert
            Assert.That(result.String, Is.EqualTo(expected), "Escaped backslashes should be handled correctly.");
        }


    }
}
