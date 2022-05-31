namespace RPGProject.Questing
{
    public interface IPredicateEvaluator
    {
        bool? Evaluate(string _predicate, string[] _parameters);
    }
}