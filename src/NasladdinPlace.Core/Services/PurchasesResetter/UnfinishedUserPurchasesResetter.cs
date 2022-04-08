using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.PurchasesResetter
{
    public class UnfinishedUserPurchasesResetter : IUserPurchasesResetter
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnfinishedUserPurchasesResetter(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task Reset(int userId)
        {
            var activePosOperation = 
                await _unitOfWork.PosOperations.GetLatestUnpaidOfUserAsync(userId);

            var labeledGoods = activePosOperation.LabeledGoods;
            
            foreach (var labeledGood in labeledGoods)
            {
                labeledGood.MarkAsNotBelongingToUserOrPos();
            }

            var checkItems = activePosOperation.CheckItems;

            foreach (var checkItem in checkItems)
            {
                checkItem.MarkAsDeleted();
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}