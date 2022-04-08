using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.Utilities.DateTimeConverter;
using Newtonsoft.Json;

namespace NasladdinPlace.UI.Managers.Reference
{
    public class TextReferenceManager
    {
        private readonly TextReferenceModelFactory _textReferenceModelFactory = new TextReferenceModelFactory();

        private readonly IUnitOfWork _unitOfWork;

        protected delegate TextReferenceModel GetTextReferenceDataDelegate(string filter);
        protected Dictionary<TextReferenceSources, GetTextReferenceDataDelegate> TextReferenceGetData;

        public TextReferenceManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            TextReferenceGetData = new Dictionary<TextReferenceSources, GetTextReferenceDataDelegate>
            {
                { TextReferenceSources.ImagesMediaContent, GetImageMediaContents},
                { TextReferenceSources.Users, GetUsers },
                { TextReferenceSources.PointOfSales, GetPointOfSales },
                { TextReferenceSources.Cities, GetCities },
                { TextReferenceSources.LabeledGoods, GetLabeledGoods },
                { TextReferenceSources.Goods, GetGoods },
                { TextReferenceSources.GoodCategories, GetGoodCategories },
                { TextReferenceSources.Makers, GetMakers }
            };           
        }

        public TextReferenceModel GetData(TextReferenceSources source, string filter)
        {
           if (TextReferenceGetData.ContainsKey(source))
                return TextReferenceGetData[source](filter);

           throw new NotImplementedException("Не удалось получить данные по " + source);
        }

        private TextReferenceModel GetCities(string filterModel)
        {
            var vmFilter = string.IsNullOrEmpty(filterModel)
                ? new SimplePageFilterModel()
                : JsonConvert.DeserializeObject<SimplePageFilterModel>(filterModel);

            var filter = vmFilter.Filter;
            var withoutFilters = string.IsNullOrWhiteSpace(filter);

            var query = _unitOfWork.Cities.GetAllIncludingCountry();

            if (!withoutFilters)
            {
                query = query.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
            }

            return _textReferenceModelFactory.Create(
                query: query,
                pageInfoModel: vmFilter,
                keys: new[] { 0, 2 },
                headers: new[] { "Город", "Страна" },
                getData: list => list.Select(x => new List<object> { x.Name, x.Country.Name, x.Id }).ToList());
        }

        private TextReferenceModel GetImageMediaContents(string filterModel)
        {
            var vmFilter = string.IsNullOrEmpty(filterModel)
                ? new SimplePageFilterModel()
                : JsonConvert.DeserializeObject<SimplePageFilterModel>(filterModel);
            
            var filter = vmFilter.Filter;
            var withoutFilters = string.IsNullOrWhiteSpace(filter);

            var query = _unitOfWork.GetRepository<MediaContent>().GetAll().Where(m => m.ContentType == MediaContentType.Image);

            if (!withoutFilters)
            {
                query = query.Where(x => x.FileName.ToLower().Contains(filter.ToLower()));
            }

            query = query.OrderByDescending(x => x.UploadDateTime);

            return _textReferenceModelFactory.Create(
                query: query,
                pageInfoModel: vmFilter,
                keys: new[] { 0, 1, 2 },
                headers: new[] { "Идентификатор", "Дата добавления", "Имя файла" },
                getData: list => list.Select(x => new List<object> {x.Id, UtcDateTimeToMoscowDateTimeAsString(x.UploadDateTime), x.FileName}).ToList());
        }

        private TextReferenceModel GetLabeledGoods(string filterModel)
        {
            var vmFilter = string.IsNullOrEmpty(filterModel)
                ? new SimplePageFilterModel()
                : JsonConvert.DeserializeObject<SimplePageFilterModel>(filterModel);

            var filter = vmFilter.Filter;
            var withoutFilters = string.IsNullOrWhiteSpace(filter);

            var query = _unitOfWork.LabeledGoods.GetAllIncludingGood();

            if (!withoutFilters)
            {
                query = query.Where(x => x.Label.ToLower().Contains(filter.ToLower()) ||
                                         x.Good.Name.ToLower().Contains(filter.ToLower()));
            }

            query = query.OrderByDescending(x => x.Id);

            return _textReferenceModelFactory.Create(
                query: query,
                pageInfoModel: vmFilter,
                keys: new[] {0, 3},
                headers: new[] {"Метка", "Товар", "Состояние"},
                getData: list => list.Select(x => new List<object>
                {
                    x.Label,
                    x.Good == null ? "—" : x.Good.Name,
                    x.IsDisabled ? "Заблокированная" : "Разблокированная",
                    x.Id
                }).ToList());
        }

