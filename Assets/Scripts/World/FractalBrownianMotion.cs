using UnityEngine;
using UnityEditor;
using System;

public class FractalBrownianMotion
{
    float h_fBm = 1.2f;
    float lacunarity_fBm = 3.7f;
    int octaves_fBm = 12;
    float offset_fBm = 0.32f;
    float time;
    public FractalBrownianMotion() {
    }
    public float GetHeight(int x, int y)
    {
        float uv = (fBm(new Vector2(x, y) * 3, h_fBm, lacunarity_fBm, octaves_fBm, offset_fBm)); 

        return uv;
    }


    private float rand(Vector2 co) {
        float x = (float)Math.Sin(Vector2.Dot(co, new Vector2(12.9898f, 78.233f))) * 43758.5453f;
        return fract(x);
    }

    private Vector2 GetRandomGradient(Vector2 xy, float seed)
    {
        float x = seed * rand(xy);
        return new Vector2((float)Math.Cos(x), (float)Math.Sin(x));
    }
    private float SmoothInterpolation(float v)
    {
        return 6 * v * v * v * v * v - 15 * v * v * v * v + 10 * v * v * v;
    }

    private float Mix(float x, float y, float al)
    {
        return (1 - al) * x + al * y;
    }

    private float fract(float x)
    {
        return (float)x - (float)Math.Floor(x);
    }

    private float PerlinNoise(Vector2 pos, float seed)
    {
        Vector2 bl = new Vector2((float)Math.Floor(pos.x), (float)Math.Floor(pos.y));
        Vector2 br = bl + new Vector2(1, 0);
        Vector2 ul = bl + new Vector2(0, 1);
        Vector2 ur = bl + new Vector2(1, 1);

        Vector2 a = pos - bl;
        Vector2 b = pos - br;
        Vector2 c = pos - ul;
        Vector2 d = pos - ur;

        float s = Vector2.Dot(GetRandomGradient(bl, seed), a);
        float t = Vector2.Dot(GetRandomGradient(br, seed), b);
        float u = Vector2.Dot(GetRandomGradient(ul, seed), c);
        float v = Vector2.Dot(GetRandomGradient(ur, seed), d);

        float st = Mix(s, t, SmoothInterpolation(fract(pos.x)));
        float uv_f = Mix(u, v, SmoothInterpolation(fract(pos.x)));
        return Mix(st, uv_f, SmoothInterpolation(fract(pos.y)));

    }

    private float fBm(Vector2 pos, float h, float l, int octaves, float offset)
    {
        float v = 0;
        Vector2 p = pos;
        for (int i = 0; i < octaves; i++)
        {
            v += ((PerlinNoise(p, 10) + offset) * (float)Math.Pow(l, -h * i));
            p *= l;
            // h-=0.2;
        }
        return v;
    }


    private float distance(Vector2 a, Vector2 b)
    {
        return (float)Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
    }
}