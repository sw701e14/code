using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library;
using System.Collections.Generic;



namespace LibraryTests
{
    [TestClass]
    public class SerializeMarkovChainTest
    {
        private Database database;
        private MarkovChain markovChain;
        private MarkovChain markovChain2;

        [TestInitialize]
        public void setup()
        {
            database = new Database();
            LibraryTests.DatabaseLoader.EnsureDB();

            markovChain = new MarkovChain(3);
            markovChain[0,0] = 0; markovChain[0,1] = 0.5; markovChain[0,2] = 0.5;
            markovChain[1,0] = 0.3; markovChain[1,1] = 0.3; markovChain[1,2] = 0.4;
            markovChain[2,0] = 0.3; markovChain[2,1] = 0.4; markovChain[2,2] = 0.3;
            markovChain.CreateChain();

            markovChain2 = new MarkovChain(3);
            markovChain2[0, 0] = 0.9; markovChain2[0, 1] = 0.05; markovChain2[0, 2] = 0.05;
            markovChain2[1, 0] = 0.7; markovChain2[1, 1] = 0.2; markovChain2[1, 2] = 0.1;
            markovChain2[2, 0] = 0.2; markovChain2[2, 1] = 0.2; markovChain2[2, 2] = 0.6;
            markovChain2.CreateChain();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            database.Dispose();
            database = null;
        }

        [TestMethod]
        public void StoreAndRetrieveMultipleMarkovChains()
        {
            List<MarkovChain> allMarkovChains = new List<MarkovChain> { markovChain, markovChain2 };

            for (int i = 0; i < allMarkovChains.Count; i++)
            {
                MarkovChain expected = allMarkovChains[i];

                database.RunSession(session => BuildMarkov.InsertMarkovChain(session, expected));

                MarkovChain actual = database.RunSession(session => session.GetMarkovChain(i));

                for (int j = 0; j < expected.size; j++)
                {
                    for (int k = 0; k < expected.size; k++)
                    {
                        Assert.AreEqual(expected[j,k], actual[j,k]);
                    }
                }
            }
        }

    }
}
