using System;
using System.Collections.Generic;
using Castle.ActiveRecord;
using Castle.Components.Validator;
using Rhino.Commons;
using StoneHaven.Models.Services;

namespace StoneHaven.Models
{
    [ActiveRecord("dbo.QuotationItems")]
    public class QuoteItem
    {
        [PrimaryKey]
        public virtual int Id { get; set; }

        [BelongsTo("ProjectId"), ValidateNonEmpty]
        public virtual Project Project { get; set; }
        [BelongsTo("QuotationId")]
        public virtual Quotation Quotation { get; set; }
        [BelongsTo("ColorGroupId")]
        public virtual ColorGroup ColorGroup { get; set; }
        [BelongsTo("EdgeId")]
        public virtual Edge Edge { get; set; }
        [BelongsTo("ThicknesId")]
        public virtual Thicknes Thicknes { get; set; }
        [BelongsTo("SinkId")]
        public virtual Sink Sink { get; set; }
        [BelongsTo("StoveId")]
        public virtual Stove Stove { get; set; }

        [Property]
        public virtual int NumberOfSinks { get; set; }
        [Property]
        public virtual int Bumpouts { get; set; }
        [Property]
        public virtual int Arches { get; set; }
        [Property]
        public virtual decimal Adjustment { get; set; }

        private int supplyType = 1;

        [Property]
        public virtual int SupplyType
        {
            get { return supplyType; }
            set { supplyType = value; }
        }

        private IList<CounterMeasurement> counterMeasurements = new List<CounterMeasurement>();

        [HasMany(typeof(CounterMeasurement), Lazy = true, Inverse = false, Where = "Type='Counter'", Cascade = ManyRelationCascadeEnum.Delete)]
        public virtual IList<CounterMeasurement> CounterMeasurements
        {
            get { return counterMeasurements; }
            set { counterMeasurements = value; }
        }

        private IList<BackSplashMeasurement> backSplashMeasurements = new List<BackSplashMeasurement>();

        [HasMany(typeof(BackSplashMeasurement), Lazy = true, Inverse = false, Where = "Type='BackSplash'", Cascade = ManyRelationCascadeEnum.Delete)]
        public virtual IList<BackSplashMeasurement> BackSplashMeasurements
        {
            get { return backSplashMeasurements; }
            set { backSplashMeasurements = value; }
        }

        private IList<EdgeMeasurement> edgeMeasurements = new List<EdgeMeasurement>();

        [HasMany(typeof(EdgeMeasurement), Lazy = true, Inverse = false, Where = "Type='Edge'", Cascade = ManyRelationCascadeEnum.Delete)]
        public virtual IList<EdgeMeasurement> EdgeMeasurements
        {
            get { return edgeMeasurements; }
            set { edgeMeasurements = value; }
        }

        private QuoteItemTotal total;

        
        public QuoteItemTotal Total()
        {
            if(total==null) 
            total = IoC.Resolve<IQuoteServices>().CalculateProjectTotal(this);
             return total;
    } 

        public IDictionary<string,string> ColorGroupPrices(int type)
        {
            ColorGroupTypeEnum colorType = (ColorGroupTypeEnum)type;
            return IoC.Resolve<IQuoteServices>().CalculatePricesForAllColorGroups((this),colorType);
        }
    }

    public class QuoteItemTotal
    {
        public decimal SqFt_ColorgroupTotal;
        public decimal SqFt_BackSplashTotal;
        public decimal SqFt_EdgeTotal;
        public decimal Total_Sink;
        public int Total_NrOfSinks;
        public decimal SinkPrice;
        public int Total_NrOfBumpouts;
        public decimal BumpoutPrice;
        public int Total_NrOfArches;
        public decimal ArchePrice;
        public double TotalSqFt;
        public decimal SqFt_ColorgroupPrice;
        public decimal SqFt_EdgePrice;
        public decimal Total_Colorgroup;
        public decimal Total_Edge;
        public decimal Total_Stove;
        public decimal Total_Bumpouts;
        public decimal Total_Arches;
        public decimal Total_SuplayAndInstall;
        public decimal Total_Adjustment;
        public decimal Total_Tax;
        public decimal Total;
        public decimal Total_Backsplash;
        public decimal ThicknesPrice;
        public decimal Total_Thicknes;
    }
}