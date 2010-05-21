using Rhino.Commons;

namespace StoneHaven.Models.Services.DB
{
    public class EdgeServices : IEdgeServices
    {
        public Edge SaveEdge(Edge edge)
        {
            Repository<Edge>.Save(edge);
            return edge;
        }

        public EdgePrice SaveEdgePrice(EdgePrice price)
        {
            Repository<EdgePrice>.Save(price);
            return price;

        }
    }
}
