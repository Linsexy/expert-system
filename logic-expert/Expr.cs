using System;
using System.Linq.Expressions;

namespace logic_expert
{
    namespace inference
    {
        public enum Type
        {
            LEAF,
            NODE,
            DEFAULT
        }

        public enum Value
        {
            UNDEFINED,
            TRUE,
            FALSE
        }
        
        public abstract class Expr
        {
            public Type type;
        }

        public class Branch : Expr
        {
            public Branch(Expr _lhs, char _op, Expr _rhs)
            {
                lhs = _lhs;
                op = _op;
                rhs = _rhs;
            }
            
            public char op;
            public Expr lhs;
            public Expr rhs;
        }
        
        public class Rule : Expr
        {
            public string Name;
            public Value Val;
            
            public Rule(String rule)    
            {
                if (rule[0] == '!')
                {
                    Val = Value.FALSE;
                    Name = rule.Substring(1, rule.Length - 1);
                }
                else
                {
                    Val = Value.TRUE;
                    Name = rule;
                }
            }
        }
    }
}