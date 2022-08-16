namespace RPGProject.Core
{
    public static class MathAssistant
    {
        public static bool IsBetween(float _numberToTest, float _minExclusive, float _maxInclusive)
        {
            return (_numberToTest > _minExclusive && _numberToTest <= _maxInclusive);
        }
    }
}