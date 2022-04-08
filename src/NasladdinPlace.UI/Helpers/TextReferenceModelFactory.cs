using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.UI.Managers.Reference.Models;

namespace NasladdinPlace.UI.Helpers
{
    public class TextReferenceModelFactory
    {
        public TextReferenceModel Create<T>(IQueryable<T> query, PageInfoModel pageInfoModel, Action<TextReferenceModel, List<T>> init)
        {
            var textReferenceModel = new TextReferenceModel();
            if (pageInfoModel.PageCount.HasValue)
            {
                textReferenceModel.PageSize = pageInfoModel.PageSize;
                textReferenceModel.PageCount = query.Count();
                textReferenceModel.Page = pageInfoModel.Page;
            }
            else
            {
                textReferenceModel.PageSize = 20;
                textReferenceModel.PageCount = query.Count();
                textReferenceModel.Page = 1;
            }

            var data = query.Skip((textReferenceModel.Page - 1) * textReferenceModel.PageSize).Take(textReferenceModel.PageSize).ToList();

            init(textReferenceModel, data);

            return textReferenceModel;
        }

        public TextReferenceModel Create<T>(IQueryable<T> query, PageInfoModel pageInfoModel, int[] keys, string[] headers, Func<List<T>, List<List<object>>> getData)
        {
            return Create(query, pageInfoModel, (model, result) =>
            {
                model.Headers = headers.ToList();
                model.Keys = keys.ToList();
                model.Table = getData(result);
            });
        }
    }
}
