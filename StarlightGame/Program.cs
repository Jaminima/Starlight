using StarlightGame.GL;
using StarlightGame.StarlightLib;
using StarlightGame.StarlightLib.Types;

namespace StarlightGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Scene scene = new Scene();

            Window window = new Window(scene);

            window.Run();
        }
    }
}
