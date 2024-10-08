#pragma once

#include "Event.h"
#include "Vector2D.h"

class KillEvent : public Event
{
public:
	KillEvent(Vector2D killLocation);
	~KillEvent();

	Vector2D getLocation() const {	return mLocation;	};
private:
	Vector2D mLocation;
};