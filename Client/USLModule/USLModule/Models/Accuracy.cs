namespace USLModule.Models
{
    public class Accuracy
    {
        public int ShotsFired;
        public int ShotsHit;

        public Accuracy(int shotsFired, int shotsHit)
        {
            ShotsFired = shotsFired;
            ShotsHit = shotsHit;
        }
        
    }
}