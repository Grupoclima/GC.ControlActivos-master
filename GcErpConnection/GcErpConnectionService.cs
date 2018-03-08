using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Abp.Application.Services;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Categories;
using AdministracionActivosSobrantes.Cellars;
using AdministracionActivosSobrantes.Projects;
using AdministracionActivosSobrantes.ResponsiblePerson;
using AdministracionActivosSobrantes.Stocks;
using AdministracionActivosSobrantes.Stocks.Dto;

namespace GcErpConnection
{
    public class GcErpConnectionService : ApplicationService, IGcErpConnectionService
    {
        DataAccessERP connectionDataAccessErp;
        public bool Error { get; set; }
        public string ErrorDescription { get; set; }
        private readonly IAssetManager _assetManager;
        private readonly IProjectManager _projectManager;
        private readonly ICellarManager _cellarManager;
        private readonly ICategoryManager _categoryManager;
        private readonly IStockManager _stockManager;
        private readonly IResponsiblePersonManager _responsibleManager;

        public GcErpConnectionService(IAssetManager assetManager,
            IProjectManager projectManager, ICellarManager cellarManager,
            ICategoryManager categoryManager, IStockManager stockManager,
            IResponsiblePersonManager responsibleManager)
        {
            _responsibleManager = responsibleManager;
            _assetManager = assetManager;
            _projectManager = projectManager;
            _cellarManager = cellarManager;
            _categoryManager = categoryManager;
            _stockManager = stockManager;
        }

        public void ConnectErp(string userERP, string pass, string company)
        {
            string user = userERP;//ConfigurationManager.AppSettings["DbUser"];
            string password = pass;//ConfigurationManager.AppSettings["DbPassword"];
            string server = ConfigurationManager.AppSettings["ServerAddress"];
            string dataBase = ConfigurationManager.AppSettings["DbConnection"];
            string port = ConfigurationManager.AppSettings["ServerPort"];

            connectionDataAccessErp = new DataAccessERP(
                    "DATA SOURCE = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = " + server + ")(PORT = " + port +
                    "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + dataBase + "))); USER ID = " + user + "; PASSWORD = " + password);

