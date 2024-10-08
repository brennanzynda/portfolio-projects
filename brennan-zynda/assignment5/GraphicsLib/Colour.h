#pragma once
#include <allegro5/allegro_color.h>
#include <PerformanceTracker.h>

typedef ALLEGRO_COLOR ALLEGRO_COLOUR;

class Colour : public Trackable
{
public:
	Colour();
	Colour(int r, int g, int b, int a = 255);
	~Colour();
	inline ALLEGRO_COLOUR getColour() { return mColour; }
private:
	ALLEGRO_COLOUR mColour;
};