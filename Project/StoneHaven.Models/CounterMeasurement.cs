using System;
using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord("CounterMeasurements", DiscriminatorValue = "Counter")]
    public class CounterMeasurement : Measurement
    {
        public override double GetTotalSqFt()
        {
            return Math.Round((Length*Width)/144,2);
        }
    }
}