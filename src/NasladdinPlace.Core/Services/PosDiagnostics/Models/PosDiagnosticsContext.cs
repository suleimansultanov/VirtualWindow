namespace NasladdinPlace.Core.Services.PosDiagnostics.Models
{
    public class PosDiagnosticsContext
    {
        public int UserId { get; }
        public int PosId { get; }

        public PosDiagnosticsContext(int userId, int posId)
        {
            UserId = userId;
            PosId = posId;
        }
    }
}