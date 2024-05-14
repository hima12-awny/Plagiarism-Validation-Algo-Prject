
namespace PalgirismValidation
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

        public void addSim(SimtyInfo sim)
        {

            ComponentBasedThreeItems<SimtyInfo> component =
                new ComponentBasedThreeItems<SimtyInfo>(
                    sim,
                    sim.getMaxPrc,
                    sim.line_matches,
                    sim.line_idx);

            edges[edges_size++] = component;

            vertexs.Add(sim.v1);
            vertexs.Add(sim.v2);
        }


        public List<SimtyInfo> MaximumSpanningTree()
        {
            int n_vertices = vertexs.Count();

            List<SimtyInfo> new_sims = new List<SimtyInfo>();

            DisjointSetForest sets = new DisjointSetForest(vertexs);

            Array.Sort(edges);

            int n_edges = 0;


            foreach (var edge in edges)
            {

                SimtyInfo currSim = edge.idx;

                int v1Root = sets.find(currSim.v1);
                int v2Root = sets.find(currSim.v2);

                if (v1Root != v2Root)
                {
                    sets.union_with_roots(v1Root, v2Root);

                    new_sims.Add(currSim);

                    n_edges++;
                    if (n_edges == (n_vertices - 1)) break;
                }
            }

            return new_sims;
        }


    }
}
