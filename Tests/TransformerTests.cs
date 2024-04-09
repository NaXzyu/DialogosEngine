namespace DialogosEngine.Tests
{
    [TestFixture]
    public class TransformerTests
    {
        [Test]
        public void Transform_ShouldCorrectlyScaleValue()
        {
            // Arrange
            var random = new Random();
            float value = (float)random.NextDouble() * 2f - 1f; // Random value in range [-1, 1]
            TestContext.WriteLine($"Testing Transform with value: {value}");

            // Act
            float result = Transformer.Transform(ref value);
            TestContext.WriteLine($"Transformed result: {result}");

            // Assert
            Assert.That(result, Is.InRange(0f, 1f), "The result should be within the range [0, 1].");
            TestContext.WriteLine($"Test passed: The transformed result is within the expected range.");
        }

        [Test]
        public void RoundMax_ShouldCorrectlyRoundFirstValue()
        {
            // Arrange
            var random = new Random();
            float value = (float)random.NextDouble() * 2f - 1f; // Random value in range [-1, 1]
            TestContext.WriteLine($"Testing RoundMax with value: {value}");

            // Act
            int result = Transformer.RoundMax(ref value);
            TestContext.WriteLine($"Rounded max result: {result}");

            // Assert
            Assert.That(result, Is.InRange(0, Lexer.k_MaxBufferLength), "The result should be within the range [0, Lexer.k_MaxChars].");
            TestContext.WriteLine($"Test passed: The rounded max result is within the expected range.");
        }

        [Test]
        public void SoftMax_ShouldReturnNormalizedProbabilities()
        {
            // Arrange
            var random = new Random();
            float[] values = Enumerable.Repeat(0, 10).Select(_ => (float)random.NextDouble() * 2f - 1f).ToArray();
            TestContext.WriteLine($"Testing SoftMax with an array of random values.");

            // Act
            float[] result = Transformer.SoftMax(ref values);
            TestContext.WriteLine($"SoftMax result: {string.Join(", ", result)}");

            // Assert
            Assert.That(result.Sum(), Is.EqualTo(1f).Within(0.01f), "The sum of the SoftMax result should be close to 1.");
            foreach (var prob in result)
            {
                Assert.That(prob, Is.InRange(0f, 1f), "Each probability should be within the range [0, 1].");
            }
            TestContext.WriteLine($"Test passed: The SoftMax result is a normalized probability distribution.");
        }

        [Test]
        public void Transform_ShouldHandleEdgeCases()
        {
            // Arrange
            float minValue = -1f; // Minimum value in range [-1, 1]
            float maxValue = 1f;  // Maximum value in range [-1, 1]
            TestContext.WriteLine($"Testing Transform with edge cases.");

            // Act
            float minResult = Transformer.Transform(ref minValue);
            float maxResult = Transformer.Transform(ref maxValue);
            TestContext.WriteLine($"Transformed min result: {minResult}");
            TestContext.WriteLine($"Transformed max result: {maxResult}");

            // Assert
            Assert.That(minResult, Is.EqualTo(0f), "The transformed min result should be 0.");
            Assert.That(maxResult, Is.EqualTo(1f), "The transformed max result should be 1.");
            TestContext.WriteLine($"Test passed: The transformed results for edge cases are within the expected range.");
        }

        [Test]
        public void RoundMax_ShouldHandleZeroAndMaxValues()
        {
            // Arrange
            float zeroValue = -1f; // Corresponds to 0 after transformation
            float maxValue = 1f;   // Corresponds to maxScale after transformation
            TestContext.WriteLine($"Testing RoundMax with zero and max values.");

            // Act
            int zeroResult = Transformer.RoundMax(ref zeroValue);
            int maxResult = Transformer.RoundMax(ref maxValue);
            TestContext.WriteLine($"Rounded max result for zero value: {zeroResult}");
            TestContext.WriteLine($"Rounded max result for max value: {maxResult}");

            // Assert
            Assert.That(zeroResult, Is.EqualTo(0), "The rounded result for zero value should be 0.");
            Assert.That(maxResult, Is.EqualTo(Lexer.k_MaxBufferLength), "The rounded result for max value should be Lexer.k_MaxChars.");
            TestContext.WriteLine($"Test passed: The rounded max results for zero and max values are within the expected range.");
        }

        [Test]
        public void SoftMax_ShouldHandleUniformValues()
        {
            // Arrange
            float[] uniformValues = Enumerable.Repeat(0.5f, 10).ToArray(); // Array of uniform values
            TestContext.WriteLine($"Testing SoftMax with uniform values.");

            // Act
            float[] result = Transformer.SoftMax(ref uniformValues);
            TestContext.WriteLine($"SoftMax result: {string.Join(", ", result)}");

            // Assert
            Assert.That(result.Sum(), Is.EqualTo(1f).Within(0.01f), "The sum of the SoftMax result should be close to 1.");
            foreach (var prob in result)
            {
                Assert.That(prob, Is.EqualTo(0.1f).Within(0.01f), "Each probability should be close to 0.1 for uniform values.");
            }
            TestContext.WriteLine($"Test passed: The SoftMax result is a normalized probability distribution for uniform values.");
        }

        [Test]
        public void RoundMax_ShouldHandleNegativeScaling()
        {
            // Arrange
            float negativeValue = -1f; // Value that would result in negative scaling
            TestContext.WriteLine($"Testing RoundMax with negative scaling value: {negativeValue}");

            // Act
            int result = Transformer.RoundMax(ref negativeValue);
            TestContext.WriteLine($"Rounded max result: {result}");

            // Assert
            Assert.That(result, Is.EqualTo(0), "The rounded result should be 0 for negative scaling.");
            TestContext.WriteLine($"Test passed: The rounded max result is correct for negative scaling.");
        }

        [Test]
        public void SoftMax_ShouldBeStable()
        {
            // Arrange
            float[] largeValues = { 1000f, 1000f, 1000f }; // Large values that could cause instability
            TestContext.WriteLine($"Testing SoftMax with large values.");

            // Act
            float[] result = Transformer.SoftMax(ref largeValues);
            TestContext.WriteLine($"SoftMax result: {string.Join(", ", result)}");

            // Assert
            Assert.IsFalse(result.Any(float.IsNaN), "The SoftMax result should not contain NaN values.");
            TestContext.WriteLine($"Test passed: The SoftMax result is stable and does not contain NaN values.");
        }

        [Test]
        public void Transform_InversionShouldReturnOriginalValue()
        {
            // Arrange
            var random = new Random();
            float originalValue = (float)random.NextDouble() * 2f - 1f; // Random value in range [-1, 1]
            float transformedValue = Transformer.Transform(ref originalValue);
            float invertedValue = (transformedValue * 2f) - 1f; // Inverse operation
            TestContext.WriteLine($"Testing Transform inversion with original value: {originalValue}");

            // Act & Assert
            Assert.That(invertedValue, Is.EqualTo(originalValue).Within(0.0001f), "The inverted value should be close to the original value.");
            TestContext.WriteLine($"Test passed: The Transform inversion returned the original value within tolerance.");
        }

        [Test]
        public void Transform_WithInvalidInput_ShouldClampValues()
        {
            // Arrange
            float tooLow = -2f; // Value below the valid range
            float tooHigh = 2f; // Value above the valid range
            TestContext.WriteLine($"Testing Transform with invalid input values.");

            // Act
            float resultLow = Transformer.Transform(ref tooLow);
            float resultHigh = Transformer.Transform(ref tooHigh);
            TestContext.WriteLine($"Transformed result for too low value: {resultLow}");
            TestContext.WriteLine($"Transformed result for too high value: {resultHigh}");

            // Assert
            Assert.That(resultLow, Is.EqualTo(0f), "The result should be clamped to 0 for too low values.");
            Assert.That(resultHigh, Is.EqualTo(1f), "The result should be clamped to 1 for too high values.");
            TestContext.WriteLine($"Test passed: The Transform method clamped the values as expected.");
        }

        [Test]
        public void SoftMax_WithZeroArray_ShouldHandleGracefully()
        {
            // Arrange
            float[] zeroArray = new float[10]; // Array of zeros
            TestContext.WriteLine($"Testing SoftMax with an array of zeros.");

            // Act
            float[] result = Transformer.SoftMax(ref zeroArray);
            TestContext.WriteLine($"SoftMax result: {string.Join(", ", result)}");

            // Assert
            Assert.That(result.Sum(), Is.EqualTo(1f).Within(0.01f), "The sum of the SoftMax result should be close to 1.");
            Assert.That(result.All(v => v == 0.1f), "Each value should be equal to 0.1 for an array of zeros.");
            TestContext.WriteLine($"Test passed: The SoftMax method handled an array of zeros gracefully.");
        }

        [Test]
        public void SoftMax_WithSingleLargeValue_ShouldReturnExpectedDistribution()
        {
            // Arrange
            float[] values = new float[] { 1f, 1f, 10f }; // Array with a single large value
            TestContext.WriteLine($"Testing SoftMax with a single large value.");

            // Act
            float[] result = Transformer.SoftMax(ref values);
            TestContext.WriteLine($"SoftMax result: {string.Join(", ", result)}");

            // Assert
            Assert.That(result[2], Is.EqualTo(1f).Within(0.01f), "The probability for the large value should be close to 1.");
            Assert.That(result[0], Is.EqualTo(0f).Within(0.01f), "The probability for the small values should be close to 0.");
            Assert.That(result[1], Is.EqualTo(0f).Within(0.01f), "The probability for the small values should be close to 0.");
            TestContext.WriteLine($"Test passed: The SoftMax result is as expected for an array with a single large value.");
        }
    }
}
