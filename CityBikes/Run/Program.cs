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
            //For testing the bikesnearby method..
            /*
            foreach (var item in BikesNearby.GetBikesNearby(new GPSLocation(2,-2)))
            {
                Console.WriteLine("bikeID: " + item.Item1 + " Distance to given point: " + item.Item2.Latitude + ", " + item.Item2.Longitude);
            }
            Console.ReadKey();
             */
            //testBikeStandstill();

            

            Database context = new Database();
            List<GPSLocation> data = new List<GPSLocation>();
            
            foreach (var item in from location in context.gps_data select location)
            {
                data.Add(new GPSLocation(item.latitude, item.longitude));
            }

            var dist = GPSLocation.Distance(data[0], data[1]);

            int count = 0;
            GPSLocation[] sub = new GPSLocation[500];
            for (int i = 0; i < 500; i++)
            {
                sub[i] = data[i];
            }
            foreach (var item in ClusteringTechniques.DBSCAN(sub, 4, 200)) //new GPSLocation[] {new GPSLocation(10, 10), new GPSLocation(11, 11), new GPSLocation(12, 12)}
            {
                Console.WriteLine("Cluster: " + count + " ");
                count++;
                /*Console.WriteLine("Neighborhood:");
                foreach (var n in item)
                {                    
                    foreach (var x in n.Neighborhood)
                    {                        
                        Console.WriteLine(x.Location.ToString());
                    }
                }*/
            }
            
            Console.ReadKey();
        }
    }
}
