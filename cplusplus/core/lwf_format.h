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

#ifndef LWF_FORMAT_H
#define	LWF_FORMAT_H

#include "lwf_type.h"

namespace LWF {

struct Data;

namespace Format {

enum Constant
{
	HEADER_SIZE = 324,

	FORMAT_VERSION_0 = 0x13,
	FORMAT_VERSION_1 = 0x12,
	FORMAT_VERSION_2 = 0x11,

	FORMAT_VERSION_COMPAT_0 = 0x12,
	FORMAT_VERSION_COMPAT_1 = 0x10,
	FORMAT_VERSION_COMPAT_2 = 0x10,

	FORMAT_TYPE = 0,

	OPTION_USE_SCRIPT = (1 << 0),
	OPTION_USE_TEXTUREATLAS = (1 << 1),
	OPTION_COMPRESSED = (1 << 2),

	MATRIX_FLAG = (1 << 31),
	MATRIX_FLAG_MASK = MATRIX_FLAG,
	COLORTRANSFORM_FLAG = (1 << 31),

	TEXTUREFORMAT_NORMAL = 0,
	TEXTUREFORMAT_PREMULTIPLIEDALPHA = 1,

	BLEND_MODE_NORMAL = 0,
	BLEND_MODE_ADD = 1,
	BLEND_MODE_LAYER = 2,
	BLEND_MODE_ERASE = 3,
	BLEND_MODE_MASK = 4,
};

struct StringBase
{
	int stringId;
};

struct TextureBase
{
	int stringId;
	int format;
	int width;
	int height;
	float scale;
};

struct Texture : public TextureBase
{
	string filename;

	Texture() {}
	Texture(const TextureBase &base)
	{
		stringId = base.stringId;
		format = base.format;
		width = base.width;
		height = base.height;
		scale = base.scale;
	}
    virtual ~Texture() {}

	void SetFilename(const Data *data);
	virtual const string &GetFilename(const Data *data) const;
};

struct TextureReplacement : public Texture
{
	TextureReplacement(string fname, Constant fmt, int w, int h, float s)
	{
		filename = fname;
		format = (int)fmt;
		width = w;
		height = h;
		scale = s;
	}

	const string &GetFilename(const Data *data) const
	{
		return filename;
	}
};

struct TextureFragmentBase
{
	int stringId;
	int textureId;
	int rotated;
	int x;
	int y;
	int u;
	int v;
	int w;
	int h;
};

struct TextureFragment : public TextureFragmentBase
{
	string filename;

	TextureFragment() {}
	TextureFragment(const TextureFragmentBase &base)
	{
		stringId = base.stringId;
		textureId = base.textureId;
		rotated = base.rotated;
		x = base.x;
		y = base.y;
		u = base.u;
		v = base.v;
		w = base.w;
		h = base.h;
	}
    virtual ~TextureFragment() {}

	void SetFilename(const Data *data);
	virtual const string &GetFilename(const Data *data) const;
};

struct TextureFragmentReplacement : public TextureFragment
{
	TextureFragmentReplacement(string fname, int texId, int rot,
		int tx, int ty, int tu, int tv, int tw, int th)
	{
		filename = fname;
		textureId = texId;
		rotated = rot;
		x = tx;
		y = ty;
		u = tu;
		v = tv;
		w = tw;
		h = th;
	}

	const string &GetFilename(const Data *data) const
	{
		return filename;
	}
};

struct Bitmap
{
	int matrixId;
	int textureFragmentId;
};

struct BitmapEx
{
	enum Attribute
	{
		REPEAT_S = (1 << 0),
		REPEAT_T = (1 << 1)
	};

	int matrixId;
	int textureFragmentId;
	int attribute;
	float u;
	float v;
	float w;
	float h;
};

struct Font
{
	int stringId;
	float letterspacing;
};

struct TextProperty
{
	enum Align
	{
		LEFT = 0,
		RIGHT = 1,
		CENTER = 2,
		ALIGN_MASK = 0x3,
		VERTICAL_BOTTOM = (1 << 2),
		VERTICAL_MIDDLE = (2 << 2),
		VERTICAL_MASK = 0xc,
	};

	int maxLength;
	int fontId;
	int fontHeight;
	int align;
	int leftMargin;
	int rightMargin;
	float letterSpacing;
	int leading;
	int strokeColorId;
	int strokeWidth;
	int shadowColorId;
	int shadowOffsetX;
	int shadowOffsetY;
	int shadowBlur;
};

struct Text
{
	int matrixId;
	int nameStringId;
	int textPropertyId;
	int stringId;
	int colorId;
	int width;
	int height;
};

struct ParticleData
{
	int stringId;
};

struct Particle
{
	int matrixId;
	int colorTransformId;
	int particleDataId;
};

struct ProgramObject : public StringBase
{
	int width;
	int height;
	int matrixId;
	int colorTransformId;
};

struct GraphicObject
{
	enum Type
	{
		BITMAP,
		BITMAPEX,
		TEXT,
		GRAPHIC_OBJECT_MAX
	};

