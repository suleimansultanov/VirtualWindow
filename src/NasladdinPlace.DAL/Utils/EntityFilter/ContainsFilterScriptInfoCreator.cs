using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils.EntityFilter.Contracts;

namespace NasladdinPlace.DAL.Utils.EntityFilter
{
    public class ContainsFilterScriptInfoCreator : IFilterScriptInfoCreator
    {
        private readonly string _entityProperty;
        private readonly FilterItemModel _itemModel;

        public ContainsFilterScriptInfoCreator(string entityProperty, FilterItemModel itemModel)
        {
            _entityProperty = entityProperty;
            _itemModel = itemModel;
        }

        public FilterScriptInfo Create(int index)
        {
            var script = $"{_entityProperty} != null && {_entityProperty}.Contains(@{index})";
            return new FilterScriptInfo(script, _itemModel.Value);
        }
    }
}