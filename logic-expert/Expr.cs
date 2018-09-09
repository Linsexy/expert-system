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
            FALSE = 0,
            TRUE = 1,
            UNDEFINED
        }
        
        public abstract class Expr
        {
            public Type type;
        }

        public class Branch : Expr
        {
            public Branch(Expr _lhs, char _op, Expr _rhs)
            {
                type = Type.NODE;
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
                type = Type.LEAF;
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