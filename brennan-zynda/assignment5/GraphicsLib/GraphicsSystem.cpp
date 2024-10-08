#include "GraphicsSystem.h"
using namespace std;
GraphicsSystem::GraphicsSystem(int width, int height)
	:Trackable("GraphicsSystemInts")
{
	mDisplayWidth = width;
	mDisplayHeight = height;
}

GraphicsSystem::~GraphicsSystem()
{
	cleanupSystem();
}

void GraphicsSystem::initSystem()
{
	if (!al_init())
	{
		cout << "error initting Allegro\n";
	}
	if (!al_init_image_addon())
	{
		cout << "error - Image Add-on not initted\n";
	}
	if (!al_init_font_addon())
	{
		cout << "error - Font Add-on not initted\n";
	}
	if (!al_init_ttf_addon())
	{
		cout << "error - TTF Add-on not initted\n";
	}
	if (!al_init_primitives_addon())
	{
		cout << "error - primitives Add-on not initted\n";
	}
	mpDisplay = al_create_display(mDisplayWidth, mDisplayHeight);
}

void GraphicsSystem::cleanupSystem()
{
	al_destroy_display(mpDisplay);
}

void GraphicsSystem::drawSprite(const Vector2D& drawLocation, Sprite* sprite, float scale)
{
	al_draw_scaled_bitmap(sprite->getBuffer()->mpBitmap, sprite->getLocation().getX(), sprite->getLocation().getY(), sprite->getWidth(), sprite->getHeight(), drawLocation.getX(), drawLocation.getY(), sprite->getWidth() * scale, sprite->getHeight() * scale, 0);
}

void GraphicsSystem::drawBuffer(const Vector2D& drawLocation, GraphicsBuffer * buffer, float scale)
{
	al_draw_scaled_bitmap(buffer->mpBitmap, 0, 0, buffer->getWidth(), buffer->getHeight(), drawLocation.getX(), drawLocation.getY(), buffer->getWidth() * scale, buffer->getHeight() * scale, 0);
}

void GraphicsSystem::drawSpriteOnBuffer(GraphicsBuffer * target, const Vector2D& drawLocation, Sprite* sprite, float scale)
{
	ALLEGRO_BITMAP * current = al_get_target_bitmap();
	al_set_target_bitmap(target->mpBitmap);
	drawSprite(drawLocation, sprite, scale);
	al_set_target_bitmap(current);
}

void GraphicsSystem::writeText(const Vector2D& drawLocation, Font * font, Colour * colour, string text)
{
	al_draw_text(font->getFont(), colour->getColour(), drawLocation.getX(), drawLocation.getY(), 0, text.c_str());
}

void GraphicsSystem::writeText(GraphicsBuffer * target, const Vector2D& drawLocation, Font * font, Colour * colour, string text)
{
	ALLEGRO_BITMAP * current = al_get_target_bitmap();
	al_set_target_bitmap(target->mpBitmap);
	writeText(drawLocation, font, colour, text);
	al_set_target_bitmap(current);
	//delete current;
}

void GraphicsSystem::saveBuffer(GraphicsBuffer * target, const string& filename)
{
	al_save_bitmap(filename.c_str(), target->mpBitmap);
}

GraphicsBuffer GraphicsSystem::getBackBuffer()
{
	return GraphicsBuffer(getAllegroBackBuffer());
}