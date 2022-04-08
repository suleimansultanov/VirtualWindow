namespace NasladdinPlace.UI.Managers.Reference.UniReferencesManagers.Models
{
    public class LabeledGoodsGroupByGoodViewModelForFilter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GoodId { get; set; }
        public int PosId { get; set; }
        public decimal Price { get; set; }
        public string Label { get; set; }
        public bool IsDisabled { get; set; }
    }
}