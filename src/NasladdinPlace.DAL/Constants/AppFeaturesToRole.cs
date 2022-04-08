using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.DAL.Constants
{
    public static class AppFeaturesToRole
    {
        public static Dictionary<string, List<Type>> GetAvailableAppFeaturesForRoles()
        {
            return new Dictionary<string, List<Type>>
            {
                {
                    Roles.Logistician,
                    new List<Type>
                    {
                        typeof(PosDoorsManagementPermission),
                        typeof(PosMonitoringAndSupportPermission),
                        typeof(PromotionSettingsPermission),
                        typeof(DiscountCrudPermission),
                        typeof(ReadOnlyAccess),
                        typeof(DocumentGoodsMovingPermission),
                        typeof(LabeledGoodManagementPermission),
                    }
                },
                {
                    Roles.Operation,
                    new List<Type>
                    {
                        typeof(PosCrudPermission),
                        typeof(PosVersionManagementPermission),
                        typeof(PosDoorsManagementPermission),
                        typeof(PosMonitoringAndSupportPermission),
                        typeof(ReadOnlyAccess),
                        typeof(LabeledGoodManagementPermission)
                    }
                },
                {
                    Roles.Commerce,
                    new List<Type>
                    {
                        typeof(PosScreenTemplateEditPermission),
                        typeof(PromotionSettingsPermission),
                        typeof(DiscountCrudPermission),
                        typeof(ReadOnlyAccess)
                    }
                },
                {
                    Roles.CatalogOperator,
                    new List<Type>
                    {
                        typeof(GoodCrudPermission),
                        typeof(DocumentGoodsMovingPermission),
                        typeof(DiscountCrudPermission),
                        typeof(ReportUploadingPermission),
                        typeof(MakerCrudPermission),
                        typeof(ReadOnlyAccess)
                    }
                },
                {
                    Roles.Hotline,
                    new List<Type>
                    {
                        typeof(DocumentGoodsMovingPermission),
                        typeof(ReadOnlyAccess)
                    }
                },
                {
                    Roles.ReadOnly,
                    new List<Type>
                    {
                        typeof(ReadOnlyAccess)
                    }
                }
            };
        }
    }
}
