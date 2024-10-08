#include "AnimPrototype.h"

AnimPrototype::AnimPrototype()
{
	mCurrentSprite = 0;
}

AnimPrototype::~AnimPrototype()
{
}

AnimPrototype::AnimPrototype(std::string key, std::vector<Sprite*> spriteVector, int timePerFrame)
{
	mSpriteVector = spriteVector;
	mCurrentSprite = 0;
	mKey = key;
	mTimePerFrame = timePerFrame;
}

/*AnimPrototype::AnimPrototype(GraphicsBuffer * buffer, Vector2D location, int numRows, int numColumns, std::string key)
{
	for (int i = 0; i < numRows; i++)
	{
		for (int j = 0; j < numColumns; j++)
		{
			Sprite * temp = new Sprite(buffer, Vector2D(location.getX() + (i * buffer->getWidth()/numRows),location.getY() + (j * buffer->getHeight()/numColumns)), buffer->getWidth() / numRows, buffer->getHeight() / numColumns);
			addSprite(temp);
			delete temp;
			temp = nullptr;
		}
	}
	mCurrentSprite = 0;
	mKey = key;
}*/

void AnimPrototype::addSprite(Sprite * sprite)
{
	mSpriteVector.push_back(sprite);
}