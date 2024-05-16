
namespace PlagiarismValidation
{

    public class MSTGraph
    {

        ComponentBasedThreeItems<SimtyInfo>[] edges;
        HashSet<int> vertexs;
        int edges_size;

        public MSTGraph(int n_edges)
        {
            vertexs = new HashSet<int>();
            edges = new ComponentBasedThreeItems<SimtyInfo>[n_edges];
            edges_size = 0;
        }

        // O(1)
        public void addSim(SimtyInfo sim)
        {
            // O(1)
            ComponentBasedThreeItems<SimtyInfo> component =
                new ComponentBasedThreeItems<SimtyInfo>(
                    sim,
                    sim.getMaxPrc,
                    sim.line_matches,
                    sim.line_idx);

            edges[edges_size++] = component;

            vertexs.Add(sim.v1); // (1)
            vertexs.Add(sim.v2);
        }


        // O(N*log(N))
        public List<SimtyInfo> MaximumSpanningTree()
        {
            // O(1)
            int n_vertices = vertexs.Count();

            List<SimtyInfo> new_sims = new List<SimtyInfo>();

            // Make Set with O(V)
            DisjointSetForest sets = new DisjointSetForest(vertexs);

            // Max => E*log(E)
            Array.Sort(edges);

            int n_edges = 0;

            // for E
            foreach (var edge in edges)
            {
                // O(1)
                SimtyInfo currSim = edge.idx;

                // log(V)
                int v1Root = sets.find(currSim.v1);

                // log(V)
                int v2Root = sets.find(currSim.v2);

                if (v1Root != v2Root)
                {

                    // lee than log(V)
                    sets.union_with_roots(v1Root, v2Root);

                    // insert list O(1)
                    new_sims.Add(currSim);

                    n_edges++;

                    // to get max V-1 edges
                    if (n_edges == (n_vertices - 1)) break;
                }
            }


            // make  set O(V)
            // find  set O(log(V))
            // union set O(log(V))

            // Sorting Edges by E*log(E)

            // sum = E*log(E) + O(logV) + O(V) = E*log(E)
            //     = N*log(N)

            return new_sims;
        }

    }
}
