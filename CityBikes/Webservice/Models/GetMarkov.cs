using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models
{
    public class GetMarkov
    {
        public MarkovChain GetMarkovChain(int column = 0)
        {
            

            

            return BuildMarkov.deserializeMarkovChain(data);
        }
    }
}