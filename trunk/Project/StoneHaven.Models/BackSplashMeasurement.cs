using System;
using Castle.ActiveRecord;

namespace StoneHaven.Models
{
    [ActiveRecord(DiscriminatorValue = "BackSplash")]
    public class BackSplashMeasurement : Measurement
    {
        public override double GetTotalSqFt()
        {
            return Math.Round((Length * Width) / 144, 2);
        }

    }
}