using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class PosDoorsState : Entity
    {
        public static PosDoorsState Closed(int posId)
        {
            return new PosDoorsState(posId, null, DoorsState.DoorsClosed);
        }

        public static PosDoorsState RightDoorOpened(int posId, int posOperationId)
        {
            return new PosDoorsState(posId, posOperationId, DoorsState.RightDoorOpened);
        }

        public static PosDoorsState LeftDoorOpened(int posId, int posOperationId)
        {
            return new PosDoorsState(posId, posOperationId, DoorsState.LeftDoorOpened);
        }

        public int PosId { get; private set; }
        public DoorsState State { get; private set; }
        public int? PosOperationId { get; private set; }
        public DateTime DateCreated { get; private set; }

        public Pos Pos { get; private set; }
        public PosOperation PosOperation { get; private set; }

        protected PosDoorsState()
        {
            // intentionally left empty
        }

        protected PosDoorsState(int posId, int? posOperationId, DoorsState state)
        {
            PosId = posId;
            State = state;
            PosOperationId = posOperationId;
            DateCreated = DateTime.UtcNow;
        }
    }
}