using StarlightGame.StarlightLib.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlightGame.GL
{
    internal class Scene
    {
        public Camera Camera = new Camera() { X = 0, Y = 0, Zoom = 1.0f };
        public Entity[] Entities;
        public uint entityHead = 0;

        public Entity Player => Entities[0];

        public Scene() {
            this.Entities = new Entity[100000];

            this.InitPlayer();

            //this.RandomProjectiles(90000);

            this.RandomEnemies(1000);
        }

        private void InitPlayer()
        {
            Entity player = new Entity
            {
                Layer = EntityLayer.Foreground,
                Type = EntityType.Player,
                Mass = 1,
                Scale = 10,
                X = 10,
                Y = 10,
            };

            this.Entities[0] = player;
            entityHead = 1;
        }

        private void RandomEnemies(int count)
        {
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                Entity enemy = new Entity
                {
                    Layer = EntityLayer.Foreground,
                    Type = EntityType.Enemy,
                    Mass = 1,
                    Scale = 5,
                    X = (float)(rand.NextDouble() * 20000 - 10000),
                    Y = (float)(rand.NextDouble() * 20000 - 10000),
                };
                AddEntity(enemy);
            }
        }

        private void RandomProjectiles(int count)
        {
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                Entity proj = new Entity
                {
                    Layer = EntityLayer.Foreground,
                    Type = EntityType.Projectile,
                    Mass = 0.1f,
                    Scale = 2,
                    X = (float)(rand.NextDouble() * 20000 - 10000),
                    Y = (float)(rand.NextDouble() * 20000 - 10000),
                    //VX = (float)(rand.NextDouble() * 20 - 10),
                    //VY = (float)(rand.NextDouble() * 20 - 10),
                };
                AddEntity(proj);
            }
        }

        public uint AddEntity(Entity entity)
        {
            if (entityHead >= Entities.Length)
                return uint.MaxValue;
            Entities[entityHead] = entity;
            entityHead++;
            return entityHead - 1;
        }

        public void RemoveEntity(int index)
        {
            if (index < 0 || index >= entityHead)
                return;
            Entities[index] = Entities[entityHead - 1];
            Entities[entityHead - 1] = new Entity();
            entityHead--;
        }
    }
}
