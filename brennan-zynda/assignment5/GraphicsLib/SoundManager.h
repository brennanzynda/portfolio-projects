#pragma once
#include <string>
#include <allegro5/allegro_acodec.h>
#include <allegro5/allegro_audio.h>
#include <map>
#include "Trackable.h"

class SoundManager : public Trackable
{
public:
	SoundManager();
	~SoundManager();
	void initManager();
	void cleanupManager();

	void playSound(const std::string& key, float volume, bool shouldLoop = false);
	void stopSounds();
	void addSound(const std::string& key, const std::string& filename);
private:
	std::map<std::string, ALLEGRO_SAMPLE*> mSoundMap;
};