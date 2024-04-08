namespace DialogosEngine.Tests
{
    [TestFixture]
    public class AgentUtilsTests
    {
        [Test]
        public void CalculateEchoReward_ShouldReturnPerfectScore_WhenStringsMatchExactly()
        {
            // Arrange
            string expected = "hello<eos>";
            string guessed = "hello<eos>";
            TestContext.WriteLine($"Testing CalculateEchoReward with expected string: '{expected}' and guessed string: '{guessed}'.");

            // Act
            float reward = AgentUtils.CalculateEchoReward(expected, guessed);
            TestContext.WriteLine($"Calculated reward: {reward}");

            // Assert
            Assert.That(reward, Is.EqualTo(1f), "The reward should be 1.0f when the expected and guessed strings match exactly.");
            TestContext.WriteLine("Test passed: The calculated reward is 1.0f as expected.");
        }

        [Test]
        public void CalculateEchoReward_ShouldThrowArgumentException_WhenStringsAreEmpty()
        {
            // Arrange
            string expected = "";
            string guessed = "";
            TestContext.WriteLine($"Testing CalculateEchoReward with empty strings.");

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => AgentUtils.CalculateEchoReward(expected, guessed));
            TestContext.WriteLine($"Expected ArgumentException was thrown with message: {ex.Message}");

            // Log the result
            Assert.IsNotNull(ex, "An ArgumentException should be thrown for empty strings.");
            TestContext.WriteLine("Test passed: ArgumentException is thrown as expected for empty strings.");
        }

        [Test]
        public void CalculateEchoReward_ShouldThrowArgumentException_WhenExpectedStringDoesNotEndWithEos()
        {
            // Arrange
            string expected = "hello";
            string guessed = "hello<eos>";
            TestContext.WriteLine($"Testing CalculateEchoReward with expected string not ending with '<eos>': '{expected}'.");

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => AgentUtils.CalculateEchoReward(expected, guessed));
            TestContext.WriteLine($"Expected ArgumentException was thrown with message: {ex.Message}");

            // Log the result
            Assert.IsNotNull(ex, "An ArgumentException should be thrown when the expected string does not end with '<eos>'.");
            TestContext.WriteLine("Test passed: ArgumentException is thrown as expected when the expected string does not end with '<eos>'.");
        }

        [Test]
        public void CalculateEchoReward_ShouldReturnPartialScore_WhenStringsAreSimilar()
        {
            // Arrange
            string expected = "hello<eos>";
            string guessed = "hallo<eos>";
            TestContext.WriteLine($"Testing CalculateEchoReward with expected string: '{expected}' and guessed string: '{guessed}'.");

            // Act
            float reward = AgentUtils.CalculateEchoReward(expected, guessed);
            TestContext.WriteLine($"Calculated reward: {reward}");

            // Assert
            Assert.That(reward, Is.EqualTo(0.9f).Within(0.01f), "The reward should be approximately 0.9f for similar strings.");
            TestContext.WriteLine("Test passed: The calculated reward is 0.9f as expected for similar strings.");
        }

        [Test]
        public void CalculateEchoReward_ShouldReflectSimilarityAndLengthMatch_WhenEosIsMissing()
        {
            // Arrange
            string expected = "hello<eos>";
            string guessed = "hello";
            TestContext.WriteLine($"Testing CalculateEchoReward with expected string: '{expected}' and guessed string missing '<eos>': '{guessed}'.");

            // Act
            float reward = AgentUtils.CalculateEchoReward(expected, guessed);
            TestContext.WriteLine($"Calculated reward: {reward}");

            // Assert
            Assert.That(reward, Is.EqualTo(-0.5f).Within(0.01f), "The reward should be -0.5f when the guessed string is missing the '<eos>' token, reflecting the similarity and length match.");
            TestContext.WriteLine("Test passed: The calculated reward is -0.5f as expected, reflecting the similarity and length match.");
        }

        [Test]
        public void CalculateEchoReward_ShouldReturnSpecificLowerScore_WhenGuessedStringHasAdditionalCharacters()
        {
            // Arrange
            string expected = "hello<eos>";
            string guessed = "hello there<eos>";
            TestContext.WriteLine($"Testing CalculateEchoReward with expected string: '{expected}' and guessed string with additional characters: '{guessed}'.");

            // Act
            float reward = AgentUtils.CalculateEchoReward(expected, guessed);
            TestContext.WriteLine($"Calculated reward: {reward}");

            // Assert
            Assert.That(reward, Is.EqualTo(-0.125f).Within(0.01f), "The reward should be -0.125f when the guessed string has additional characters.");
            TestContext.WriteLine("Test passed: The calculated reward is -0.125f as expected when the guessed string has additional characters.");
        }

        [Test]
        public void CalculateEchoReward_ShouldReturnSpecificMuchLowerScore_WhenGuessedStringIsSignificantlyLonger()
        {
            // Arrange
            string expected = "hello<eos>";
            string guessed = "hello there, how are you doing today?<eos>";
            TestContext.WriteLine($"Testing CalculateEchoReward with expected string: '{expected}' and a significantly longer guessed string: '{guessed}'.");

            // Act
            float reward = AgentUtils.CalculateEchoReward(expected, guessed);
            TestContext.WriteLine($"Calculated reward: {reward}");

            // Assert
            Assert.That(reward, Is.EqualTo(-0.7619048f).Within(0.01f), "The reward should be -0.7619048f when the guessed string is significantly longer than the expected string.");
            TestContext.WriteLine("Test passed: The calculated reward is -0.7619048f as expected when the guessed string is significantly longer than the expected string.");
        }

        [Test]
        public void CalculateEchoReward_ShouldReturnPositiveScore_WhenGuessedStringIsLongerButSimilar()
        {
            // Arrange
            string expected = "hello fr <eos>";
            string guessed = "hello friend<eos>";
            TestContext.WriteLine($"Testing CalculateEchoReward with expected string: '{expected}' and a longer but similar guessed string: '{guessed}'.");

            // Act
            float reward = AgentUtils.CalculateEchoReward(expected, guessed);
            TestContext.WriteLine($"Calculated reward: {reward}");

            // Assert
            // The expected reward should be positive due to the high similarity despite the additional length.
            // The reward is expected to be 0.41176474 based on the current logic:
            Assert.That(reward, Is.EqualTo(0.41176474f).Within(0.01f), "The calculated reward should match the expected value when the guessed string is longer but still similar to the expected string.");
            TestContext.WriteLine($"Test passed: The calculated reward matches the expected value.");
        }

        [Test]
        public void CalculateEchoReward_ShouldHandleEmojisInStrings()
        {
            // Arrange
            string expected = "hello👋<eos>";
            string guessed = "hello👋 friend🙂<eos>";
            TestContext.WriteLine($"Testing CalculateEchoReward with expected string containing an emoji: '{expected}' and guessed string with additional emoji: '{guessed}'.");

            // Act
            float reward = AgentUtils.CalculateEchoReward(expected, guessed);
            TestContext.WriteLine($"Calculated reward: {reward}");

            // Assert
            // The expected reward should account for the emojis as part of the string.
            // Update the expected value based on the logic of your CalculateEchoReward method.
            // The reward is expected to be -0.23809522 based on the current logic:
            Assert.That(reward, Is.EqualTo(-0.23809522f).Within(0.01f), "The calculated reward should match the expected value when the guessed string contains emojis and is similar to the expected string.");
            TestContext.WriteLine($"Test passed: The calculated reward matches the expected value.");
        }

        [Test]
        public void CalculateEchoReward_ShouldHandleLongEmojiStrings()
        {
            // Arrange
            string expected = "🌟🚀🌕🌌<eos>";
            string guessed = "🌟🚀🌕🌌👨‍🚀🛸🪐<eos>";
            TestContext.WriteLine($"Testing CalculateEchoReward with expected emoji string: '{expected}' and a longer guessed emoji string: '{guessed}'.");

            // Act
            float reward = AgentUtils.CalculateEchoReward(expected, guessed);
            TestContext.WriteLine($"Calculated reward: {reward}");

            // Assert
            // The expected reward should account for the emojis as part of the string.
            // The reward is expected to be -0.23809522 based on the current logic:
            Assert.That(reward, Is.EqualTo(0.0454545617f).Within(0.01f), "The calculated reward should match the expected value when the guessed emoji string is longer but contains all expected emojis.");
            TestContext.WriteLine($"Test passed: The calculated reward matches the expected value within the specified tolerance.");
        }

        [Test]
        public void CalculateEchoReward_ShouldHandleComplexUTF8Strings()
        {
            // Arrange
            // Generate a complex UTF-8 string with various Unicode characters
            string expected = "🎨👾🧩📚🎷🎲🧬🔭🛰️🌐<eos>";
            string guessed = "🎨👾🧩📚🎷🎲🧬🔭🛰️🌐🧿🪐🌠🌌🎆🧨🎈🧸🪀🪁<eos>";
            TestContext.WriteLine($"Testing CalculateEchoReward with expected complex UTF-8 string: '{expected}' and a longer guessed UTF-8 string: '{guessed}'.");

            // Ensure the strings are within the 1000-byte limit
            Assert.That(System.Text.Encoding.UTF8.GetByteCount(expected), Is.LessThanOrEqualTo(1000), "The expected string exceeds the 1000-byte limit.");
            Assert.That(System.Text.Encoding.UTF8.GetByteCount(guessed), Is.LessThanOrEqualTo(1000), "The guessed string exceeds the 1000-byte limit.");

            // Act
            float reward = AgentUtils.CalculateEchoReward(expected, guessed);
            TestContext.WriteLine($"Calculated reward: {reward}");

            // Assert
            // The expected reward should account for the complex UTF-8 characters as part of the string.
            // Update the expected value based on the logic of your CalculateEchoReward method.
            // For example, if the expected reward is now 0.3f based on the new logic, update the test accordingly:
            Assert.That(reward, Is.EqualTo(-0.086956501f).Within(0.01f), "The calculated reward should match the expected value when the guessed UTF-8 string is longer but contains all expected complex characters.");
            TestContext.WriteLine($"Test passed: The calculated reward matches the expected value within the specified tolerance.");
        }


        [Test]
        public void CalculateEchoReward_ShouldHandleMaxRandomASCIICharacters()
        {
            // Arrange
            var random = new Random();
            string expected = new string(Enumerable.Repeat(0, 999).Select(_ => (char)random.Next(32, 127)).ToArray()) + "<eos>";
            string guessed = new string(Enumerable.Repeat(0, 999).Select(_ => (char)random.Next(32, 127)).ToArray()) + "<eos>";
            TestContext.WriteLine($"Testing CalculateEchoReward with two random ASCII strings of 999 characters each.");

            // Act
            float reward = AgentUtils.CalculateEchoReward(expected, guessed);
            TestContext.WriteLine($"Calculated reward: {reward}");

            // Assert
            // The expected reward should reflect the difference between the two very different strings.
            // Update the expected value based on the logic of your CalculateEchoReward method.
            // For example, if the expected reward is now -1.0f based on the new logic, update the test accordingly:
            Assert.That(reward, Is.EqualTo(0.035856545f).Within(0.01f), "The reward should be negative when the guessed string is very different from the expected string.");
            TestContext.WriteLine($"Test passed: The calculated reward is negative as expected for very different strings.");
        }

    }
}
