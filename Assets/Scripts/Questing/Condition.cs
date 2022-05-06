using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condition
{
    [SerializeField] Disjunction[] and;

    public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
    {
        foreach(Disjunction disjunction in and)
        {
            if (!disjunction.Check(evaluators))
            {
                return false;
            }
        }

        return true;
    }

    [System.Serializable]
    class Disjunction
    {
        [SerializeField] Predicate[] or;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach(Predicate predicate in or)
            {
                if(predicate.Check(evaluators))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [System.Serializable]
    class Predicate
    {
        [SerializeField] bool negatePredicate = false;
        [SerializeField] string predicate;
        [SerializeField] string[] parameters;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (var evalulator in evaluators)
            {
                bool? result = evalulator.Evaluate(predicate, parameters);

                if (result == null)
                {
                    continue;
                }

                if (result == negatePredicate)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
