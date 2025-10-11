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
        private float ttlSweepTimer = 0.0f;
        private const float TTL_SWEEP_INTERVAL = 1.0f;
        private float enactTimer = 0.0f;
        private const float ENACT_INTERVAL = 0.1f;

        public void SimulateEntities(Scene scene, float dt)
        {
            // Update TimeAlive for all entities
            for (int i = 0; i < scene.entityHead; i++)
            {
                scene.Entities[i].TimeAlive += dt;
            }

            AmpSharp.UpdateEntities(scene.Entities, dt);

            ttlSweepTimer += dt;
            bool doSweep = false;
            if (ttlSweepTimer >= TTL_SWEEP_INTERVAL)
            {
                doSweep = true;
                ttlSweepTimer -= TTL_SWEEP_INTERVAL;
            }

            enactTimer += dt;
            if (enactTimer >= ENACT_INTERVAL)
            {
                EnactEntityEvents(scene, doSweep);
                enactTimer -= ENACT_INTERVAL;
            }
        }

        private void EnactEntityEvents(Scene scene, bool doSweep)
        {
            for (int i = 0; i < scene.entityHead; i++)
            {
                Entity entity = scene.Entities[i];

                if (doSweep && entity.TimeToLive != 0)
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

                    case EntityEvent.FireCannon:
                        {
                            float projSpeed = 200.0f;
                            float rad = entity.RotationDeg * (float)Math.PI / 180.0f;
                            float vx = (float)Math.Sin(rad) * projSpeed;
                            float vy = (float)Math.Cos(rad) * projSpeed;
                            Entity proj = new Entity
                            {
                                Layer = EntityLayer.Foreground,
                                Type = EntityType.Cannon,
                                Mass = 0.1f,
                                Scale = 2,
                                X = entity.X,
                                Y = entity.Y,
                                VX = vx + entity.VX,
                                VY = vy + entity.VY,
                                TimeToLive = 5.0f,
                                RotationDeg = entity.RotationDeg
                            };
                            scene.AddEntity(proj);
                            entity.QueuedEvent = EntityEvent.None;
                        }
                        break;

                    case EntityEvent.FireMissile:
                        {
                            float projSpeed = 10.0f;
                            float rad = entity.RotationDeg * (float)Math.PI / 180.0f;
                            float vx = (float)Math.Sin(rad) * projSpeed;
                            float vy = (float)Math.Cos(rad) * projSpeed;
                            Entity proj = new Entity
                            {
                                Layer = EntityLayer.Foreground,
                                Type = EntityType.Missile,
                                Mass = 0.2f,
                                Scale = 3,
                                X = entity.X,
                                Y = entity.Y,
                                VX = vx + entity.VX,
                                VY = vy + entity.VY,
                                TimeToLive = 10.0f,
                                RotationDeg = entity.RotationDeg
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
