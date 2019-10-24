using System;

namespace MersenneTwister.MT
{
    public interface Imt19937
    {
        void init_genrand(uint s);
        void init_by_array(uint[] init_key, int key_length);
        int genrand_int31();
        uint genrand_int32();
        double genrand_real2();
        double genrand_res53();
    }
}
