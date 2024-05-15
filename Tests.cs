
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OfficeOpenXml;
using System.IO;
using System;
using static OfficeOpenXml.ExcelErrorValue;
using Microsoft.Office.Interop.Excel;

namespace PalgirismValidation
{

    public partial class Tests
    {

        public  void printO(dynamic? obj = null)
        {
            Console.Write(obj?.ToString() ?? "");
        }
        public  void printOl(dynamic? obj = null)
        {
            Console.WriteLine(obj?.ToString() ?? "");
        }

        
        public bool compareStatFileResult(
            List<ComponentBasedTwoItems<Tuple<List<int>, List<Tuple<int, int>>>>> sol_conntComponents,
            string file_path)
        {
            var sol_components = sol_conntComponents;


            int rows = 0;
            bool isFaild = false;

            using (ExcelPackage excelPks = new ExcelPackage(new FileInfo(file_path)))
            {


                ExcelWorksheet ws = excelPks.Workbook.Worksheets[0];
                rows = ws.Dimension.Rows;

                Parallel.For(2, rows + 1, (row, loopState) =>
                {
                    string act_str_vertices = ((string)ws.Cells[row, 2].Value).Replace(" ", "").Replace(",", "");

                    double avgSim = Convert.ToDouble(ws.Cells[row, 3].Value);
                    int compCount = Convert.ToInt32(ws.Cells[row, 4].Value);

                    var sol_component = sol_components[row - 2];


                    bool isSameComponentCount = sol_component.item2 == compCount;
                    bool isSameComponentAvgeSim = sol_component.item1 == avgSim;


                    if (isSameComponentAvgeSim == false)
                    {
                        printOl($"the ac avgSim[{row}]: {avgSim} and your avgSim: {sol_component.item1}");
                        isFaild = true;
                        loopState.Stop();
                    }

                    //if (isSameComponentCount == false)
                    //{
                    //    printOl($"the ac compCount[{row}]: {compCount} and your comCount: {sol_component.item2}");
                    //    isFaild = true;
                    //    loopState.Stop();
                    //}


                    if (!(isSameComponentAvgeSim || isSameComponentCount))
                    {
                        string sol_str_vertices = string.Join("", sol_component.idx.Item1);

                        printOl($"the act_str_vertices[{row}]: {act_str_vertices} and your sol_str_vertices: {sol_str_vertices}");
                        isFaild = true;
                        loopState.Stop();
                    }

                    //string sol_str_vertices = string.Join("", sol_component.idx.Item1);

                    //if (sol_str_vertices != act_str_vertices && !(isSameComponentAvgeSim && isSameComponentCount))
                    //{
                    //    printOl($"the act_str_vertices[{row}]: {act_str_vertices} and your sol_str_vertices: {sol_str_vertices}");
                    //    isFaild = true;
                    //    loopState.Stop();
                    //}


                });

            }

            return !isFaild;
        }


        public bool compareMSTFileResult(
            List<List<SimtyInfo>> sol_sims_2d, string file_path)
        {

            List<SimtyInfo> sol_sims_ = new List<SimtyInfo>();

            foreach (List<SimtyInfo> sublist in sol_sims_2d)
            {
                sol_sims_.AddRange(sublist);
            }


            HashSet<SimtyInfo> sol_sims = sol_sims_.AsParallel().ToHashSet();

            SimtyInfo[] ac_sims_list;

            int rows = 0;

            using (ExcelPackage excelPks = new ExcelPackage(new FileInfo(file_path)))
            {
                ExcelWorksheet ws = excelPks.Workbook.Worksheets[0];
                rows = ws.Dimension.Rows;

                 ac_sims_list = new SimtyInfo[rows-1];

                Parallel.For(2, rows + 1, (row) =>
                {
                    string hprl1 = ws.Cells[row, 1].Value.ToString() ?? "";
                    string hprl2 = ws.Cells[row, 2].Value.ToString() ?? "";

                    int lm = Convert.ToInt32(ws.Cells[row, 3].Value);

                    ac_sims_list[row - 2] = new SimtyInfo(hprl1, hprl2, lm, row - 2);

                });

            }

            bool isFaild = false;

            HashSet<SimtyInfo> ac_sims = ac_sims_list.AsParallel().ToHashSet();

            HashSet<SimtyInfo> in_sols = sol_sims.Except(ac_sims).AsParallel().ToHashSet();
            HashSet<SimtyInfo> in_acs = ac_sims.Except(sol_sims).AsParallel().ToHashSet();


            int founded_counter = 0;
            bool founded = false;
            foreach (SimtyInfo in_sol in in_sols)
            {
                int wieght = in_sol.getMaxPrc;
                int lm = in_sol.line_matches;
                founded = false;

                foreach (SimtyInfo ac_sim in in_acs)
                {
                    if(ac_sim.getMaxPrc == wieght && ac_sim.line_matches == lm)
                    {
                        in_acs.Remove(ac_sim);
                        founded = true;
                        break;
                    }
                }
                if (founded)
                {
                    ++founded_counter;
                }
                else
                {
                    printOl($"sol sim: {in_sol} cant be found");
                }
            }

            if(founded_counter != in_sols.Count)
            {
                printOl();

                printOl("in sol sims not in acs");

                foreach (var item in in_sols)
                {
                    printOl(item);
                }

                printOl();
                printOl("not in sol sims in acs");

                foreach (var item in in_acs)
                {
                    printOl(item);
                }
                printOl();

                printOl($"found {founded_counter} from all sol sims {in_sols.Count} and not enogh");
                isFaild = true;
            }

            return !isFaild;
        }



