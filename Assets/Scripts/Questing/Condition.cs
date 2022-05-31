using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Questing
{
    [Serializable]
    public class Condition
    {
        [SerializeField] Disjunction[] and;

        public bool Check(IEnumerable<IPredicateEvaluator> _evaluators)
        {
            foreach (Disjunction disjunction in and)
            {
                if (!disjunction.Check(_evaluators))
                {
                    return false;
                }
            }

            return true;
        }

        [Serializable]
        class Disjunction
        {
            [SerializeField] Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> _evaluators)
            {
                foreach (Predicate predicate in or)
                {
                    if (predicate.Check(_evaluators))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Serializable]
        class Predicate
        {
            [SerializeField] bool negatePredicate = false;
            [SerializeField] string predicate;
            [SerializeField] string[] parameters;

            public bool Check(IEnumerable<IPredicateEvaluator> _evaluators)
            {
                foreach (var evalulator in _evaluators)
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
}
