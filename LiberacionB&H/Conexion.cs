using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiberacionB_H
{
    internal class Conexion
    {
        SqlConnection con = new SqlConnection("Data Source = xxxxx; Persist Security Info = True; User ID = sa_xxxx; Password = xxxx");

        public SqlConnection OpenConnection()
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            return con;
        }

        public SqlConnection CloseConnection()
        {
            if (con.State == ConnectionState.Open)
                con.Close();
            return con;
        }
    }
}
