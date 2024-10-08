#pragma once
#include "Sprite.h"
#include "AnimPrototype.h"
#include <vector>

class Animation : public Trackable
{
public:
	Animation();
	Animation(const Animation& anim);
	Animation(AnimPrototype * data);
	Animation(std::string name, std::vector<Sprite*> spriteVector, bool shouldLoop);
	Animation(bool shouldLoop);
	~Animation();
	void addSprite(Sprite * spriteToAdd);
	void update(double dt);
	inline Sprite * getCurrentSprite() { return mSpriteVector[mCurrentSprite]; }
	void speedUpAnimation();
	void slowDownAnimation();
	std::string getName() { return mName; }
	inline int getNumSprites() { return mSpriteVector.size(); }
	inline void isAnimated(bool shouldLoop) { mShouldLoop = shouldLoop; }
	inline void pauseAnimations(bool validity) { mPaused = validity; }
	inline float getTimePerFrame() { return mTimePerFrame; }
private:
	float mTimePerFrame;
	float mTimeUntilNextFrame = 0.0f;
	int mCurrentSprite;
	bool mShouldLoop;
	bool mPaused;

	AnimPrototype * mpPrototype;

	std::string mName;
	std::vector<Sprite*> mSpriteVector;
};