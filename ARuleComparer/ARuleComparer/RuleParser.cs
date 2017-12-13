using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARuleComparer
{
    public class RuleParser
    {
        public String RuleFile { get; set; }
        public List<Rule> Rules { get; set; }


        public RuleParser(String file)
        {
            Rules = new List<Rule>();
            this.RuleFile = file;
        }

        public void ParseFile()
        {
            foreach (string line in File.ReadLines(RuleFile))
            {
                var splitRule = line.Split('=');
                var left = splitRule[0].Split(',');
                var temp = splitRule[2].Remove(0, 2);
                var temp2 = temp.Split('#');
                var right = temp2[0].Split(',');
                Rules.Add(new Rule(left, right));
            }

            Rules.Sort();
        }

    }
}
