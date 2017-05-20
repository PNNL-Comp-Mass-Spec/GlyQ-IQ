
namespace HPC_Connector
{
   
    public enum PriorityLevel
    {
        Lowest = 0,
        BelowNormal = 1,
        Normal = 2,
        AboveNormal = 3,
        Highest = 4,
    }

    public enum HardwareUnitType
    {
        Core = 0,
        Socket = 1,
        Node = 2,
    }

    public enum HPCTaskType
    {
        Basic = 0,
        ParametricSweep = 1,
        NodePrep = 2,
        NodeRelease = 3,
        Service = 4,
    }
}
