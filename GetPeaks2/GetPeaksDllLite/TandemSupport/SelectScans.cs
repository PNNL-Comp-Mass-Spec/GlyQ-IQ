using System.Collections.Generic;

namespace GetPeaksDllLite.TandemSupport
{
    public static class SelectScans
    {
        public static List<int> Ms1PrecursorScansWithTandem(List<int> scanLevels)
        {
            List<int> ms1ScansWithTandem = new List<int>();
            for (int i = 0; i < scanLevels.Count - 1; i++)
            {
                switch (scanLevels[i])
                {
                    case 1:
                        {
                            //if (scanLevels[i + 1] > 1)//ms with a msms
                            //{
                            //    ms1ScansWithTandem.Add(i);
                            //}
                            //TODO we need to add ms1 scans that are not framgneted as well
                            ms1ScansWithTandem.Add(i);
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }

            }
            return ms1ScansWithTandem;
        }
    }
}
