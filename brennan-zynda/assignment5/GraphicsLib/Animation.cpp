#include "Animation.h"

Animation::Animation()
	:Trackable("DefaultAnim")
{
	mShouldLoop = true;
	mCurrentSprite = 0;
	mTimePerFrame = 180;
	mPaused = false;
	mTimeUntilNextFrame = mTimePerFrame;
}

Animation::Animation(const Animation& anim)
	:Trackable("CopyAnim")
{
	mShouldLoop = anim.mShouldLoop;
	mCurrentSprite = 0;
	mSpriteVector = anim.mSpriteVector;
	mTimePerFrame = anim.mTimePerFrame;
	mPaused = anim.mPaused;
	mTimeUntilNextFrame = mTimePerFrame;
	mName = anim.mName;
}

Animation::Animation(AnimPrototype * data)
	:Trackable("PrototypeAnim")
{
	mShouldLoop = true;
	mCurrentSprite = 0;
	mSpriteVector = data->mSpriteVector;
	mTimePerFrame = data->mTimePerFrame;
	mPaused = false;
	mTimeUntilNextFrame = mTimePerFrame;
	mName = data->mKey;
}

Animation::Animation(std::string name, std::vector<Sprite*> spriteVector, bool shouldLoop)
	:Trackable("VectorAnim")
{
	mShouldLoop = shouldLoop;
	mCurrentSprite = 0;
	mTimePerFrame = 180;
	mSpriteVector = spriteVector;
	mPaused = false;
	mName = name;
	mTimeUntilNextFrame = mTimePerFrame;
}

Animation::Animation(bool shouldLoop)
	:Trackable("BoolAnim")
{
	mShouldLoop = shouldLoop;
	mCurrentSprite = 0;
	mTimePerFrame = 180;
	mPaused = false;
	mName = "Default";
	mTimeUntilNextFrame = mTimePerFrame;
}

Animation::~Animation()
{
}

void Animation::addSprite(Sprite * spriteToAdd)
{
	mSpriteVector.push_back(spriteToAdd);
}

void Animation::update(double dt)
{
	/*if (mPaused)
	{
		return;
	}*/
	mTimeUntilNextFrame -= (float)dt;
	if (mTimeUntilNextFrame <= 0.0f)
	{
		mCurrentSprite++;
		mCurrentSprite %= mSpriteVector.size();
		mTimeUntilNextFrame = mTimePerFrame;
	}
}

void Animation::speedUpAnimation()
{
	if (mTimePerFrame > 5)
	{
		mTimePerFrame -= 5;
	}
}

void Animation::slowDownAnimation()
{
	mTimePerFrame += 5;
}