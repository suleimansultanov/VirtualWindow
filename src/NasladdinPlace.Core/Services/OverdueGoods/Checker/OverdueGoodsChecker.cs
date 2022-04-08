using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Makers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.OverdueGoods.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;

namespace NasladdinPlace.Core.Services.OverdueGoods.Checker
{
    public class OverdueGoodsChecker : IOverdueGoodsChecker
    {
        public event EventHandler<Dictionary<OverdueType, IEnumerable<PosGoodInstances>>> OnFoundOverdueGoods;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IOverdueGoodsInfoMaker _overdueGoodsInfoMaker;

        public OverdueGoodsChecker(
            IUnitOfWorkFactory unitOfWorkFactory,
            IOverdueGoodsInfoMaker overdueGoodsInfoMaker)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (overdueGoodsInfoMaker == null)
                throw new ArgumentNullException(nameof(overdueGoodsInfoMaker));

            _unitOfWorkFactory = unitOfWorkFactory;
            _overdueGoodsInfoMaker = overdueGoodsInfoMaker;
        }

        public async Task CheckAsync()
        {
            var pointsOfSaleContent = await GetPointsOfSaleContent();
            var posGoodInstancesDictionary = _overdueGoodsInfoMaker.Make(pointsOfSaleContent);
            ReportFoundOverdueGoods(posGoodInstancesDictionary);
        }

        private void ReportFoundOverdueGoods(Dictionary<OverdueType, IEnumerable<PosGoodInstances>> posGoodInstancesDictionary)
        {
            if (!posGoodInstancesDictionary.Any())
                return;

            OnFoundOverdueGoods?.Invoke(this, posGoodInstancesDictionary);
        }

        private async Task<List<LabeledGood>> GetPointsOfSaleContent()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await unitOfWork.LabeledGoods.GetAllTiedIncludingGoodAndPosAndCurrencyAsync();
            }
        }
    }
}