using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;
using ZXing;
using BarcodeReader = ZXing.CoreCompat.System.Drawing.BarcodeReader;
using Result = ZXing.Result;

namespace NasladdinPlace.QrCodeConverter.Decoder
{
    public class QrCodeDecoder : IQrCodeDecoder
    {
        private readonly BarcodeReader _barcodeReader;

        public QrCodeDecoder()
        {
            _barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                Options =
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                },
                TryInverted = true
            };
        }
        
        public Task<ValueResult<string>> TryDecodeBase64QrCodeDataAsync(string base64QrCodeData)
        {
            ValidateQrCodeData(base64QrCodeData);

            return Task.Run(() => TryDecodeBase64QrCodeData(base64QrCodeData));
        }

        public ValueResult<string> TryDecodeBase64QrCodeData(string base64QrCodeData)
        {
            ValidateQrCodeData(base64QrCodeData);
            
            try
            {
                var decodedQrCode = DecodeBase64QrCodeData(base64QrCodeData);
                
                return string.IsNullOrEmpty(decodedQrCode) 
                    ? ValueResult<string>.Failure() 
                    : ValueResult<string>.Success(decodedQrCode);
            }
            catch (Exception ex)
            {
                return ValueResult<string>.Failure(ex.ToString());
            }
        }

        private string DecodeBase64QrCodeData(string base64QrCodeData)
        {
            var qrCodeDataBytes = Convert.FromBase64String(base64QrCodeData);

            Result decodingResult;
            using (var qrCodeDataMemoryStream = new MemoryStream(qrCodeDataBytes, 0, qrCodeDataBytes.Length))
            {
                qrCodeDataMemoryStream.Write(qrCodeDataBytes, 0, qrCodeDataBytes.Length);

                using (var qrCode = (Bitmap) Image.FromStream(qrCodeDataMemoryStream, useEmbeddedColorManagement: true))
                {
                    decodingResult = _barcodeReader.Decode(qrCode);
                }
            }

            return decodingResult != null ? decodingResult.Text : string.Empty;
        }

        private static void ValidateQrCodeData(string base64QrCodeData)
        {
            if (string.IsNullOrWhiteSpace(base64QrCodeData))
                throw new ArgumentNullException(nameof(base64QrCodeData));
        }
    }
}