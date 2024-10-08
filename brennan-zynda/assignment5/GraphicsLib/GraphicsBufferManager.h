#pragma once
#include "GraphicsBuffer.h"

class GraphicsBufferManager : public Trackable
{
public:
	GraphicsBufferManager();
	~GraphicsBufferManager();

	void addGraphicsBuffer(const std::string& key, GraphicsBuffer * bufferToAdd);
	GraphicsBuffer * getGraphicsBuffer(const std::string& key);
	void deleteGraphicsBuffer(const std::string& key);

	void clearManager();
private:
	std::map<std::string, GraphicsBuffer*> mGraphicsBufferMap;
};