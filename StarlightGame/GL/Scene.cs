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

        // Index into Entities[] of the current target; -1 if none
        public int CurrentTargetIndex = -1;

        public Entity Player => Entities[0];

        public Scene() {
            this.Entities = new Entity[100000];

            this.InitPlayer();

            //this.RandomProjectiles(90000);

            this.RandomEnemies(20);
        }

        private void InitPlayer()
        {
            Entity player = new Entity
            {
                Layer = EntityLayer.Foreground,
                Type = EntityType.Player,
                Mass = 1,
                Scale = 20,
                X = 10,
                Y = 10,
                TargetIndex = -1,
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
                    X = (float)(rand.NextDouble() * 10000 - 5000),
                    Y = (float)(rand.NextDouble() * 10000 - 5000),
                    TargetIndex = -1,
                };
                AddEntity(enemy);
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

            // If removing the current target, clear target selection
            if (index == CurrentTargetIndex)
            {
                CurrentTargetIndex = -1;
            }

            Entities[index] = Entities[entityHead - 1];
            Entities[entityHead - 1] = new Entity();
            entityHead--;
        }

        // Select the closest enemy in front of the player within a field-of-view
        public void UpdateTargetSelection()
        {
            int bestIdx = -1;
            float bestScore = float.MaxValue;
            var player = Entities[0];

            // Parameters
            float maxDist = 800.0f;
            float fovDeg = 70.0f; // +/- from forward
            float playerRad = player.RotationDeg * (float)Math.PI / 180.0f;
            float fwdX = (float)Math.Sin(playerRad);
            float fwdY = (float)Math.Cos(playerRad);

            for (int i = 1; i < entityHead; i++)
            {
                var e = Entities[i];
                if (e.Type != EntityType.Enemy)
                    continue;

                float dx = e.X - player.X;
                float dy = e.Y - player.Y;
                float dist2 = dx * dx + dy * dy;
                if (dist2 <= 1e-4f) dist2 = 1e-4f;
                float dist = (float)Math.Sqrt(dist2);
                if (dist > maxDist)
                    continue;

                float dirX = dx / dist;
                float dirY = dy / dist;
                float dot = dirX * fwdX + dirY * fwdY; // cos(angle)
                float angleDeg = (float)(Math.Acos(Math.Clamp(dot, -1.0f, 1.0f)) * 180.0 / Math.PI);
                if (angleDeg > fovDeg)
                    continue;

                // Score by angle first then distance
                float score = angleDeg * 1000.0f + dist;

                // Add hysteresis: prefer current target by reducing its score
                if (i == CurrentTargetIndex)
                {
                    score -= 500.0f; // Bonus to keep current target
                }

                if (score < bestScore)
                {
                    bestScore = score;
                    bestIdx = i;
                }
            }

            CurrentTargetIndex = bestIdx;
        }
    }
}