        private TextReferenceModel GetUsers(string filterModel)
        {
            var vmFilter = string.IsNullOrEmpty(filterModel)
                ? new SimplePageFilterModel()
                : JsonConvert.DeserializeObject<SimplePageFilterModel>(filterModel);

            var filter = vmFilter.Filter;
            var withoutFilters = string.IsNullOrWhiteSpace(filter);

            var query = _unitOfWork.Users.GetAll().Select(u => new {u.Email, u.UserName, u.PhoneNumber, u.Id});

            if (!withoutFilters)
            {
                query = query.Where(x => x.Email.Contains(vmFilter.Filter) || 
                                         x.UserName.Contains(vmFilter.Filter) ||
                                         x.PhoneNumber.Contains(vmFilter.Filter));
            }

            query = query.OrderBy(x => x.UserName);

            return _textReferenceModelFactory.Create(
                query: query,
                pageInfoModel: vmFilter,
                keys: new[] { 0, 3 },
                headers: new[] { "Логин", "Email", "Номер телефона" },
                getData: list => list.Select(x => new List<object> { x.UserName, x.Email, x.PhoneNumber, x.Id }).ToList());
        }

        private TextReferenceModel GetPointOfSales(string filterModel)
        {
            var vmFilter = string.IsNullOrEmpty(filterModel)
                ? new SimplePageFilterModel()
                : JsonConvert.DeserializeObject<SimplePageFilterModel>(filterModel);

            var filter = vmFilter.Filter;
            var withoutFilters = string.IsNullOrWhiteSpace(filter);

            var query = _unitOfWork.PointsOfSale.GetAll().Select(p => new { p.Name, p.Street, p.Id });

            if (!withoutFilters)
            {
                query = query.Where(x => x.Name.Contains(vmFilter.Filter) ||
                                         x.Street.Contains(vmFilter.Filter));
            }

            query = query.OrderBy(x => x.Name);

            return _textReferenceModelFactory.Create(
                query: query,
                pageInfoModel: vmFilter,
                keys: new[] { 0, 2 },
                headers: new[] { "Имя витрины", "Адрес" },
                getData: list => list.Select(x => new List<object> { x.Name, x.Street, x.Id }).ToList());
        }

        private TextReferenceModel GetGoods(string filterModel)
        {
            var vmFilter = string.IsNullOrEmpty(filterModel)
                ? new SimplePageFilterModel()
                : JsonConvert.DeserializeObject<SimplePageFilterModel>(filterModel);

            var filter = vmFilter.Filter;
            var withoutFilters = string.IsNullOrWhiteSpace(filter);

            var query = _unitOfWork.Goods.GetAll().Select(p => new { p.Name, p.Id });

            if (!withoutFilters)
            {
                query = query.Where(x => x.Name.Contains(vmFilter.Filter));
            }

            query = query.OrderBy(x => x.Name);

            return _textReferenceModelFactory.Create(
                query: query,
                pageInfoModel: vmFilter,
                keys: new[] { 0, 1 },
                headers: new[] { "Название" },
                getData: list => list.Select(x => new List<object> { x.Name, x.Id }).ToList());
        }

        private TextReferenceModel GetGoodCategories(string filterModel)
        {
            var vmFilter = string.IsNullOrEmpty(filterModel)
                ? new SimplePageFilterModel()
                : JsonConvert.DeserializeObject<SimplePageFilterModel>(filterModel);

            var filter = vmFilter.Filter;
            var withoutFilters = string.IsNullOrWhiteSpace(filter);

            var query = _unitOfWork.GoodCategories.GetAll();

            if (!withoutFilters)
            {
                query = query.Where(x => x.Name.Contains(vmFilter.Filter));
            }

            query = query.OrderBy(x => x.Name);

            return _textReferenceModelFactory.Create(
                query: query,
                pageInfoModel: vmFilter,
                keys: new[] { 0, 1 },
                headers: new[] { "Название" },
                getData: list => list.Select(x => new List<object> { x.Name, x.Id }).ToList());
        }

        private TextReferenceModel GetMakers(string filterModel)
        {
            var vmFilter = string.IsNullOrEmpty(filterModel)
                ? new SimplePageFilterModel()
                : JsonConvert.DeserializeObject<SimplePageFilterModel>(filterModel);

            var filter = vmFilter.Filter;
            var withoutFilters = string.IsNullOrWhiteSpace(filter);

            var query = _unitOfWork.Makers.GetAll();

            if (!withoutFilters)
            {
                query = query.Where(x => x.Name.Contains(vmFilter.Filter));
            }

            query = query.OrderBy(x => x.Name);

            return _textReferenceModelFactory.Create(
                query: query,
                pageInfoModel: vmFilter,
                keys: new[] { 0, 1 },
                headers: new[] { "Название" },
                getData: list => list.Select(x => new List<object> { x.Name, x.Id }).ToList());
        }

        private string UtcDateTimeToMoscowDateTimeAsString(DateTime dateTime)
        {
            var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(dateTime);
            return SharedDateTimeConverter.ConvertDateHourMinutePartsToString(moscowDateTime);
        }
    }
}
