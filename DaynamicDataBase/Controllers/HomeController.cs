using DaynamicDataBase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.PortableExecutable;

namespace DaynamicDataBase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration m_config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            m_config = config;
        }


        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                Tabels = ListTables()
            };

            return View(model);
        }


        public List<AutoComplateViewModel> ListTables()
        {
            using SqlConnection _connection = new SqlConnection(GetConnectionString());

            _connection.Open();

            List<AutoComplateViewModel> tables = new List<AutoComplateViewModel>();
            DataTable dt = _connection.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                tables.Add(new AutoComplateViewModel
                {
                    Text = tablename,
                    Value = tablename
                });
            }

            _connection.Close();
            return tables;
        }

        public string GetConnectionString()
        {
            return m_config.GetConnectionString("DefaultConnection") ?? "";

        }

        public IActionResult GetColumnsByTabelName(string tabelName)
        {
            try
            {
                var model = new TabelViewModel();


                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    var command = new SqlCommand($"select * from {tabelName}", conn);
                    conn.Open();

                    SqlDataAdapter dscmd = new SqlDataAdapter(command);

                    DataSet ds_1 = new DataSet();
                    dscmd.Fill(ds_1);
                    var columns = new List<string>();
                    var rows = new List<RowItems>();



                    foreach (var item in ds_1.Tables[0].Columns)
                    {
                        columns.Add(item.ToString());
                    }

                    model.Columns = columns;

                    if (ds_1.Tables[0].Rows.Count > 0) //loop through all rows of dataset
                    {
                        var rowCount = 0;

                        foreach (DataRow row in ds_1.Tables[0].Rows)
                        {

                            foreach (var item in row.ItemArray)
                            {
                                rows.Add(new RowItems
                                {
                                    RowId = rowCount,
                                    Value = item.ToString()
                                });
                            }

                            rowCount++;
                        }

                        model.RowItems = rows;

                        model.Rows = rows.GroupBy(c => c.RowId).Select(c => c.Key).ToList();

                    }

                }

                return PartialView("_Tabel", model);
            }
            catch (Exception)
            {
                return PartialView("_Tabel", new TabelViewModel());

            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}