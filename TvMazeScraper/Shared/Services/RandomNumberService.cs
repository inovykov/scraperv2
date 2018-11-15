using System;

namespace Shared.Services
{
    public class RandomNumberService : IRandomNumberService
    {
        private readonly Random _random;

        public RandomNumberService()
        {
            _random = new Random();
        }

        public int GetRandom(int maxValue)
        {
            return GetRandom(0, maxValue);
        }

        public int GetRandom(int minvalue, int maxValue)
        {
            return _random.Next(minvalue, maxValue);
        }
    }
}
