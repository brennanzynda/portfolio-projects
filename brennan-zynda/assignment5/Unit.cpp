#include "Unit.h"
#include "Game.h"
#include "KillEvent.h"

/*Unit::Unit(Vector2D location, int currentSpeed, Vector2D velocity, bool isAnimated)
{
	mLocation = location;
	mIsAnimated = isAnimated;
	mVelocity = velocity;
	mVelocity.normalize();
	mVelocity *= currentSpeed;
}*/

Unit::Unit(Vector2D location, int currentSpeed, Animation anim, Animation alternate, Vector2D velocity, bool isAnimated)
	:Trackable("Unit")
{
	mLocation = location;
	mMainAnim = anim;
	mAltAnim = alternate;
	mCurrentAnim = anim;
	mIsAnimated = isAnimated;
	mVelocity = velocity;
	mCurrentSprite = 0;
	mTimePerFrame = mCurrentAnim.getTimePerFrame();
	//mCurrentAnim = 0;
	//mVelocity.normalize();
	//mVelocity *= currentSpeed;
}

Unit::~Unit()
{
}

void Unit::update(double dt)
{
	if (mLocation.getX() >= Game::getInstance()->getGraphicsSystem()->getWidth() || mLocation.getX() <= 0)
	{
		KillEvent test(mLocation);
		EventSystem::getInstance()->fireEvent(test);
	}
	else if (mLocation.getY() >= Game::getInstance()->getGraphicsSystem()->getHeight() || mLocation.getY() <= 0)
	{
		KillEvent test(mLocation);
		EventSystem::getInstance()->fireEvent(test);
	}
	//mCurrent->update(dt);
	// Change position based on velocity
	mLocation += mVelocity * (dt / 1000);
	
	//mTimeUntilNextFrame -= (float)dt;

	mCurrentAnim.update(dt);

}

void Unit::setNewLoc(Vector2D location)
{
	mLocation = location;
}

void Unit::draw()
{
	Game::getInstance()->getGraphicsSystem()->drawSprite(mLocation, mCurrentAnim.getCurrentSprite());
}

void Unit::slowDownAnim()
{
	//mAnimation.slowDownAnimation();
}

void Unit::speedUpAnim()
{
	//mAnimation.speedUpAnimation();
}

void Unit::switchAnimationState()
{
	mIsAnimated = !mIsAnimated;
	//mAnimation->isAnimated(mIsAnimated);
}

void Unit::setAnimation(Animation newAnim)
{
	mCurrentAnim = newAnim;
	mTimePerFrame = mCurrentAnim.getTimePerFrame();
}

bool Unit::isPointOnUnit(Vector2D point)
{
	if (point.getX() >= mLocation.getX() && point.getX() <= mLocation.getX() + mMainAnim.getCurrentSprite()->getWidth())
	{
		if (point.getY() >= mLocation.getY() && point.getY() <= mLocation.getY() + mMainAnim.getCurrentSprite()->getHeight())
		{
			return true;
		}
	}
	return false;
}

void Unit::pauseAnimation(bool isPaused)
{
	mIsAnimated = isPaused;
	//mAnimation.isAnimated(!isPaused);
}