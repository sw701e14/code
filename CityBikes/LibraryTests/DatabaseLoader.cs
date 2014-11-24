using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryTests
{
    public static class DatabaseLoader
    {
        private static bool loaded = false;
        private static bool dataLoadCancelled;

        public static void EnsureDB()
        {
            if (!loaded)
            {
                var runTests = MessageBox.Show(
                    "Data will be loaded from the test_data.sql file in order to execute tests.",
                    "Executing database tests", MessageBoxButtons.OKCancel);

                if (runTests == DialogResult.OK)
                {
                    dataLoadCancelled = false;

                    if (!System.IO.File.Exists("test_data.sql"))
                    {
                        dataLoadCancelled = true;
                        Assert.Inconclusive("test_data.sql file not found. Check that your working directory is the TestData directory.");
                        return;
                    }
                    string sqlContent = System.IO.File.ReadAllText("test_data.sql");

                    using (Database database = new Database())
                        database.RunSession(session =>
                        {
                            session.Execute(sqlContent);
                        });
                }
                else if (runTests == DialogResult.Cancel)
                    dataLoadCancelled = true;

                loaded = true;
            }
            
            if (dataLoadCancelled)
                Assert.Inconclusive("Testing database was not loaded. Test could not be executed.");
        }
    }
}
