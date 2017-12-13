using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARuleComparer
{
    public class Rule : IComparable
    {
        public RuleSide Left { get; set; }

        public RuleSide Right { get; set; }

        public Rule(IEnumerable<String> left, IEnumerable<String> right)
        {
            this.Left = new RuleSide(left);
            this.Right = new RuleSide(right);
        }

        /// <summary>
        /// Compare two Rules
        /// First Compare Left sides, only if equal right sides
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Rule r2 = obj as Rule;
            int left = this.Left.CompareTo(r2.Left);

            if (left == 0)
                return this.Right.CompareTo(r2.Right);

            return left;
        }

        public override string ToString()
        {
            return Left.ToString() + "==> " + Right.ToString();
        }
    }

    public class RuleComparer : IEqualityComparer<Rule>
    {

        public bool Equals(Rule x, Rule y)
        {
            //Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y))
                return true;

            //Check whether the products' properties are equal. 
            return x.CompareTo(y) == 0;
        }

        public int GetHashCode(Rule obj)
        {
            //Get hash code for the Name field if it is not null. 
            int leftSide = obj.Left.GetHashCode();

            //Get hash code for the Code field. 
            int rightSide = obj.Right.GetHashCode();

            //Calculate the hash code for the product. 
            return leftSide ^ rightSide;
        }
    }


    public class RuleSide : IComparable
    {
        public String[] Items { get; set; }

        public RuleSide(IEnumerable<String> items)
        {
            var array = items.ToArray();
            Array.Sort(array);
            this.Items = array;

        }

        public bool IsEmpty()
        {
            return Items.Length == 0;
        }

        public int CompareTo(object obj)
        {
            RuleSide r2 = obj as RuleSide;

            if (this.IsEmpty() && r2.IsEmpty())
                return 0;

            return CompareRuleSides(this, r2, 0, 0);

        }

        /// <summary>
        /// Compare 2 rule sides per Item
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="index"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        private int CompareRuleSides(RuleSide s1, RuleSide s2, int index, int last)
        {
            var itemA = s1.Items.Length > index ? s1.Items[index] : null;
            var itemB = s2.Items.Length > index ? s2.Items[index] : null;

            if (itemA == null && itemB == null)
                return last;
            else if (itemA == null)
                return -1;
            else if (itemB == null)
                return 1;

            int compare = itemA.CompareTo(itemB);

            if (compare == 0)
                return CompareRuleSides(s1, s2, index + 1, compare);
            else
                return compare;
        }

        public override string ToString()
        {
            return String.Join(",", Items);
        }

        public override int GetHashCode()
        {
            var hash = 0;
            foreach (string item in this.Items)
            {
                hash = hash ^ item.GetHashCode();
            }
            //Calculate the hash code for the product. 
            return hash;
        }
    }
}
