#region License
//
// Procedural planet renderer.
// Copyright (c) 2008-2011 INRIA
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// Proland is distributed under a dual-license scheme.
// You can obtain a specific license from Inria: proland-licensing@inria.fr.
//
// Authors: Justin Hawkins 2014.
// Modified by Denis Ovchinnikov 2015-2017
#endregion

namespace UnityEngine
{
    public class PerlinNoise
    {
        const int B = 256;
        private readonly int[] m_perm = new int[B + B];

        public PerlinNoise(int seed)
        {
            Random.InitState(seed);

            int i, j, k;
            for (i = 0; i < B; i++)
            {
                m_perm[i] = i;
            }

            while (--i != 0)
            {
                k = m_perm[i];
                j = Random.Range(0, B);
                m_perm[i] = m_perm[j];
                m_perm[j] = k;
            }

            for (i = 0; i < B; i++)
            {
                m_perm[B + i] = m_perm[i];
            }

        }

        float FADE(float t)
        {
            return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
        }

        float LERP(float t, float a, float b)
        {
            return (a) + (t) * ((b) - (a));
        }

        float GRAD1(int hash, float x)
        {
            int h = hash & 15;
            float grad = 1.0f + (h & 7);
            if ((h & 8) != 0) grad = -grad;
            return (grad * x);
        }

        float GRAD2(int hash, float x, float y)
        {
            int h = hash & 7;
            float u = h < 4 ? x : y;
            float v = h < 4 ? y : x;
            return (((h & 1) != 0) ? -u : u) + (((h & 2) != 0) ? -2.0f * v : 2.0f * v);
        }


        float GRAD3(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = (h < 4) ? y : (h == 12 || h == 14) ? x : z;
            return (((h & 1) != 0) ? -u : u) + (((h & 2) != 0) ? -v : v);
        }

        public float Noise1D(float x)
        {
            //returns a noise value between -0.5 and 0.5
            int ix0, ix1;
            float fx0, fx1;
            float s, n0, n1;

            ix0 = (int)Mathf.Floor(x); // Integer part of x
            fx0 = x - ix0; // Fractional part of x
            fx1 = fx0 - 1.0f;
            ix1 = (ix0 + 1) & 0xff;
            ix0 = ix0 & 0xff; // Wrap to 0..255

            s = FADE(fx0);

            n0 = GRAD1(m_perm[ix0], fx0);
            n1 = GRAD1(m_perm[ix1], fx1);
            return 0.188f * LERP(s, n0, n1);
        }

        public float Noise2D(float x, float y)
        {
            //returns a noise value between -0.75 and 0.75
            int ix0, iy0, ix1, iy1;
            float fx0, fy0, fx1, fy1, s, t, nx0, nx1, n0, n1;

            ix0 = (int)Mathf.Floor(x); // Integer part of x
            iy0 = (int)Mathf.Floor(y); // Integer part of y
            fx0 = x - ix0; // Fractional part of x
            fy0 = y - iy0; // Fractional part of y
            fx1 = fx0 - 1.0f;
            fy1 = fy0 - 1.0f;
            ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
            iy1 = (iy0 + 1) & 0xff;
            ix0 = ix0 & 0xff;
            iy0 = iy0 & 0xff;

            t = FADE(fy0);
            s = FADE(fx0);

            nx0 = GRAD2(m_perm[ix0 + m_perm[iy0]], fx0, fy0);
            nx1 = GRAD2(m_perm[ix0 + m_perm[iy1]], fx0, fy1);

            n0 = LERP(t, nx0, nx1);

            nx0 = GRAD2(m_perm[ix1 + m_perm[iy0]], fx1, fy0);
            nx1 = GRAD2(m_perm[ix1 + m_perm[iy1]], fx1, fy1);

            n1 = LERP(t, nx0, nx1);

            return 0.507f * LERP(s, n0, n1);
        }

        public float Noise3D(float x, float y, float z)
        {
            //returns a noise value between -1.5 and 1.5
            int ix0, iy0, ix1, iy1, iz0, iz1;
            float fx0, fy0, fz0, fx1, fy1, fz1;
            float s, t, r;
            float nxy0, nxy1, nx0, nx1, n0, n1;

            ix0 = (int)Mathf.Floor(x); // Integer part of x
            iy0 = (int)Mathf.Floor(y); // Integer part of y
            iz0 = (int)Mathf.Floor(z); // Integer part of z
            fx0 = x - ix0; // Fractional part of x
            fy0 = y - iy0; // Fractional part of y
            fz0 = z - iz0; // Fractional part of z
            fx1 = fx0 - 1.0f;
            fy1 = fy0 - 1.0f;
            fz1 = fz0 - 1.0f;
            ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
            iy1 = (iy0 + 1) & 0xff;
            iz1 = (iz0 + 1) & 0xff;
            ix0 = ix0 & 0xff;
            iy0 = iy0 & 0xff;
            iz0 = iz0 & 0xff;

            r = FADE(fz0);
            t = FADE(fy0);
            s = FADE(fx0);

            nxy0 = GRAD3(m_perm[ix0 + m_perm[iy0 + m_perm[iz0]]], fx0, fy0, fz0);
            nxy1 = GRAD3(m_perm[ix0 + m_perm[iy0 + m_perm[iz1]]], fx0, fy0, fz1);
            nx0 = LERP(r, nxy0, nxy1);

            nxy0 = GRAD3(m_perm[ix0 + m_perm[iy1 + m_perm[iz0]]], fx0, fy1, fz0);
            nxy1 = GRAD3(m_perm[ix0 + m_perm[iy1 + m_perm[iz1]]], fx0, fy1, fz1);
            nx1 = LERP(r, nxy0, nxy1);

            n0 = LERP(t, nx0, nx1);

            nxy0 = GRAD3(m_perm[ix1 + m_perm[iy0 + m_perm[iz0]]], fx1, fy0, fz0);
            nxy1 = GRAD3(m_perm[ix1 + m_perm[iy0 + m_perm[iz1]]], fx1, fy0, fz1);
            nx0 = LERP(r, nxy0, nxy1);

            nxy0 = GRAD3(m_perm[ix1 + m_perm[iy1 + m_perm[iz0]]], fx1, fy1, fz0);
            nxy1 = GRAD3(m_perm[ix1 + m_perm[iy1 + m_perm[iz1]]], fx1, fy1, fz1);
            nx1 = LERP(r, nxy0, nxy1);

            n1 = LERP(t, nx0, nx1);

            return 0.936f * LERP(s, n0, n1);
        }
    }
}