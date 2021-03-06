﻿namespace Bottleships.Logic
{
    public class ShotResult
    {
        public Shot Shot { get; set; }

        public Clazz Class { get; set; }

        public bool WasAHit { get; set; }

        public bool WasASink { get; set; }

        public bool WasFreshDamage { get; set; }

        public ShotResult(Shot shot, Clazz clazz, bool wasAHit, bool wasASink, bool wasFreshDamage)
        {
            this.Shot = shot;
            this.Class = clazz;
            this.WasAHit = wasAHit;
            this.WasASink = wasASink;
            this.WasFreshDamage = wasFreshDamage;
        }
    }
}