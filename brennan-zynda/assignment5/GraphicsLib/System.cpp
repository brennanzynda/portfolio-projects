#include "System.h"

System::System(int dispWidth, int dispHeight)
{
	mDispWidth = dispWidth;
	mDispHeight = dispHeight;
}

System::~System()
{
	cleanupSystem();
}

void System::initSystem()
{
	mpSystem = new GraphicsSystem(mDispWidth,mDispHeight);
	mpSystem->initSystem();
	al_install_mouse();
	al_install_keyboard();
}

void System::cleanupSystem()
{
	delete mpSystem;
}

bool System::getKeyState(KeyCode key)
{
	ALLEGRO_KEYBOARD_STATE keyState;
	al_get_keyboard_state(&keyState);
	if (al_key_down(&keyState, key))
	{
		return true;
	}
	return false;
}

bool System::getMouseButtonState(int button)
{
	ALLEGRO_MOUSE_STATE state;
	al_get_mouse_state(&state);
	if (al_mouse_button_down(&state, button))
	{
		return true;
	}
	return false;
}

Vector2D System::getMouseLocation()
{
	ALLEGRO_MOUSE_STATE state;
	al_get_mouse_state(&state);
	return Vector2D(state.x,state.y);
}