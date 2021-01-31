using A7.Helpers;
using A7.Services;
using NUnit.Framework;

namespace A7.Tests
{
    [TestFixture]
    public class EncodingDecodingTests
    {
        [TestCase(1, 1, "00", "00")]
        [TestCase(1, 1, "01", "10")]
        [TestCase(1, 1, "10", "11")]
        [TestCase(1, 1, "11", "01")]
        [TestCase(3, 0, "0", "00000000")]
        [TestCase(3, 0, "1", "11111111")]
        [TestCase(3, 3, "00110011", "01101110")]
        [TestCase(3, 3, "00110010", "11101110")]
        [TestCase(3, 2, "0100010", "01010000")]
        [TestCase(4, 2, "01101001010", "1010111111111010")]
        [TestCase(4, 2, "00000000000", "0000000000000000")]
        [TestCase(4, 2, "11111111111", "1110100010000001")]
        [TestCase(5, 2, "1111111111111111", "01111110111010001110100010000001")]
        public void EncodesCorrectly(int m, int r, string inputVector, string expectedEncodedVector)
        {
            // Arrange.
            var generatorMatrix = MatrixHelper.CreateGeneratorMatrix(m, r);
            var initialVector = ConversionService.ConvertBinaryStringToVector(inputVector);

            // Act.
            var encoded = Encoder.EncodeVector(initialVector, generatorMatrix);
            var actualEncodedVector = string.Join("", encoded);

            // Assert.
            Assert.AreEqual(expectedEncodedVector, actualEncodedVector);
        }

        [TestCase(1, 1, "00", "00")]
        [TestCase(1, 1, "10", "01")]
        [TestCase(1, 1, "11", "10")]
        [TestCase(1, 1, "01", "11")]
        [TestCase(3, 0, "00000000", "0")]
        [TestCase(3, 0, "11111111", "1")]
        [TestCase(3, 2, "01010000", "0100010")]
        [TestCase(3, 3, "01101110", "00110011")]
        [TestCase(3, 3, "11101110", "00110010")]
        [TestCase(4, 2, "1010111111111010", "01101001010")]
        [TestCase(4, 2, "1010111111111011", "01101001010")]
        [TestCase(4, 2, "1010111011111010", "01101001010")]
        [TestCase(4, 2, "0000000000000000", "00000000000")]
        [TestCase(4, 2, "1110100010000001", "11111111111")]
        [TestCase(5, 2, "01111110111010001110100010000001", "1111111111111111")]
        public void DecodesCorrectly(int m, int r, string encodedVector, string expectedDecodedVector)
        {
            // Arrange.
            var generatorMatrix = MatrixHelper.CreateGeneratorMatrix(m, r);
            var initialVector = ConversionService.ConvertBinaryStringToVector(encodedVector);

            // Act.
            var decoded = Decoder.DecodeVector(initialVector, generatorMatrix, m, r);
            var actualDecodedVector = string.Join("", decoded);

            // Assert.
            Assert.AreEqual(expectedDecodedVector, actualDecodedVector);
        }
    }
}