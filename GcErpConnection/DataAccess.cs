using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GcErpConnection
{
    public abstract class DataAccess
    {
        public DataAccess() { }
        ~DataAccess() { }
        public abstract bool state();
        public abstract void connect();
        public abstract void disconnect();
        public abstract DataSet runQuerySqlDataSet(string sql, string table, Object[] myParamArrayObject);
        public abstract DataSet runQuerySqlDataSet(string sql, string table);
        public abstract DataSet runQuerySqlDataSet(string sql);
        public abstract string runSqlReturning(string sql, Object[] myParamArrayObject, string retparameter);
        public abstract void runSql(string sql);
        public abstract void runSql(string sql, Object[] myParamArray, ref string number);
        public abstract void runSql(string sql, Object[] myParamArray);
        public abstract int NextValueSequence(string name);
        public abstract void startTransaction();
        public abstract void commitTransaction();
        public abstract void rollBackTransaccion();
        public abstract void cleanState();

        public abstract string runSqlReturningstrdep(Guid user);

        public abstract DateTime getDateTime();
        public abstract bool IsError
        {
            set;
            get;
        }
        public abstract string ErrorDescription
        {
            set;
            get;
        }
        public abstract string Schema
        {
            get;
            set;
        }
        public abstract string TypeConnection
        {
            get;
        }
        public abstract string UserDataBase
        {
            get;
            set;
        }
        public abstract string UserName { get; set; }
        public abstract string Company { get; set; }
        public abstract string Port
        {
            get;
            set;
        }
        public abstract string Password
        {
            get;
            set;
        }
        public abstract string DataBase
        {
            get;
            set;
        }
        public abstract string Server
        {
            get;
            set;
        }
    }
}
