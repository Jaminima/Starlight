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

            // CPU-side collision: missiles with player or active shield
            HandleMissileCollisions(scene);

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

        private void HandleMissileCollisions(Scene scene)
        {
            if (scene.entityHead == 0) return;

            ref var player = ref scene.Entities[0];
            bool shieldActive = player.LastEvent == EntityEvent.Shields && player.EventTime + 5.0f > player.TimeAlive;
            float playerRadius = player.Scale;
            float shieldRadius = player.Scale * 3.0f; // must match renderer circle outline radius multiplier

            for (int i = 1; i < scene.entityHead; i++)
            {
                ref var e = ref scene.Entities[i];
                if (e.Type != EntityType.Missile) continue;

                float dx = e.X - player.X;
                float dy = e.Y - player.Y;
                float dist2 = dx * dx + dy * dy;

                float collisionRadius = playerRadius + e.Scale;
                bool hitPlayer = dist2 <= collisionRadius * collisionRadius;

                bool hitShield = false;
                if (!hitPlayer && shieldActive)
                {
                    float shieldCollision = shieldRadius + e.Scale;
                    hitShield = dist2 <= shieldCollision * shieldCollision;
                }

                if (hitPlayer || hitShield)
                {
                    // Spawn explosion entity
                    var explosion = new Entity
                    {
                        Layer = EntityLayer.Foreground,
                        Type = EntityType.Explosion,
                        Scale = e.Scale * 2.0f,
                        X = e.X,
                        Y = e.Y,
                        TimeToLive = 0.6f,
                        RotationDeg = 0,
                    };
                    scene.AddEntity(explosion);

                    // Remove the missile
                    scene.RemoveEntity(i);
                    i--; // adjust index after removal

                    // If not shielded, kill the player (optional: set die event)
                    if (hitPlayer && !shieldActive)
                    {
                        player.QueuedEvent = EntityEvent.Die;
                    }
                }
            }

            scene.Entities[0] = player;
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
                            entity.EventTime = entity.TimeAlive;
                            entity.LastEvent = entity.QueuedEvent;
                            entity.QueuedEvent = EntityEvent.None;
                        }
                        break;

                    case EntityEvent.FireMissile:
                        {
                            if (entity.EventTime + 2.5f > entity.TimeAlive)
                                break;

                            float projSpeed = 100.0f;
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
                }

                scene.Entities[i] = entity;
            }
        }
    }
}
