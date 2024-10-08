#pragma once

#include "Event.h"

class StartEvent :public Event
{
public:
	StartEvent() : Event(EventType(START_EVENT)) {};
	~StartEvent() {};

private:
};