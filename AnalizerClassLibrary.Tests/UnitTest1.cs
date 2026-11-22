using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Collections.Generic;
using AnalaizerClassLibrary;

namespace AnalaizerClassLibrary.Tests
{
    [TestClass]
    public class CheckCurrencyTests
    {
        private const string ConnectionString = @"Server=localhost\SQLEXPRESS;Database=CalcTestsDB;Integrated Security=True;TrustServerCertificate=True;";

        public static IEnumerable<object[]> GetTestDataFromDb()
        {
            var list = new List<object[]>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT InputExpression, ExpectedResult, Description FROM CheckCurrencyTests";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string input = reader.GetString(0);
                        bool expected = reader.GetBoolean(1);
                        string desc = reader.IsDBNull(2) ? "" : reader.GetString(2);

                        list.Add(new object[] { input, expected, desc });
                    }
                }
            }

            return list;
        }

        [DataTestMethod]
        [DynamicData(nameof(GetTestDataFromDb), DynamicDataSourceType.Method)]
        public void CheckCurrency_DbData_ReturnsExpected(string expression, bool expected, string description)
        {
            AnalaizerClass.expression = expression;
            AnalaizerClass.ShowMessage = false;

            bool actual = AnalaizerClass.CheckCurrency();

            Assert.AreEqual(expected, actual, $"Failed on: {description}");
        }
    }
}