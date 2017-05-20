using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64.Objects;

namespace SingleLinkage
{
    public static class AtomCompare
    {
        public static bool Compare(Atom baseAtom, Atom testAtom, int correctValueForReturn, int digitsToCompare)
        {
            bool successffull = false;

            int sumSquared = 0;

            if (digitsToCompare <= baseAtom.Composition.Length)//overflow to ensure digits is low enough
            {
                for (int i = 0; i < digitsToCompare; i++)
                {
                    int sum = baseAtom.Composition[i] - testAtom.Composition[i];
                    sumSquared += sum*sum;
                }

                if (sumSquared == correctValueForReturn)
                {
                    successffull = true;
                    Console.WriteLine("We have a match!");

                    PrintComposition(baseAtom);
                    PrintComposition(testAtom);
                    Console.WriteLine(Environment.NewLine);

                    if (baseAtom.Composition[3] > 5)
                    {
                        Console.WriteLine("a big one just went by....");
                        if (baseAtom.Composition[3] > 10)
                        {
                            Console.WriteLine("a White Whale just went by....");
                        }
                    }
                }
            }
            return successffull;
        }

        private static void PrintComposition(Atom atom)
        {
            if (atom.Composition.Length == 4)
            {
                Console.WriteLine("  The composition is " + atom.Composition[0] + "," + atom.Composition[1] + "," + atom.Composition[2] + "," + atom.Composition[3]);
            }

            if (atom.Composition.Length == 5)
            {
                Console.WriteLine("  The composition is " + atom.Composition[0] + "," + atom.Composition[1] + "," + atom.Composition[2] + "," + atom.Composition[3] + "," + atom.Composition[4]);
            }
        }
    }


}
