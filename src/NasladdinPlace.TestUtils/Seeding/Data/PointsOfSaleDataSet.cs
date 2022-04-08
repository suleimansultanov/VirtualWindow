using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class PointsOfSaleDataSet : DataSet<Pos>
    {
        protected override Pos[] Data => new[]
        {
            new Pos(
                id: 0,
                name: "Витрина 1",
                abbreviatedName: "Test1",
                address: Address.FromCityStreetAtCoordinates(1, "Профсоюзная 3", new Location())
            )
            {
                QrCode = "DA28092D-ED0F-4E6A-B19F-43CF3FDA44B2",
                AreNotificationsEnabled = true,
                QrCodeGenerationType = PosQrCodeGenerationType.Dynamic,
                SensorControllerType = SensorControllerType.New,
                PosActivityStatus = PosActivityStatus.Active
            },
            new Pos(
                id: 0,
                name: "Витрина 2",
                abbreviatedName: "Test2",
                address: Address.FromCityStreetAtCoordinates(1, "Тыныстанова 14", new Location(10, 11))
            )
            {
                QrCode = "9C9FA2DD-1CCC-4885-8D3D-86F19123261B",
                AreNotificationsEnabled = true,
                QrCodeGenerationType = PosQrCodeGenerationType.Dynamic,
                SensorControllerType = SensorControllerType.Legacy,
                PosActivityStatus = PosActivityStatus.Active
            },
            new Pos(
                id: 0,
                name: "Витрина 3",
                abbreviatedName: "Test3",
                address: Address.FromCityStreetAtCoordinates(1, "Советская 1", new Location(11, 1))
            )
            {
                QrCode = "85F26D5D-746F-4F12-8140-EB69432A5502",
                AreNotificationsEnabled = true,
                QrCodeGenerationType = PosQrCodeGenerationType.Dynamic,
                SensorControllerType = SensorControllerType.Esp,
                PosActivityStatus = PosActivityStatus.Active
            }
        };
    }
}