namespace NasladdinPlace.Spreadsheets.Models
{
    public struct Color
    {
        public int Red { get; }
        public int Green { get; }
        public int Blue { get; }

        public Color(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }
}