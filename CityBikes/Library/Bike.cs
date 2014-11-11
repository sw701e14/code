using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Bike
    {
        private uint id;

        public Bike(uint id)
        {
            this.id = id;
        }

        public uint Id
        {
            get { return id; }
        }

        public override int GetHashCode()
        {
            return (int)id;
        }
    }
}
