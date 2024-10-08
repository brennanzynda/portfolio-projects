#include "ClickEvent.h"

ClickEvent::ClickEvent(const Vector2D& location)
	:Event(CLICK_EVENT), mLocation(location)
{
}

ClickEvent::~ClickEvent()
{
}