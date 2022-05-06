public interface IPredicateEvaluator
{
    bool? Evaluate(string predicate, string[] parameters);
}
