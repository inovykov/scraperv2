namespace Shared.Services
{
    public interface IRandomNumberService
    {
        int GetRandom(int maxValue);
        int GetRandom(int minvalue, int maxValue);
    }
}