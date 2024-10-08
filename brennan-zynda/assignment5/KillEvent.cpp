#include "KillEvent.h"

KillEvent::KillEvent(Vector2D killLocation) : Event(KILL_EVENT), mLocation(killLocation)
{
}

KillEvent::~KillEvent()
{
}