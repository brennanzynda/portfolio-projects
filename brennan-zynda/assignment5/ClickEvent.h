#pragma once

#include <Vector2D.h>
#include "Event.h"

class ClickEvent :public Event
{
public:
	ClickEvent(const Vector2D& location);
	~ClickEvent();

	Vector2D getLocation() const { return mLocation; }

private:
	Vector2D mLocation;
};
