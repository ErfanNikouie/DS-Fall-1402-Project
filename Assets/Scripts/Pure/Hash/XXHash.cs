using System;
using System.Runtime.CompilerServices;

public static class XXHash
{
    private const uint PRIME32_1 = 2654435761U;
    private const uint PRIME32_2 = 2246822519U;
    private const uint PRIME32_3 = 3266489917U;
    private const uint PRIME32_4 = 668265263U;
    private const uint PRIME32_5 = 374761393U;

    public static uint CalculateHash(string s, uint seed = 0)
    {
        return CalculateHash(System.Text.Encoding.ASCII.GetBytes(s), seed);
    }

    public static uint CalculateHash(byte[] data, uint seed = 0)
    {
        int length = data.Length;
        int currentIndex = 0;
        uint hash;

        if (length >= 16)
        {
            int limit = length - 16;
            uint v1 = seed + PRIME32_1 + PRIME32_2;
            uint v2 = seed + PRIME32_2;
            uint v3 = seed + 0;
            uint v4 = seed - PRIME32_1;

            do
            {
                v1 += ToUInt32(data, currentIndex) * PRIME32_2;
                v1 = RotateLeft(v1, 13);
                v1 *= PRIME32_1;
                currentIndex += 4;

                v2 += ToUInt32(data, currentIndex) * PRIME32_2;
                v2 = RotateLeft(v2, 13);
                v2 *= PRIME32_1;
                currentIndex += 4;

                v3 += ToUInt32(data, currentIndex) * PRIME32_2;
                v3 = RotateLeft(v3, 13);
                v3 *= PRIME32_1;
                currentIndex += 4;

                v4 += ToUInt32(data, currentIndex) * PRIME32_2;
                v4 = RotateLeft(v4, 13);
                v4 *= PRIME32_1;
                currentIndex += 4;
            } while (currentIndex <= limit);

            hash = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
        }
        else
        {
            hash = seed + PRIME32_5;
        }

        hash += (uint)length;

        while (currentIndex <= length - 4)
        {
            hash += ToUInt32(data, currentIndex) * PRIME32_3;
            hash = RotateLeft(hash, 17) * PRIME32_4;
            currentIndex += 4;
        }

        while (currentIndex < length)
        {
            hash += data[currentIndex] * PRIME32_5;
            hash = RotateLeft(hash, 11) * PRIME32_1;
            currentIndex++;
        }

        hash ^= hash >> 15;
        hash *= PRIME32_2;
        hash ^= hash >> 13;
        hash *= PRIME32_3;
        hash ^= hash >> 16;

        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint RotateLeft(uint value, int count)
    {
        return (value << count) | (value >> (32 - count));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ToUInt32(byte[] data, int startIndex)
    {
        return BitConverter.ToUInt32(data, startIndex);
    }
}
