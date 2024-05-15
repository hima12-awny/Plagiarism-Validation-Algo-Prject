
using System.Collections.Concurrent;

namespace PalgirismValidation
{

    public enum Color
    {
        white,
        gray,
        black
    }

    public class BFSGraph
    {

        Dictionary<int, Color> colors;

        HashSet<int> vertexsHash;
        List<int> vertexsList;

        Dictionary<int, List<int>> adjList;
        List<SimtyInfo> simsInfos;

        Dictionary<
            Tuple<int, int>, 
            SimtyInfo
            > dicSimsInfos;


        public List<ComponentBasedTwoItems<Tuple<List<int>, List<Tuple<int, int>>>>> connectedComponentsAndSims;

        public BFSGraph()
        {
            colors = new Dictionary<int, Color>();
            vertexsHash = new HashSet<int>();
            adjList = new Dictionary<int, List<int>>();
            simsInfos = new List<SimtyInfo>();
            dicSimsInfos = new Dictionary<Tuple<int, int>, SimtyInfo>();

            vertexsList = new List<int>();

            connectedComponentsAndSims = new List<ComponentBasedTwoItems<Tuple<List<int>, List<Tuple<int, int>>>>>();
        }

        public void addSim(SimtyInfo smInfo)
        {

            // O(1)
            simsInfos.Add(smInfo);

            // O(1) with no hash collisions, becouse of the diffrents Edges
            dicSimsInfos[
                Tuple.Create(
                    smInfo.v1 > smInfo.v2? smInfo.v2:smInfo.v1, // get min v
                    smInfo.v1 > smInfo.v2? smInfo.v1:smInfo.v2  // get max v
                    )
                ] = smInfo;


            // O(1)
            if (vertexsHash.Contains(smInfo.v1) == false)
            {
                vertexsList.Add(smInfo.v1);
                vertexsHash.Add(smInfo.v1);
            }

            // O(1)
            if (vertexsHash.Contains(smInfo.v2) == false)
            {
                vertexsList.Add(smInfo.v2);
                vertexsHash.Add(smInfo.v2);
            }

            // O(1)
            if (adjList.ContainsKey(smInfo.v1) == false)
            {
                adjList[smInfo.v1] = new List<int>();
            }

            // O(1)
            if (adjList.ContainsKey(smInfo.v2) == false)
            {
                adjList[smInfo.v2] = new List<int>();
            }


            // O(1)
            adjList[smInfo.v1].Add(smInfo.v2);
            adjList[smInfo.v2].Add(smInfo.v1);
        }

        public void getConnetedComponents(
            out List<
                ComponentBasedTwoItems<
                    Tuple<
                        List<int>, 
                        List<Tuple<int, int>>
                        >
                    >
                > sol_connectedComponentsAndSims
            )
        {

            connectedComponentsAndSims = new List<ComponentBasedTwoItems<Tuple<List<int>, List<Tuple<int, int>>>>>();

            colors = new Dictionary<int, Color>();

            // O(V)
            foreach (var v in vertexsList)
            {
                colors[v] = Color.white;
            }


            // O(V + E)
            // V: the enumber of vertexs
            // E: number of edges (N) most beggest one
            // so Complexty is O(E)

            foreach (var v in vertexsList)
            {
                if (colors[v] == Color.white)
                {
                    BFS(v);
                }
            }

            sol_connectedComponentsAndSims = connectedComponentsAndSims;


            // so The Whole Complexty is O(E) + O(V) = O(E) -> E Is bigger Than V
            // E is the number of Edges = N

            // The Whole Complexty = O(N)
        }

        public void BFS(int source)
        {

            List<int> connectedComponent = new List<int>(); 
            List<Tuple<int, int>> connectedSim   = new List<Tuple<int, int>>();

            double totalSims = 0;

            Queue<int> q = new Queue<int>();
            q.Enqueue(source);

            // O(V + E)
            while (q.Count() != 0)
            {

                // max O(V) to touch every vertex
                int u = q.Dequeue();

                //O(1)
                connectedComponent.Add(u);

                // max O(E)
                foreach (int v in adjList[u])
                {

                    // O(1)
                    if (colors[v] == Color.white)
                    {
                        colors[v] = Color.gray;
                        q.Enqueue(v);
                    }

                    // O(1)
                    if (colors[v] != Color.black)
                    {

                        Tuple<int, int> tempTuple = Tuple.Create(u > v ? v : u,  /* get min */
                                                                 u > v ? u : v   /* get max */ );

                        SimtyInfo tempSim = dicSimsInfos[tempTuple];

                        // O(1)
                        connectedSim.Add(tempTuple);

                        totalSims += (tempSim.prc1 + tempSim.prc2);
                    }


                }
                
                colors[u] = Color.black;
            }

            double avgSim = Math.Round(totalSims / (connectedSim.Count * 2), 1);


            connectedComponentsAndSims.Add(
                new ComponentBasedTwoItems<
                    Tuple<
                        List<int>,              // connected ids
                        List<Tuple<int, int>>> // connetced edges
                        >(
                            Tuple.Create(connectedComponent, connectedSim),
                            avgSim,
                            connectedComponent.Count()
                        )
                );


            // sum = O(V + E) 
            // O(V + E) = O(E) -> (E begger than V)
            // and E is number of Edges so E = N
            // final Complexty is O(N)

        }

        public List<List<SimtyInfo>> getMST()
        {
            int n_components = connectedComponentsAndSims.Count();

            // Use ConcurrentDictionary to store results
            ConcurrentDictionary<int, List<SimtyInfo>> resultsDict = new ConcurrentDictionary<int, List<SimtyInfo>>();


            // Parallelize the loop
            Parallel.For(0, n_components, i =>
            {
                List<Tuple<int, int>> connectedSimsLocal = this.connectedComponentsAndSims[i].idx.Item2;

                MSTGraph mstGraph = new MSTGraph(connectedSimsLocal.Count);

                foreach (var edge in connectedSimsLocal)
                {
                    mstGraph.addSim(dicSimsInfos[edge]);
                }

                List<SimtyInfo> newSimInfos = mstGraph.MaximumSpanningTree();

                // Add newSimInfos to resultsDict based on index i
                resultsDict.TryAdd(i, newSimInfos);
            });

            // Convert resultsDict to a list of SimtyInfo in the correct order
            List<List<SimtyInfo>> results = new List<List<SimtyInfo>>();
            for (int i = 0; i < n_components; i++)
            {
                if (resultsDict.TryGetValue(i, out var simtyInfos))
                {
                    results.Add(simtyInfos);
                }
            }

            return results;
        }

    }
}
