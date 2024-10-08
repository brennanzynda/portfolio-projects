#pragma once
#include "Unit.h"
#include "AnimPrototype.h"

class UnitManager : public Trackable
{
public:
	UnitManager(Animation starting, Animation alternate);
	~UnitManager();

	void addUnit(Unit * unitToAdd);
	void removeUnit(int index);

	void switchLastUnitAnim(Animation anim);

	void switchUnitsAt(Vector2D location);

	void removeUnitsAt(Vector2D deleteLocation);

	void clearManager();

	void updateUnits(double dt);
	void drawUnits();
	void pauseUnits(bool isPaused);
private:
	std::vector<Unit*> mUnitVector;
	Animation mFirstAnim;
	Animation mSecondAnim;
};