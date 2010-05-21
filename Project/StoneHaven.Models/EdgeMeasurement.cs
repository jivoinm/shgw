using System;
using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.EdgeMeasurements", DiscriminatorValue = "Edge")]
    public class EdgeMeasurement : Measurement
    {
        public override double GetTotalSqFt()
        {
            return Math.Round(Length/12,2);
        }
    }
}