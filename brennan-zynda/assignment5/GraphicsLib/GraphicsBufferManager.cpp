#include "GraphicsBufferManager.h"

GraphicsBufferManager::GraphicsBufferManager()
{
}

GraphicsBufferManager::~GraphicsBufferManager()
{
	clearManager();
}

void GraphicsBufferManager::addGraphicsBuffer(const std::string& key, GraphicsBuffer * bufferToAdd)
{
	mGraphicsBufferMap.insert_or_assign(key, bufferToAdd);
}

GraphicsBuffer * GraphicsBufferManager::getGraphicsBuffer(const std::string& key)
{
	auto iter = mGraphicsBufferMap.find(key);
	if (iter != mGraphicsBufferMap.end())
	{
		return iter->second;
	}
	return nullptr;
}

void GraphicsBufferManager::deleteGraphicsBuffer(const std::string& key)
{
	auto iter = mGraphicsBufferMap.find(key);
	if (iter != mGraphicsBufferMap.end())
	{
		delete iter->second;
		mGraphicsBufferMap.erase(key);
	}
}

void GraphicsBufferManager::clearManager()
{
	for (auto iter = mGraphicsBufferMap.begin(); iter != mGraphicsBufferMap.end(); iter++)
	{
		delete iter->second;
	}
	mGraphicsBufferMap.clear();
}