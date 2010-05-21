namespace StoneHaven.Models.Services
{
    public interface IColorGroupServices
    {
        ColorGroup SaveColorGroup(ColorGroup group);
        ColorGroupPrice SaveColorGroupPrice(ColorGroupPrice price);
    }
}