using System;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils.EntityFilter.Contracts;

namespace NasladdinPlace.DAL.Utils.EntityFilter
{
    public class FilterScriptInfoCreatorFactory : IFilterScriptInfoCreatorFactory
    {
        public IFilterScriptInfoCreator Create(EntityFilterContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            switch (context.ItemModel.FilterType)
            {
                case FilterTypes.Contains:
                    return new ContainsFilterScriptInfoCreator(context.Property, context.ItemModel);
                case FilterTypes.Equals:
                case FilterTypes.Greater:
                case FilterTypes.Less:
                case FilterTypes.GreaterOrEquals:
                case FilterTypes.LessOrEquals:
                    var castTypeFactory = new CastTypeFactory(context);
                    return new EqualityFilterScriptInfoCreator(context.Property, context.ItemModel, castTypeFactory);
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(context),
                        context.ItemModel.FilterType,
                        $"{nameof(context.ItemModel.FilterType)} has not been supported yet."
                    );
            }
        }
    }
}