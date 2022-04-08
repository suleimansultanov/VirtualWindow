using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.UI.ViewModels.Documents
{
    public class DocumentGoodsMovingViewModel
    {
        public int Id { get; set; }
        public Guid? ErpId { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsPosted { get; set; }
        public bool IsDeleted { get; set; }
        public ApplicationUser User { get; set; }
        public PosOperation PosOperation { get; set; }
        public string PosName { get; set; }
        public ICollection<DocumentGoodsMovingTableItem> TablePart { get; set; }
        public int BalanceAtBeginingSum
        {
            get { return TablePart.Sum(item => item.BalanceAtBegining); }
        }
        public int BalanceAtEndSum
        {
            get { return TablePart.Sum(item => item.BalanceAtEnd); }
        }
        public int IncomeSum
        {
            get { return TablePart.Sum(item => item.Income); }
        }
        public int OutcomeSum
        {
            get { return TablePart.Sum(item => item.Outcome); }
        }
    }
}
