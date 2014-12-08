using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shared.DAL;

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



                    using (Database database = new Database())
                        database.RunSession(session =>
                        {
                            dataLoadCancelled = !session.TestDatabase();

                            if (dataLoadCancelled == true)
                                Assert.Inconclusive("test_data.sql file not found. Check that test_data.sql is in LibraryTest/bin/debug");
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