        public (bool, double) runTestCaseN(
            ref string case_file_path, 
            string level, 
            int case_number)
        {

            string input_case_path = case_file_path + "Input.xlsx";
            string statFile_case_path = case_file_path + "StatFile.xlsx";
            string mstFile_case_path = case_file_path + "mst_file.xlsx";


            SimtyInfo[] simtyInfos;
            double loadDataTime;

            readInputFile(
                case_number, 
                input_case_path, 
                out simtyInfos, 
                out loadDataTime);

            BFSGraph graph;
            bool groups_passed, mst_passed = false;
            double total_groups_time;


            GetGroups(
                level,
                case_number,
                statFile_case_path,
                simtyInfos,
                out graph,
                out groups_passed,
                out total_groups_time);


            // get all components withput cycles for each connected component.
            double total_mst_time = GetMST(
                level,
                case_number,
                mstFile_case_path,
                graph,
                ref mst_passed);


            double total_time = loadDataTime + total_groups_time + total_mst_time;
            return (groups_passed && mst_passed, total_time);
        }

        private void readInputFile(
            int case_number, 
            string input_case_path, 
            out SimtyInfo[] simtyInfos, 
            out double loadDataTime)
        {

            printOl($"\n+ Case: {case_number}");
            printO($"+ Load Data from file... ");

            Stopwatch loadDataStopWatch = new Stopwatch();

            loadDataStopWatch.Start();

            int rows = 0;

            SimtyInfo[] temp_simtyInfos;

            using (ExcelPackage excelPkg = new ExcelPackage(new FileInfo(input_case_path)))
            {

                ExcelWorksheet ws = excelPkg.Workbook.Worksheets[0];

                rows = ws.Dimension.Rows;

                temp_simtyInfos = new SimtyInfo[rows - 1];

                Parallel.For(2, rows + 1, (row) =>
                {

                    var cell_1 = ws.Cells[row, 1];
                    string hprl1 = (string)cell_1.Value;

                    Uri? real_h1 = cell_1?.Hyperlink ?? null;

                    var cell_2 = ws.Cells[row, 2];
                    string hprl2 = (string)cell_2.Value;
                    Uri? real_h2 = cell_2?.Hyperlink ?? null;

                    int lm = Convert.ToInt32(ws.Cells[row, 3].Value);

                    int currIndex = row - 2;

                    temp_simtyInfos[currIndex] = new SimtyInfo(hprl1, hprl2, lm, currIndex, real_h1, real_h2);

                });

            }

            /*using (ExcelPackage excelPks = new ExcelPackage(new FileInfo(input_case_path)))
            {

                ExcelWorksheet ws = excelPks.Workbook.Worksheets[0];
                rows = ws.Dimension.Rows;

                object[,] values = ws.Cells[1, 1, rows, 3].Value as Object[,] ?? new object[0, 0];


                in_simtyInfos = new SimtyInfo[rows - 1];


                Parallel.For(1, rows, (row) =>
                {

                    string hprl1 = (string)values[row, 0];
                    string hprl2 = (string)values[row, 1];

                    int lm = Convert.ToInt32(values[row, 2]);

                    in_simtyInfos[row - 1] = new SimtyInfo(hprl1, hprl2, lm, row - 1);

                });

            }*/


            loadDataStopWatch.Stop();
            loadDataTime = loadDataStopWatch.Elapsed.TotalMilliseconds;
            printOl($"Time({loadDataTime} ms)");

            simtyInfos = temp_simtyInfos;
        }

