using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Utils
{
    public static class RandomHelper
    {
        public static float RandomBinomial(float max)
        {
            return Random.Range(0, max) - Random.Range(0, max);
        }

        public static float RandomBinomial()
        {
            return Random.Range(0, 1.0f) - Random.Range(0, 1.0f);
        }

        public static int RollD6()
        {
            return (int)System.Math.Round((decimal)Random.Range(0, 6)); ;
        }

        public static int RollD10()
        {
            return (int)System.Math.Round((decimal)Random.Range(0, 10)); ;
        }

        public static int RollD12()
        {
            return (int)System.Math.Round((decimal)Random.Range(0, 12));
        }

        public static int RollD20()
        {
            return (int)System.Math.Round((decimal) Random.Range(0, 20));
        }
    }
}
