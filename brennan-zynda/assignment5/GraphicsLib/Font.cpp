#include "Font.h"

Font::Font()
{
	mpFont = al_create_builtin_font();
}

Font::Font(std::string filepath, int size)
{
	mFontSize = size;
	mpFont = al_load_ttf_font(filepath.c_str(), mFontSize, ALLEGRO_ALIGN_CENTER);
}

Font::Font(std::string filepath, int size, Alignment alignment)
{
	mFontSize = size;
	if (alignment == CENTER)
	{
		mpFont = al_load_ttf_font(filepath.c_str(), mFontSize, ALLEGRO_ALIGN_CENTER);
	}
	else if (alignment == LEFT)
	{
		mpFont = al_load_ttf_font(filepath.c_str(), mFontSize, ALLEGRO_ALIGN_LEFT);
	}
	else if (alignment == RIGHT)
	{
		mpFont = al_load_ttf_font(filepath.c_str(), mFontSize, ALLEGRO_ALIGN_RIGHT);
	}
}

Font::~Font()
{
	al_destroy_font(mpFont);
}