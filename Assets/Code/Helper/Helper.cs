namespace WORLDGAMEDEVELOPMENT
{
    public static class Helper
    {
        public static bool GetBoolFromFloat(float value)
        {
            bool res = false;
            if (value == 1)
            {
                res = true;
            }
            return res;
        }
    }
}