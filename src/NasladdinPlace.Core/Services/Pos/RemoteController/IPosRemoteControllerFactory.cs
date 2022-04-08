namespace NasladdinPlace.Core.Services.Pos.RemoteController
{
    public interface IPosRemoteControllerFactory
    {
        IPosRemoteController Create(int posId);
    }
}