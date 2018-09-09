using System;

namespace logic_expert
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("you have to provide an input file");
                return 1;
            }
            Console.WriteLine("reading from " + args[0]);
            var parser = new Parser(args[0]);
            parser.Parse();
            var inferenceEngine = new InferenceEngine(parser._facts, parser._implyRules, parser._ifAndOnlyRules);
            var queries = parser._queries;
            foreach (var query in queries)
            {
                var val = inferenceEngine.Infer(query);
                Console.WriteLine("{0} : {1}", query, val);
            }
            return 0;
        }
    }
}
