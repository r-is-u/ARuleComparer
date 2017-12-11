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
                var right = splitRule[1].Split(',');
                Rules.Add(new Rule(left, right));
            }

            Rules.Sort();
        }

    }
}
