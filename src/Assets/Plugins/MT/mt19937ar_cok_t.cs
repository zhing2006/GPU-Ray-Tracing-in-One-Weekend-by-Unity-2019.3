/*
   A C-program for MT19937, with initialization improved 2002/2/10.
   Coded by Takuji Nishimura and Makoto Matsumoto.
   This is a faster version by taking Shawn Cokus's optimization,
   Matthe Bellew's simplification, Isaku Wada's real version.

   Before using, initialize the mt by using init_genrand(seed)
   or init_by_array(init_key, key_length).

   Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
   All rights reserved.

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

     1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

     2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

     3. The names of its contributors may not be used to endorse or promote
        products derived from this software without specific prior written
        permission.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


   Any feedback is very welcome.
   http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
   email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
*/

using System;
using System.Runtime.CompilerServices;

namespace MersenneTwister.MT
{
    public
    sealed class mt19937ar_cok_t : mt_base_t, Imt19937
    {
        /* Period parameters */
        private const int N = 624;
        private const int M = 397;
        private const uint MATRIX_A = 0x9908b0dfU; /* constant vector a */
        private const uint UMASK = 0x80000000U; /* most significant w-r bits */
        private const uint LMASK = 0x7fffffffU; /* least significant r bits */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MIXBITS(uint u, uint v)
        {
            return (((u) & UMASK) | ((v) & LMASK));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint TWIST(uint u, uint v)
        {
            return ((MIXBITS(u, v) >> 1) ^ (((v & 1U) != 0) ? MATRIX_A : 0U));
        }

        private readonly uint[] mt = new uint[N]; /* the array for the state vector  */
        private int left = 1;
        private int initf = 0;
        private uint next;

        /* initializes mt[N] with a seed */
        public void init_genrand(uint s)
        {
            var mt = this.mt;
            uint j;
            mt[0] = s;
            for (j = 1; j < N; j++) {
                mt[j] = (1812433253U * (mt[j - 1] ^ (mt[j - 1] >> 30)) + j);
                /* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
                /* In the previous versions, MSBs of the seed affect   */
                /* only MSBs of the array mt[].                        */
                /* 2002/01/09 modified by Makoto Matsumoto             */
            }
            left = 1;
            initf = 1;
        }

        /* initialize by an array with array-length */
        /* init_key is the array for initializing keys */
        /* key_length is its length */
        /* slight change for C++, 2004/2/26 */
        public void init_by_array(uint[] init_key, int key_length)
        {
            var mt = this.mt;
            init_genrand(19650218U);
            uint i = 1;
            uint j = 0;
            int k = (N > key_length ? N : key_length);
            for (; k != 0; k--) {
                mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525U)) + init_key[j] + j; /* non linear */
                i++;
                j++;
                if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
                if (j >= key_length) {
                    j = 0;
                }
            }
            for (k = N - 1; k != 0; k--) {
                mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941U)) - i; /* non linear */
                i++;
                if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
            }

            mt[0] = 0x80000000U; /* MSB is 1; assuring non-zero initial array */
            left = 1;
            initf = 1;
        }

        private void next_state()
        {
            var mt = this.mt;

            /* if init_genrand() has not been called, */
            /* a default initial seed is used         */
            if (initf == 0) {
                init_genrand(5489U);
            }

            left = N;
            next = 0;

            uint p = 0;
            for (int j = N - M + 1; --j > 0; p++) {
                mt[p] = mt[p + M] ^ TWIST(mt[p], mt[p + 1]);
            }

            for (int j = M; --j > 0; p++) {
                mt[p] = mt[p + (M - N)] ^ TWIST(mt[p], mt[p + 1]);
            }

            mt[p] = mt[p + (M - N)] ^ TWIST(mt[p], mt[0]);
        }

        private static readonly uint[] mag01 = new[] { 0x0U, MATRIX_A };

        /* generates a random number on [0,0xffffffff]-interval */
        public uint genrand_int32()
        {
            uint y;
            if (--left == 0) {
                next_state();
            }

            y = this.mt[next++];

            /* Tempering */
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            return y;
        }

        /* generates a random number on [0,0x7fffffff]-interval */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int genrand_int31()
        {
            return (int)(genrand_int32() >> 1);
        }

        /* generates a random number on [0,1]-real-interval */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double genrand_real1()
        {
            return genrand_int32() * (1.0 / 4294967295.0);
            /* divided by 2^32-1 */
        }

        /* generates a random number on [0,1)-real-interval */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double genrand_real2()
        {
            return genrand_int32() * (1.0 / 4294967296.0);
            /* divided by 2^32 */
        }

        /* generates a random number on (0,1)-real-interval */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double genrand_real3()
        {
            return (genrand_int32() + 0.5) * (1.0 / 4294967296.0);
            /* divided by 2^32 */
        }
        /* These real versions are due to Isaku Wada, 2002/01/09 added */

        /* generates a random number on [0,1) with 53-bit resolution*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double genrand_res53()
        {
            uint a = genrand_int32() >> 5, b = genrand_int32() >> 6;
            return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
        }
    }
}
