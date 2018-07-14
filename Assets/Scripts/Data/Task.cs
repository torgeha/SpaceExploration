using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public abstract class Task
    {
        public int DurationInMonths { get; set; } // How long it will take to complete
        public float ProgressInMonths { get; set; } // Progress, starts at 0
        public int Complexity { get; set; } // How hard complex this task is to do

        protected Task(int durationInMonths, int complexity) 
        {
            DurationInMonths = durationInMonths;
            ProgressInMonths = 0;
            Complexity = complexity;
        }

        public bool IsComplete()
        {
            return DurationInMonths - ProgressInMonths <= 0;
        }

        public void AddMonthsToProgress(float months)
        {
            if (ProgressInMonths < DurationInMonths)
                ProgressInMonths += months;
        }

        public float GetProgressPercentage()
        {
            return (ProgressInMonths / (float)DurationInMonths * 100.0f);
        }

        public override string ToString()
        {
            return "Dur: " + DurationInMonths + "-Comp: " + Complexity + "-Prog: " + ProgressInMonths;
        }
    }
}