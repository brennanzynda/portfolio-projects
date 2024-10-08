#pragma once

#include <map>
#include <Trackable.h>

class Event;
class EventListener;
enum EventType;

using namespace std;

class EventSystem:public Trackable
{
public:
	static EventSystem* getInstance() { return mspInstance; };
	static void createInstance() { mspInstance = new EventSystem; };
	static void deleteInstance();

	void fireEvent( const Event& theEvent );
	void addListener( EventType type, EventListener* pListener );
	void removeListener( EventType type, EventListener* pListener );
	void removeListenerFromAllEvents( EventListener* pListener );

private:
	EventSystem();
	~EventSystem();

	static EventSystem* mspInstance;

	multimap< EventType, EventListener* > mListenerMap;

	void dispatchAllEvents( const Event& theEvent );

};

