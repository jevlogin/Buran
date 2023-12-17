using FirebaseWebGL.Scripts.Objects;


namespace WORLDGAMEDEVELOPMENT
{
    public class GameUser : FirebaseUser
    {
        public float TravelDistance;
        public float MoneyCount;
        public float MaxDiscount;

        public override string ToString()
        {
            var str = $"TravelDistance = {TravelDistance}\n" +
                $"MoneyCount = {MoneyCount}\n" +
                $"MaxDiscount = {MaxDiscount}\n" +
                $"Phone = {phoneNumber}";
            return str;
        }
    }
}