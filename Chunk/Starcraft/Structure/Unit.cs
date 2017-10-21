using System;

namespace Chunk.Starcraft.Structure
{
    /// <summary>Starcraft Unit [UNIT]</summary>
    public struct Unit {
        /// <summary>Class Instance</summary>
        public UInt32 Class;
        /// <summary>Unit Position</summary>
        public UInt16 X, Y;
        /// <summary>Unit ID</summary>
        public UInt16 ID;
        /// <summary>Unit Properties</summary>
        public UnitProperties Properties;
        /// <summary>Next Unit</summary>
        public UInt32 Next;
    }

    /// <summary>Starcraft Unit Properties [UPRP]</summary>
    public struct UnitProperties {
        /// <summary>Link Type</summary>
        public UInt16 Link;
        /// <summary>Unit State Valid</summary>
        public UInt16 Valid;
        /// <summary>Unit Data Used</summary>
        public UInt16 Used;
        /// <summary>Player</summary>
        public Byte Player;
        /// <summary>Unit Points</summary>
        public Byte HP, SP, EP;
        /// <summary>Resource Amount</summary>
        public UInt32 Resource;
        /// <summary>Hangar Count</summary>
        public UInt16 Hangar;
        /// <summary>Unit State</summary>
        public UInt16 State;
        /// <summary>Empty</summary>
        public UInt32 Unused;
    }


    /*
    LinkType
        0x200   Nydus
        0x400   Addon

    Flags
        0x01    Cloak
        0x02    Burrow
        0x04    Transit
        0x08    Hallucination
        0x10    Invincible

    Used
        0x01    Player
        0x02    Health
        0x04    Shield
        0x08    Energy
        0x10    Resource
        0x20    Hangar
    */
}
