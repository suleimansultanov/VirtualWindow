namespace NasladdinPlace.Core.Services.Authorization.Models
{
    /* Naming convention: Entity_Action = {number}.
     * 
     * New entities should start with next hundred.
     * For example: Entity1_Action1 = 0, Entity1_Action2 = 1, Entity2_Action1 = 100, Entity2_Action2 = 101.
     * Such approach is necessary for painless addition of new actions to existing entities.
     */
    public enum AppFeature
    {
        AllowedPosMode_CreateOrDelete = 0,
        AllowedPosMode_Read = 1
    }
}