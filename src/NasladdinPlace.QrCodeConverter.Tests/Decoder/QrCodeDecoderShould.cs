using System;
using FluentAssertions;
using NasladdinPlace.QrCodeConverter.Decoder;
using NasladdinPlace.Tests.Constants;
using NUnit.Framework;

namespace NasladdinPlace.QrCodeConverter.Tests.Decoder
{
    public class QrCodeDecoderShould
    {
        private IQrCodeDecoder _qrCodeDecoder;
        
        [SetUp]
        public void Setup()
        {
            _qrCodeDecoder = new QrCodeDecoder();
        }

        [Test]
        public void ShouldDecodeQrCodeSuccessfullyWhenCorrectQrCodeIsGiven()
        {
            var decodedQrCodeResult = _qrCodeDecoder.TryDecodeBase64QrCodeDataAsync(TestQrCodeConstants.DefaultQrCodeAsBase64String).Result;
            decodedQrCodeResult.Succeeded.Should().BeTrue();
            decodedQrCodeResult.Error.Should().BeNullOrEmpty();
            decodedQrCodeResult.Value.Should().Be(TestQrCodeConstants.DefaultQrCodeValue);
        }
        
        [TestCase(null)]
        [TestCase("")]
        public void ThrowExceptionWhenQrCodeIsNullOrEmpty(string qrCode)
        {
            Assert.Throws<ArgumentNullException>(() => _qrCodeDecoder.TryDecodeBase64QrCodeData(qrCode));
        }

        [TestCase("fadkfjad;jfk;adskjf")]
        [TestCase("/9j/4AAQSkZJRgABAQEAYABg23")]
        public void ReturnErrorWhenQrCodeHasIncorrectFormat(string qrCode)
        {
            var decodedQrCodeResult = _qrCodeDecoder.TryDecodeBase64QrCodeDataAsync(qrCode).Result;
            decodedQrCodeResult.Succeeded.Should().BeFalse();
            decodedQrCodeResult.Error.Should().NotBeNullOrEmpty();
            decodedQrCodeResult.Value.Should().BeNullOrEmpty();
        }
    }
}