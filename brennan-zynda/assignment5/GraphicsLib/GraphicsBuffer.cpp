#include "GraphicsBuffer.h"
#include <cassert>

using namespace std;

GraphicsBuffer::GraphicsBuffer(const std::string& filename)
	:Trackable("GraphicsBufferString")
{
	mpBitmap = al_load_bitmap(filename.c_str());
	assert(mpBitmap);
	mShouldDelete = true;
}

GraphicsBuffer::GraphicsBuffer(int width, int height)
	:Trackable("GraphicsBufferInts")
{
	mpBitmap = al_create_bitmap(width, height);
	assert(mpBitmap);
	mShouldDelete = true;
}

GraphicsBuffer::GraphicsBuffer(Colour * colour, int width, int height)
	:Trackable("GraphicsBufferColour")
{
	ALLEGRO_BITMAP * current = al_get_target_bitmap();
	mpBitmap = al_create_bitmap(width, height);
	al_set_target_bitmap(mpBitmap);
	al_clear_to_color(colour->getColour());
	al_set_target_bitmap(current);
	mShouldDelete = true;
}

GraphicsBuffer::GraphicsBuffer(ALLEGRO_BITMAP * pBitmap)
	:Trackable("GraphicsBufferBuffer")
{
	mpBitmap = pBitmap;
	mShouldDelete = false;
}

GraphicsBuffer::~GraphicsBuffer()
{
	if (mShouldDelete)
	{
		al_destroy_bitmap(mpBitmap);
	}
}