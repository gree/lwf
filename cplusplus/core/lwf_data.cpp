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

#include "lwf_data.h"
#include "lwf_animation.h"

#ifndef LWF_USE_LZMA
# define LWF_USE_LZMA 1
#endif
#if LWF_USE_LZMA
# include "LzmaDec.h"
# include "Alloc.h"
#endif

#define	P(n)	advanceP(p, n)
#define	READ(container, what, T)							\
	do {													\
		container.reserve(header.what.length);				\
		for (int i = 0; i < header.what.length; ++i)	{	\
			const T *tp = (const T *)P(sizeof(T));			\
			if (tp >= (const T *)end)						\
				break;										\
			container.push_back(*tp);						\
		}													\
	} while (0)

namespace LWF {

static const char *advanceP(const char *&p, size_t n)
{
	const char *pp = p;
	p += n;
	return pp;
}

static int readInt32(const char *&pp)
{
	const unsigned char *p = (const unsigned char *)pp;
	int v = (p[3] << 24) | (p[2] << 16) | (p[1] <<  8) | (p[0] <<  0);
	pp += 4;
	return v;
}

static vector<int> readAnimation(const char *p, size_t offset, size_t length)
{
	vector<int> array;
	const char *end = p + offset + length;
	p += offset;

	for (;;) {
		if (p >= end)
			return array;

		char code = *p++;
		array.push_back(code);

		switch (code) {
		case Animation::PLAY:
		case Animation::STOP:
		case Animation::NEXTFRAME:
		case Animation::PREVFRAME:
			break;

		case Animation::GOTOFRAME:
		case Animation::GOTOLABEL:
		case Animation::EVENT:
		case Animation::CALL:
			array.push_back(readInt32(p));
			break;

		case Animation::SETTARGET:
			{
				int count = readInt32(p);
				array.push_back(count);
				for (int i = 0; i < count; ++i) {
					int target = readInt32(p);
					array.push_back(target);
				}
			}
			break;

		case Animation::END:
			return array;
		}
	}
}

Data::Data()
	: valid(false)
{
	static unsigned char data[] = {
		0x4c, 0x57, 0x46, 0x00, 0x12, 0x10, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x3c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x01, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0x00, 0x44, 0x01, 0x00, 0x00,
		0x0c, 0x00, 0x00, 0x00, 0x50, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x50, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x58, 0x01, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x58, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x58, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x5c, 0x01, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x5c, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
		0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x01, 0x00, 0x00,
		0x01, 0x00, 0x00, 0x00, 0x68, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x68, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x68, 0x01, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x68, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x68, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x68, 0x01, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x68, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
		0x70, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x01, 0x00, 0x00,
		0x01, 0x00, 0x00, 0x00, 0x8c, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
		0x94, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0xa4, 0x01, 0x00, 0x00,
		0x5f, 0x72, 0x6f, 0x6f, 0x74, 0x00, 0x6e, 0x75, 0x6c, 0x6c, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3f,
		0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x05, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
	};

	Load(data, sizeof(data));
}

Data::Data(const void *bytes, size_t length)
	: valid(false)
{
	Load(bytes, length);
}

#if LWF_USE_LZMA
static void *SzAlloc(void *p, size_t size) {p = p; return MyAlloc(size);}
static void SzFree(void *p, void *address) {p = p; MyFree(address);}
static ISzAlloc g_Alloc = {SzAlloc, SzFree};

static bool Decompress(
	const char *p, size_t length, vector<char> &decompressed)
{
	SizeT destLen = decompressed.size();
	const Byte *src = (const Byte *)p;
	SizeT srcLen = length;
	ELzmaStatus status = LZMA_STATUS_NOT_SPECIFIED;

	uint64_t uncompressed_size = 0;
	for (size_t i = 0; i < 8; ++i)
		uncompressed_size |= (uint64_t)((unsigned char)p[5 + i]) << (i * 8);
	if (uncompressed_size != (uint64_t)decompressed.size())
		return false;

	SRes res = LzmaDecode((Byte *)&decompressed[0], &destLen,
		src + 13, &srcLen, src, 5, LZMA_FINISH_END, &status, &g_Alloc);

	return res == SZ_OK && (status == LZMA_STATUS_FINISHED_WITH_MARK ||
		status == LZMA_STATUS_MAYBE_FINISHED_WITHOUT_MARK);
}
#endif

void Data::Load(const void *bytes, size_t length)
{
	if (length < Format::HEADER_SIZE)
		return;

	const char *p = (const char *)bytes;
	const char *end = p + length;
	header = *(const Format::Header *)P(sizeof(Format::Header));
	if (header.id0 != 'L' ||
			header.id1 != 'W' ||
			header.id2 != 'F' ||
			header.id3 != Format::FORMAT_TYPE ||
			header.formatVersion0 != Format::FORMAT_VERSION_0 ||
			header.formatVersion1 != Format::FORMAT_VERSION_1 ||
			header.formatVersion2 != Format::FORMAT_VERSION_2)
		return;

#if LWF_USE_LZMA
	vector<char> decompressed;
#endif

	if ((header.option & Format::OPTION_COMPRESSED) != 0) {
		if (header.lwfLength <= Format::HEADER_SIZE)
			return;
#if LWF_USE_LZMA
		decompressed.resize(header.lwfLength - Format::HEADER_SIZE);
		if (Decompress(p, length - Format::HEADER_SIZE, decompressed)) {
			p = &decompressed[0];
			end = p + decompressed.size();
		} else {
			return;
		}
#else
		return;
#endif
	} else {
		if (length < header.lwfLength)
			return;
	}

	const char *stringByteData = P(header.stringBytes.length);
	const char *animationByteData = P(header.animationBytes.length);
	vector<Format::String> stringData;
	vector<Format::Animation> animationData;

	READ(translates, translate, Translate);
	READ(matrices, matrix, Matrix);
	READ(colors, color, Color);
	READ(alphaTransforms, alphaTransform, AlphaTransform);
	READ(colorTransforms, colorTransform, ColorTransform);
	READ(objects, objectData, Format::Object);
	READ(textures, texture, Format::TextureBase);
	READ(textureFragments, textureFragment, Format::TextureFragmentBase);
	READ(bitmaps, bitmap, Format::Bitmap);
	READ(bitmapExs, bitmapEx, Format::BitmapEx);
	READ(fonts, font, Format::Font);
	READ(textProperties, textProperty, Format::TextProperty);
	READ(texts, text, Format::Text);
	READ(particleDatas, particleData, Format::ParticleData);
	READ(particles, particle, Format::Particle);
	READ(programObjects, programObject, Format::ProgramObject);
	READ(graphicObjects, graphicObject, Format::GraphicObject);
	READ(graphics, graphic, Format::Graphic);
	READ(animationData, animation, Format::Animation);
	READ(buttonConditions, buttonCondition, Format::ButtonCondition);
	READ(buttons, button, Format::Button);
	READ(labels, label, Format::Label);
	READ(instanceNames, instanceName, Format::InstanceName);
	READ(events, eventData, Format::Event);
	READ(places, place, Format::Place);
	READ(controlMoveMs, controlMoveM, Format::ControlMoveM);
	READ(controlMoveCs, controlMoveC, Format::ControlMoveC);
	READ(controlMoveMCs, controlMoveMC, Format::ControlMoveMC);
	READ(controls, control, Format::Control);
	READ(frames, frame, Format::Frame);
	READ(movieClipEvents, movieClipEvent, Format::MovieClipEvent);
	READ(movies, movie, Format::Movie);
	READ(movieLinkages, movieLinkage, Format::MovieLinkage);
	READ(stringData, stringData, Format::String);

	if (p != end) {
		memset(&header, 0, sizeof(header));
		return;
	}

	strings.reserve(stringData.size());
	vector<Format::String>::const_iterator
		dit(stringData.begin()), ditend(stringData.end());
	for (; dit != ditend; ++dit) {
		string str(stringByteData + dit->stringOffset, dit->stringLength);
		strings.push_back(str);
	}

	animations.reserve(animationData.size());
	vector<Format::Animation>::const_iterator
		ait(animationData.begin()), aitend(animationData.end());
	for (; ait != aitend; ++ait)
		animations.push_back(readAnimation(
			animationByteData, ait->animationOffset, ait->animationLength));

	int i = 0;
	vector<string>::const_iterator sit(strings.begin()), sitend(strings.end());
	for (; sit != sitend; ++sit)
		stringMap[*sit] = i++;

	i = 0;
	vector<Format::InstanceName>::const_iterator
		nit(instanceNames.begin()), nitend(instanceNames.end());
	for (; nit != nitend; ++nit)
		instanceNameMap[nit->stringId] = i++;

	i = 0;
	vector<Format::Event>::const_iterator
		eit(events.begin()), eitend(events.end());
	for (; eit != eitend; ++eit)
		eventMap[eit->stringId] = i++;

	i = 0;
	vector<Format::MovieLinkage>::const_iterator
		lit(movieLinkages.begin()), litend(movieLinkages.end());
	for (; lit != litend; ++lit) {
		movieLinkageMap[lit->stringId] = i++;
		movieLinkageNameMap[lit->movieId] = lit->stringId;
	}

	i = 0;
	vector<Format::ProgramObject>::const_iterator
		oit(programObjects.begin()), oitend(programObjects.end());
	for (; oit != oitend; ++oit)
		programObjectMap[oit->stringId] = i++;

	i = 0;
	labelMap.resize(movies.size());
	vector<Format::Movie>::const_iterator
		mit(movies.begin()), mitend(movies.end());
	for (; mit != mitend; ++mit) {
		int o = mit->labelOffset;
		for (int j = 0; j < mit->labels; ++j) {
			Format::Label l = labels[o + j];
			labelMap[i][l.stringId] = l.frameNo;
		}
		++i;
	}

	vector<Format::Texture>::iterator
		tit(textures.begin()), titend(textures.end());
	for (; tit != titend; ++tit)
		tit->SetFilename(this);
	vector<Format::TextureFragment>::iterator
		fit(textureFragments.begin()), fitend(textureFragments.end());
	for (; fit != fitend; ++fit)
		fit->SetFilename(this);

	name = strings[header.nameStringId];
	useScript = (header.option & Format::OPTION_USE_SCRIPT) != 0;
	useTextureAtlas = (header.option & Format::OPTION_USE_TEXTUREATLAS) != 0;

	valid = true;
}

bool Data::Check()
{
	return valid;
}

bool Data::ReplaceTexture(int index,
	const Format::TextureReplacement &textureReplacement)
{
	if (index < 0 || index >= (int)textures.size())
		return false;

	textures[index] = textureReplacement;
	return true;
}

bool Data::ReplaceTextureFragment(int index,
	const Format::TextureFragmentReplacement &textureFragmentReplacement)
{
	if (index < 0 || index >= (int)textureFragments.size())
		return false;

	textureFragments[index] = textureFragmentReplacement;
	return true;
}

}	// namespace LWF
