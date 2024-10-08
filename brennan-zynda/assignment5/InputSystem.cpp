#include "InputSystem.h"
#include "Game.h"
#include "EventListener.h"
#include "ExitEvent.h"
#include "ClickEvent.h"
#include "StartEvent.h"

InputSystem::InputSystem(System * system)
	:Trackable("InputSystem")
{
	mpSystem = system;
}

bool InputSystem::checkKeyPress(System::KeyCode key)
{
	if (mpSystem->getKeyState(key))
	{
		return true;
	}
	return false;
}

void InputSystem::checkAllInputs()
{
	// User Input
	// Exit Function
	if (mpSystem->getKeyState(System::KEY_CODE_ESC))
	{
		if (!mLastFrameEsc)
		{
			ExitEvent test;
			EventSystem::getInstance()->fireEvent(test);
			mLastFrameEsc = !mLastFrameEsc;
		}
		
	}
	else if (!mpSystem->getKeyState(System::KEY_CODE_ESC))
	{
		if (mLastFrameEsc)
		{
			mLastFrameEsc = !mLastFrameEsc;
		}

	}
	// Switch Animation
	/*else if (mpSystem->getKeyState(System::KEY_CODE_ENTER))
	{
		//std::cout << "Enter" << std::endl;
		Game::getInstance()->switchLastAnimation();
	}
	else if (mpSystem->getKeyState(System::KEY_CODE_SPACE))
	{
		//std::cout << "Space" << std::endl;
		Game::getInstance()->pauseAnimations();
	}*/
	// Left Click
	if (mpSystem->getMouseButtonState(1))
	{
		if (!mLastFrameClick)
		{
			//std::cout << "MouseClick" << std::endl;
			ClickEvent test(mpSystem->getMouseLocation());
			EventSystem::getInstance()->fireEvent(test);
			mLastFrameClick = !mLastFrameClick;
		}
	}
	else if (!mpSystem->getMouseButtonState(1))
	{
		if (mLastFrameClick)
		{
			mLastFrameClick = !mLastFrameClick;
		}
	}

	// Space Bar
	if (mpSystem->getKeyState(System::KEY_CODE_SPACE))
	{
		if (!mLastFrameSpace)
		{
			StartEvent test;
			EventSystem::getInstance()->fireEvent(test);
			mLastFrameSpace = !mLastFrameSpace;
		}

	}
	else if (!mpSystem->getKeyState(System::KEY_CODE_SPACE))
	{
		if (mLastFrameSpace)
		{
			mLastFrameSpace = !mLastFrameSpace;
		}
	}
	// Right Click
	/*else if (mpSystem->getMouseButtonState(2))
	{
		//std::cout << "MouseClick" << std::endl;
		Game::getInstance()->getUnitManager()->removeUnitsAt(mpSystem->getMouseLocation());
	}*/
}