        void GetGroups(
            string level, 
            int case_number, 
            string statFile_case_path,
            SimtyInfo[] simtyInfos, 
            out BFSGraph graph, 
            out bool groups_passed, 
            out double total_groups_time)
        {

            Stopwatch sol_stat_stopwatch = new Stopwatch();
            sol_stat_stopwatch.Start();

            // Construct the Graph
            graph = new BFSGraph();

            // O(N) * O(1) = O(N)
            foreach (SimtyInfo simInfo in simtyInfos)
            {   
                // O(1)
                graph.addSim(simInfo);
            }

            // get connected components

            List<ComponentBasedTwoItems<Tuple<List<int>, List<Tuple<int, int>>>>> sol_conneted_compnents;
            graph.getConnetedComponents(out sol_conneted_compnents);

            sol_stat_stopwatch.Stop();

            // Get the elapsed time
            double sol_state_time = sol_stat_stopwatch.Elapsed.TotalMilliseconds;

            groups_passed = false;
            total_groups_time = sol_state_time;

            double save_statFile_time = GenSaveFiles.SaveStatFileReults(ref sol_conneted_compnents, level, case_number);
            total_groups_time += save_statFile_time;

            if (compareStatFileResult(sol_conneted_compnents, statFile_case_path))
            {
                printOl($" + Correct Groups Algo in     {sol_state_time} ms");

                printOl($"   + Genearte/Save StateFile  {save_statFile_time} ms");
                printOl($"   + Groups Total (Algo/genSave) time: {Math.Round(total_groups_time, 5)} ms");

                groups_passed = true;
            }
            else
            {
                printOl(" - Wrong Groups");
            }

            printOl();
        }


        private double GetMST(
            string level, 
            int case_number, 
            string mstFile_case_path, 
            BFSGraph graph, 
            ref bool mst_passed)
        {
            Stopwatch sol_mst_stopwatch = new Stopwatch();

            sol_mst_stopwatch.Start();
            List<List<SimtyInfo>> sol_sims = graph.getMST();
            sol_mst_stopwatch.Stop();

            double sol_mst_time = sol_mst_stopwatch.Elapsed.TotalMilliseconds;

            double total_mst_time = sol_mst_time;
            double save_mst_time = GenSaveFiles.SaveMSTFileReults(sol_sims, level, case_number);
            total_mst_time += save_mst_time;

            if (compareMSTFileResult(sol_sims, mstFile_case_path))
            {
                printOl($" + Correct MST Alog in      {sol_mst_time} ms");
                printOl($"   + Genearte/Save MST file {save_mst_time} ms");

                printOl($"   + MST Total (Algo/genSave) time: {Math.Round(total_mst_time, 5)} ms");


                mst_passed = true;
            }
            else
            {
                printOl(" - Wrong MST");
            }

            return total_mst_time;
        }


        public void runSmapleTest()
        {
            string case_file_path = $@"K:\C#\Algo\project\Plagiarism Validation\Test Cases\Sample\";


            printOl("----------------------------------------------");

            printOl("Running Smaple Test:");

            int passed_cases = 0;
            float n_cases = 6;

            for (int i = 1; i <= n_cases; i++)
            {
                string curr_path = case_file_path + $"\\{i}-";

                (bool passed, double time) = runTestCaseN(ref curr_path, "Sample", i);

                if (passed)
                {
                    ++passed_cases;
                    printOl($"++ Test Case({i}/{n_cases}): PASSED in {Math.Round(time, 5)} ms");
                }
                else
                {
                    printOl($"-- Test Case({i}/{n_cases}): FAILED  in {Math.Round(time, 5)} ms");
                }
                printOl("");
            }
            printOl($"Final Evoluation: {(int)((passed_cases / n_cases) * 100)}%");
            printOl("----------------------------------------------");

        }

        public void runTestCasesLevel(int level, float n_cases=2)
        {
            string level_str = "";

            switch (level)
            {
                case 1:
                    level_str = "Easy"; break;

                case 2:
                    level_str = "Medium"; break;

                case 3:
                    level_str = "Hard"; break;

                default:
                    level_str = "";
                    break;
            }

            string case_file_path = $@"K:\C#\Algo\project\Plagiarism Validation\Test Cases\Complete\";

            printOl("----------------------------------------------");

            printOl($"Running Test Case Leve {level_str}:");

            int passed_cases = 0;
            for (int i = 1; i <= n_cases; i++)
            {

                string test_file_path = case_file_path + level_str + "\\" + i.ToString() + "-";

                (bool passed, double time) = runTestCaseN(ref test_file_path, level_str, i);

                if (passed)
                {
                    ++passed_cases;
                    printOl($"++ Test Case({i}/{n_cases}): PASSED in {Math.Round(time, 5)} ms");
                }
                else
                {
                    printOl($"-- Test Case({i}/{n_cases}): FAILED in {Math.Round(time, 5)} ms");
                }
                printOl();
            }
            printOl($"Final Evoluation: {(int)((passed_cases / n_cases) * 100)}%");
            printOl("----------------------------------------------");

        }
    }
}
