#include "Colour.h"

Colour::Colour()
{
	mColour = al_map_rgba(0, 0, 0, 0);
}

Colour::Colour(int r, int g, int b, int a)
{
	mColour = al_map_rgba(r, g, b, a);
}

Colour::~Colour()
{
}