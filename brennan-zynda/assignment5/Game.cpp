#include "Game.h"
#include "ClickEvent.h"
#include "KillEvent.h"
#include "ExitEvent.h"
#include <string>
#include <fstream>
#include <math.h>
#include <random>
#include <time.h>

Game * Game::mspInstance = nullptr;

Game::Game(int dispWidth, int dispHeight)
{
	mDispWidth = dispWidth;
	mDispHeight = dispHeight;
	mCurrentAnim = 0;
	mShouldSpawn = true;
	mCurrentGameLevel = 1;
}

Game::~Game()
{
	cleanupGame();
}

void Game::initInstance(int dispWidth, int dispHeight)
{
	if (mspInstance == nullptr)
	{
		mspInstance = new Game(dispWidth,dispHeight);
	}
}

void Game::cleanupInstance()
{
	if (mspInstance != nullptr)
	{
		delete mspInstance;
	}
	mspInstance = nullptr;
}

void Game::initGame(const string& dataFile)
{
	readFile(dataFile);

	srand((unsigned int) time);

	mpSystem = new System(mDispWidth, mDispHeight);
	mpSystem->initSystem();

	mpInputSystem = new InputSystem(mpSystem);

	mpGraphicsBufferManager = new GraphicsBufferManager();

	mpAnimManager = new AnimationManager();

	mpSoundManager = new SoundManager();
	mpSoundManager->initManager();
	mpSoundManager->addSound("BGM", mMUSIC_FILENAME);
	mpSoundManager->addSound("PointGain", mPOINT_GAIN_FILENAME);
	mpSoundManager->addSound("PointLoss", mPOINT_LOSS_FILENAME);

	mpBackgroundColour = new Colour(0, 0, 0);
	mpFont = new Font(mASSET_PATH + mFONT_FILENAME, mFONT_SIZE);

	mpTextColour = new Colour(255, 255, 255);

	mpGraphicsBufferManager->addGraphicsBuffer("BlackBuffer", new GraphicsBuffer(mpBackgroundColour,mDispWidth,mDispHeight));
	mpGraphicsBufferManager->addGraphicsBuffer("OrbBuffer", new GraphicsBuffer(mASSET_PATH + mORB_FILENAME));

	mOrbDimensions = Vector2D(mpGraphicsBufferManager->getGraphicsBuffer("OrbBuffer")->getWidth() / (mNUM_ORBS_IN_ROW*4), mpGraphicsBufferManager->getGraphicsBuffer("OrbBuffer")->getHeight() / (mNUM_ORBS_IN_COLUMN*4));

	// Make Pink Orbs
	int pinkOrbXLocation = mpGraphicsBufferManager->getGraphicsBuffer("OrbBuffer")->getWidth() / 4;

	int pinkOrbYLocation = mpGraphicsBufferManager->getGraphicsBuffer("OrbBuffer")->getHeight() / 2;
	for (int i = 0; i < mNUM_ORBS_IN_COLUMN; i++)
	{
		for (int j = 0; j < mNUM_ORBS_IN_ROW; j++)
		{
			mPinkOrbSprites.push_back(new Sprite(mpGraphicsBufferManager->getGraphicsBuffer("OrbBuffer"), Vector2D(mOrbDimensions.getX() * j, mOrbDimensions.getY() * i)+Vector2D(pinkOrbXLocation, pinkOrbYLocation), mOrbDimensions.getX(), mOrbDimensions.getY()));
		}
	}
	mpPinkOrbs = new AnimPrototype("Pink", mPinkOrbSprites, mFRAME_RATE * 10);

	// Make Blue Orbs
	int blueOrbXLocation = 3 * mpGraphicsBufferManager->getGraphicsBuffer("OrbBuffer")->getWidth() / 4;
	
	int blueOrbYLocation = mpGraphicsBufferManager->getGraphicsBuffer("OrbBuffer")->getHeight() / 2;
	for (int i = 0; i < mNUM_ORBS_IN_COLUMN; i++)
	{
		for (int j = 0; j < mNUM_ORBS_IN_ROW; j++)
		{
			mBlueOrbSprites.push_back(new Sprite(mpGraphicsBufferManager->getGraphicsBuffer("OrbBuffer"), Vector2D(mOrbDimensions.getX() * j, mOrbDimensions.getY() * i) + Vector2D(blueOrbXLocation, blueOrbYLocation), mOrbDimensions.getX(), mOrbDimensions.getY()));
		}
	}
	mpBlueOrbs = new AnimPrototype("Blue", mBlueOrbSprites, mFRAME_RATE * 10);

	
	//mpUnitManager->addUnit(new Unit(getRandomVector(mMinXSpawn, mMaxXSpawn, mMinYSpawn, mMaxYSpawn), mCurrentGameLevel, mBlueOrbAnim, mPinkOrbAnim, getRandomVector(mMinVelocity, mMaxVelocity, mMinVelocity, mMaxVelocity)));
	//mpUnitManager->addUnit(new Unit(getRandomVector(mMinXSpawn, mMaxXSpawn, mMinYSpawn, mMaxYSpawn), mCurrentGameLevel, mPinkOrbAnim, mBlueOrbAnim, getRandomVector(mMinVelocity, mMaxVelocity, mMinVelocity, mMaxVelocity)));

	mOrbOffset = Vector2D(mOrbDimensions.getX() / 2, mOrbDimensions.getY() / 2);

	mpAnimManager->addPrototype(mpBlueOrbs, "Blue");
	mpAnimManager->addPrototype(mpPinkOrbs, "Pink");
	mpUnitManager = new UnitManager(mpAnimManager->createAnimation("Blue"), mpAnimManager->createAnimation("Pink"));
	//EventSystem::createInstance();

	mpHUD = new HUD(mpFont, mpSystem->getGraphicsSystem());
}

