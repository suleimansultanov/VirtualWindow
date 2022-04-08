namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement
{
    public interface ICipher
    {
        string Encrypt(string plainText, string passPhrase);
        string Decrypt(string cipherText, string passPhrase);
    }
}