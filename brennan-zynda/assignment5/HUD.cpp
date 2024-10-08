#include "HUD.h"

HUD::HUD(Font * font, GraphicsSystem * system)
	:Trackable("HUD")
{
	mpFont = font;
	mpSystem = system;
}

HUD::~HUD()
{
}

void HUD::printTextAtLocation(Vector2D location, Colour * colour, const std::string& text)
{
	mpSystem->writeText(location, mpFont, colour, text);
}