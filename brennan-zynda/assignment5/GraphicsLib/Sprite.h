#pragma once
#include "GraphicsBuffer.h"
#include <Vector2D.h>

class Sprite : public Trackable
{
public:
	Sprite();
	Sprite(GraphicsBuffer * buffer, Vector2D location, int width, int height);
	~Sprite();
	inline Vector2D getLocation() { return mSourceLocation; }
	inline int getHeight() { return mHeight; }
	inline int getWidth() { return mWidth; }
	inline GraphicsBuffer * getBuffer() { return mpBuffer; }
private:
	GraphicsBuffer * mpBuffer;
	Vector2D mSourceLocation;
	int mHeight, mWidth;
};