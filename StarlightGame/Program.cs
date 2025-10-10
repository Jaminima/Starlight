namespace StarlightGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            float [] data = new float[] { 1, 2, 3, 4, 5 };

            var result = StarlightLib.AmpSharp.SquareArray(data);

            Console.WriteLine(string.Join(", ", result));
        }
    }
}
