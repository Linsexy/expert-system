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
            var inferenceEngine = InferenceEngine()
            return 0;
        }
    }
}
