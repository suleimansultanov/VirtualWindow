using NasladdinPlace.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.HardToDetectLabels
{
    public class HardToDetectLabelsPrinter : IHardToDetectLabelsPrinter
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILabeledGoodsPrinter _labeledGoodsPrinter;

        public HardToDetectLabelsPrinter(
            IUnitOfWorkFactory unitOfWorkFactory,
            ILabeledGoodsPrinter labeledGoodsPrinter)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _labeledGoodsPrinter = labeledGoodsPrinter;
        }
        
        public async Task<string> PrintAsync(int posId, IEnumerable<string> labels)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var pos = await unitOfWork.PointsOfSale.GetByIdAsync(posId);
                
                var labeledGoods = await unitOfWork.LabeledGoods.GetByLabelsAsync(labels);

                return _labeledGoodsPrinter.Print(
                    $"{Emoji.Rocket} Неисправные RFID метки витрины {pos.Name}:", labeledGoods
                );
            }
        }
    }
}