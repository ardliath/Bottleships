namespace Bottleships.Logic
{
    public class ShotResult
    {
        public Shot Shot { get; set; }
        public bool WasAHit { get; set; }

        public ShotResult(Shot shot, bool wasAHit)
        {
            this.Shot = shot;
            this.WasAHit = wasAHit;
        }
    }
}