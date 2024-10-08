#include <iostream>
#include <cassert>
#include <string>
#include <vector>
#include <random>
#include <time.h>
#include "Game.h"

#include <PerformanceTracker.h>
#include <MemoryTracker.h>

#include "GraphicsSystem.h"

using namespace std;

int main()
{

	const int DISP_WIDTH = 800;
	const int DISP_HEIGHT = 600;

	EventSystem::createInstance();
	Game::initInstance(DISP_WIDTH, DISP_HEIGHT);
	Game::getInstance()->initGame("data2.txt");

	Game::getInstance()->doLoop();

	Game::getInstance()->cleanupInstance();
	EventSystem::deleteInstance();

	MemoryTracker::getInstance()->reportAllocations(cout);
	system("pause");
	return 0;

}