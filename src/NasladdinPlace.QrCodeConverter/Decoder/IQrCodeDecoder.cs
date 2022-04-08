using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.QrCodeConverter.Decoder
{
    public interface IQrCodeDecoder
    {
        Task<ValueResult<string>> TryDecodeBase64QrCodeDataAsync(string base64QrCodeData);
        ValueResult<string> TryDecodeBase64QrCodeData(string base64QrCodeData);
    }
}