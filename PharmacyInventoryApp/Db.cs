using System.Configuration;
using System.Data.SqlClient;

namespace PharmacyInventoryApp
{
    public static class Db
    {
        public static SqlConnection GetConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["PharmacyDB"].ConnectionString;
            return new SqlConnection(cs);
        }
    }
}
