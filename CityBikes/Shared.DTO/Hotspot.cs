﻿using Shared.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    /// <summary>
    /// Represents a hotspot in the system and provides methods for saving/loading hotspots to/from the database.
    /// </summary>
    public class Hotspot
    {
        private static Dictionary<uint, Hotspot> hotspots;
        static Hotspot()
        {
            hotspots = new Dictionary<uint, Hotspot>();
        }

        /// <summary>
        /// Creates a new hotspot in the database.
        /// The convex hull that makes up the actual hotspot is automatically applied to <paramref name="dataPoints"/>.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> that should be used to insert the hotspot.</param>
        /// <param name="dataPoints">The data points that make up the hotspot. Note that the convex hull is automatically found by this method.</param>
        /// <returns>A new <see cref="Hotspot"/> constructed from <see cref="dataPoints"/></returns>
        public static Hotspot CreateHotspot(DatabaseSession session, GPSLocation[] dataPoints)
        {
            dataPoints = GPSLocation.GetConvexHull(dataPoints);

            if (dataPoints.Length == 0)
                return null;

            uint id = session.InsertHotSpot(dataPoints.Select(x => Tuple.Create(x.Latitude, x.Longitude)).ToArray());
            Hotspot hs = new Hotspot(id, dataPoints);

            hotspots.Add(id, hs);

            return hs;
        }
        /// <summary>
        /// Loads the hotspot in the database that is identified by <paramref name="id"/>.
        /// This uses caching such that a single hotspot is only loaded from the database once.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> that should be used to insert the hotspot.</param>
        /// <param name="id">The hotspot identifier.</param>
        /// <returns>An instance of <see cref="Hotspot"/> representing the data stored in the database for <paramref name="id"/>.</returns>
        public static Hotspot LoadHotspot(DatabaseSession session, uint id)
        {
            if (hotspots.ContainsKey(id))
                return hotspots[id];
            else
            {
                var data = session.GetHotspot(id);
                if (data == null)
                    throw new ArgumentOutOfRangeException("id", "No hotspot with id " + id + " exists in the database.");
                else
                {
                    Hotspot hs = new Hotspot(id, data.Select(x => new GPSLocation(x.Item1, x.Item2)).ToArray());
                    hotspots.Add(id, hs);
                    return hs;
                }
            }
        }
        /// <summary>
        /// Loads all hotspots in the database.
        /// This updates the caching provided by the <see cref="Hotspot"/> class but will always query the database for any data.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> that should be used to insert the hotspot.</param>
        /// <returns>An array of all <see cref="Hotspot"/>s currently in the database.</returns>
        public static Hotspot[] LoadAllHotspots(DatabaseSession session)
        {
            List<uint> known = hotspots.Keys.ToList();
            var alldata = session.GetAllHotspots();
            
            for (int i = 0; i < alldata.Length; i++)
            {
                uint id = alldata[i].Item1;
                if (hotspots.ContainsKey(id))
                {
                    known.Remove(id);
                    continue;
                }

                Hotspot hs = new Hotspot(id, alldata[i].Item2.Select(x => new GPSLocation(x.Item1, x.Item2)).ToArray());
                hotspots.Add(id, hs);
            }

            foreach (var k in known)
                hotspots.Remove(k);

            return hotspots.Values.ToArray();
        }

        private readonly GPSLocation[] dataPoints;
        internal readonly uint id;

        private Hotspot(uint id, GPSLocation[] dataPoints)
        {
            if (dataPoints == null)
                throw new ArgumentNullException("dataPoints");

            if (dataPoints.Length == 0)
                throw new ArgumentException("A hotspot must have at least one point.", "dataPoints");

            this.id = id;

            this.dataPoints = new GPSLocation[dataPoints.Length];
            dataPoints.CopyTo(this.dataPoints, 0);
        }

        public bool Contains(GPSLocation location)
        {
            //inspired by http://stackoverflow.com/a/14998816
            bool result = false;
            int j = dataPoints.Length - 1;
            for (int i = 0; i < dataPoints.Length; i++)
            {
                if (dataPoints[i].Latitude < location.Latitude && dataPoints[j].Latitude >= location.Latitude || dataPoints[j].Latitude < location.Latitude && dataPoints[i].Latitude >= location.Latitude)
                {
                    if (dataPoints[i].Longitude + (location.Latitude - dataPoints[i].Latitude) / (dataPoints[j].Latitude - dataPoints[i].Latitude) * (dataPoints[j].Longitude - dataPoints[i].Longitude) < location.Longitude)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
        public bool Contains(GPSData data)
        {
            return Contains(data.Location);
        }

        public double DistanceTo(GPSLocation location)
        {
            double dist = Math.Min(distanceToLine(dataPoints[0], dataPoints[dataPoints.Length - 1], location), dataPoints[0].DistanceTo(location));

            for (int i = 1; i < dataPoints.Length; i++)
            {
                var d = dataPoints[i].DistanceTo(location);
                if (d < dist) dist = d;
                d = distanceToLine(dataPoints[i], dataPoints[i - 1], location);
                if (d < dist) dist = d;
            }

            return dist;
        }
        public double DistanceTo(GPSData data)
        {
            return DistanceTo(data.Location);
        }

        private static double distanceToLine(GPSLocation lineA, GPSLocation lineB, GPSLocation point)
        {
            var y = lineB.Latitude - lineA.Latitude;
            var x = lineB.Longitude - lineA.Longitude;
            var val = Math.Abs(y * point.Longitude - x * point.Latitude + lineB.Longitude * lineA.Latitude - lineB.Latitude * lineA.Longitude);
            return (double)val / Math.Sqrt((double)(y * y + x * x));
        }

        public GPSLocation[] getDataPoints()
        {
            return dataPoints;
        }

        public uint GetId()
        {
            return id;
        }
    }
}
