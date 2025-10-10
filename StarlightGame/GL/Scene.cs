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
        public Entity[] Entities;
        private int entityHead = 0;

        public Entity Player => Entities[0];

        public Scene() {
            this.Entities = new Entity[1000];

            this.InitPlayer();
        }

        private void InitPlayer()
        {
            Entity player = new Entity
            {
                Layer = EntityLayer.Foreground,
                Type = EntityType.Player,
                Mass = 1,
                Scale = 10,
                X = 100,
                Y = 100,
            };

            this.Entities[0] = player;
            entityHead = 1;
        }

        public int AddEntity(Entity entity)
        {
            if (entityHead >= Entities.Length)
                return -1;
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
