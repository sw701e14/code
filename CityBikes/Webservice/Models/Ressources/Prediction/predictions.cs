using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.Ressources.Prediction
{
    public class predictions
    {
        List<prediction> allpredictions;

        public predictions()
        {
            allpredictions = new List<prediction>();

            allpredictions.AddRange(Data.GetPredictions());

        }
    }
}