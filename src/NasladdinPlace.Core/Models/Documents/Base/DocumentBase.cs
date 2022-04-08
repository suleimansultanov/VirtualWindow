using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Models.Documents.Contracts;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Models.Documents.Base
{
    public abstract class DocumentBase<T> : Entity, IDocument<T> where T: class 
    {
        public Guid? ErpId { get; private set; }
        public string DocumentNumber { get; set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? ModifiedDate { get; private set; }
        public ICollection<T> TablePart { get; set; }
        public bool IsPosted { get; private set; }
        public bool IsDeleted { get; private set; }

        protected DocumentBase()
        {
            TablePart = new Collection<T>();
            CreatedDate = DateTime.UtcNow;
        }

        public Result MarkAsDeleted()
        {
            if (IsDeleted)
                return Result.Failure("Can not mark document as Deleted because it has been already marked as Deleted");

            IsDeleted = true;

            return Result.Success();
        }

        public Result MarkAsPosted()
        {
            if (IsPosted)
                return Result.Failure("Can not mark document as Posted because it has been already marked as Posted");

            IsPosted = true;

            return Result.Success();
        }

        public Result MarkAsCanceled()
        {
            if (!IsPosted)
                return Result.Failure("Can not mark document as Canceled because it has been already marked as Canceled");

            IsPosted = false;

            return Result.Success();
        }

        public void AddTablePartItems(ICollection<T> tableItems)
        {
            if (tableItems == null)
                throw new ArgumentNullException(nameof(tableItems));

            TablePart = tableItems;
        }
    }
}
