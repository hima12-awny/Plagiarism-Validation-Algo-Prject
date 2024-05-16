using System.Collections;

namespace PlagiarismValidation
{

    public class ComponentBasedTwoItems<TIdx> : IComparable<ComponentBasedTwoItems<TIdx>>
    {
        public TIdx idx;

        public double item1;
        public int item2;

        public ComponentBasedTwoItems(TIdx idx, double item1, int item2)
        {
            this.idx = idx;
            this.item1 = item1;
            this.item2 = item2;
        }

        public override string ToString()
        {

            string idxString;
            if (idx is IEnumerable enumerable)
            {
                idxString = "[" + string.Join(", ", enumerable.Cast<object>().Take(10)) + "]";
            }
            else
            {
                idxString = idx.ToString();
            }


            return $"{idxString}, item1: {item1}, item2: {item2}";
        }

        public int CompareTo(ComponentBasedTwoItems<TIdx>? other)
        {
            int item1Comparison = other?.item1.CompareTo(this.item1) ?? 0;
            if (item1Comparison != 0)
            {
                return item1Comparison;
            }

            return this.item2.CompareTo(other?.item2);
        }
    }

    public class ComponentBasedThreeItems<TIdx> : IComparable<ComponentBasedThreeItems<TIdx>>
    {
        public TIdx idx;
        public double item1;
        public int item2;
        public int item3;

        public ComponentBasedThreeItems(TIdx idx, double item1, int item2, int item3)
        {
            this.idx = idx;
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
        }

        public override string ToString()
        {
            string idxString;
            if (idx is IEnumerable enumerable)
            {
                idxString = "[" + string.Join(", ", enumerable.Cast<object>().Take(10)) + "]";
            }
            else
            {
                idxString = idx?.ToString() ?? "";
            }

            return $"{idxString}, item1: {item1}, item2: {item2}, item3: {item3}";
        }

        public int CompareTo(ComponentBasedThreeItems<TIdx>? other)
        {
            // Compare by item1 first
            int item1Comparison = other?.item1.CompareTo(this.item1) ?? 0;
            if (item1Comparison != 0)
            {
                return item1Comparison;
            }

            // If item1 values are equal, compare by item2
            int item2Comparison = other?.item2.CompareTo(this.item2) ?? 0;
            if (item2Comparison != 0)
            {
                return item2Comparison;
            }

            // If item2 values are also equal, compare by item3
            return this.item3.CompareTo(other?.item3);
        }
    }


    public class SortedComponent<TIdx> : List<ComponentBasedTwoItems<TIdx>>
    {
        // Override the Add method to maintain sorting
        public new void Add(ComponentBasedTwoItems<TIdx> newComponent)
        {
            int index = BinarySearch(newComponent, Comparer<ComponentBasedTwoItems<TIdx>>.Default);
            if (index < 0)
                index = ~index;
            Insert(index, newComponent);
        }
    }

    public class SortedComponentThreeItems<TIdx> : List<ComponentBasedThreeItems<TIdx>>
    {
        // Override the Add method to maintain sorting
        public new void Add(ComponentBasedThreeItems<TIdx> newComponent)
        {
            int index = BinarySearch(newComponent, Comparer<ComponentBasedThreeItems<TIdx>>.Default);
            if (index < 0)
                index = ~index;
            Insert(index, newComponent);
        }
    }
}
