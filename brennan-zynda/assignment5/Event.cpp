#include "Event.h"

Event::Event(EventType type)
:mType(type)
{
}

Event::~Event()
{
}

const string& Event::getEventName() const
{
	if( mType > INVALID_EVENT_TYPE && mType < NUM_EVENT_TYPES )
	{
		return EVENT_NAMES[mType];
	}
	else
	{
		static const string nullString;
		return nullString;
	}
}
