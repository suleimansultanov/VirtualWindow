namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement
{
    public interface IStatelessPosTokenManager
    {
        string GeneratePosToken(int posId);
        bool IsPosTokenValid(int posId, string token);
        bool TryGetPosIdIfTokenValid(string token, out int posId);
    }
}