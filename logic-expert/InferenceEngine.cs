using System.Collections.Generic;
using logic_expert.inference;

namespace logic_expert
{
    public class InferenceEngine
    {
        public InferenceEngine(Dictionary<string, Value> baseFacts,
                            Dictionary<Rule, List<Expr>> implyRules,
                            Dictionary<Rule, Expr> ifAndOnlyRules)
        {
            _facts = baseFacts;
            _implyRules = implyRules;
            _ifAndOnlyRules = ifAndOnlyRules;
        }

        public Value Infer(string rule)
        {
            /* We should add a detection of incoherences */
            //_facts[rule] = Evaluate(_ifAndOnlyRules[]);
        }
        private Dictionary<string, Value> _facts; //rulename - actual value
        private Dictionary<Rule, List<Expr>> _implyRules; //A given Rule is satisfied when any Expr is validated
        private Dictionary<Rule, Expr> _ifAndOnlyRules; // A given Rule is satisfied when Expr is validated
    }
}