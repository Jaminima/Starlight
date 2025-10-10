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
            var particles = new Particle[3];
            particles[0] = new Particle { X = 0, Y = 0, Z = 0, Vx = 1, Vy = 0, Vz = 0, Mass = 1 };
            particles[1] = new Particle { X = 0, Y = 0, Z = 0, Vx = 0, Vy = 1, Vz = 0, Mass = 2 };
            particles[2] = new Particle { X = 0, Y = 0, Z = 0, Vx = 0, Vy = 0, Vz = 1, Mass = 3 };

            StarlightLib.AmpSharp.UpdateParticles(particles, 0.5f);

            for (int i = 0; i < particles.Length; i++)
            {
                Console.WriteLine($"P{i}: ({particles[i].X}, {particles[i].Y}, {particles[i].Z}) m={particles[i].Mass}");
            }
        }
    }
}
