using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public class MissionContractTask : Task
    {
        public int Value { get; set; }

        public MissionContractTask(int duration, int complexity, int value): base(duration, complexity)
        {
            Value = value;
        }

        public override string ToString()
        {
            return base.ToString() + "-rew: " + Value;
        }

    }
}
