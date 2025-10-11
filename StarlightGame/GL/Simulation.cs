using StarlightGame.StarlightLib;
using StarlightGame.StarlightLib.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlightGame.GL
{
    internal class Simulation
    {
        public void SimulateEntities(Scene scene, float dt)
        {
            AmpSharp.UpdateEntities(scene.Entities, dt);

            EnactEntityEvents(scene, dt);
        }

        private void EnactEntityEvents(Scene scene, float dt)
        {
            for (int i = 0; i < scene.entityHead; i++)
            {
                Entity entity = scene.Entities[i];

                entity.TimeAlive += dt;

                if (entity.TimeToLive != 0)
                {
                    if (entity.TimeAlive >= entity.TimeToLive)
                    {
                        scene.RemoveEntity(i);
                        i--;
                        continue;
                    }
                }

                switch (entity.QueuedEvent)
                {
                    default:
                    case EntityEvent.None:
                        break;

                    case EntityEvent.FireWeapons:
                        {
                            float projSpeed = 200.0f;
                            float rad = entity.RotationDeg * (float)Math.PI / 180.0f;
                            float vx = (float)Math.Sin(rad) * projSpeed;
                            float vy = (float)Math.Cos(rad) * projSpeed;
                            Entity proj = new Entity
                            {
                                Layer = EntityLayer.Foreground,
                                Type = EntityType.Projectile,
                                Mass = 0.1f,
                                Scale = 2,
                                X = entity.X,
                                Y = entity.Y,
                                VX = vx + entity.VX,
                                VY = vy + entity.VY,
                                TimeToLive = 5.0f
                            };
                            scene.AddEntity(proj);
                            entity.QueuedEvent = EntityEvent.None;
                        }
                        break;
                }

                scene.Entities[i] = entity;
            }
        }
    }
}
