#pragma once
#include "GraphicsSystem.h"

class System : public Trackable
{
public:
	enum KeyCode
	{
		KEY_CODE_F = ALLEGRO_KEY_F,
		KEY_CODE_S = ALLEGRO_KEY_S,
		KEY_CODE_ESC = ALLEGRO_KEY_ESCAPE,
		KEY_CODE_ENTER = ALLEGRO_KEY_ENTER,
		KEY_CODE_SPACE = ALLEGRO_KEY_SPACE
	};
	System(int dispWidth, int dispHeight);
	~System();
	void initSystem();
	void cleanupSystem();
	bool getKeyState(KeyCode key);
	bool getMouseButtonState(int button);
	Vector2D getMouseLocation();
	inline GraphicsSystem * getGraphicsSystem() { return mpSystem; }
private:
	GraphicsSystem * mpSystem;
	int mDispWidth, mDispHeight;
};