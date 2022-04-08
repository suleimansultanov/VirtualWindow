namespace NasladdinPlace.Dtos.Pos
{
    public class PosLogTypeDto
    {
        public int PosLogType { get; private set; }

        public  int PosId { get; private set; }

        public PosLogTypeDto(int posId, int posLogType)
        {
            PosId = posId;
            PosLogType = posLogType;
        }
    }
}
