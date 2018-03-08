using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace GcErpConnection
{
    public class DataAccessERP : DataAccess
    {
        public OracleConnection connection;
        private OracleTransaction transaction;
        private bool isTransaction;
        private string schema;
        string typeConnection = "1";
        static int instances = 0;

        public DataAccessERP(string connectionString)
        {
            try
            {
                cleanState();
                connection = new OracleConnection(connectionString);
                instances += 1;
                if (instances > 1)
                    return;
                try
                {
                    connection.Open();
                }
                catch (Exception error)
                {
                    instances = 0;
                    this.IsError = true;
                    this.ErrorDescription = error.Message;
                }
            }
            catch (OracleException xp)
            {
                instances = 0;
                exceptionProcessing(xp);
            }
        }

        // Indica el estado de la persistencia
        public override bool state()
        {
            cleanState();

            string message = "";

            // estado dela conexión
            switch (connection.State)
            {
                case ConnectionState.Broken:
                    message = "Error-0010 |||| La conexión con la base de datos fue interrumpida.";
                    break;
                case ConnectionState.Closed:
                    message = "Error-0011 |||| La conexión con la base de datos fue cerrada o no pudo ser establecida.";
                    break;
                case ConnectionState.Connecting:
                    message = "Error-0012 |||| Conectandose.";
                    break;
                case ConnectionState.Executing:
                    message = "Error-0013 |||| Ejecutando.";
                    break;
                case ConnectionState.Fetching:
                    message = "Error-0014 |||| Extrayendo.";
                    break;
                case ConnectionState.Open:
                    message = "Estado |||| Abierta.";
                    break;
            }

            // cargar la propiedad con el estado de la conexion
            ErrorDescription = message;

            if ((connection.State == ConnectionState.Open) ||
                (connection.State == ConnectionState.Executing) ||
                (connection.State == ConnectionState.Fetching))
                return true;
            else
                return false;
        }

        // destructor
        ~DataAccessERP()
        {
        }

        //
        public override void connect()
        {
            try
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                    instances = 1;
                }
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
            }
            finally
            {

            }
        }

        //
        public override void disconnect()
        {
            try
            {
                connection.Close();
                instances = 0;
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
            }
        }

        //Manipulacion de select
        public override DataSet runQuerySqlDataSet(string sql)
        {
            cleanState();
            OracleCommand command = new OracleCommand(sql, connection);
            command.CommandType = CommandType.Text;
            command.InitialLONGFetchSize = -1;

            OracleDataAdapter oDataAdapter = new OracleDataAdapter(command);
            DataSet oDataSet = new DataSet();
            if (this.isTransaction)
            {
                oDataAdapter.SelectCommand.Transaction = this.transaction;
            }
            try
            {
                oDataAdapter.Fill(oDataSet);
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
            }
            return oDataSet;
        }

        //
        public override DataSet runQuerySqlDataSet(string sql, string table, Object[] myParamArrayObjects)
        {
            cleanState();
            OracleCommand command = new OracleCommand(sql, connection);
            command.CommandType = CommandType.Text;
            command.InitialLONGFetchSize = -1;
            foreach (object myParamArrayObject in myParamArrayObjects)
            {
                command.Parameters.Add(((OracleParameter)myParamArrayObject));
            }
            OracleDataAdapter oDataAdapter = new OracleDataAdapter(command);
            DataSet oDataSet = new DataSet();
            if (this.isTransaction)
            {
                oDataAdapter.SelectCommand.Transaction = this.transaction;
            }
            try
            {
                oDataAdapter.Fill(oDataSet, table);
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
            }

            return oDataSet;
        }

        public override DataSet runQuerySqlDataSet(string sql, string table)
        {
            cleanState();
            OracleDataAdapter oDataAdapter = new OracleDataAdapter(sql, connection);
            DataSet oDataSet = new DataSet();
            try
            {
                oDataAdapter.Fill(oDataSet, table);
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
            }
            return oDataSet;
        }

        public override void runSql(String pSql)
        {
            cleanState();
            OracleCommand command = null;
            try
            {
                command = new OracleCommand(pSql, connection);
                if (this.isTransaction)
                {
                    command.Transaction = this.transaction;
                }
                command.ExecuteNonQuery();
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
            }

        }

        public override void runSql(string pSql, Object[] myParamArrayObjects, ref string pNumero)
        {
            cleanState();
            try
            {
                OracleCommand cmd = new OracleCommand(pSql, connection);
                cmd.CommandType = CommandType.Text;
                foreach (object myParamArrayObject in myParamArrayObjects)
                {
                    cmd.Parameters.Add((OracleParameter)myParamArrayObject);
                }

                if (this.isTransaction)
                {
                    cmd.Transaction = this.transaction;
                }
                pNumero = "";
                pNumero = cmd.ExecuteScalar().ToString();
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
            }
        }

        public override void runSql(string sql, Object[] myParamArrayObjects)
        {
            cleanState();
            try
            {
                OracleCommand cmd = new OracleCommand(sql, connection);
                cmd.CommandType = CommandType.Text;
                foreach (object myParamArrayObject in myParamArrayObjects)
                {
                    cmd.Parameters.Add((OracleParameter)myParamArrayObject);
                }
                if (this.isTransaction)
                {
                    cmd.Transaction = this.transaction;
                }
                cmd.ExecuteNonQuery();
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
            }
        }

        public override int NextValueSequence(string pNombre)
        {
            int valor = 0;
            cleanState();
            try
            {
                DataSet dsetDatos = new DataSet();
                dsetDatos = this.runQuerySqlDataSet("SELECT " + pNombre + ".NEXTVAL from dual ");
                valor = Convert.ToInt32(dsetDatos.Tables[0].Rows[0][0].ToString());
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
            }
            return valor;
        }

        public override void startTransaction()
        {
            if (this.isTransaction == false)
            {
                this.transaction = this.connection.BeginTransaction();
                this.isTransaction = true;
            }
        }


        public override void commitTransaction()
        {
            if (this.isTransaction)
            {
                this.transaction.Commit();
                this.isTransaction = false;
            }
        }


        public override void rollBackTransaccion()
        {
            if (this.isTransaction)
            {
                this.transaction.Rollback();
                this.isTransaction = false;
            }
        }

        public override DateTime getDateTime()
        {

            string sql = "SELECT getdate()";

            DataSet dsetDatos = new DataSet();
            dsetDatos = runQuerySqlDataSet(sql, "Fecha");

            return Convert.ToDateTime(dsetDatos.Tables[0].Rows[0][0].ToString());
        }

        public override string UserDataBase { get; set; }
        public override string UserName { get; set; }
        public override string Company { get; set; }

        public override string Server { get; set; }

        public override string DataBase { get; set; }

        public override string Password { get; set; }

        public override string Port { get; set; }

        public override bool IsError { get; set; }

        public override string ErrorDescription { get; set; }

        public override string Schema { get; set; }
        public override string TypeConnection
        {
            get { return this.typeConnection; }
        }

        public override void cleanState()
        {
            this.ErrorDescription = "";
            this.IsError = false;
        }

        private void exceptionProcessing(OracleException exception)
        {
            IsError = true;
            ErrorDescription += " \n" + exception.Message + " \n";
            ErrorDescription += exception.ErrorCode + " \n";
            ErrorDescription += exception.StackTrace;
        }

        public override string runSqlReturning(string sql, Object[] myParamArrayObjects, string retParameter)
        {
            cleanState();

            try
            {
                OracleCommand command = new OracleCommand(sql, connection);
                command.CommandType = CommandType.Text;
                foreach (object myParamArrayObject in myParamArrayObjects)
                {
                    command.Parameters.Add((OracleParameter)myParamArrayObject);
                }

                if (this.isTransaction)
                {
                    command.Transaction = this.transaction;
                }

                command.ExecuteNonQuery();
                return command.Parameters["" + retParameter].Value.ToString();
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
                return "";
            }
        }
        public override string runSqlReturningstrdep(Guid user)
        {
            string retru = null;
            cleanState();
            try
            {
                String userio = "2EE8896FB17E48FA9925793BEFBF59A8";

                string SQL = "SELECT \"DEPARTAMENTO\" FROM \"UCASCHEMA\".\"User\" WHERE \"User\".\"Id\" = '" + user+ "'";
                OracleCommand command = new OracleCommand(SQL, connection);
                command.CommandType = CommandType.Text;
               OracleDataReader reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    while (reader.Read())
                    {
                        retru = reader.GetString(1);
                    }
                   
                }
                
            }
            catch (OracleException error)
            {
                exceptionProcessing(error);
                return "";
            }
            return retru.ToString();
        }
    }
}
