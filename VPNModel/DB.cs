///// Erkan Esen
///// Feb. 2018 
///// VPNSpot
///// SQLite CRUD operations
/////
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPNModel
{
    public class DB
    {
        private string dbName;

        public string DbName
        {
            get { return dbName; }
            set { dbName = value; }
        }

        public DB(string dbName)
        {
            this.dbName = dbName;
        }

        /// <summary>
        /// if not exist db file => create db file
        /// </summary>
        /// <param name="dbName"></param>
        public void CheckDB()
        {
            if (!File.Exists(dbName))
                SQLiteConnection.CreateFile(dbName);
            CreateTables();
        }

        /// <summary>
        /// SQLite create tables if no exist tables 
        /// </summary>
        public void CreateTables()
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {

                    command.CommandText = "SELECT name FROM sqlite_master WHERE name='Connections'";
                    var name = command.ExecuteScalar();

                    // check account table exist or not 
                    // if exist do nothing 
                    if (name != null && name.ToString() == "Connections")
                        return;
                    // acount table not exist, create table and insert 
                    command.CommandText = "CREATE TABLE `Connections` ( `Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,`VpnName` TEXT NOT NULL, `ServerAddress` TEXT NOT NULL, `UserName` TEXT, `Password` TEXT, `Company` TEXT, `Location` TEXT )";
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
        }


        /// <summary>
        /// SQLite Check Exist Records in VpnName Column by Same Name
        /// </summary>
        /// <param name="VpnName"></param>
        /// <returns></returns>
        public bool IsThereRecord(string VpnName)
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {
                    command.CommandText = "SELECT VpnName FROM Connections WHERE VpnName='" + VpnName + "'";
                    var name = command.ExecuteScalar();
                    if (name != null && name.ToString() == VpnName)
                    {
                        return true;
                    }
                }
                con.Close();
            }
            return false;
        }

        /// <summary>
        /// SQLite Check Exist Records in VpnName Column by Same Name Expect Id
        /// </summary>
        /// <param name="VpnName"></param>
        /// <returns></returns>
        public bool IsThereRecord(string VpnName, long Id)
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {
                    command.CommandText = "SELECT VpnName FROM Connections WHERE VpnName='" + VpnName + "' AND Id!=" + Id.ToString() + "";
                    var name = command.ExecuteScalar();
                    if (name != null && name.ToString() == VpnName)
                    {
                        return true;
                    }
                }
                con.Close();
            }
            return false;
        }

        /// <summary>
        /// SQLite Get All Records
        /// </summary>
        /// <returns></returns>
        public List<VPNobject> ListVpnRecords()
        {
            List<VPNobject> dbVPNList = new List<VPNobject>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "select * from Connections order by Id desc";
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        dbVPNList.Add(new VPNobject
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            VpnName = (string)reader["VpnName"],
                            ServerAddress = (string)reader["ServerAddress"],
                            UserName = (string)reader["UserName"],
                            Password = (string)reader["Password"],
                            Company = (string)reader["Company"],
                            Location = (string)reader["Location"]
                        });
                    }
                }
                con.Close();
            }
            return dbVPNList;
        }

        /// <summary>
        /// SQLite Get Records by Company and Location
        /// </summary>
        /// <returns></returns>
        public List<VPNobject> ListVpnRecords(string company, string location)
        {
            List<VPNobject> dbVPNList = new List<VPNobject>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "select * from Connections where Company = '" + company + "' AND Location = '" + location + "' order by VpnName";
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        dbVPNList.Add(new VPNobject
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            VpnName = (string)reader["VpnName"],
                            ServerAddress = (string)reader["ServerAddress"],
                            UserName = (string)reader["UserName"],
                            Password = (string)reader["Password"],
                            Company = (string)reader["Company"],
                            Location = (string)reader["Location"]
                        });
                    }
                }
                con.Close();
            }
            return dbVPNList;
        }

        /// <summary>
        /// SQLite Get Data List of Location Column
        /// </summary>
        /// <returns></returns>
        public List<string> GetLocationList()
        {
            List<string> list = new List<string>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                string sql = "select Location from Connections group by Location";
                SQLiteCommand command = new SQLiteCommand(sql, con);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add((string)reader["Location"]);
                }
            }

            return list;
        }

        /// <summary>
        /// SQLite Get Data List of Location Column
        /// </summary>
        /// <returns></returns>
        public List<string> GetLocationList(string company)
        {
            List<string> list = new List<string>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                string sql = "select Location from Connections where Company = '" + company + "' group by Location";
                SQLiteCommand command = new SQLiteCommand(sql, con);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add((string)reader["Location"]);
                }
            }

            return list;
        }

        /// <summary>
        /// SQLite Get Data List of Company Column
        /// </summary>
        /// <returns></returns>
        public List<string> GetCompanyList()
        {
            List<string> list = new List<string>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                string sql = "select Company from Connections group by Company";
                SQLiteCommand command = new SQLiteCommand(sql, con);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add((string)reader["Company"]);
                }
            }

            return list;
        }

        /// <summary>
        /// SQLite Get Records by Company Columns
        /// </summary>
        /// <param name="Company"></param>
        /// <returns></returns>
        public List<VPNobject> GetVPNsByCompany(string Company)
        {
            List<VPNobject> list = new List<VPNobject>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "select * from Connections where Company='" + Company + "' order by VpnName";
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new VPNobject
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            VpnName = (string)reader["VpnName"],
                            ServerAddress = (string)reader["ServerAddress"],
                            UserName = (string)reader["UserName"],
                            Password = (string)reader["Password"],
                            Company = (string)reader["Company"],
                            Location = (string)reader["Location"]
                        });
                    }
                }
                con.Close();
            }

            return list;
        }

        /// <summary>
        /// SQLite Get Records by Location Columns
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        public List<VPNobject> GetVPNsByLocation(string Location)
        {
            List<VPNobject> list = new List<VPNobject>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "select * from Connections where Location='" + Location + "' order by VpnName";
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new VPNobject
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            VpnName = (string)reader["VpnName"],
                            ServerAddress = (string)reader["ServerAddress"],
                            UserName = (string)reader["UserName"],
                            Password = (string)reader["Password"],
                            Company = (string)reader["Company"],
                            Location = (string)reader["Location"]
                        });
                    }
                }
                con.Close();
            }
            return list;
        }


        /// <summary>
        /// SQLite Insert Record
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void AddRecord(ref VPNobject data)
        {

            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "insert into Connections (VpnName,ServerAddress,UserName,Password,Company,Location) values(@VpnName,@ServerAddress,@UserName,@Password,@Company,@Location); " +
                        "select last_insert_rowid();";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@VpnName", data.VpnName));
                    command.Parameters.Add(new SQLiteParameter("@ServerAddress", data.ServerAddress));
                    command.Parameters.Add(new SQLiteParameter("@UserName", data.UserName));
                    command.Parameters.Add(new SQLiteParameter("@Password", data.Password));
                    command.Parameters.Add(new SQLiteParameter("@Company", data.Company));
                    command.Parameters.Add(new SQLiteParameter("@Location", data.Location));
                    object obj = command.ExecuteScalar();
                    data.Id = (long)obj;

                }
                con.Close();
            }

        }

        /// <summary>
        /// SQLite Delete Record
        /// </summary>
        /// <param name="id"></param>
        /// <returns>-1 an Error</returns>
        public int DeleteRecord(long id)
        {
            int status;
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "delete from Connections where Id=" + id.ToString() + "";
                    status = command.ExecuteNonQuery();
                }
                con.Close();
            }
            return status;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>-1 an Error</returns>
        public int UpdateRecord(VPNobject data)
        {
            int status;
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbName))
            {

                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "update Connections set VpnName=@VpnName, ServerAddress=@ServerAddress, UserName=@UserName, Password=@Password, Company=@Company, Location=@Location where Id=@Id; " + "";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@Id", data.Id));
                    command.Parameters.AddWithValue("@VpnName", data.VpnName);
                    command.Parameters.AddWithValue("@ServerAddress", data.ServerAddress);
                    command.Parameters.AddWithValue("@UserName", data.UserName);
                    command.Parameters.AddWithValue("@Password", data.Password);
                    command.Parameters.AddWithValue("@Company", data.Company);
                    command.Parameters.AddWithValue("@Location", data.Location);
                    status = command.ExecuteNonQuery();
                }
                con.Close();
            }
            return status;
        }
    }

    public static class DBStatic
    {
        public static DB db = new DB("VPNSpotDB");

    }
}