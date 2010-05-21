namespace StoneHaven.Models.Services
{
    public interface IThicknesServices
    {
        Thicknes SaveThicknes(Thicknes thicknes);
        ThicknesPrice SaveThicknesPrice(ThicknesPrice price);
    }
}