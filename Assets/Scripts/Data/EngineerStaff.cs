using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public class EngineerStaff : Staff
    {
        public EngineerStaff(int proficiency, int productivity, int salary) : base(proficiency, productivity, salary)
        {
        }

        public override string ToString()
        {
            return "Engineer - " + base.ToString();
        }
    }
}
