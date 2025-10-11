#include "pch.h"

enum EntityLayer {
	Layer_Background = 0,
	Layer_Midground = 1,
	Layer_Foreground = 2,
	Layer_UI = 3
};

enum EntityType {
	Type_Player = 0,
	Type_Enemy = 1,
	Type_Cannon = 2,
	Type_Missile = 3,
	Type_Powerup = 4,
	Type_Environment = 5,
	Type_Explosion = 6
};

enum EntityEvent {
	Event_None = 0,
	Event_FireCannon = 1,
	Event_FireMissile = 2,
	Event_Shields = 3,
	Event_Die = 4,
    Event_Explosion = 5
};

#pragma pack(push, 4)
struct Entity {
	EntityLayer layer;
	EntityType type;
	EntityEvent lastEvent;
	EntityEvent queuedEvent;
	float eventTime;
	float x, y; 
	float vx, vy; 
	float mass; 
	float rotation;
	float scale;
	float timeToLive;
	float timeAlive;
};
#pragma pack(pop)