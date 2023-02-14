using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditorReborn.API.Enums
{
    [Flags]
    internal enum GravityGunMode
    {
        Movement = 1,
        Rotate = 2,
        Gravity = 4,

    }
}
