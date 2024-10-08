#include "AnimationManager.h"

AnimationManager::AnimationManager()
	:Trackable("AnimationManager")
{
}

AnimationManager::~AnimationManager()
{
	cleanupManager();
}

Animation AnimationManager::createAnimation(std::string key)
{
	auto iter = mAnimMap.find(key);

	if (iter != mAnimMap.end())
	{
		Animation temp(iter->second);
		return temp;
	}
	return Animation();
}

void AnimationManager::addPrototype(AnimPrototype * data, std::string key)
{
	mAnimMap.insert_or_assign(key, data);
}

void AnimationManager::cleanupManager()
{
	//for (auto iter = mAnimMap.begin(); iter != mAnimMap.end(); iter++)
	//{
	//	delete iter->second;
	//}
	mAnimMap.clear();
}