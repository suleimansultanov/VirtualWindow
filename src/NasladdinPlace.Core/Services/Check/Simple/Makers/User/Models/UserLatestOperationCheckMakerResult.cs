using System;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models
{
    public class UserLatestOperationCheckMakerResult
    {
        public static UserLatestOperationCheckMakerResult Success(SimpleCheck simpleCheck, PosOperation checkPosOperation)
        {
            if (simpleCheck == null)
                throw new ArgumentNullException(nameof(simpleCheck));
            if (checkPosOperation == null)
                throw new ArgumentNullException(nameof(checkPosOperation));
            
            return new UserLatestOperationCheckMakerResult(UserOperationCheckMakerStatus.Success, simpleCheck, checkPosOperation);
        }

        public static UserLatestOperationCheckMakerResult PosOperationNotFound()
        {
            return new UserLatestOperationCheckMakerResult(UserOperationCheckMakerStatus.PosOperationNotFound, null, null);
        }
        
        public UserOperationCheckMakerStatus Status { get; }
        public SimpleCheck Check { get; }
        public PosOperation CheckPosOperation { get; }

        private UserLatestOperationCheckMakerResult(UserOperationCheckMakerStatus status, SimpleCheck check, PosOperation checkPosOperation)
        {
            Status = status;
            Check = check;
            CheckPosOperation = checkPosOperation;
        }
    }
}