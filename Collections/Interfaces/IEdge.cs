namespace Nikos.Collections.Graphs
{
    public interface IEdge
    {
        IVertex V_1 { get; set; }
        IVertex V_2 { get; set; }
        decimal Cost { get; set; }
        bool Oriented { get; }
    }
}