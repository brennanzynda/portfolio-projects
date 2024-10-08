#include "EventListener.h"
#include "EventSystem.h"

EventListener::EventListener()
	:Trackable("EventListener")
{
}

EventListener::~EventListener()
{
	EventSystem::getInstance()->removeListenerFromAllEvents( this );
}
