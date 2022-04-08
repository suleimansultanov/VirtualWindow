using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL;
using NasladdinPlace.TestUtils.Cleaning.Contracts;

namespace NasladdinPlace.TestUtils.Cleaning
{
    public class SqlServerDbCleaner : IDbCleaner
    {
        private readonly ApplicationDbContext _context;

        public SqlServerDbCleaner(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public void Clean()
        {
            ClearTablesData();
            ReseedIdentityTables();
            DetachAllEntries();
        }

        private void 
            ClearTablesData()
        {
            var tables = new[]
            {
                nameof(ApplicationDbContext.AppFeatureItemsToRoles),
                nameof(ApplicationDbContext.AppFeatureItems),
                nameof(ApplicationDbContext.PointsOfSaleToRoles),
                nameof(ApplicationDbContext.ConfigurationValues),
                nameof(ApplicationDbContext.ConfigurationKeys),
                nameof(ApplicationDbContext.PromotionSettings),
                nameof(ApplicationDbContext.PosDiscounts),
                nameof(ApplicationDbContext.DiscountRuleValues),
                nameof(ApplicationDbContext.DiscountRules),
                nameof(ApplicationDbContext.Discounts),
                nameof(ApplicationDbContext.PosImages),
                nameof(ApplicationDbContext.GoodImages),
                nameof(ApplicationDbContext.BankTransactionInfosVersionTwo),
                nameof(ApplicationDbContext.BankTransactionInfos),
                nameof(ApplicationDbContext.FiscalizationCheckItems),
                nameof(ApplicationDbContext.FiscalizationInfosVersionTwo),
                nameof(ApplicationDbContext.FiscalizationInfos),
                nameof(ApplicationDbContext.UserRoles),
                nameof(ApplicationDbContext.Roles),
                nameof(ApplicationDbContext.UsersBonusPoints),
                nameof(ApplicationDbContext.Feedbacks),
                nameof(ApplicationDbContext.CheckItemsAuditHistory),
                nameof(ApplicationDbContext.PosOperationTransactionCheckItems),
                nameof(ApplicationDbContext.PosOperationTransactions),
                nameof(ApplicationDbContext.CheckItems),
                nameof(ApplicationDbContext.DocumentGoodsMovingLabeledGoods),
                nameof(ApplicationDbContext.DocumentGoodsMovingTableItems),
                nameof(ApplicationDbContext.DocumentsGoodsMoving),
                nameof(ApplicationDbContext.LabeledGoodsTrackingHistory),
                nameof(ApplicationDbContext.LabeledGoods),
                nameof(ApplicationDbContext.Currencies),
                nameof(ApplicationDbContext.PosTemperatures),
                nameof(ApplicationDbContext.PosDoorsStates),
                nameof(ApplicationDbContext.PosOperations),
                nameof(ApplicationDbContext.ProteinsFatsCarbohydratesCalories),
                nameof(ApplicationDbContext.Goods),
                nameof(ApplicationDbContext.PaymentCards),
                nameof(ApplicationDbContext.UserNotifications),
                nameof(ApplicationDbContext.Users),
                nameof(ApplicationDbContext.AllowedPosModes),
                nameof(ApplicationDbContext.PosAbnormalSensorMeasurements),
                nameof(ApplicationDbContext.PointsOfSale),
                nameof(ApplicationDbContext.Makers),
                nameof(ApplicationDbContext.Cities),
                nameof(ApplicationDbContext.Countries),
                nameof(ApplicationDbContext.PromotionLogs),
                nameof(ApplicationDbContext.ReportsUploadingInfo)
            };

            var deleteCommands = GenerateCommandsForTables(tables, GenerateDeleteCommandForTable);
            var commands = new List<string>
            {
                CreateSettingTableColumnToNullSql(nameof(ApplicationDbContext.Users), nameof(ApplicationUser.ActivePaymentCardId)),
                CreateSettingTableColumnToNullSql(nameof(ApplicationDbContext.PosOperationTransactions), nameof(PosOperationTransaction.LastBankTransactionInfoId)),
                CreateSettingTableColumnToNullSql(nameof(ApplicationDbContext.PosOperationTransactions), nameof(PosOperationTransaction.LastFiscalizationInfoId)),
                CreateSettingTableColumnToDefaultNumberValueSql(nameof(ApplicationDbContext.PointsOfSale), nameof(Pos.PosScreenTemplateId)),
                CreateDeleteExceptFirstRowsSql(nameof(ApplicationDbContext.PosScreenTemplates))
            };
            commands.AddRange(deleteCommands);
            ExecuteSqlCommands(commands);
        }

        private static string CreateSettingTableColumnToNullSql(string tableName, string fieldName)
        {
            return CreateSettingTableColumnToValueSql(tableName, fieldName, "NULL");
        }

        private static string CreateSettingTableColumnToDefaultNumberValueSql(string tableName, string fieldName)
        {
            return CreateSettingTableColumnToValueSql(tableName, fieldName, default(int).ToString());
        }

        private static string CreateSettingTableColumnToValueSql(string tableName, string fieldName, string value)
        {
            return $"UPDATE {tableName} SET {fieldName} = {value};";
        }

        private static string CreateDeleteExceptFirstRowsSql(string tableName, int countSkipRows = 1, string orderType = "ASC")
        {
            return $"DELETE FROM {tableName} WHERE Id NOT IN(SELECT TOP {countSkipRows} Id FROM {tableName} ORDER BY Id {orderType})";
        }

        private void ReseedIdentityTables()
        {
            var tables = new[]
            {
                nameof(ApplicationDbContext.PosImages),
                nameof(ApplicationDbContext.GoodImages),
                nameof(ApplicationDbContext.BankTransactionInfosVersionTwo),
                nameof(ApplicationDbContext.FiscalizationInfosVersionTwo),
                nameof(ApplicationDbContext.PosOperationTransactions),
                nameof(ApplicationDbContext.BankTransactionInfos),
                nameof(ApplicationDbContext.UsersBonusPoints),
                nameof(ApplicationDbContext.Feedbacks),
                nameof(ApplicationDbContext.CheckItemsAuditHistory),
                nameof(ApplicationDbContext.CheckItems),
                nameof(ApplicationDbContext.LabeledGoodsTrackingHistory),
                nameof(ApplicationDbContext.LabeledGoods),
                nameof(ApplicationDbContext.Currencies),
                nameof(ApplicationDbContext.FiscalizationInfos),
                nameof(ApplicationDbContext.PosOperationTransactionCheckItems),
                nameof(ApplicationDbContext.FiscalizationCheckItems),
                nameof(ApplicationDbContext.PosOperations),
                nameof(ApplicationDbContext.Goods),
                nameof(ApplicationDbContext.PaymentCards),
                nameof(ApplicationDbContext.UserNotifications),
                nameof(ApplicationDbContext.Users),
                nameof(ApplicationDbContext.PosAbnormalSensorMeasurements),
                nameof(ApplicationDbContext.PosTemperatures),
                nameof(ApplicationDbContext.PosDoorsStates),
                nameof(ApplicationDbContext.PointsOfSale),
                nameof(ApplicationDbContext.Makers),
                nameof(ApplicationDbContext.Cities),
                nameof(ApplicationDbContext.Countries),
                nameof(ApplicationDbContext.PosDiscounts),
                nameof(ApplicationDbContext.DiscountRuleValues),
                nameof(ApplicationDbContext.DiscountRules),
                nameof(ApplicationDbContext.Discounts),
                nameof(ApplicationDbContext.ReportsUploadingInfo),
                nameof(ApplicationDbContext.PosScreenTemplates),
                nameof(ApplicationDbContext.DocumentGoodsMovingLabeledGoods),
                nameof(ApplicationDbContext.DocumentGoodsMovingTableItems),
                nameof(ApplicationDbContext.DocumentsGoodsMoving)
            };
            var reseedCommands = GenerateCommandsForTables(tables, GenerateReseedCommandForTable);
            ExecuteSqlCommands(reseedCommands);
        }

        private IEnumerable<string> GenerateCommandsForTables(IEnumerable<string> tables, Func<string, string> commandGenerator)
        {
            var deleteCommands = new Collection<string>();
            foreach (var table in tables)
            {
                deleteCommands.Add(commandGenerator(table));
            }
            return deleteCommands.ToArray();
        }

        private void ExecuteSqlCommands(IEnumerable<string> sqlCommands)
        {
            foreach (var sqlCommand in sqlCommands)
            {
                _context.Database.ExecuteSqlCommand(sqlCommand);
            }
        }
        
        private static string GenerateDeleteCommandForTable(string table)
        {
            return $"DELETE FROM {table};";
        }

        private static string GenerateReseedCommandForTable(string table)
        {
            return $"DBCC CHECKIDENT({table}, RESEED, 0);";
        }

        private void DetachAllEntries()
        {
            foreach (var dbEntityEntry in _context.ChangeTracker.Entries().ToImmutableList())
            {
                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }
    }
}