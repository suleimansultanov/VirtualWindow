using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;
using ZXing;
using ZXing.CoreCompat.System.Drawing;

namespace NasladdinPlace.QrCodeConverter.Encoder
{
    public class QrCodeEncoder : IQrCodeEncoder
    {
        private readonly BarcodeWriter _barcodeWriter;
        
        public QrCodeEncoder(int qrCodeDimensionSize)
        {
            _barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options =
                {
                    Width = qrCodeDimensionSize, 
                    Height = qrCodeDimensionSize
                }
            };
        }
        
        public Task<ValueResult<string>> TryEncodeToBase64StringAsync(string qrCodeValue)
        {
            ValidateQrCodeValue(qrCodeValue);
            
            return Task.Run(() => TryEncodeToBase64String(qrCodeValue));
        }

        public ValueResult<string> TryEncodeToBase64String(string qrCodeValue)
        {
            ValidateQrCodeValue(qrCodeValue);

            try
            {
                var encodedQrCode = EncodeToBase64String(qrCodeValue);
                return ValueResult<string>.Success(encodedQrCode);
            }
            catch (Exception ex)
            {
                return ValueResult<string>.Failure(ex.ToString());
            }
        }

        public Task<ValueResult<byte[]>> TryEncodeToByteArrayAsync(string qrCodeValue)
        {
            ValidateQrCodeValue(qrCodeValue);

            return Task.Run(() => TryEncodeToByteArray(qrCodeValue));
        }

        public ValueResult<byte[]> TryEncodeToByteArray(string qrCodeValue)
        {
            ValidateQrCodeValue(qrCodeValue);

            try
            {
                var encodedQrCode = EncodeToByteArray(qrCodeValue);
                return ValueResult<byte[]>.Success(encodedQrCode);
            }
            catch (Exception ex)
            {
                return ValueResult<byte[]>.Failure(ex.ToString());
            }
        }

        private byte[] EncodeToByteArray(string qrCodeValue)
        {
            var result = _barcodeWriter.Write(qrCodeValue);
            using (var barcodeBitmap = new Bitmap(result))
            {
                using (var memoryStream = new MemoryStream())
                {
                    barcodeBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return memoryStream.ToArray();
                }
            }
        }

        private string EncodeToBase64String(string qrCodeValue)
        {
            var qrCodeByteArray = EncodeToByteArray(qrCodeValue);
            return Convert.ToBase64String(qrCodeByteArray);

        }

        private static void ValidateQrCodeValue(string qrCodeValue)
        {
            if (string.IsNullOrWhiteSpace(qrCodeValue))
                throw new ArgumentNullException(nameof(qrCodeValue));
        }
    }
}