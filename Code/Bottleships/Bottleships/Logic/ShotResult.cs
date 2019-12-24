namespace Bottleships.Logic
{
    public class ShotResult
    {
        public Shot Shot { get; set; }
        public bool WasAHit { get; set; }

        public bool WasASink { get; set; }

        public ShotResult(Shot shot, bool wasAHit, bool wasASink)
        {
            this.Shot = shot;
            this.WasAHit = wasAHit;
            this.WasASink = wasASink;
        }
    }
}