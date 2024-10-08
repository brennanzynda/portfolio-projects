#pragma once
#include "Vector2D.h"
#include "System.h"

class Unit : public Trackable
{
public:
	//Unit(Vector2D location, int currentSpeed, Vector2D velocity = Vector2D(0, 0), bool isAnimated = true);
	Unit(Vector2D location, int currentSpeed, Animation anim, Animation alternate, Vector2D velocity = Vector2D(0, 0), bool isAnimated = true);
	~Unit();

	void update(double dt);
	void setNewLoc(Vector2D location);
	void draw();

	void slowDownAnim();
	void speedUpAnim();

	void switchAnimationState();
	std::string getAnimation() { return mCurrentAnim.getName(); }
	void setAnimation(Animation newAnim);
	bool isPointOnUnit(Vector2D point);
	void pauseAnimation(bool isPaused);
private:
	Vector2D mLocation, mVelocity;
	Animation mMainAnim, mAltAnim, mCurrentAnim;
	int mCurrentSprite, mTimeUntilNextFrame, mTimePerFrame;
	bool mIsAnimated;
};