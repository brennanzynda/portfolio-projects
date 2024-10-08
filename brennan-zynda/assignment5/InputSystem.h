#pragma once
#include "System.h"

class InputSystem : public Trackable
{
public:
	InputSystem(System * system);
	bool checkKeyPress(System::KeyCode key);
	void checkAllInputs();
private:
	System * mpSystem;
	bool mLastFrameClick = false;
	bool mLastFrameEsc = false;
	bool mLastFrameSpace = false;
	bool mPausedAnims = false;
};