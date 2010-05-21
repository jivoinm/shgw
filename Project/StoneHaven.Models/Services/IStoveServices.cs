namespace StoneHaven.Models.Services
{
    public interface IStoveServices
    {
        Stove SaveStove(Stove stove);
        StovePrice SaveStovePrice(StovePrice price);
    }
}