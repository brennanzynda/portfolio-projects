#include "UnitManager.h"
#include "Game.h"

UnitManager::UnitManager(Animation starting, Animation alternate)
	:Trackable("UnitManager")
{
	mFirstAnim = starting;
	mSecondAnim = alternate;
}

UnitManager::~UnitManager()
{
	clearManager();
}

void UnitManager::removeUnitsAt(Vector2D deleteLocation)
{
	for (unsigned int i = 0; i < mUnitVector.size(); i++)
	{
		if (mUnitVector[i]->isPointOnUnit(deleteLocation))
		{
			removeUnit(i);
			break;
		}
	}
}

void UnitManager::updateUnits(double dt)
{
	for (unsigned int i = 0; i < mUnitVector.size(); i++)
	{
		mUnitVector[i]->update(dt);
	}
}

void UnitManager::drawUnits()
{
	for (unsigned int i = 0; i < mUnitVector.size(); i++)
	{
		mUnitVector[i]->draw();
	}
}

void UnitManager::clearManager()
{
	for (unsigned int i = 0; i < mUnitVector.size(); i++)
	{
		delete mUnitVector[i];
	}
	mUnitVector.clear();
}

void UnitManager::addUnit(Unit * unitToAdd)
{
	mUnitVector.push_back(unitToAdd);
}

/*Unit * UnitManager::getUnit(const std::string& key)
{
	auto iter = mUnitMap.find(key);
	if (iter != mUnitMap.end())
	{
		return iter->second;
	}
	return nullptr;
}*/

void UnitManager::removeUnit(int index)
{
	if (mUnitVector[index]->getAnimation() == "Pink")
	{
		Game::getInstance()->removePoints();
	}
	else
	{
		Game::getInstance()->addPoints();
	}
	//delete mUnitVector[index];
	mUnitVector.erase(mUnitVector.begin() + index);
}

void UnitManager::pauseUnits(bool isPaused)
{
	for (unsigned int i = 0; i < mUnitVector.size(); i++)
	{
		mUnitVector[i]->pauseAnimation(isPaused);
	}
}

void UnitManager::switchLastUnitAnim(Animation anim)
{
	if (mUnitVector.size() != 0)
	{
		mUnitVector[mUnitVector.size() - 1]->setAnimation(anim);
	}
}

void UnitManager::switchUnitsAt(Vector2D location)
{
	//std::cout << "Switch" << std::endl;
	for (unsigned int i = 0; i < mUnitVector.size(); i++)
	{
		if (mUnitVector[i]->isPointOnUnit(location))
		{
			if (mUnitVector[i]->getAnimation() == mFirstAnim.getName())
			{
				mUnitVector[i]->setAnimation(mSecondAnim);
			}
			else
			{
				mUnitVector[i]->setAnimation(mFirstAnim);
			}
		}
	}
}