using System.Collections.Generic;
using System.Data.SqlClient;
using AnalaizerClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalizerClassLibrary.Tests
{
    [TestClass]
    public class AdditionalExpressionTests
    {
        // Рядок підключення до локальної бази даних SQL Server
        private const string ConnectionString = @"Server=localhost\SQLEXPRESS;Database=CalcTestsDB;Integrated Security=True;TrustServerCertificate=True;";

        // Отримуємо тестові дані з таблиці AdditionalExpressionTests
        public static IEnumerable<object[]> GetAdditionalTestData()
        {
            var testCases = new List<object[]>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT InputExpression, ExpectedResult, Description FROM dbo.AdditionalExpressionTests ORDER BY Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string input = reader.GetString(0);
                        bool expected = reader.GetBoolean(1);
                        string description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);

                        // Додаємо черговий набір даних у список для сумісності з .NET Framework 4.7.2
                        testCases.Add(new object[] { input, expected, description });
                    }
                }
            }

            return testCases;
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAdditionalTestData), DynamicDataSourceType.Method)]
        public void CheckCurrency_AdditionalRequirements_FromDb(string expression, bool expected, string description)
        {
            // Встановлюємо вхідний вираз та вимикаємо спливаючі вікна під час тестування
            AnalaizerClass.expression = expression;
            AnalaizerClass.ShowMessage = false;

            bool actual = AnalaizerClass.CheckCurrency();

            Assert.AreEqual(expected, actual, $"Test failed for case: {description}");
        }
    }
}