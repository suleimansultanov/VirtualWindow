using Microsoft.AspNetCore.Http;

namespace NasladdinPlace.UI.Extensions
{
    public static class SessionExtensions
    {
        private const string CurrentPosSessionKey = "current_pos";

        public static void SetCurrentPosId(this ISession session, int id)
        {
            session.SetInt32(CurrentPosSessionKey, id);
        }

        public static bool TryGetCurrentPosId(this ISession session, out int id)
        {
            id = 0;
            
            var posId = session.GetInt32(CurrentPosSessionKey);
            if (!posId.HasValue) 
                return false;
            
            id = posId.Value;
            
            return true;
        }
    }
}