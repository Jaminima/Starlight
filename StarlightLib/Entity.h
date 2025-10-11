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
	Type_Projectile = 2,
	Type_Powerup = 3,
	Type_Environment = 4
};

enum EntityEvent {
	Event_None = 0,
	Event_FireWeapons = 1,
	Event_Die = 2
};

#pragma pack(push, 4)
struct Entity {
	EntityLayer layer;
	EntityType type;
	EntityEvent queuedEvent;
	float x, y; 
	float vx, vy; 
	float mass; 
	float rotation;
	float scale;
	float timeToLive;
	float timeAlive;
};
#pragma pack(pop)