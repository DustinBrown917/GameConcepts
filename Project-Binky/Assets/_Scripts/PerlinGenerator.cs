using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PerlinGenerator : MonoBehaviour {

    private static int[] permutation;
    private static int permutationRepeats = 2;
    private static int permutationLength = 256;

    [SerializeField] private int repeat = -1;
    [SerializeField] private float scale = 20.0f;
    [SerializeField] private int octaves = 1;
    [SerializeField] private float persistence = 1.0f;
    [SerializeField] private float[] steps;
    private float stepValue;

    static PerlinGenerator()
    {
        BuildPermutationTable();
    }

    private static void BuildPermutationTable()
    {
        System.Random rnd = new System.Random();

        permutation = new int[permutationLength * permutationRepeats];
        List<int> orderedNums = new List<int>();

        for (int i = 0; i < permutationLength; i++)
        {
            orderedNums.Add(i);
        }

        List<int> copiedNums;

        int baseIndex;

        for (int repeats = 0; repeats < permutationRepeats; repeats++)
        {
            baseIndex = repeats * permutationLength;
            if (baseIndex >= permutation.Length) { baseIndex = permutation.Length - 1; }

            copiedNums = new List<int>(orderedNums);

            int targetIndex;
            for (int i = 0; i < permutationLength; i++)
            {
                targetIndex = rnd.Next(copiedNums.Count);
                permutation[baseIndex + i] = copiedNums[targetIndex];
                copiedNums.RemoveAt(targetIndex);
            }
        }
    }

    private static float PerlinFade(float t)
    {
        //Ken Perlin's fade function. (6t^5 - 15t^4 + 10t^3)
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float GetGradient(int hash, float x, float y, float z)
    {
        switch (hash & 0xF)
        {
            case 0x0: return x + y;
            case 0x1: return -x + y;
            case 0x2: return x - y;
            case 0x3: return -x - y;
            case 0x4: return x + z;
            case 0x5: return -x + z;
            case 0x6: return x - z;
            case 0x7: return -x - z;
            case 0x8: return y + z;
            case 0x9: return -y + z;
            case 0xA: return y - z;
            case 0xB: return -y - z;
            case 0xC: return y + x;
            case 0xD: return -y + z;
            case 0xE: return y - x;
            case 0xF: return -y - z;
            default: return 0; // never happens
        }
    }

    public float GetPerlin(float x, float y, float z)
    {
        float totalNoise = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxPossibleNoise = 0;

        if(steps.Length > 0)
        {
            stepValue = 1.0f / steps.Length;
        }
        
        

        for (int i = 0; i < octaves; i++)
        {
            totalNoise += GetSimplePerlin(x * frequency, y * frequency, z * frequency) * amplitude;

            maxPossibleNoise += amplitude;

            amplitude *= persistence;
            frequency *= 2;
        }

        float finalNoise = totalNoise / maxPossibleNoise;

        if (steps.Length > 0)
        {
            float newNoise = 0.0f;
            for (int i = steps.Length - 1; i >= 0; i--)
            {
                if (finalNoise > steps[i])
                {
                    
                    newNoise = stepValue * ((float)i + 1.0f);
                    //Debug.Log(finalNoise.ToString() + " > " + steps[i].ToString() + " newNoise: " + newNoise.ToString());
                    break;
                }
            }
            finalNoise = newNoise;
        }

        return finalNoise;
    }

    public float GetSimplePerlin(float x, float y, float z)
    {
        //Handle scale
        x *= scale; //Debug.Log(x);
        y *= scale;
        z *= scale;

        //Handle repeat overflows in the coordinates.
        if(repeat > 0)
        {
            x = x % repeat;
            y = y % repeat;
            z = z % repeat;
        }

        //Establish unit cube coords. Bind to 0-255 to avoid overflow errors
        int cubeX = (int)x & 255; //Debug.Log((int)x + " vs " + cubeX);
        int cubeY = (int)y & 255;
        int cubeZ = (int)z & 255;
        //Get location within cube unit.
        float localX = x - (int)x;
        float localY = y - (int)y;
        float localZ = z - (int)z;

        float u = PerlinFade(localX);
        float v = PerlinFade(localY);
        float w = PerlinFade(localZ);

        //Hash the eight coordinates of the Unit Cube found above.
        int p1, p2, p3, p4, p5, p6, p7, p8;

        p1 = permutation[permutation[permutation[cubeX] + cubeY] + cubeZ];
        p2 = permutation[permutation[permutation[cubeX] + Increment(cubeY)] + cubeZ];
        p3 = permutation[permutation[permutation[cubeX] + cubeY] + Increment(cubeZ)];
        p4 = permutation[permutation[permutation[cubeX] + Increment(cubeY)] + Increment(cubeZ)];
        p5 = permutation[permutation[permutation[Increment(cubeX)] + cubeY] + cubeZ];
        p6 = permutation[permutation[permutation[Increment(cubeX)] + Increment(cubeY)] + cubeZ];
        p7 = permutation[permutation[permutation[Increment(cubeX)] + cubeY] + Increment(cubeZ)];
        p8 = permutation[permutation[permutation[Increment(cubeX)] + Increment(cubeY)] + Increment(cubeZ)];

        float x1, y1, x2, y2;

        x1 = Mathf.Lerp(GetGradient(p1, localX, localY, localZ), GetGradient(p5, localX - 1, localY, localZ), u);
        x2 = Mathf.Lerp(GetGradient(p2, localX, localY - 1, localZ), GetGradient(p6, localX - 1, localY - 1, localZ), u);

        y1 = Mathf.Lerp(x1, x2, v);

        x1 = Mathf.Lerp(GetGradient(p3, localX, localY, localZ - 1), GetGradient(p7, localX - 1, localY, localZ - 1), u);
        x2 = Mathf.Lerp(GetGradient(p4, localX, localY - 1, localZ - 1), GetGradient(p8, localX - 1, localY - 1, localZ - 1), u);

        y2 = Mathf.Lerp(x1, x2, v);

        return (Mathf.Lerp(y1, y2, w) + 1) * 0.5f;

    }

    private int Increment(int num)
    {
        num++;
        if(repeat > 0) { num %= repeat; }

        return num;
    }

}
