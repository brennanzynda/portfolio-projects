#pragma once

#include "Event.h"

class ExitEvent :public Event
{
public:
	ExitEvent() : Event(EventType(EXIT_EVENT)) {};
	~ExitEvent() {};

private:
};