	int graphicObjectType;
	int graphicObjectId;
};

struct Graphic
{
	int graphicObjectId;
	int graphicObjects;
};

struct Object
{
	enum Type
	{
		BUTTON,
		GRAPHIC,
		MOVIE,
		BITMAP,
		BITMAPEX,
		TEXT,
		PARTICLE,
		PROGRAMOBJECT,
		ATTACHEDMOVIE,
		OBJECT_MAX
	};

	int objectType;
	int objectId;
};

struct Animation
{
	int animationOffset;
	int animationLength;
};

struct ButtonCondition
{
	enum Condition
	{
		ROLLOVER       = (1 << 0),
		ROLLOUT	       = (1 << 1),
		PRESS          = (1 << 2),
		RELEASE        = (1 << 3),
		DRAGOUT        = (1 << 4),
		DRAGOVER       = (1 << 5),
		RELEASEOUTSIDE = (1 << 6),
		KEYPRESS       = (1 << 7)
	};

	int condition;
	int keyCode;
	int animationId;
};

struct Button
{
	int width;
	int height;
	int matrixId;
	int colorTransformId;
	int conditionId;
	int conditions;
};

struct Label : public StringBase
{
	int frameNo;
};

struct InstanceName : public StringBase
{
};

struct Event : public StringBase
{
};

struct String
{
	int stringOffset;
	int stringLength;
};

struct PlaceCompat
{
	int depth;
	int objectId;
	int instanceId;
	int matrixId;
};

struct Place
{
	int depth;
	int objectId;
	int instanceId;
	int matrixId;
	int blendMode;
};

struct ControlMoveM
{
	int placeId;
	int matrixId;
};

struct ControlMoveC
{
	int placeId;
	int colorTransformId;
};

struct ControlMoveMC
{
	int placeId;
	int matrixId;
	int colorTransformId;
};

struct Control
{
	enum Type
	{
		MOVE,
		MOVEM,
		MOVEC,
		MOVEMC,
		ANIMATION,
		CONTROL_MAX
	};

	int controlType;
	int controlId;
};

struct Frame
{
	int controlOffset;
	int controls;
};

struct MovieClipEvent
{
	enum ClipEvent
	{
		LOAD	   = (1 << 0),
		UNLOAD	   = (1 << 1),
		ENTERFRAME = (1 << 2),
	};

	int clipEvent;
	int animationId;
};

struct Movie
{
	int depths;
	int labelOffset;
	int labels;
	int frameOffset;
	int frames;
	int clipEventId;
	int clipEvents;
};

struct MovieLinkage : public StringBase
{
	int movieId;
};

struct ItemArray
{
	int offset;
	int length;
};

struct Header
{
	unsigned char id0;
	unsigned char id1;
	unsigned char id2;
	unsigned char id3;
	unsigned char formatVersion0;
	unsigned char formatVersion1;
	unsigned char formatVersion2;
	unsigned char option;
	int width;
	int height;
	int frameRate;
	int rootMovieId;
	int nameStringId;
	int backgroundColor;
	ItemArray stringBytes;
	ItemArray animationBytes;
	ItemArray translate;
	ItemArray matrix;
	ItemArray color;
	ItemArray alphaTransform;
	ItemArray colorTransform;
	ItemArray objectData;
	ItemArray texture;
	ItemArray textureFragment;
	ItemArray bitmap;
	ItemArray bitmapEx;
	ItemArray font;
	ItemArray textProperty;
	ItemArray text;
	ItemArray particleData;
	ItemArray particle;
	ItemArray programObject;
	ItemArray graphicObject;
	ItemArray graphic;
	ItemArray animation;
	ItemArray buttonCondition;
	ItemArray button;
	ItemArray label;
	ItemArray instanceName;
	ItemArray eventData;
	ItemArray place;
	ItemArray controlMoveM;
	ItemArray controlMoveC;
	ItemArray controlMoveMC;
	ItemArray control;
	ItemArray frame;
	ItemArray movieClipEvent;
	ItemArray movie;
	ItemArray movieLinkage;
	ItemArray stringData;
	int lwfLength;
};

}	// namespace Format
}	// namespace LWF

#endif
