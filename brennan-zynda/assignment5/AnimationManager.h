#pragma once
#include "Animation.h"
#include "AnimPrototype.h"
#include "Trackable.h"

class AnimationManager : public Trackable
{
public:
	AnimationManager();
	Animation createAnimation(std::string key);
	~AnimationManager();

	void addPrototype(AnimPrototype * data, std::string key);
	void cleanupManager();
private:
	std::map<std::string, AnimPrototype*> mAnimMap;
};