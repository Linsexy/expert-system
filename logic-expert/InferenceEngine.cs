using System;
using System.Collections.Generic;
using logic_expert.inference;
using Type = logic_expert.inference.Type;

namespace logic_expert
{
    public class InferenceEngine
    {
        public InferenceEngine(Dictionary<string, Value> baseFacts,
                            Dictionary<Rule, List<Expr>> implyRules,
                            Dictionary<string, Tuple<Rule, Expr>> ifAndOnlyRules)
        {
            _facts = baseFacts;
            _implyRules = implyRules;
            _ifAndOnlyRules = ifAndOnlyRules;
        }
        
        private bool EvaluateOp(bool lhs, char op, bool rhs)
        {
            switch (op)
            {
                case '+':
                    return lhs && rhs;
                case '|':
                    return lhs || rhs;
                case '^':
                    return lhs ^ rhs;
                default:
                    throw new ArgumentException("Unknown operator");
            }
        }
        
        private bool Evaluate(Expr expr)
        {
            if (expr.type == Type.LEAF)
            {
                return (expr as Rule).Val == Infer((expr as Rule).Name);
            }

            var branch = (expr as Branch);
            return EvaluateOp(Evaluate(branch.lhs), branch.op, Evaluate(branch.rhs));
        }
        
        public Value Infer(string rule)
        {
            /* We should add a detection of incoherences */
            var val = Value.UNDEFINED;
            if (_ifAndOnlyRules.ContainsKey(rule))
            {
                var ret = Evaluate(_ifAndOnlyRules[rule].Item2);
                if (!ret)
                    val = Value.FALSE;
                else
                    val = Value.TRUE;
                _facts[rule] = val;
                return val;
            }
            if (_facts.ContainsKey(rule))
                return _facts[rule];
            return Value.UNDEFINED;
        }
        private Dictionary<string, Value> _facts; //rulename - actual value
        private Dictionary<Rule, List<Expr>> _implyRules; //A given Rule is satisfied when any Expr is validated
        private Dictionary<string, Tuple<Rule, Expr>> _ifAndOnlyRules; // A given Rule is satisfied when Expr is validated
    }
}