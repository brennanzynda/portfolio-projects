#pragma once
#include <allegro5/allegro_font.h>
#include <allegro5/allegro_ttf.h>
#include <fstream>
#include <PerformanceTracker.h>

enum Alignment
{
	CENTER,
	LEFT,
	RIGHT
};

class Font : public Trackable
{
public:
	Font();
	Font(std::string filepath, int size);
	Font(std::string filepath, int size, Alignment alignment);
	~Font();
	inline int getFontSize() { return mFontSize; }
	inline ALLEGRO_FONT * getFont() { return mpFont; }
private:
	ALLEGRO_FONT * mpFont;
	int mFontSize;
};