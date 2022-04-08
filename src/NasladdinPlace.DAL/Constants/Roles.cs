namespace NasladdinPlace.DAL.Constants
{
    public class Roles
    {
        private Roles()
        {
            // intentionally left empty
        }
        
        public const string CommaSeparatedAllRoles = Admin + ", " + Logistician + ", " + Operation + ", " + Hotline + ", " + Commerce + ", " + CatalogOperator + ", " + ReadOnly;
        public const string CommaSeparatedAdminAndLogistician = Admin + ", " + Logistician;
        public const string Admin = "Admin";
        public const string Logistician = "Logistician";
        public const string Operation = "Operation";
        public const string Hotline = "Hotline";
        public const string Commerce = "Commerce";
        public const string CatalogOperator = "CatalogOperator";
        public const string ReadOnly = "ReadOnly";
        public static readonly string[] All = {Admin, Logistician, Operation, Hotline, Commerce, CatalogOperator, ReadOnly};
    }
}