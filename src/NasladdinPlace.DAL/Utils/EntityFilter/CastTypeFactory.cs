using System;
using System.Linq;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils.EntityFilter.Contracts;

namespace NasladdinPlace.DAL.Utils.EntityFilter
{
    public class CastTypeFactory : ICastTypeFactory
    {
        private readonly EntityFilterContext _context;

        public CastTypeFactory(EntityFilterContext context)
        {
            _context = context;
        }

        public Type Create()
        {
            switch (_context.ItemModel.ForceCastType)
            {
                case CastTypes.Int32:
                    return typeof(int);
                case CastTypes.DateTime:
                    return typeof(DateTime);
                case CastTypes.Byte:
                    return typeof(byte);
                default:
                    var propertyInfos = _context.Type.GetProperties().FirstOrDefault(x => x.Name == _context.ItemModel.PropertyName);
                    return propertyInfos?.PropertyType;
            }
        }
    }
}