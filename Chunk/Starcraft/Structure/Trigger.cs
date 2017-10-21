using System.Runtime.InteropServices;
using System;

namespace Chunk.Starcraft.Structure
{
    /// <summary>Starcraft Trigger [TRIG, MBRF]</summary>
    public struct Trigger {
        /// <summary>Trigger Conditions [16]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public Condition[] Conditions;
        /// <summary>Trigger Actions [64]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Action[] Actions;
        /// <summary>Trigger Execute Flags</summary>
        public UInt32 Flags;
        /// <summary>Trigger Players [28]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public Byte[] Players;

        /// <summary>Trigger Condition</summary>
        public struct Condition {
            /// <summary>Location</summary>
            public UInt32 Location;
            /// <summary>Players</summary>
            public UInt32 Players;
            /// <summary>Unit Quantity, Resource</summary>
            public UInt32 Quantity;
            /// <summary>Unit ID</summary>
            public UInt16 Unit;
            /// <summary>Numberic Comparison, Switch State</summary>
            public Byte Compare;
            /// <summary>Condition ID</summary>
            public Byte ID;
            /// <summary>Resource Type, Score Type, Switch Number</summary>
            public Byte Type;
            /// <summary>Condition Flags</summary>
            public Byte Flags;
            /// <summary>Next Condition</summary>
            public UInt16 Next;
        }

        public struct Action {
            /// <summary>Location</summary>
            public UInt32 Location;
            /// <summary>Text String</summary>
            public Int32 Text;
            /// <summary>Wave String</summary>
            public Int32 Wave;
            /// <summary>Time</summary>
            public UInt32 Time;
            /// <summary>Players</summary>
            public UInt32 Players;
            /// <summary>Action Parameter</summary>
            public UInt32 Param;
            /// <summary>Unit Type, Score Type, Resource Type, Alliance Status</summary>
            public UInt16 Type;
            /// <summary>Action ID</summary>
            public Byte ID;
            /// <summary>Unit Quantity, Switch Action, Unit Order, Number Modifiers</summary>
            public Byte Param2;
            /// <summary>Action Flags</summary>
            public UInt16 Flags;
            /// <summary>Next Action</summary>
            public UInt16 Next;
        }
    }

    /*
    Players
        0   Player 1
        1   Player 2
        2   Player 3
        3   Player 4
        4   Player 5
        5   Player 6
        6   Player 7
        7   Player 8
        8   Player 9
        9   Player 10
        10  Player 11
        11  Player 12
        12  None
        13  Current Player
        14  Foes
        15  Allies
        16  Neutral Players
        17  All Players
        18  Force 1
        19  Force 2
        20  Force 3
        21  Force 4
        26  Non Allied Victory Players

    Modifiers
        0   More Than
        1   Less Than
        2   Is True
        3   Is False
        4   Set True
        5   Set False
        6   Not
        7   Set To
        8   Add
        9   Subtract
        10  Exactly
        11  Randomize
        
    Flags
        0x01    Ignore Wait Once
        0x02    Disable
        0x04    Always Display
        0x08    Unit Properties Used
        0x10    Unit Type Used
        0x20    Unit ID Used
    
    Execute Flags
        0x01    Skip Conditions Once
        0x02    Ignore Defeat, Draw
        0x04    Disable
        0x08    Ignore Transmission Triggers, Ping Trigger Once
        0x10    Has Paused Trigger
        0x20    Wait Skipping Disabled Once
    */
}
