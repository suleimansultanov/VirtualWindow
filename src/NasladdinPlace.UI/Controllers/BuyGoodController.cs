using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models.Goods;
using System;
using System.Text;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.UI.ViewModels.Goods;

namespace NasladdinPlace.UI.Controllers
{
    public class BuyGoodController : Controller
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfigurationReader _configurationReader;

        public BuyGoodController(IUnitOfWorkFactory unitOfWorkFactory, IConfigurationReader configurationReader)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if(configurationReader == null)
                throw new ArgumentNullException(nameof(configurationReader));

            _unitOfWorkFactory = unitOfWorkFactory;
            _configurationReader = configurationReader;
        }

        [Route("[controller]/{id:int}")]
        public async Task<IActionResult> GetGoodById(int id)
        {
            Good good;

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                good = await unitOfWork.Goods.GetByIdIncludingImagesCategoryAndLabeledGoodsAsync(id);
            }

            if (good == null)
                return NotFound();

            var buyGoodViewModel = Mapper.Map<BuyGoodViewModel>(good);
            buyGoodViewModel.ImagePath = CombineUrl(good.GetGoodImagePathOrDefault());

            return View("Index", buyGoodViewModel);
        }

        [Route("[controller]/{tag}")]
        public async Task<IActionResult> GetGoodByLabel(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return NotFound();
            
            Good good;
            
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var labeledGood = await unitOfWork.LabeledGoods.GetByLabelAsync(GetFormattedLabel(tag, 2, " "));

                if (labeledGood == null || labeledGood.GoodId == null)
                    return NotFound();

                good = await unitOfWork.Goods.GetByIdIncludingImagesCategoryAndLabeledGoodsAsync((int) labeledGood.GoodId);
            }

            if (good == null)
                return NotFound();

            var buyGoodViewModel = Mapper.Map<BuyGoodViewModel>(good);
            buyGoodViewModel.ImagePath = CombineUrl(good.GetGoodImagePathOrDefault());

            return View("Index", buyGoodViewModel);
        }

        private string CombineUrl(string imagePath)
        {
            var baseApiUrl = _configurationReader.GetBaseApiUrl();

            if (!string.IsNullOrEmpty(imagePath))
                return ConfigurationReaderExt.CombineUrlParts(baseApiUrl, imagePath);

            var defaultImagePath = _configurationReader.GetDefaultImagePath();

            return ConfigurationReaderExt.CombineUrlParts(baseApiUrl, defaultImagePath);
        }

        public string GetFormattedLabel(string label, int groupSize, string separator)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < label.Length; i++)
            {
                if (i != 0 && (label.Length - i) % groupSize == 0)
                {
                    sb.Append(separator);
                }

                sb.Append(label[i]);
            }

            return sb.ToString();
        }
    }
}