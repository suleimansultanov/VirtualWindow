namespace NasladdinPlace.Api.Dtos.Pos
{
    public class PointOfSaleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public double Temperature { get; set; }
        public bool RestrictedAccess { get; set; }
    }
}
