using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.GeneratedDatabaseModel;
using Library.Clustering;

namespace Run
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 0;
            foreach (var item in ClusteringTechniques.DBSCAN(new GPSLocation[]
            { new GPSLocation(1, 1), new GPSLocation(2, 1), new GPSLocation(0, 0), new GPSLocation(10, 0) }, 3, 2)) //new GPSLocation(10,10), new GPSLocation(11,11), new GPSLocation(12,12)
            {
                Console.WriteLine("Cluster: " + count + " ");
                count++;
                Console.WriteLine("Neighborhood:");
                foreach (var n in item)
                {                    
                    foreach (var x in n.Neighborhood)
                    {                        
                        Console.WriteLine(x.Location.ToString());
                    }
                }
            }
            
            Console.ReadKey();
        }
    }
}
