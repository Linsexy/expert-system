using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using logic_expert.expr;
using logic_expert.inference;
using Type = logic_expert.inference.Type;

namespace logic_expert
{
    using InvalidExpression = ArgumentException;
    public class Parser
    {   
        private static char[] _operators = {'+', '|', '^'};
        private static string[] _relations = {"=>", "<=>"};
        
        //improvement : Parsing should be done in more steps, where the tree is modified and/or enriched
        public Parser(string fileName)
        {
            _filename = fileName;
        }

        private void Parse()
        {
            var lines = System.IO.File.ReadAllLines(_filename);

            foreach (var line in lines)
            {
                Console.WriteLine("analyzing " + line);
                var toTokenize = line;
                var posComment = line.IndexOf('#');
                if (posComment != -1)
                    toTokenize = line.Substring(0, posComment);

                //check pt interrog etc... puis envoyer a la bonne fonction
                Tokenize(toTokenize);
            }
        }

        
        private Expr MakeExpr(List<string> tokens)
        {
            if (tokens.Count == 1)
            {
                return new Rule(tokens.First());
            }
            else if (tokens.Count >= 3)
            {
                //in case of parenthesis this code should be adapted
                if (_operators.Contains(tokens[1][0]))
                {
                    var lhs = new Rule(tokens.First());
                    var rhs = MakeExpr(tokens.GetRange(0, 2));
                    var op = tokens[1][0];
            
                    if (rhs.type == Type.NODE
                        && Array.IndexOf(_operators, op) < Array.IndexOf(_operators, (rhs as Branch).op)) 
                        // if rhs.priority < current node.priority
                    {
                        var currentNode = new Branch(lhs, op, (rhs as Branch).lhs);
                        (rhs as Branch).lhs = currentNode;
                        return rhs;
                    }

                    return new Branch(lhs, op, rhs);
                }
                throw new InvalidExpression("operator " + tokens[1][0] + "doesn't exists");
            }
            throw new InvalidExpression("bizarre");
        }
        
        private List<Rule> TokenizeRule(string rules)
        {
            var ret = new List<Rule>();

            var rule = (Rule) MakeExpr(new List<String> {rules.Split(new char[] {' ', '\t'}).ToList()[0]});
            ret.Add(rule); //parse simple rules only for now
            //virer les OR/XOR, contruire une entrée dans la liste pour chaque opérande du &&
            
            return ret;
        }
        
        private void Tokenize(string toTokenize)
        {
            var posImply = toTokenize.IndexOf(_relations[0]);
            if (posImply == -1)
            {
                var posIAOF = toTokenize.IndexOf(_relations[1]);
                if (posIAOF == -1)
                {
                    Console.WriteLine("no relation found");
                    return;
                }
                Console.WriteLine("if and only if found");
                var rhs = MakeExpr(toTokenize.Substring(posIAOF)
                                    .Split(new char[]{' ', '\t'}).ToList());
                var lhs = TokenizeRule(toTokenize.Substring(0, posIAOF - 1));

                _ifAndOnlyRules[lhs.First()] = rhs;
                
                return;
            }
/*            Console.WriteLine("imply found");
            var tokens = toTokenize.Split(new char[]{' ', '\t'}).ToList();
            _implyRules[getRuleName(toTokenize.Substring(posImply))] = 
                makeExpr(toTokenize.Substring(0, posImply - 1));
                */
        }

        private string _filename;
        private Dictionary<string, Value> _facts; //rulename - actual value
        private Dictionary<Rule, List<Expr>> _implyRules; //A given Rule is satisfied when any Expr is validated
        private Dictionary<Rule, Expr> _ifAndOnlyRules; // A given Rule is satisfied when Expr is validated
    }
}