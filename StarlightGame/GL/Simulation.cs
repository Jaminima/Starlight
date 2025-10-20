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
                            if (entity.EventTime + 0.2f > entity.TimeAlive)
                                break;

                            float rad;
                            int targetIndex = Weapons.ResolveTargetIndex(scene, entity);
                            if (targetIndex >= 0 && targetIndex < scene.entityHead)
                            {
                                var target = scene.Entities[targetIndex];
                                rad = Weapons.ComputeDirectAimAngle(entity, target);
                            }
                            else
                            {
                                rad = entity.RotationDeg * (float)Math.PI / 180.0f;
                            }

                            float projSpeed = 400.0f;
                            float vx = (float)Math.Sin(rad) * projSpeed;
                            float vy = (float)Math.Cos(rad) * projSpeed;
                            float offset = entity.Scale * 1.0f;
                            float offsetX = (float)Math.Sin(rad) * offset;
                            float offsetY = (float)Math.Cos(rad) * offset;
                            Entity proj = new Entity
                            {
                                Layer = EntityLayer.Foreground,
                                Type = EntityType.Cannon,
                                Mass = 0.1f,
                                Scale = 2,
                                X = entity.X + offsetX,
                                Y = entity.Y + offsetY,
                                VX = vx + entity.VX,
                                VY = vy + entity.VY,
                                TimeToLive = 5.0f,
                                RotationDeg = rad * 180.0f / (float)Math.PI,
                                TargetIndex = targetIndex,
                            };
                            scene.AddEntity(proj);
                            entity.EventTime = entity.TimeAlive;
                            entity.LastEvent = entity.QueuedEvent;
                            entity.QueuedEvent = EntityEvent.None;
                        }
                        break;

                    case EntityEvent.FireMissile:
                        {
                            if (entity.EventTime + 0.5f > entity.TimeAlive)
                                break;

                            float rad;
                            int targetIndex = Weapons.ResolveTargetIndex(scene, entity);
                            float projSpeed = 100.0f;
                            if (targetIndex >= 0 && targetIndex < scene.entityHead)
                            {
                                var target = scene.Entities[targetIndex];
                                rad = Weapons.ComputeLeadAimAngle(entity, target, projSpeed);
                            }
                            else
                            {
                                // No valid target; fire forward
                                rad = entity.RotationDeg * (float)Math.PI / 180.0f;
                            }

                            float vx = (float)Math.Sin(rad) * projSpeed;
                            float vy = (float)Math.Cos(rad) * projSpeed;
                            float offset = entity.Scale * 1.8f;
                            float offsetX = (float)Math.Sin(rad) * offset;
                            float offsetY = (float)Math.Cos(rad) * offset;
                            Entity proj = new Entity
                            {
                                Layer = EntityLayer.Foreground,
                                Type = EntityType.Missile,
                                Mass = 0.2f,
                                Scale = 3,
                                X = entity.X + offsetX,
                                Y = entity.Y + offsetY,
                                VX = vx + entity.VX,
                                VY = vy + entity.VY,
                                TimeToLive = 10.0f,
                                RotationDeg = rad * 180.0f / (float)Math.PI,
                                TargetIndex = targetIndex,
                            };
                            scene.AddEntity(proj);
                            entity.EventTime = entity.TimeAlive;
                            entity.LastEvent = entity.QueuedEvent;
                            entity.QueuedEvent = EntityEvent.None;
                        }
                        break;

                    case EntityEvent.Shields:
                        {
                            if (entity.EventTime + 5.0f < entity.TimeAlive)
                                break;

                            entity.EventTime = entity.TimeAlive;
                            entity.LastEvent = entity.QueuedEvent;
                            entity.QueuedEvent = EntityEvent.None;
                        }
                        break;

                    case EntityEvent.Die:
                        {
                            // Spawn explosion for death and remove entity quickly
                            var explosion = new Entity
                            {
                                Layer = EntityLayer.Foreground,
                                Type = EntityType.Explosion,
                                Scale = entity.Scale * 2.0f,
                                X = entity.X,
                                Y = entity.Y,
                                TimeToLive = 0.7f,
                            };
                            scene.AddEntity(explosion);

                            // For now, just clear TTL to remove on next sweep
                            entity.TimeToLive = 0.1f;
                            entity.EventTime = entity.TimeAlive;
                            entity.LastEvent = entity.QueuedEvent;
                            entity.QueuedEvent = EntityEvent.None;
                        }
                        break;

                    case EntityEvent.Explosion:
                        {
                            // Spawn explosion for projectile hit and remove projectile quickly
                            var explosion = new Entity
                            {
                                Layer = EntityLayer.Foreground,
                                Type = EntityType.Explosion,
                                Scale = entity.Scale * 2.0f,
                                X = entity.X,
                                Y = entity.Y,
                                TimeToLive = 0.6f,
                            };
                            scene.AddEntity(explosion);

                            // Remove the projectile quickly
                            entity.TimeToLive = 0.1f;
                            entity.EventTime = entity.TimeAlive;
                            entity.LastEvent = entity.QueuedEvent;
                            entity.QueuedEvent = EntityEvent.None;
                        }
                        break;
                }

                scene.Entities[i] = entity;
            }
        }
    }
}
