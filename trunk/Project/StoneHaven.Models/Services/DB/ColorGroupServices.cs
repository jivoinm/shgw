using Rhino.Commons;

namespace StoneHaven.Models.Services.DB
{
    public class ColorGroupServices : IColorGroupServices
    {
        public ColorGroup SaveColorGroup(ColorGroup group)
        {
            Repository<ColorGroup>.Save(group);
            return group;
        }

        public ColorGroupPrice SaveColorGroupPrice(ColorGroupPrice price)
        {
            Repository<ColorGroupPrice>.Save(price);
            return price;
        }
    }

}