using System.Collections.Generic;
using StoneHaven.Models;

namespace StoneHaven.Web.Forms
{
    public class JSONPageResult
    {
        public int page { get; set; }
        public int total { get; set; }
        IList<IRow> _rows = new List<IRow>();

        public IList<IRow> rows
        {
            get { return _rows; }
            set { _rows = value; }
        }
    }
    
    public interface IRow
    {
        string id { get; set; }
        string[] cell { get; set; }
    }

    public class Row : IRow
    {
        public string id { get; set; }
        public string[] cell { get; set; }
    }

    public class MeasurementRow : Row
    {
        public Measurement Measurement { get; set; }
        public int Index { get; set; }
//        private string[] _cell;
//
//        public string id { get; set; }
//
//        public string[] cell
//        {
//            get
//            {
//                //_cell = new string[]
//                //           {
//                //                string.Format("<input type=\"hidden\" id=\"{0}_{2}_Id\" name=\"{0}[{2}].Id\" value=\"{1}\">", id, Measurement.Id,Index),
//                //                string.Format("<input type=\"text\" id=\"{0}_{2}_Length\" name=\"{0}[{2}].Length\" value=\"{1}\">", id, Measurement.Length,Index),
//                //                string.Format("<input type=\"text\" id=\"{0}_{2}_Width\" name=\"{0}[{2}].Width\" value=\"{1}\">", id, Measurement.Width,Index),
//                //                string.Format("{0}", Measurement.GetTotalSqFt())
//                //           };
//
//                return _cell;
//            }
//            set { _cell = value; }
//        }
    }
}