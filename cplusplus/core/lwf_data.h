/*
 * Copyright (C) 2013 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

#ifndef LWF_DATA_H
#define	LWF_DATA_H

#include "lwf_format.h"

namespace LWF {

struct Data {
	Format::Header header;
	vector<Translate> translates;
	vector<Matrix> matrices;
	vector<Color> colors;
	vector<AlphaTransform> alphaTransforms;
	vector<ColorTransform> colorTransforms;
	vector<Format::Object> objects;
	vector<Format::Texture> textures;
	vector<Format::TextureFragment> textureFragments;
	vector<Format::Bitmap> bitmaps;
	vector<Format::BitmapEx> bitmapExs;
	vector<Format::Font> fonts;
	vector<Format::TextProperty> textProperties;
	vector<Format::Text> texts;
	vector<Format::ParticleData> particleDatas;
	vector<Format::Particle> particles;
	vector<Format::ProgramObject> programObjects;
	vector<Format::GraphicObject> graphicObjects;
	vector<Format::Graphic> graphics;
	vector<vector<int> > animations;
	vector<Format::ButtonCondition> buttonConditions;
	vector<Format::Button> buttons;
	vector<Format::Label> labels;
	vector<Format::InstanceName> instanceNames;
	vector<Format::Event> events;
	vector<Format::Place> places;
	vector<Format::ControlMoveM> controlMoveMs;
	vector<Format::ControlMoveC> controlMoveCs;
	vector<Format::ControlMoveMC> controlMoveMCs;
	vector<Format::Control> controls;
	vector<Format::Frame> frames;
	vector<Format::MovieClipEvent> movieClipEvents;
	vector<Format::Movie> movies;
	vector<Format::MovieLinkage> movieLinkages;
	vector<string> strings;

	map<string, int> stringMap;
	map<int, int> instanceNameMap;
	map<int, int> eventMap;
	map<int, int> movieLinkageMap;
	map<int, int> movieLinkageNameMap;
	map<int, int> programObjectMap;
	vector<map<int, int> > labelMap;

	map<string, bool> resourceCache;

	string name;
	bool useScript;
	bool useTextureAtlas;
	bool valid;

	Data();
	Data(const void *bytes, size_t length);
	bool Check();
	bool ReplaceTexture(int index,
		const Format::TextureReplacement &textureReplacement);
	bool ReplaceTextureFragment(int index,
		const Format::TextureFragmentReplacement &textureFragmentReplacement);

private:
	void Load(const void *bytes, size_t length);
};

}	// namespace LWF

#endif
