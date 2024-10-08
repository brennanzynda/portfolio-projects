#pragma once

#include "Sprite.h"
#include "Colour.h"
#include "Font.h"
#include "Animation.h"
#include "GraphicsBuffer.h"

#include <Vector2D.h>
#include <allegro5/allegro_color.h>
#include <allegro5/allegro_font.h>
#include <allegro5/allegro_image.h>
#include <allegro5/allegro_primitives.h>
#include <allegro5/allegro_ttf.h>

class GraphicsSystem : public Trackable
{
public:
	GraphicsSystem(int width, int height);
	~GraphicsSystem();
	inline void flipDisplay() { al_flip_display(); }
	inline int getHeight() { return mDisplayHeight; }
	inline int getWidth() { return mDisplayWidth; }
	void initSystem();
	void cleanupSystem();
	GraphicsBuffer getBackBuffer();
	void drawSprite(const Vector2D& drawLocation, Sprite* sprite, float scale = 1.0);
	void drawBuffer(const Vector2D& drawLocation, GraphicsBuffer * buffer, float scale = 1.0);
	void drawSpriteOnBuffer(GraphicsBuffer * target, const Vector2D& drawLocation, Sprite* sprite, float scale = 1.0);
	void writeText(const Vector2D& drawLocation, Font * font, Colour * colourToDraw, std::string text);
	void writeText(GraphicsBuffer * target, const Vector2D& drawLocation, Font * font, Colour * colour, std::string text);
	void saveBuffer(GraphicsBuffer * target, const std::string& filename);
private:
	ALLEGRO_DISPLAY * mpDisplay;
	int mDisplayWidth, mDisplayHeight;
	inline ALLEGRO_BITMAP * getAllegroBackBuffer() { return al_get_backbuffer(mpDisplay); }
};