void Game::cleanupGame()
{
	EventSystem::getInstance()->removeListener(EventType(CLICK_EVENT), this);
	EventSystem::getInstance()->removeListener(EventType(KILL_EVENT), this);
	EventSystem::getInstance()->removeListener(EventType(EXIT_EVENT), this);
	EventSystem::getInstance()->removeListener(EventType(START_EVENT), this);
	//EventSystem::deleteInstance();
	int numSprites = mPinkOrbSprites.size();
	for (int i = 0; i < numSprites; i++)
	{
		delete mPinkOrbSprites[i];
		delete mBlueOrbSprites[i];
	}
	delete mpHUD;
	delete mpPinkOrbs;
	delete mpBlueOrbs;
	delete mpBackgroundColour;
	delete mpTextColour;
	delete mpFont;
	delete mpAnimManager;
	delete mpUnitManager;
	delete mpSoundManager;
	delete mpGraphicsBufferManager;
	delete mpInputSystem;
	delete mpSystem;
}

void Game::doLoop()
{
	Game::getInstance()->getGraphicsSystem()->drawBuffer(Vector2D(0, 0), mpGraphicsBufferManager->getGraphicsBuffer("BlackBuffer"));

	EventSystem::getInstance()->addListener(EventType(CLICK_EVENT), this);
	EventSystem::getInstance()->addListener(EventType(KILL_EVENT), this);
	EventSystem::getInstance()->addListener(EventType(EXIT_EVENT), this);
	EventSystem::getInstance()->addListener(EventType(START_EVENT), this);

	PerformanceTracker performanceTracker;
	performanceTracker.startTracking("loop");
	Timer timer;
	mCurrentPoints = mStartingPoints;
	mpSoundManager->playSound("BGM", 1.0, true);
	float lastFrameTime = 0.0;
	float totalTime = 0.0;
	while (mShouldContinue)
	{	
		performanceTracker.clearTracker("loop");
		performanceTracker.startTracking("loop");
		timer.start();
		mpInputSystem->checkAllInputs();
		if (mIsStarted)
		{
			// Update
			mpUnitManager->updateUnits(mFRAME_RATE);
			Game::getInstance()->getGraphicsSystem()->drawBuffer(Vector2D(0, 0), mpGraphicsBufferManager->getGraphicsBuffer("BlackBuffer"));
			mpHUD->printTextAtLocation(Vector2D(((mDispWidth / 8) * 7) + 2, 0), mpTextColour, std::to_string(std::roundf(1000/lastFrameTime)));
			mpUnitManager->drawUnits();
			mpHUD->printTextAtLocation(Vector2D(0, 0), mpTextColour, "Score: " + std::to_string(mCurrentPoints));
			if (shouldCreateNewUnit())
			{
				int random = rand() % 2;
				if (random == 0)
				{
					createNewUnit(mpAnimManager->createAnimation("Blue"), mpAnimManager->createAnimation("Pink"));
				}
				else
				{
					createNewUnit(mpAnimManager->createAnimation("Pink"), mpAnimManager->createAnimation("Blue"));
				}
			}

			if (mCurrentPoints <= 0)
			{
				endGame();
			}			
		}
		else
		{
			Game::getInstance()->getGraphicsSystem()->drawBuffer(Vector2D(0, 0), mpGraphicsBufferManager->getGraphicsBuffer("BlackBuffer"));
			
			mpHUD->printTextAtLocation(Vector2D(mDispWidth/4, mDispHeight/2), mpTextColour, "Press Space To Start");
		}

		Game::getInstance()->getGraphicsSystem()->flipDisplay();
		timer.sleepUntilElapsed(mFRAME_RATE);
		
		performanceTracker.stopTracking("loop");
		std::cout << performanceTracker.getElapsedTime("loop") << std::endl;
		lastFrameTime = performanceTracker.getElapsedTime("loop");
	}
}

