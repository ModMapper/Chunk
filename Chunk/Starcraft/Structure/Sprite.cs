using System;

namespace Chunk.Starcraft.Structure
{
    /// <summary>Starcraft Sprite [THG2]</summary>
    public struct Sprite {
        /// <summary>Sprite ID, Unit ID</summary>
        public UInt16 ID;
        /// <summary>Sprite Position</summary>
        public UInt16 X, Y;
        /// <summary>Player</summary>
        public Byte Player;
        /// <summary>Empty</summary>
        public Byte Unused;
        /// <summary>Sprite Flags</summary>
        public UInt16 Flags;
    }

    /*
    Flags
        0x01    Gun Trap
        0x10    Cover Ground
        0x80    Blocking
        0x100   Play sprite
        0x800   Draw as sprite
        0x1000  Sprite
        0x2000  Unit
        0x4000  Flipped
        0x8000  Disabled
     */
}
