namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement
{
    public interface IStatelessTokenManager
    {
        string GenerateToken(byte[] tokenData);
        bool IsValidToken(string token);
        byte[] GetTokenData(string token);
    }
}