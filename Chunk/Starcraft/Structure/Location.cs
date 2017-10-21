using System;

namespace Chunk.Starcraft.Structure
{
    /// <summary>Starcraft Location [MRGN]</summary>
    public struct Location {
        /// <summary>Position 1</summary>
        public Int32 Left, Top;
        /// <summary>Position 2</summary>
        public Int32 Right, Bottom;
        /// <summary>Location Name</summary>
        public Int16 Name;
        /// <summary>Location Flags</summary>
        public Int16 Flags;
    }

    /*
    Flags
        0x01    Low Ground
        0x02    Medium Ground
        0x04    High Ground
        0x08    Low Air
        0x10    Medium Air
        0x20    High Air
    */
}
