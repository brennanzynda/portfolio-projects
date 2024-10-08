#pragma once

#include <Vector2D.h>
#include "GraphicsSystem.h"
#include "Trackable.h"

class HUD : public Trackable
{
public:
	HUD(Font * font, GraphicsSystem * system);
	~HUD();
	void printTextAtLocation(Vector2D location, Colour * colour, const std::string& text);
private:
	Font * mpFont;
	GraphicsSystem * mpSystem;
};
