namespace PlagiarismValidation
{
    public struct Edge
    {
        public int v1, v2, cost;

        public Edge(int v1, int v2, int cost)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.cost = cost;
        }
        public Tuple<int, int> toVs => Tuple.Create(v1, v2);
    }
}
