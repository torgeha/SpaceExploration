using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public class ScientistStaff : Staff
    {
        public ScientistStaff(int proficiency, int productivity, int salary) : base(proficiency, productivity, salary)
        {
            
        }

        public override string ToString()
        {
            return "Scientist - " + base.ToString();
        }

    }
}