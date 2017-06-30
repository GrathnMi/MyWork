using System;
using System.Collections;
using System.Collections.Generic;

namespace ShipData
{
    public class BuffDebuff
    {
        public Parts.partTypes AffectedStat { get; private set; }
        public float AddAmount { get; private set; }
        public float Multiplyer { get; private set; }
        public float Duration { get; set; }
        public string Name { get; set; }

        public BuffDebuff(Parts.partTypes affectedStat, float addAmount, float multiplyer, float durationInSeconds, string name = "no name")
        {
            AffectedStat = affectedStat;
            AddAmount = addAmount;
            Multiplyer = multiplyer;
            Duration = durationInSeconds;
            Name = name;
        }

    }
}
