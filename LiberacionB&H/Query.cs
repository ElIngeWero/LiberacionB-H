using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Remoting.Messaging;

namespace LiberacionB_H
{
    internal class Query
    {

        Conexion con = new Conexion();
        SqlCommand sqlC = new SqlCommand();
        SqlDataReader Read;
        DataTable tabla1 = new DataTable();

        public (string, string) GetPNSN(string etiqueta)
        {
            try
            {
                sqlC.Connection = con.OpenConnection();
                sqlC.CommandText = "xxxx";
                sqlC.CommandType = CommandType.StoredProcedure;
                sqlC.CommandTimeout = 5;

                // Agregar parámetros de entrada si es necesario
                sqlC.Parameters.AddWithValue("@LineID", 35);
                sqlC.Parameters.AddWithValue("@StationID", 10);
                sqlC.Parameters.AddWithValue("@SubStationID", 0);
                sqlC.Parameters.AddWithValue("@ProductPartNumber", 10);
                sqlC.Parameters.AddWithValue("@LabelData", etiqueta);

                // Agregar parámetro de salida para PartNumber
                SqlParameter outputParameter = new SqlParameter();
                outputParameter.ParameterName = "@PartNumber";
                outputParameter.SqlDbType = SqlDbType.NVarChar;
                outputParameter.Direction = ParameterDirection.Output;
                outputParameter.Size = 50;
                sqlC.Parameters.Add(outputParameter);

                // Agregar parámetro de salida para SerialNumber
                SqlParameter outputParameter2 = new SqlParameter();
                outputParameter2.ParameterName = "@SerialNumber";
                outputParameter2.SqlDbType = SqlDbType.NVarChar;
                outputParameter2.Direction = ParameterDirection.Output;
                outputParameter2.Size = 50;
                sqlC.Parameters.Add(outputParameter2);


                Read = sqlC.ExecuteReader();


                string partNumber = sqlC.Parameters["@PartNumber"].Value.ToString();
                string serialNumber = sqlC.Parameters["@SerialNumber"].Value.ToString();


                return (partNumber, serialNumber);
            }
            catch (SqlException ex)
            {
                return (null, null);
                throw ex;
            }
            finally
            {
                con.CloseConnection();
            }

        }

        public DataTable GetTolerances(string partNumber)
        {
            sqlC.Connection = con.OpenConnection();
            sqlC.CommandText = $"SELECT * FROM xxxx WHERE ProductPartNumber = {partNumber} ";
            sqlC.CommandType = CommandType.Text;
            sqlC.CommandTimeout = 5;
            Read = sqlC.ExecuteReader();
            tabla1.Load(Read);
            con.CloseConnection();
            return tabla1;
        }

        public (string, int) GetBatchNumber(string partnumber, string serialnumber)
        {
            string batch = " ";
            int sta = 10;
            //if (StationID == "332")
            //{
            //    StationID = "32";
            //}
            sqlC.Connection = con.OpenConnection();
            sqlC.CommandText = $"SELECT BatchNumber, Status FROM xxxx WHERE PartNumber = '{partnumber}' AND SerialNumber = '{serialnumber}'";
            sqlC.CommandType = CommandType.Text;
            sqlC.CommandTimeout = 9;
            Read = sqlC.ExecuteReader();
            if (Read.Read())
            {
                batch = Read["BatchNumber"].ToString();
                sta = Convert.ToInt32(Read["Status"]);
            }
            con.CloseConnection();
            return (batch, sta);
        }

        public string GetCurrentBatch(string celda, string cavidad)
        {
            string batch = " ";
            sqlC.Connection = con.OpenConnection();
            sqlC.CommandText = $"SELECT BatchNumber FROM xxxx WHERE StationID = {celda} AND CavityID = '{cavidad}'";
            sqlC.CommandType = CommandType.Text;
            sqlC.CommandTimeout = 9;
            Read = sqlC.ExecuteReader();
            if (Read.Read())
            {
                batch = Read["BatchNumber"].ToString();
            }
            con.CloseConnection();
            return batch;
        }


        public void insertDatos(List<string> listaWA, List<int> listamed, List<string> listachk, string partnumber, string serialnumber, string batchnumber)
        {    
            StringBuilder concatenado = new StringBuilder();

            for (int i = 0; i < listaWA.Count; i++)
            {
                concatenado.Append($"(GETDATE(), '{partnumber}', '{serialnumber}', '{batchnumber}', '{listaWA[i]}', '{listamed[i]}', '{listachk[i]}'),");
            }

            string cadena = concatenado.ToString().TrimEnd(',');

            string insertD = $"INSERT INTO xxxx " +
                $"([DateandTime],[PartNumber],[SerialNumber],[BatchNumber],[WeldAglet],[Lenght],[DTest])" +
                $" VALUES {cadena}";

            sqlC.Connection = con.OpenConnection();
            sqlC.CommandText = insertD;
            sqlC.CommandType = CommandType.Text;
            sqlC.CommandTimeout = 9;
            Read = sqlC.ExecuteReader();
            con.CloseConnection();
        }

        public void UpdateBatchParts(string batchnumber, int status)
        {

            sqlC.Connection = con.OpenConnection();
            sqlC.CommandText = $"UPDATE xxxx SET [Status] = {status} WHERE BatchNumber = '{batchnumber}'";
            sqlC.CommandType = CommandType.Text;
            sqlC.CommandTimeout = 9;
            Read = sqlC.ExecuteReader();
            con.CloseConnection();
        }

        public string batchdate(string batchnumber)
        {
            string batch = " ";
            sqlC.Connection = con.OpenConnection();
            sqlC.CommandText = $"SELECT TOP(1) DateandTime FROM xxxx WHERE BatchNumber = '{batchnumber}'";
            sqlC.CommandType = CommandType.Text;
            sqlC.CommandTimeout = 9;
            Read = sqlC.ExecuteReader();
            if (Read.Read())
            {
                batch = Read["DateandTime"].ToString();
            }
            con.CloseConnection();
            return batch;
        }

        public List<string> PiezasValidas(string batchnumber)
        {
            List<string> datosObtenidos = new List<string>();

            sqlC.Connection = con.OpenConnection();
            sqlC.CommandText = $"SELECT TOP(10) SerialNumber FROM xxxx WHERE BatchNumber = '{batchnumber}' ORDER BY DateandTime DESC";
            sqlC.CommandType = CommandType.Text;
            sqlC.CommandTimeout = 9;
            Read = sqlC.ExecuteReader();
            while (Read.Read())
            {
                string dato = Read.GetString(0); // Suponiendo que la columna es de tipo string
                datosObtenidos.Add(dato);
            }
            return datosObtenidos;
        }

    }
    
}
