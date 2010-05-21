using Rhino.Commons;

namespace StoneHaven.Models.Services.DB
{
    public class ThicknesServices : IThicknesServices
    {
        public Thicknes SaveThicknes(Thicknes thicknes)
        {
            Repository<Thicknes>.Save(thicknes);
            return thicknes;
        }

        public ThicknesPrice SaveThicknesPrice(ThicknesPrice price)
        {
            Repository<ThicknesPrice>.Save(price);
            return price;

        }
    }
}