#pragma once
#include "System.h"
#include "UnitManager.h"
#include "InputSystem.h"
#include "GraphicsBufferManager.h"
#include "SoundManager.h"
#include "EventSystem.h"
#include "HUD.h"
#include "AnimationManager.h"
#include "EventListener.h"
#include <GraphicsSystem.h>

class Game : public EventListener
{
public:
	Game(int dispWidth, int dispHeight);
	~Game();

	inline static Game * getInstance() { assert(mspInstance != nullptr); return mspInstance; }
	static void initInstance(int dispWidth, int dispHeight);
	static void cleanupInstance();

	void initGame(const string& dataFile);
	void cleanupGame();
	void doLoop();
	void createNewUnit(Animation anim, Animation alt);
	void pauseAnimations();
	void handleEvent(const Event &theEvent);
	void removePoints();
	void addPoints();

	inline void continueLoop(bool shouldContinue) { mShouldContinue = shouldContinue; }
	inline UnitManager * getUnitManager() { return mpUnitManager; }
	//inline std::vector<Sprite*> getSmurfAnimSprites() { return mSmurfSprites; }
	inline GraphicsSystem * getGraphicsSystem() { return mpSystem->getGraphicsSystem(); }
private:
	static Game * mspInstance;

	bool shouldCreateNewUnit();
	void readFile(const string& fileName);
	void endGame();

	System * mpSystem;
	InputSystem * mpInputSystem;

	AnimationManager * mpAnimManager;
	AnimPrototype * mpPinkOrbs;
	AnimPrototype * mpBlueOrbs;
	GraphicsBufferManager * mpGraphicsBufferManager;
	SoundManager * mpSoundManager;
	UnitManager * mpUnitManager;

	bool mShouldContinue = true;
	bool mIsStarted = false;
	int mDispWidth, mDispHeight;

	std::vector<Sprite*> mPinkOrbSprites;
	std::vector<Sprite*> mBlueOrbSprites;

	Vector2D mDrawLocation;
	Vector2D mOrbDimensions;
	Vector2D mOrbOffset;	
	Vector2D getRandomVector(int minX, int maxX, int minY, int maxY);

	Colour * mpBackgroundColour;
	Colour * mpTextColour;
	HUD * mpHUD;
	Font * mpFont;

	bool mShouldSpawn;
	int mCurrentAnim;

	int mCurrentGameLevel;

	int mMinVelocity, mMaxVelocity;
	int mPointGain, mPointLoss;
	int mStartingPoints, mCurrentPoints;
	int mMinXSpawn, mMaxXSpawn;
	int mMinYSpawn, mMaxYSpawn;
	int mUnitSpawnChance;

	const std::string mASSET_PATH = "..\\..\\shared\\assets\\";

	// Orbs
	const std::string mORB_FILENAME = "glowing-balls.png";

	// Font
	const std::string mFONT_FILENAME = "cour.ttf";
	const int mFONT_SIZE = 32;

	// Music and sounds
	const std::string mMUSIC_FILENAME = "music2.ogg";
	const std::string mPOINT_LOSS_FILENAME = "pointloss.ogg";
	const std::string mPOINT_GAIN_FILENAME = "pointgain.ogg";

	const int mNUM_ORBS = 24;
	const int mNUM_ORBS_IN_ROW = 3;
	const int mNUM_ORBS_IN_COLUMN = 2;
	const float mFRAME_RATE = 16.7;
};