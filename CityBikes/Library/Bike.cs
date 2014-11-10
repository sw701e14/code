using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Bike
    {
        private long id;

        public Bike(long id)
        {
            this.id = id;
        }

        public long Id
        {
            get { return id; }
        }
    }
}
