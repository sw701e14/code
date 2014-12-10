using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.Ressources.Predictions
{
    public class AllPredictions
    {
        List<Prediction> Predictions;

        public AllPredictions()
        {
            Predictions = new List<Prediction>();

            Predictions.AddRange(Data.GetPredictions());

        }
    }
}