            if (!connectionDataAccessErp.state())
            {
                //MessageBox.Show(
                //    "Error inesperado al intentar establecer la conexión con la base de datos del ERP. Detalle técnico: " +
                //    connectionDataAccessErp.ErrorDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool CheckUserDatabase(string userERP, string pass, string company)
        {
            string user = userERP;//ConfigurationManager.AppSettings["DbUser"];
            string password = pass;//ConfigurationManager.AppSettings["DbPassword"];
            string server = ConfigurationManager.AppSettings["ServerAddress"];
            string dataBase = ConfigurationManager.AppSettings["DbConnection"];
            string port = ConfigurationManager.AppSettings["ServerPort"];

            connectionDataAccessErp = new DataAccessERP(
                "DATA SOURCE = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = " + server + ")(PORT = " + port +
                "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + dataBase + "))); USER ID = " + user +
                "; PASSWORD = " + password);
            if (!connectionDataAccessErp.state())
            {
                connectionDataAccessErp.disconnect();
                return false;
            }
            connectionDataAccessErp.disconnect();
            return true;
        }

        //public bool CheckUserErp(string compania, string usuario, string contrasena)
        //{
        //    SisIntegracionClimaSoapClient temp = new SisIntegracionClimaSoapClient();
        //    bool autenticado = false;
        //    string mensajeError = string.Empty;
        //    return temp.ValidarUsuario(compania, usuario, contrasena, out autenticado, out mensajeError);
        //}

        private void CleanErrors()
        {
            Error = false;
            ErrorDescription = "";
        }


        public IList<string> GetErpCompanies()
        {
            CleanErrors();
            DataSet returnResultDataSet = new DataSet();
            IList<string> result = new List<string>();
            try
            {
                string executeQuery = "SELECT * FROM UCASCHEMA.V_UCA_CONJUNTOS";
                returnResultDataSet = connectionDataAccessErp.runQuerySqlDataSet(executeQuery);
                foreach (DataRow dr in returnResultDataSet.Tables[0].Rows)
                    result.Add(Convert.ToString(dr["CONJUNTO"]));

                if (connectionDataAccessErp.IsError)
                {
                    Error = true;
                    ErrorDescription = connectionDataAccessErp.ErrorDescription;
                }
            }
            catch (Exception xp)
            {
                Error = true;
                ErrorDescription = xp.Message;
            }
            return result;
        }

        public IList<ResponsiblePerson> GetResposiblesPersons(string company, Guid user)
        {
            CleanErrors();
            IList<ResponsiblePerson> result = new List<ResponsiblePerson>();
            try
            {
                string executeQuery = "SELECT * FROM UCASCHEMA.V_UCA_RESPONSABLES";
                var returnResultDataSet = connectionDataAccessErp.runQuerySqlDataSet(executeQuery);
                foreach (DataRow dr in returnResultDataSet.Tables[0].Rows)
                {
                    ResponsiblePerson newRp = ResponsiblePerson.Create(Convert.ToString(dr["NOMBRE"]), Convert.ToString(dr["RESPONSABLE"]), user, DateTime.Now, company);
                    result.Add(newRp);
                }

                if (connectionDataAccessErp.IsError)
                {
                    Error = true;
                    ErrorDescription = connectionDataAccessErp.ErrorDescription;
                }
            }
            catch (Exception xp)
            {
                Error = true;
                ErrorDescription = xp.Message;
            }
            return result;
        }
      /*  public IList<ResponsiblePerson> GetProyectosDepreciables(string company, string proyecto)
        {
            CleanErrors();
            IList<ResponsiblePerson> result = new List<ResponsiblePerson>();
            try
            {
                string executeQuery = "SELECT * FROM UCASCHEMA.V_UCA_RESPONSABLES WHERE CENTRO_COSTO";
                var returnResultDataSet = connectionDataAccessErp.runQuerySqlDataSet(executeQuery);
                foreach (DataRow dr in returnResultDataSet.Tables[0].Rows)
                {
                    ResponsiblePerson newRp = ResponsiblePerson.Create(Convert.ToString(dr["NOMBRE"]), Convert.ToString(dr["RESPONSABLE"]), user, DateTime.Now, company);
                    result.Add(newRp);
                }

                if (connectionDataAccessErp.IsError)
                {
                    Error = true;
                    ErrorDescription = connectionDataAccessErp.ErrorDescription;
                }
            }
            catch (Exception xp)
            {
                Error = true;
                ErrorDescription = xp.Message;
            }
            return result;
        }*/

        public void InsertResponsiblesDb(IList<ResponsiblePerson> list)
        {
            //int count = 300;
            foreach (ResponsiblePerson entity in list)
            {
                //if (count == 0)
                //    break;
                _responsibleManager.Create(entity);
                //count--;
            }
        }


        #region Proyects Functions
        public IList<Project> GetErpProyects(string company, Guid user)
        {
            //Leer todos los Conjuntos
            CleanErrors();
            DataSet returnResultDataSet = new DataSet();
            try
            {
                string executeQuery = "SELECT * FROM UCASCHEMA.V_UCA_CENTROSCOSTO WHERE COMPANIA = '" + company + "'";
                returnResultDataSet = connectionDataAccessErp.runQuerySqlDataSet(executeQuery);
                if (connectionDataAccessErp.IsError)
                {
                    Error = true;
                    ErrorDescription = connectionDataAccessErp.ErrorDescription;
                }
            }
            catch (Exception xp)
            {
                Error = true;
                ErrorDescription = xp.Message;
            }
            return MapProjectToList(returnResultDataSet, user, company);
        }


        public IList<StockMap> GetAllStocksByCompany(string company)
        {
            //Leer todos los Conjuntos
            CleanErrors();
            DataSet returnResultDataSet = new DataSet();
            try
            {
                string executeQuery = "SELECT \"AmountInput\", \"AmountOutput\", \"CellarId\"  FROM \"Stock\" WHERE \"CompanyName\" like '%" + company + "%'";
                returnResultDataSet = connectionDataAccessErp.runQuerySqlDataSet(executeQuery);
                if (connectionDataAccessErp.IsError)
                {
                    Error = true;
                    ErrorDescription = connectionDataAccessErp.ErrorDescription;
                }
            }
            catch (Exception xp)
            {
                Error = true;
                ErrorDescription = xp.Message;
            }
            return MapStockToList(returnResultDataSet, company);
        }

        public IList<StocksMapReport> GetAllStocksByCompanyReport(string company)
        {
            //Leer todos los Conjuntos
            CleanErrors();
            DataSet returnResultDataSet = new DataSet();
            try
            {
                string executeQuery = "SELECT asset.\"Name\", cellar.\"Name\" as \"CellarName\", stock.\"AssetQtyOutputs\", stock.\"AssetQtyInputs\", " +
                                      "stock.\"AssetQtyOutputsBlocked\", stock.\"CellarId\", stock.\"CompanyName\" " +
                                      "FROM \"UCASCHEMA\".\"Stock\" stock " +
                                      "INNER JOIN \"UCASCHEMA\".\"Asset\" asset ON stock.\"AssetId\" = asset.\"Id\" " +
                                      "INNER JOIN \"UCASCHEMA\".\"Cellar\" cellar ON stock.\"CellarId\" = cellar.\"Id\" " +
                                      "WHERE stock.\"CompanyName\" like '%" + company + "%'" + "and asset.\"CompanyName\" like '%" + company + "%'";
                returnResultDataSet = connectionDataAccessErp.runQuerySqlDataSet(executeQuery);
                if (connectionDataAccessErp.IsError)
                {
                    Error = true;
                    ErrorDescription = connectionDataAccessErp.ErrorDescription;
                }
            }
            catch (Exception xp)
            {
                Error = true;
                ErrorDescription = xp.Message;
            }
            return MapStockToListReport(returnResultDataSet, company);
        }


        private IList<StockMap> MapStockToList(DataSet intDataSet, string company)
        {
            List<StockMap> list = new List<StockMap>();
            DataRowCollection dtc = intDataSet.Tables[0].Rows;
            foreach (DataRow dr in dtc)
            {
                double amountInput = Double.Parse(dr["AmountInput"].ToString());
                double amountOutput = Double.Parse(dr["AmountOutput"].ToString());
                byte[] binaryData = dr["CellarId"] as byte[];
                Guid cellarId = new Guid(binaryData);
                //Guid.Parse(dr["CellarId"].ToString());

                StockMap stock = new StockMap();
                stock.AmountInput = amountInput;
                stock.AmountOutput = amountOutput;
                stock.CellarId = cellarId;
                list.Add(stock);
            }
            return list;
        }

        private IList<StocksMapReport> MapStockToListReport(DataSet intDataSet, string company)
        {
            List<StocksMapReport> list = new List<StocksMapReport>();
            DataRowCollection dtc = intDataSet.Tables[0].Rows;
            foreach (DataRow dr in dtc)
            {
                string nameAsset = dr["Name"].ToString();
                string companyName = dr["CompanyName"].ToString();
                string nameCellar = dr["CellarName"].ToString();
                double assetQtyOutputs = Double.Parse(dr["AssetQtyOutputs"].ToString());
                double assetQtyInputs = Double.Parse(dr["AssetQtyInputs"].ToString());
                double assetQtyOutputsBlocked = Double.Parse(dr["AssetQtyOutputsBlocked"].ToString());
                byte[] binaryData = dr["CellarId"] as byte[];
                Guid cellarId = new Guid(binaryData);
                //Guid.Parse(dr["CellarId"].ToString());

                StocksMapReport stock = new StocksMapReport();
                stock.AssetQtyOutputs = assetQtyOutputs;
                stock.AssetQtyInputs = assetQtyInputs;
                stock.AssetQtyOutputsBlocked = assetQtyOutputsBlocked;
                stock.NameAsset = nameAsset;
                stock.NameCellar = nameCellar;
                stock.CellarId = cellarId;
                stock.CompanyName = companyName;
                list.Add(stock);
            }
            return list;
        }


        public void InsertProjectsDb(IList<Project> list)
        {
            int count = 0;
            foreach (Project entity in list)
            {
                if (count <= 15)
                {
                    _projectManager.Create(entity);
                    count++;
                }
            }
        }

        private IList<Project> MapProjectToList(DataSet intDataSet, Guid user, string company)
        {
            List<Project> list = new List<Project>();
            DataRowCollection dtc = intDataSet.Tables[0].Rows;
            foreach (DataRow dr in dtc)
            {
                string name = dr["DESCRIPCION"].ToString();
                string description = dr["DESCRIPCION"].ToString();
                string code = dr["CENTRO_COSTO"].ToString();
                string companyName = company;

                Project entity = Project.Create(name, code, description, null, null, EstadoProyecto.Active, user, "", companyName);
                list.Add(entity);
            }
            return list;
        }
        #endregion

        #region Assets Functions

        public IList<Asset> GetErpAssets(string company, Guid user)
        {
            //Leer todos los Conjuntos
            CleanErrors();
            DataSet returnResultDataSet = new DataSet();
            try
            {
                string executeQuery = "SELECT * FROM UCASCHEMA.V_UCA_ACTIVOS WHERE COMPANIA = '" + company + "'";
                returnResultDataSet = connectionDataAccessErp.runQuerySqlDataSet(executeQuery);
                if (connectionDataAccessErp.IsError)
                {
                    Error = true;
                    ErrorDescription = connectionDataAccessErp.ErrorDescription;
                }
            }
            catch (Exception xp)
            {
                Error = true;
                ErrorDescription = xp.Message;
            }
            var category = _categoryManager.GetCategory("Categoria 7", company);
            Guid? categoryEntityId = null;
            if (category != null)
                categoryEntityId = category.Id;

            return MapAssetToList(returnResultDataSet, user, categoryEntityId, company);
        }

        public void InsertAssetsDb(IList<Asset> listAssets, Guid userId, string company)
        {
            int count = 0;
            Cellar cellar = _cellarManager.GetAllCellars(company).FirstOrDefault();
            foreach (Asset asset in listAssets)
            {
                if (count < 500)
                {
                    _assetManager.Create(asset);
                    if (asset.Price < 1)
                        asset.Price = 1;

                    Stock stock = Stock.Create(cellar.Id, asset.Id, 1, asset.Price, userId, asset.CreationTime, company);
                    _stockManager.Create(stock);
                }
                count++;
            }
        }

        private IList<Asset> MapAssetToList(DataSet intDataSet, Guid user, Guid? category, string company)
        {
            List<Asset> list = new List<Asset>();
            DataRowCollection dtc = intDataSet.Tables[0].Rows;
            foreach (DataRow dr in dtc)
            {
                string name = dr["DESCRIPCION"].ToString();
                string description = dr["DESCRIPCION"].ToString();
                string code = dr["CODIGO"].ToString();
                string barcode = dr["CODIGO_BARRAS"].ToString();
                DateTime adminssionDate = Convert.ToDateTime(dr["FECHA_ADQUISICION"]);
                DateTime? purchaseDate = Convert.ToDateTime(dr["FECHA_ADQUISICION"]);
                double price = Convert.ToDouble(dr["COSTO_ORIG_DOLAR"]);
                double depreciationMonthsQty = Convert.ToDouble(dr["PLAZO_VIDA_UTIL"]);
                AssetType assetType = AssetType.Asset;
                Guid? categoryId = category;
                Guid creatorid = user;
                DateTime createDateTime = Convert.ToDateTime(dr["CREATEDATE"]);
                string brand = "n/d";
                string modelstr = "n/d";
                string plate = "n/d";
                string series = dr["NUMERO_SERIE"].ToString();
                bool isAToolKit = false;
                string companyName = dr["COMPANIA"].ToString();

                Asset asset = Asset.Create(name, description, code, barcode,
                    adminssionDate, purchaseDate, price, depreciationMonthsQty, assetType, categoryId, creatorid,
                    createDateTime, brand, modelstr, plate, series, isAToolKit, companyName);
                list.Add(asset);
            }
            return list;
        }
        #endregion
        public void DisconnectErp()
        {
            connectionDataAccessErp.disconnect();
        }
        public string getdep (Guid user)
        {
            string usuariodep = null;
            //Leer todos los Conjuntos
            CleanErrors();
            try
            {
                usuariodep = connectionDataAccessErp.runSqlReturningstrdep(user);
            }
            catch(Exception ex)
            {
                Error = true;
                ErrorDescription = ex.Message;
            }

            return usuariodep;

        }
    }
}
