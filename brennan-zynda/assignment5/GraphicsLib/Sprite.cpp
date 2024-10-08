#include "Sprite.h"

Sprite::Sprite()
{
}

Sprite::Sprite(GraphicsBuffer * buffer, Vector2D location, int width, int height)
	:Trackable("Sprite")
{
	mpBuffer = buffer;
	mSourceLocation = location;
	mWidth = width;
	mHeight = height;
}

Sprite::~Sprite()
{
}