using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;

namespace HolidayTest1
{
    public class SQLiteDatabase : IDisposable
    {
        private const string defconnstr = "Data Source={0};Version=3;New=False;";
        // other options: ;Read Only=True;FailIfMissing=True;Compress=True;

        private readonly string connstr;
        private SQLiteConnection conn = null;
        private SQLiteCommand command = null;
        private ArrayList paramlist = null;
        private SQLiteTransaction transaction = null;

        public SQLiteDatabase(string dbname)
        {
            paramlist = new ArrayList();
            connstr = string.Format(defconnstr, dbname);
        }

        public SQLiteDatabase(Dictionary<string, string> connectionOpts)
        {
            connstr = string.Empty;
            foreach (KeyValuePair<string, string> row in connectionOpts)
            {
                connstr += string.Format("{0}={1};", row.Key, row.Value);
            }
        }

        ~SQLiteDatabase()
        {
            Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Disconnect(false);

            if (conn != null)
                conn.Dispose();
        }

        #endregion

        public void CheckConnection()
        {
            if (conn == null)
                conn = new SQLiteConnection(connstr);

            if (conn.State == ConnectionState.Closed || command == null || command.Connection == null)
            {
                command = new SQLiteCommand(conn);
                conn.Open();
            }
        }

        public void Disconnect(bool commit)
        {
            if (commit)
                Commit();
            
            if (transaction != null)
            {
                transaction.Dispose();
                transaction = null;
            }

            if (paramlist != null)
                paramlist.Clear();

            if (command != null)
            {
                try
                {
                    command.Parameters.Clear();
                }
                catch (Exception) { }
                command.Dispose();
                command = null;
            }
        }

        public void Disconnect()
        {
            Disconnect(true);
        }

        public void BeginTransaction()
        {
            if (transaction == null || transaction.Connection == null)
            {
                if (transaction != null && transaction.Connection == null)
                {
                    transaction.Dispose();
                    transaction = null;
                }

                CheckConnection();
                transaction = conn.BeginTransaction();
            }
        }

        public void Commit()
        {
            if (transaction != null)
            {
                if (conn != null && conn.State != ConnectionState.Closed && transaction.Connection != null)
                {
                    transaction.Commit();
                }
                
                transaction.Dispose();
                transaction = null;
            }
        }


        public void Rollback()
        {
            if (transaction != null)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception) { }

                transaction.Dispose();
                transaction = null;
            }
        }

        public void AddParameter(string name, object value, DbType paramType)
        {
            CheckConnection();
            
            SQLiteParameter param = new SQLiteParameter(name, paramType);
            param.Direction = ParameterDirection.Input;
            param.Value = value;
            paramlist.Add(param);
        }

        public void AddParameters(Dictionary<string, string> connectionOpts)
        {
            foreach (KeyValuePair<string, string> row in connectionOpts)
            {
                AddParameter(row.Key, row.Value, DbType.String);
            }
        }

        private void Prepare()
        {
            command.Parameters.Clear();
            command.Parameters.AddRange(paramlist.ToArray());
        }

        public void ClearParameters()
        {
            paramlist.Clear();
        }

        public int ExecuteNonQuery(string sql)
        {
            try
            {
                CheckConnection();
                command.CommandText = sql;
                Prepare();
                //MessageBox.Show(command.CommandText);

                int retval = command.ExecuteNonQuery();

                return retval;
            }
            catch (Exception err)
            {
                Rollback();
                Disconnect(false);
                throw err;
            }
        }

        public object ExecuteScalar(string sql)
        {
            try
            {
                CheckConnection();
                command.CommandText = sql;
                Prepare();

                object retval = command.ExecuteScalar();

                return retval;
            }
            catch (Exception err)
            {
                Rollback();
                Disconnect(false);
                throw err;
            }
        }

        public System.Data.IDataReader ExecuteReader(string sql)
        {
            try
            {
                CheckConnection();
                command.CommandText = sql;
                Prepare();

                return (System.Data.IDataReader)command.ExecuteReader();
            }
            catch (Exception err)
            {
                Rollback();
                Disconnect(false);
                throw err;
            }
        }

        public DataTable GetDataTable(string sql)
        {
            return GetDataTable(sql, null);
        }

        public DataTable GetDataTable(string sql, IEnumerable<string> allowDBNullColumns)
        {
            var dt = new DataTable();
            if (allowDBNullColumns != null)
                foreach (var s in allowDBNullColumns)
                {
                    dt.Columns.Add(s);
                    dt.Columns[s].AllowDBNull = true;
                }
            try
            {
                var reader = ExecuteReader(sql);
                dt.Load(reader);
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        public long Insert(String tableName, Dictionary<String, String> data)
        {
            String columns = "";
            String values = "";
            ClearParameters();
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString(CultureInfo.InvariantCulture));
                values += String.Format(" @{0},", val.Key.ToString(CultureInfo.InvariantCulture));

                bool dflag = val.Key.ToString().Contains("date");
                DateTime dval = DateTime.MinValue;
                try
                {
                    if (dflag) dval = DateTime.Parse(val.Value.ToString());
                }
                catch (Exception) { dflag = false; }
                if (dflag)
                {
                    AddParameter(
                        "@" + val.Key.ToString(CultureInfo.InvariantCulture),
                        dval,
                        DbType.Date);
                }
                else
                {
                    AddParameter(
                        "@" + val.Key.ToString(CultureInfo.InvariantCulture),
                        val.Value.ToString(CultureInfo.InvariantCulture),
                        DbType.String);
                }
            }
            columns = columns.Trim(new char[] { ' ', ',' });
            values = values.Trim(new char[] { ' ', ',' });
            ExecuteNonQuery(String.Format("insert into {0} ({1}) values ({2});",
                tableName, columns, values));
            long retval = (long)ExecuteScalar("SELECT last_insert_rowid()");
            return retval;
        }

        public bool Update(string tableName, Dictionary<string, string> data, string where)
        {
            string vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<string, string> val in data)
	            {
	                vals += String.Format(" {0} = @{1},",
                        val.Key.ToString(CultureInfo.InvariantCulture),
                        val.Key.ToString(CultureInfo.InvariantCulture));
                    AddParameter(
                        "@" + val.Key.ToString(CultureInfo.InvariantCulture),
                        val.Key.ToString(CultureInfo.InvariantCulture), DbType.String);
	            }
                vals = vals.Trim(new char[] { ' ', ',' });
            }
            try
            {
                ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }
            return returnCode;
        }

        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message, fail.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                returnCode = false;
            }
            return returnCode;
        }

        public static bool CreateDB(string filePath)
        {
            try
            {
                SQLiteConnection.CreateFile(filePath);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool ClearDB()
        {
            try
            {
                var tables = GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    ClearTable(table["NAME"].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ClearTable(String table)
        {
            try
            {
                ExecuteNonQuery(String.Format("delete from {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CompactDB()
        {
            try
            {
                ExecuteNonQuery("Vacuum;");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}