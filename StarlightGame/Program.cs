using StarlightGame.StarlightLib;
using StarlightGame.StarlightLib.Types;

namespace StarlightGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            float [] data = new float[] { 1, 2, 3, 4, 5 };

            var result = AmpSharp.SquareArray(data);

            Console.WriteLine(string.Join(", ", result));

            // Complex type interop demo
            var entities = new Entity[3];
            entities[0] = new Entity { X = 0, Y = 0, VX = 1, VY = 0, Mass = 1 };
            entities[1] = new Entity { X = 0, Y = 0, VX = 0, VY = 1, Mass = 2 };
            entities[2] = new Entity { X = 0, Y = 0, VX = 0, VY = 0, Mass = 3 };

            AmpSharp.UpdateEntities(entities, 0.005f);

            for (int i = 0; i < entities.Length; i++)
            {
                Console.WriteLine($"P{i}: ({entities[i].X}, {entities[i].Y}) m={entities[i].Mass}");
            }
        }
    }
}
