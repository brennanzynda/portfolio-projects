#pragma once
#include "Sprite.h"
#include <vector>

class AnimPrototype
{
public:
	AnimPrototype();
	AnimPrototype(std::string key, std::vector<Sprite*> spriteVector, int timePerFrame);
	//AnimPrototype(GraphicsBuffer * buffer, Vector2D location, int numRows, int numColumns, std::string key, int timePerFrame);
	~AnimPrototype();

	friend class Animation;

	void addSprite(Sprite * sprite);

	int getTimePerFrame() { return mTimePerFrame; }
	int getSpriteCount() { return mSpriteVector.size(); }

	Sprite * getSprite(int index) { return mSpriteVector[index]; }
	inline std::string getName() { return mKey; }
private:
	std::vector<Sprite*> mSpriteVector;

	std::string mKey;
	int mCurrentSprite;
	int mTimePerFrame;
};