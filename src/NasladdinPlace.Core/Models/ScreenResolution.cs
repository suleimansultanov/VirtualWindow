using System;

namespace NasladdinPlace.Core.Models
{
    public struct ScreenResolution : IEquatable<ScreenResolution>
    {
        public int Height { get; }
        public int Width { get; }

        public ScreenResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static bool operator !=(ScreenResolution firstScreenResolution, ScreenResolution secondScreenResolution)
        {
            return !firstScreenResolution.Equals(secondScreenResolution);
        }

        public static bool operator ==(ScreenResolution firstScreenResolution, ScreenResolution secondScreenResolution)
        {
            return firstScreenResolution.Equals(secondScreenResolution);
        }

        public bool Equals(ScreenResolution other)
        {
            return Height == other.Height && Width == other.Width;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ScreenResolution other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Height * 397) ^ Width;
            }
        }
    }
}