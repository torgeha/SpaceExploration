using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public class ResearchTask : Task
    {
        protected ResearchTask(int durationInMonths, int complexity) : base(durationInMonths, complexity)
        {
        }

        public override string ToString()
        {
            return "Research Task - " + base.ToString();
        }
    }
}
