using System.Diagnostics;
using OfficeOpenXml;

namespace PlagiarismValidation
{
    public partial class Tests
    {
        public static Dictionary<string, string> out_path = new Dictionary<string, string> {

            { "Sample", @"K:\C#\Algo\Minimal Spaning tree\train4.1\results_files\Sample\"},
            { "Easy", @"K:\C#\Algo\Minimal Spaning tree\train4.1\results_files\Complete\Easy\"},
            { "Medium", @"K:\C#\Algo\Minimal Spaning tree\train4.1\results_files\Complete\Medium\"},
            { "Hard", @"K:\C#\Algo\Minimal Spaning tree\train4.1\results_files\Complete\Hard\"}

        };
        public class GenSaveFiles
        {

            // O(G*log(G)) + O(G*(M*log(M)))
            public static double SaveStatFileReults(
                ref List<ComponentBasedTwoItems<Tuple<List<int>, List<Tuple<int, int>>>>> sol_conntComponents_,
                string level,
                int case_number
                )
            {
                Stopwatch saveFileStopwatch = new Stopwatch();
                saveFileStopwatch.Start();

                var sol_conntComponents = sol_conntComponents_;

                using (ExcelPackage excelPkg = new ExcelPackage())
                {
                    var worksheet = excelPkg.Workbook.Worksheets.Add("Sheet1");

                    // O(1)
                    worksheet.Cells[1, 1].Value = "Component Index";
                    worksheet.Cells[1, 2].Value = "Vertices";
                    worksheet.Cells[1, 3].Value = "Average Similarity";
                    worksheet.Cells[1, 4].Value = "Component Count";

                    int con_len = sol_conntComponents.Count; // number of groups

                    // for each group G do this for
                    // this for is = O(M*logM)
                    // this for is = O(MlogM)

                    // NOTE: Not all M have the same size
                    // so the more accurate is better
                    // the summation for M*logM for each Group
                    Parallel.For(0, con_len, (i) => {

                        var component = sol_conntComponents[i]; // for each group
                        int currRow = i + 2;

                        // M number of vertexes in this group
                        // so this cause complexity
                        // (M*logM)
                        component.idx.Item1.Sort();
                        string vertices_str = string.Join(", ", component.idx.Item1);
                        if (vertices_str == "0") return; // O(1)

                        worksheet.Cells[currRow, 1].Value = i + 1;           // Component Index O(1)
                        worksheet.Cells[currRow, 2].Value = vertices_str;    // Vertices O(1)
                        worksheet.Cells[currRow, 3].Value = component.item1; // Average Similarity O(1)
                        worksheet.Cells[currRow, 4].Value = component.item2; // Component Count O(1)
                    });
                    string output_result_statFile = out_path[level] + $"{case_number}-StatFile.xlsx";

                    // Step 4: open and Save the Excel package to a file
                    FileInfo excelFile = new FileInfo(output_result_statFile);
                    excelPkg.SaveAs(excelFile);
                    saveFileStopwatch.Stop();

                }
                return saveFileStopwatch.Elapsed.TotalMilliseconds;
            }

            // = O(N*log(N))
            public static double SaveMSTFileReults(
                in List<List<SimtyInfo>> sol_sims,
                string level,
                int case_number)
            {

                Stopwatch saveFileStopwatch = new Stopwatch();

                using (var excelPkg = new ExcelPackage())
                {
                    // O(1)
                    var worksheet = excelPkg.Workbook.Worksheets.Add("Sheet1");

                    saveFileStopwatch.Start();

                    // O(1)
                    worksheet.Cells[1, 1].Value = "File 1";
                    worksheet.Cells[1, 2].Value = "File 2";
                    worksheet.Cells[1, 3].Value = "Line Matches";

                    int rowNdx = 2;

                    foreach (var simsList in sol_sims)
                    {

                        simsList.Sort(); // O(E*log(E))

                        foreach (var sim in simsList)
                        {
                            // to handle if the URL is NULL O(1)
                            // and make it clickable hyperlink
                            worksheet.Cells[rowNdx, 1].SetHyperlink(sim.real_h1 ?? new UriBuilder(sim.hprl1).Uri);
                            worksheet.Cells[rowNdx, 2].SetHyperlink(sim.real_h2 ?? new UriBuilder(sim.hprl2).Uri);

                            worksheet.Cells[rowNdx, 1].Style.Font.UnderLine = true;
                            worksheet.Cells[rowNdx, 2].Style.Font.UnderLine = true;
                            worksheet.Cells[rowNdx, 2].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                            worksheet.Cells[rowNdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

                            // put the value of the cell (file id and percentage and line matches) O(1)
                            worksheet.Cells[rowNdx, 1].Value = sim.hprl1;
                            worksheet.Cells[rowNdx, 2].Value = sim.hprl2;
                            worksheet.Cells[rowNdx, 3].Value = sim.line_matches;
                            rowNdx++;
                        }
                    }
       
                    string output_result_statFile = out_path[level] + $"{case_number}-mst_file.xlsx";

                    // take more time with no benefit
                    //worksheet.Cells.AutoFitColumns();

                    // Step 4: Save the Excel package to a file
                    FileInfo excelFile = new FileInfo(output_result_statFile);
                    excelPkg.SaveAs(excelFile);
                    saveFileStopwatch.Stop();
                }

                // final Complexity = O(E*log(E))
                // = O(N*log(N))
                return saveFileStopwatch.Elapsed.TotalMilliseconds;

            }
        }
    }
}
