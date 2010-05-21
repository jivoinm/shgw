using Rhino.Commons;

namespace StoneHaven.Models.Services.DB
{
    public class StoveServices : IStoveServices
    {
        public Stove SaveStove(Stove stove)
        {
            Repository<Stove>.Save(stove);
            return stove;
        }

        public StovePrice SaveStovePrice(StovePrice price)
        {
            Repository<StovePrice>.Save(price);
            UnitOfWork.CurrentSession.Flush();
            return price;

        }
    }
}