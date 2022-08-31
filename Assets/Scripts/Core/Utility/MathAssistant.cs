namespace RPGProject.Core
{
    /// <summary>
    /// Useful math related functions.
    /// </summary>
    public static class MathAssistant
    {
        public static bool IsBetween(float _numberToTest, float _minExclusive, float _maxInclusive)
        {
            return (_numberToTest > _minExclusive && _numberToTest <= _maxInclusive);
        }
    }
}