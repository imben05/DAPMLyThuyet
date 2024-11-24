using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FlightManagement.Models
{
    public class DataModel
    {
        public string connectionString = "workstation id=DAPMLThuyet.mssql.somee.com;packet size=4096;user id=benso1_SQLLogin_1;pwd=kzs818eihg;data source=DAPMLThuyet.mssql.somee.com;persist security info=False;initial catalog=DAPMLThuyet;TrustServerCertificate=True";
        public ArrayList get(String sql)
        {
            ArrayList datalist = new ArrayList();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();

            using (SqlDataReader r = command.ExecuteReader())
            {
                while (r.Read()) {
                    ArrayList row = new ArrayList();
                    for (int i = 0; i < r.FieldCount; i++) {
                        row.Add(xulydulieu(r.GetValue(i).ToString()));
                    }
                    datalist.Add(row);
                }
            }
            connection.Close();
            return datalist;
        }
        public string xulydulieu(string text)
        {
            String s = text.Replace(",", "&44;");
            s = s.Replace("\"", "&34");
            s = s.Replace("'", "&39");
            s = s.Replace("\r", "");
            s = s.Replace("\n", "");
            return s;
        }
    }
}