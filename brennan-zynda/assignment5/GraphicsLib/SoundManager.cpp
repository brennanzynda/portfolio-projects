#include "SoundManager.h"
#include <iostream>

SoundManager::SoundManager()
{
}

SoundManager::~SoundManager()
{
	stopSounds();
	cleanupManager();
}

void SoundManager::initManager()
{
	if (!al_install_audio())
	{
		std::cout << "error - Audio Add-on not initted\n";
	}
	if (!al_init_acodec_addon())
	{
		std::cout << "error - Audio Codec Add-on not initted\n";
	}
	if (!al_reserve_samples(5))
	{
		std::cout << "error - samples not reserved\n";
	}
}

void SoundManager::cleanupManager()
{
	for (auto iter = mSoundMap.begin(); iter != mSoundMap.end(); iter++)
	{
		al_destroy_sample(iter->second);
	}
	mSoundMap.clear();
}

void SoundManager::playSound(const std::string& key, float volume, bool shouldLoop)
{
	auto iter = mSoundMap.find(key);
	if (iter != mSoundMap.end())
	{
		if (shouldLoop)
		{
			al_play_sample(iter->second, volume, 0.0, 1.0, ALLEGRO_PLAYMODE_LOOP, 0);
		}
		else
		{
			al_play_sample(iter->second, volume, 0.0, 1.0, ALLEGRO_PLAYMODE_ONCE, 0);
		}
	}
}

void SoundManager::stopSounds()
{
	al_stop_samples();
}

void SoundManager::addSound(const std::string& key, const std::string& filename)
{
	ALLEGRO_SAMPLE  * newSample = al_load_sample(filename.c_str());
	mSoundMap.insert_or_assign(key, newSample);
}