using System.Configuration;
using PetaPoco;

namespace Dal.DbContext
{
    public class PetaDbContext : Database
    {
        private static string ConnectionStringName
        {
            get
            {
                ConnectionStringSettings connStr = ConfigurationManager.ConnectionStrings["ConnectionString"];

                if (connStr != null)
                {
                    return connStr.Name;
                }
                return "";
            }
        }

        public PetaDbContext()
            : base(ConnectionStringName)
        {
        }

        public PetaDbContext(string connectionStr) : base(connectionStr)
        {

        }
    }
}
