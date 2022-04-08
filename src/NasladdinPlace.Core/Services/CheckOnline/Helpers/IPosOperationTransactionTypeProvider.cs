using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.CheckOnline.Helpers
{
    public interface IPosOperationTransactionTypeProvider
    {
        PosOperationTransactionType GetTransactionType(FiscalizationType fiscalizationType);
    }
}