void Game::createNewUnit(Animation anim, Animation alt)
{
	mpUnitManager->addUnit(new Unit(getRandomVector(mMinXSpawn, mMaxXSpawn, mMinYSpawn, mMaxYSpawn), mCurrentGameLevel, anim, alt, getRandomVector(mMinVelocity, mMaxVelocity, mMinVelocity, mMaxVelocity)));
	//mCurrentAnim = 0;
}

void Game::pauseAnimations()
{
	//if (mPausedAnims)
	//{
	//	mpUnitManager->pauseUnits(false);
	//}
	//else
	//{
	//	mpUnitManager->pauseUnits(true);
	//}
	//mPausedAnims = !mPausedAnims;
}

Vector2D Game::getRandomVector(int minX, int maxX, int minY, int maxY)
{
	int randX = rand() % ((maxX - minX) + 1) + minX;
	int randY = rand() % ((maxY - minY) + 1) + minY;
	return Vector2D(randX, randY);
}

void Game::readFile(const string& fileName)
{
	ifstream inputFile(fileName);
	inputFile >> mMinVelocity;
	inputFile >> mMaxVelocity;
	inputFile >> mPointGain;
	inputFile >> mPointLoss;
	inputFile >> mStartingPoints;
	inputFile >> mMinXSpawn;
	inputFile >> mMinYSpawn;
	inputFile >> mMaxXSpawn;
	inputFile >> mMaxYSpawn;
	inputFile >> mUnitSpawnChance;
}

void Game::removePoints()
{
	mpSoundManager->playSound("PointLoss", 1.0);
	mCurrentPoints -= mPointLoss;
}

void Game::addPoints()
{
	mpSoundManager->playSound("PointGain", 1.0);
	mCurrentPoints += mPointGain;
}

void Game::endGame()
{
	mpHUD->printTextAtLocation(Vector2D(mDispWidth / 8, mDispHeight / 2), mpTextColour, "Game Over - Press Escape to Exit");
	mShouldSpawn = false;
	mpUnitManager->clearManager();
}

void Game::handleEvent(const Event &theEvent)
{
	//cout << endl << theEvent.getEventName() << " Received";

	if (theEvent.getType() == CLICK_EVENT)
	{
		const ClickEvent& clickEvent = static_cast<const ClickEvent&>(theEvent);
		Game::getInstance()->getUnitManager()->switchUnitsAt(clickEvent.getLocation());
		//cout << "Click" << endl;
	}
	else if (theEvent.getType() == KILL_EVENT)
	{
		const KillEvent& killEvent = static_cast<const KillEvent&>(theEvent);
		Game::getInstance()->getUnitManager()->removeUnitsAt(killEvent.getLocation());
		//cout << "Kill" << endl;
	}
	else if (theEvent.getType() == EXIT_EVENT)
	{
		const ExitEvent& exitEvent = static_cast<const ExitEvent&>(theEvent);
		Game::getInstance()->continueLoop(false);
	}
	else if (theEvent.getType() == START_EVENT)
	{
		mIsStarted = true;
	}
}

bool Game::shouldCreateNewUnit()
{
	if (mShouldSpawn)
	{
		int random = rand() % ((100 - mUnitSpawnChance) + 1) + mUnitSpawnChance;
		if (random > 100 - mUnitSpawnChance)
		{
			//cout << "true";
			return true;

		}
	}
	//cout << "false";
	return false;
}