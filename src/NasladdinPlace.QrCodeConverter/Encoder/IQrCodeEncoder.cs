using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.QrCodeConverter.Encoder
{
    public interface IQrCodeEncoder
    {
        Task<ValueResult<string>> TryEncodeToBase64StringAsync(string qrCodeValue);
        ValueResult<string> TryEncodeToBase64String(string qrCodeValue);
        Task<ValueResult<byte[]>> TryEncodeToByteArrayAsync(string qrCodeValue);
        ValueResult<byte[]> TryEncodeToByteArray(string qrCodeValue);
    }
}