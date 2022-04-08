using NasladdinPlace.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Api.Tests.Scenarios.OverdueGoods.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;
using NasladdinPlace.TestUtils.Extensions;

namespace NasladdinPlace.Api.Tests.Scenarios.OverdueGoods.DataGenerators
{
    public class OverdueLabeledGoodsDataGenerator : IEnumerable<object[]>
    {
        private const int DefaultGoodId = 1;
        private const int FirstPosId = 1;
        private const int SecondPosId = 2;
        private const int DefaultCurrencyId = 1;
        private const string FirstLabel = "E2 00 00 16 18 0B 01 66 15 20 7E 2A";
        private const string SecondLabel = "E2 80 11 60 60 00 02 05 2A 98 4B 41";
        private const string ThirdLabel = "E2 80 11 60 60 00 02 06 2A 98 DE 41";
        private const string FourthLabel = "E2 22 70 16 18 AB 01 CC 15 20 70 1B";
        private const decimal DefaultLabeledGoodPrice = 5M;

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, FirstLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Overdue] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.OverdueInDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Fresh] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, FirstLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1))),
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, SecondLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Overdue] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 2}
                    }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, SecondLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.OverdueInDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 2}
                    }),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, SecondLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 2}
                    }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, SecondLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Fresh] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 2}
                    }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, FirstLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1))),
                    CreateOverdueLabeledGood(SecondPosId, DefaultGoodId, SecondLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Overdue] = new ExpectedOverdueGoodsInfoInstance(2, new Dictionary<int, int>
                        {
                            {FirstPosId, 1},
                            {SecondPosId, 1}
                        }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(SecondPosId, DefaultGoodId, SecondLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.OverdueInDay] = new ExpectedOverdueGoodsInfoInstance(2, new Dictionary<int, int>
                    {
                        {FirstPosId, 1},
                        {SecondPosId, 1}
                    }),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(SecondPosId, DefaultGoodId, SecondLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = new ExpectedOverdueGoodsInfoInstance(2, new Dictionary<int, int>
                    {
                        {FirstPosId, 1},
                        {SecondPosId, 1}
                    }),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(SecondPosId, DefaultGoodId, SecondLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Fresh] = new ExpectedOverdueGoodsInfoInstance(2, new Dictionary<int, int>
                    {
                        {FirstPosId, 1},
                        {SecondPosId, 1}
                    }),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, SecondLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, SecondLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueInDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, FirstLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, SecondLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Overdue] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueInDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1))),
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, SecondLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Overdue] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3))),
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, SecondLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Overdue] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance(),
                    [OverdueType.Fresh] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1))),
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, SecondLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, ThirdLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Overdue] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Fresh] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, SecondLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, ThirdLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Fresh] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Overdue] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3))),
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, SecondLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, ThirdLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Overdue] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Fresh] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1))),
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, SecondLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, ThirdLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Overdue] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Fresh] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = ExpectedOverdueGoodsInfoInstance.DefaultInstance()
                }
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FirstLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddHours(1))),
                    CreateOverdueLabeledGood(FirstPosId, DefaultGoodId, SecondLabel,
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, ThirdLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1).AddHours(1))),
                    CreateLabeledGoodWithCorrectExpirationPeriod(FirstPosId, DefaultGoodId, FourthLabel,
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(3)))
                },
                new Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance>()
                {
                    [OverdueType.Overdue] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueInDay]  = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.Fresh] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    }),
                    [OverdueType.OverdueBetweenTommorowAndNextDay] = new ExpectedOverdueGoodsInfoInstance(1, new Dictionary<int, int>
                    {
                        {FirstPosId, 1}
                    })
                }

            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static LabeledGood CreateLabeledGoodWithCorrectExpirationPeriod(int posId, int goodId, string label, ExpirationPeriod period)
        {
            return LabeledGood.NewOfPosBuilder(posId, label)
                .TieToGood(goodId, new LabeledGoodPrice(DefaultLabeledGoodPrice, DefaultCurrencyId), period)
                .Build();
        }

        private static LabeledGood CreateOverdueLabeledGood(int posId, int goodId, string label, ExpirationPeriod period)
        {
            var labeledGood = LabeledGood.NewOfPosBuilder(posId, label)
                .TieToGood(goodId, new LabeledGoodPrice(DefaultLabeledGoodPrice, DefaultCurrencyId), ExpirationPeriod.FromNowTill(DateTime.MaxValue))
                .Build();

            labeledGood.SetProperty(nameof(LabeledGood.ManufactureDate), period.ManufactureDate);
            labeledGood.SetProperty(nameof(LabeledGood.ExpirationDate), period.ExpirationDate);

            return labeledGood;
        }
    }
}