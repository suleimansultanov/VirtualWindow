using System;
using System.IO;
using System.Threading.Tasks;
using NasladdinPlace.Application.Services.FiscalizationInfos.Contracts;
using NasladdinPlace.Application.Services.FiscalizationInfos.Models;
using NasladdinPlace.Core;
using NasladdinPlace.QrCodeConverter.Encoder;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Application.Services.FiscalizationInfos
{
    public class FiscalizationInfosService : IFiscalizationInfosService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IQrCodeEncoder _qrCodeEncoder;
        private readonly string _qrCodeMimeType;

        public FiscalizationInfosService(
            IUnitOfWorkFactory unitOfWorkFactory, 
            IQrCodeEncoder qrCodeEncoder,
            string qrCodeMimeType)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (string.IsNullOrWhiteSpace(qrCodeMimeType))
                throw new ArgumentNullException(nameof(qrCodeMimeType));
            
            _unitOfWorkFactory = unitOfWorkFactory;
            _qrCodeEncoder = qrCodeEncoder;
            _qrCodeMimeType = qrCodeMimeType;
        }
        
        public async Task<ValueResult<QrCodeStream>> GetQrCodeByFicalizationInfoIdAsync(int fiscalizationInfoId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await GetQrCodeByFicalizationInfoIdAsync(unitOfWork, fiscalizationInfoId);
            }
        }

        private async Task<ValueResult<QrCodeStream>> GetQrCodeByFicalizationInfoIdAsync(IUnitOfWork unitOfWork, int fiscalizationInfoId)
        {
            var fiscalizationInfo = unitOfWork.FiscalizationInfos.GetById(fiscalizationInfoId);
            try
            {
                var qrCodeData = await _qrCodeEncoder.TryEncodeToByteArrayAsync(fiscalizationInfo.QrCodeValue);
                var qrCodeDataStream = new MemoryStream(qrCodeData.Value);
                var qrCodeStream = new QrCodeStream(qrCodeDataStream, _qrCodeMimeType);
                return ValueResult<QrCodeStream>.Success(qrCodeStream);
            }
            catch (Exception ex)
            {
                return ValueResult<QrCodeStream>.Failure(ex.ToString());
            }
        }
    }
}