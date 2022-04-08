using System;
using FluentAssertions;
using NasladdinPlace.QrCodeConverter.Decoder;
using NasladdinPlace.QrCodeConverter.Encoder;
using NasladdinPlace.Tests.Constants;
using NUnit.Framework;

namespace NasladdinPlace.QrCodeConverter.Tests.Encoder
{
    public class QrCodeEncoderShould
    {
        private const int DefaultQrCodeDimensionSize = 256;
    
        private IQrCodeEncoder _qrCodeEncoder;
        private IQrCodeDecoder _qrCodeDecoder;

        [SetUp]
        public void SetUp()
        {
            _qrCodeEncoder = new QrCodeEncoder(DefaultQrCodeDimensionSize);
            _qrCodeDecoder = new QrCodeDecoder();
        }

        [Test]
        public void EncodeQrCodeCorrectlyWhenGivenCorrectValue()
        {
            const string qrCodeValue = TestQrCodeConstants.DefaultQrCodeValue;
            
            var encodingResult = _qrCodeEncoder.TryEncodeToBase64StringAsync(qrCodeValue).Result;
            encodingResult.Succeeded.Should().BeTrue();
            encodingResult.Error.Should().BeNullOrEmpty();

            var encodedQrCode = encodingResult.Value;
            var decodingResult = _qrCodeDecoder.TryDecodeBase64QrCodeData(encodedQrCode);
            decodingResult.Succeeded.Should().BeTrue();
            decodingResult.Value.Should().Be(qrCodeValue);
        }

        [TestCase(null)]
        [TestCase("")]
        public void ThrowExceptionWhenQrCodeValueIsNullOrEmpty(string qrCodeValue)
        {
            Assert.Throws<ArgumentNullException>(() => _qrCodeEncoder.TryEncodeToBase64String(qrCodeValue));
        }
    }
}