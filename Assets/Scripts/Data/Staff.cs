using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public abstract class Staff
    {

        public int Proficiency { get; set; }
        public float Productivity { get; set; }
        public int Salary { get; set; }

        public Staff(int proficiency, float productivity, int salary)
        {
            Proficiency = proficiency;
            Productivity = productivity;
            Salary = salary;
        }

        public override string ToString()
        {
            return " - Prof: " + Proficiency + " - Prod: " + Productivity.ToString("0.0") + " - Salary: " + Salary;
        }

    }
}
