using System;

namespace MersenneTwister.MT
{
    public abstract class mt_base_t
    {
        static mt_base_t()
        {
            if (!BitConverter.IsLittleEndian) {
                throw new PlatformNotSupportedException("MersenneTwister does not support Big Endian platforms");
            }
        }
    }
}
