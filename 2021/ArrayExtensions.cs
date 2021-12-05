using System;

namespace _2021;

public static class ArrayExtensions
{
    public static ArraySegment<T> Slice<T>(this T[] arr, int offset, int count)
    {
        return new ArraySegment<T>(arr, offset, count);
    }
}