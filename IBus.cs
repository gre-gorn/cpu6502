using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators
{
    public interface IBus
    {
        byte Read(ushort addr);
        void Write(ushort addr, byte data);
    }
}
