namespace StoneHaven.Models.Services
{
    public interface IEdgeServices
    {
        Edge SaveEdge(Edge edge);
        EdgePrice SaveEdgePrice(EdgePrice price);
    }
}