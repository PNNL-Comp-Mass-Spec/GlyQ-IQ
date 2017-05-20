using System.Collections.Generic;

namespace DivideTargetsLibraryX64.Objects
{
    public class Atom
    {
        public int[] Composition { get; set; }

        public bool IsLinked { get; set; }

        public Atom(int var0, int var1, int var2, int var3)
        {
            Composition = CreateComposition(4);
            Composition[0] = var0;
            Composition[1] = var1;
            Composition[2] = var2;
            Composition[3] = var3;
            
            IsLinked = false;

        }
        
        public Atom(int var0, int var1, int var2, int var3, int var4)
        {
            Composition = CreateComposition(5);
            Composition[0] = var0;
            Composition[1] = var1;
            Composition[2] = var2;
            Composition[3] = var3;
            Composition[4] = var4;

            IsLinked = false;
        }

        public Atom(List<int> int5Long)
        {
            switch(int5Long.Count)
            {
                case 4:
                    {
                        Composition = CreateComposition(4);
                        Composition[0] = int5Long[0];
                        Composition[1] = int5Long[1];
                        Composition[2] = int5Long[2];
                        Composition[3] = int5Long[3];
                    }
                    break;
                case 5:
                    {
                        Composition = CreateComposition(5);
                        Composition[0] = int5Long[0];
                        Composition[1] = int5Long[1];
                        Composition[2] = int5Long[2];
                        Composition[3] = int5Long[3];
                        Composition[4] = int5Long[4];
                    }
                    break;
                default:
                    {
                        Composition = CreateComposition(4);
                    }
                    break;
            }
            
            IsLinked = false;
        }

        

        private static int[] CreateComposition(int length)
        {
            int[] composition = new int[length];
            return composition;
        }
    }
}
