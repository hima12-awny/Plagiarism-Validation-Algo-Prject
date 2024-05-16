namespace PlagiarismValidation
{
    public class DisjointSetForest
    {
        private Dictionary<int, int> parent;

        public DisjointSetForest(HashSet<int> vrtx)
        {

            parent = new Dictionary<int, int>();

            // O(V)
            foreach (int v in vrtx)
            {
                parent[v] = v;
            }
        }

        public int find(int v)
        {
            if (parent[v] != v)
            {
                parent[v] = find(parent[v]);
            }
            return parent[v];
        }

        public void union(int v1, int v2)
        {
            int v1Root = find(v1);
            int v2Root = find(v2);
            if (v1Root != v2Root)
            {
                parent[v1Root] = v2Root;
            }
        }

        public void union_with_roots(int v1Root, int v2Root)
        {
            if (v1Root != v2Root)
            {
                parent[v1Root] = v2Root;
            }
        }
    }
}
