using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Check.Detailed.Models;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.Dtos.Check
{
    public class DetailedCheckDto
    {
        public ICollection<DetailedCheckGood> CheckGoods { get; set; }
        public DetailedCheckSummary Summary { get; set; }
        public DetailedCheckUserInfo UserInfo { get; set; }
        public DetailedCheckPosInfo PosInfo { get; set; }
        public PosOperationStatus Status { get; set; }
        public DateTime DateStatusUpdated { get; set; }
        public DetailedCheckGoodsStatistics Statistics { get; set; }
        public FiscalizationState? FiscalizationState { get; set; }
        public CheckCorrectnessStatus CorrectnessStatus { get; set; }

        public int Id { get; set; }
    }
}
