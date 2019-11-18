namespace Bottleships.Logic
{
    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Coordinates;
            return other != null
                && other.X == this.X
                && other.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return 27 * this.X.GetHashCode() * this.Y.GetHashCode();
        }
    }
}