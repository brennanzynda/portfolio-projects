#pragma once
#include <Trackable.h>
#include <allegro5/allegro.h>
#include <string>
#include "Colour.h"

class GraphicsBuffer : public Trackable
{
	friend class GraphicsSystem;
public:
	GraphicsBuffer(const std::string& filename);
	GraphicsBuffer(int width, int height);
	GraphicsBuffer(Colour * colour, int width, int height);
	~GraphicsBuffer();
	inline int getHeight() { return al_get_bitmap_height(mpBitmap); }
	inline int getWidth() { return al_get_bitmap_width(mpBitmap); }
private:
	ALLEGRO_BITMAP * mpBitmap;
	GraphicsBuffer(ALLEGRO_BITMAP * bitmap);
	bool mShouldDelete;

	GraphicsBuffer(GraphicsBuffer&);//invalidate copy
	GraphicsBuffer operator=(GraphicsBuffer&);//invalidate assignment
};