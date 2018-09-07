using System;
using System.Collections.Generic;
using System.Linq;
using logic_expert.inference;
using Type = logic_expert.inference.Type;

namespace logic_expert
{
    using InvalidExpression = ArgumentException;
    public class Parser
    {   
        private static char[] _operators = {'+', '|', '^'};
        private static string[] _relations = {"<=>", "=>"};
        
        //improvement : Parsing should be done in more steps, where the tree is modified and/or enriched
        public Parser(string fileName)
        {
            _filename = fileName;
            _queries = new List<string>();
            _facts = new Dictionary<string, Value>();
            _ifAndOnlyRules = new Dictionary<Rule, Expr>();
            _implyRules = new Dictionary<Rule, List<Expr>>();
        }

        public void Parse()
        {
            var lines = System.IO.File.ReadAllLines(_filename);
            foreach (var line in lines)
            {
                Console.WriteLine("analyzing " + line);
                var toTokenize = line;
                var posComment = line.IndexOf('#');
                if (posComment != -1)
                    toTokenize = line.Substring(0, posComment);

                if (toTokenize.Length > 0)
                {
                    if (toTokenize[0] == '=')
                        ParseFacts(toTokenize);
                    else if (toTokenize[0] == '?')
                        ParseQueries(toTokenize);
                    else
                        Tokenize(toTokenize);
                }
            }
        }

        private void ParseFacts(String facts)
        {
            for (var i = 1; i < facts.Length; ++i)
            {
                _facts[facts[i].ToString()] = Value.TRUE;
            }
        }

        private void ParseQueries(String queries)
        {
            for (var i = 1; i < queries.Length; ++i)
            {
                if (!_facts.ContainsKey(queries[i].ToString()))
                    throw new InvalidExpression("invalid query : " + queries[i]); //better throwing an other type;
                _queries.Add(queries[i].ToString());
            }
        }
            
        private Expr MakeExpr(List<string> tokens)
        {
            if (tokens.Count == 1)
            {
                var newRule = new Rule(tokens.First());
                _facts.TryAdd(newRule.Name, Value.FALSE);
                return newRule;
            }
            else if (tokens.Count >= 3) // the else could be removed
            {
                //in case of parenthesis this code should be adapted
                if (!_operators.Contains(tokens[1][0]))
                    throw new InvalidExpression("operator " + tokens[1][0] + " doesn't exists");

                var lhs = MakeExpr(tokens.GetRange(0, 1));
                var op = tokens[1][0];
                tokens.RemoveRange(0, 2);
                var rhs = MakeExpr(tokens);
            
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
            var posIAOF = toTokenize.IndexOf(_relations[1]);
            if (posIAOF == -1)
            {
                var posImply = toTokenize.IndexOf(_relations[0]);
                if (posImply == -1)
                {
                    Console.WriteLine("no relation found");
                    return;
                }
                /*            Console.WriteLine("imply found");
            var tokens = toTokenize.Split(new char[]{' ', '\t'}).ToList();
            _implyRules[getRuleName(toTokenize.Substring(posImply))] = 
                makeExpr(toTokenize.Substring(0, posImply - 1));
                */
            }
            Console.WriteLine("if and only if found");
            var rhs = MakeExpr(toTokenize.Substring(posIAOF + 2)
                .Split(new char[]{' ', '\t'}, StringSplitOptions.RemoveEmptyEntries).ToList());
            var lhs = TokenizeRule(toTokenize.Substring(0, posIAOF - 1));

            _ifAndOnlyRules[lhs.First()] = rhs;
        }

        private string _filename;
        public List<string> _queries;
        public Dictionary<string, Value> _facts; //rulename - actual value
        public Dictionary<Rule, List<Expr>> _implyRules; //A given Rule is satisfied when any Expr is validated
        public Dictionary<Rule, Expr> _ifAndOnlyRules; // A given Rule is satisfied when Expr is validated
    }
}