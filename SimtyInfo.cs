using System.Text.RegularExpressions;

namespace PalgirismValidation
{

    public struct SimtyInfo : IComparable<SimtyInfo>
    {
        private static readonly Regex Regex_Num = new Regex(@"\d+");

        public int v1, v2;

        public int prc1, prc2, line_matches;

        public int line_idx;

        public string hprl1;
        public string hprl2;

        public SimtyInfo(
            string hprl1,
            string hprl2,
            int line_matches,
            int line_idx)
        {
            this.hprl1 = hprl1;
            this.hprl2 = hprl2;

            // O(1)
            (this.v1, this.prc1) = ExtractFidAndPrc(hprl1);
            (this.v2, this.prc2) = ExtractFidAndPrc(hprl2);

            this.line_matches = line_matches;
            this.line_idx = line_idx;
        }

        public int getMaxPrc => (prc1 > prc2 ? prc1 : prc2);

        // small number of letters consederd O(1) 
        public (int fId, int prc) ExtractFidAndPrc(string path)
        {

            path = path.Replace(" ", "");
            var items = path.Split('/');

            /*
             https://example.com/file1/(36%)

            [https:], [example.com], [file1], [(36%)]
    
             */

            int n_items = items.Length;

            string str_f = items[n_items - 2];
            string str_prc = items[n_items - 1];

            int fId = int.Parse(Regex_Num.Match(str_f).Value);
            int prc = int.Parse(Regex_Num.Match(str_prc).Value);

            return (fId, prc);
        }

        public override string ToString()
        {
            return $"SimInfo({v1} ({prc1}), {v2} ({prc2}), lm: {line_matches})";
        }

        public override int GetHashCode()
        {
            return (v1, v2, prc1, prc2, line_matches).GetHashCode();
        }

        public int CompareTo(SimtyInfo other)
        {
            int item1Comparison = other.line_matches.CompareTo(this.line_matches);

            if (item1Comparison != 0)
            {
                return item1Comparison;
            }

            return this.getMaxPrc.CompareTo(other.getMaxPrc);
        }
    }
}
