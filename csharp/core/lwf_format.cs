/*
 * Copyright (C) 2012 GREE, Inc.
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

namespace LWF {
namespace Format {

public enum Constant
{
	HEADER_SIZE = 324,

	FORMAT_VERSION_0 = 0x12,
	FORMAT_VERSION_1 = 0x10,
	FORMAT_VERSION_2 = 0x10,

	FORMAT_TYPE = 0,

	OPTION_USE_SCRIPT = (1 << 0),
	OPTION_USE_TEXTUREATLAS = (1 << 1),

	MATRIX_FLAG = (1 << 31),
	MATRIX_FLAG_MASK = MATRIX_FLAG,
	COLORTRANSFORM_FLAG = (1 << 31),

	TEXTUREFORMAT_NORMAL = 0,
	TEXTUREFORMAT_PREMULTIPLIEDALPHA = 1,
}

public class StringBase
{
	public int stringId;
}

public partial class Texture
{
	public int stringId;
	public int format;
	public int width;
	public int height;
	public float scale;
	public string filename;
}

public class TextureReplacement : Texture
{
	public TextureReplacement(string fname, Constant fmt, int w, int h, float s)
	{
		filename = fname;
		format = (int)fmt;
		width = w;
		height = h;
		scale = s;
	}
}

public partial class TextureFragment
{
	public int stringId;
	public int textureId;
	public int rotated;
	public int x;
	public int y;
	public int u;
	public int v;
	public int w;
	public int h;
	public string filename;
}

public class TextureFragmentReplacement : TextureFragment
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
}

public partial class Bitmap
{
	public int matrixId;
	public int textureFragmentId;
}

public partial class BitmapEx
{
	public enum Attribute
	{
		REPEAT_S = (1 << 0),
		REPEAT_T = (1 << 1)
	}

	public int matrixId;
	public int textureFragmentId;
	public int attribute;
	public float u;
	public float v;
	public float w;
	public float h;
}

public partial class Font
{
	public int stringId;
	public float letterspacing;
}

public partial class TextProperty
{
	public enum Align
	{
		LEFT = 0,
		RIGHT = 1,
		CENTER = 2,
		ALIGN_MASK = 0x3,
		VERTICAL_BOTTOM = (1 << 2),
		VERTICAL_MIDDLE = (2 << 2),
		VERTICAL_MASK = 0xc,
	}

	public int maxLength;
	public int fontId;
	public int fontHeight;
	public int align;
	public int leftMargin;
	public int rightMargin;
	public float letterSpacing;
	public int leading;
	public int strokeColorId;
	public int strokeWidth;
	public int shadowColorId;
	public int shadowOffsetX;
	public int shadowOffsetY;
	public int shadowBlur;
}

public partial class Text
{
	public int matrixId;
	public int nameStringId;
	public int textPropertyId;
	public int stringId;
	public int colorId;
	public int width;
	public int height;
}

public partial class ParticleData
{
	public int stringId;
}

public partial class Particle
{
	public int matrixId;
	public int colorTransformId;
	public int particleDataId;
}

public partial class ProgramObject : StringBase
{
	public int width;
	public int height;
	public int matrixId;
	public int colorTransformId;
}

public partial class GraphicObject
{
	public enum Type
	{
		BITMAP,
		BITMAPEX,
		TEXT,
		GRAPHIC_OBJECT_MAX
	}

	public int graphicObjectType;
	public int graphicObjectId;
}

public partial class Graphic
{
	public int graphicObjectId;
	public int graphicObjects;
}

public partial class Object
{
	public enum Type
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
	}

	public int objectType;
	public int objectId;
}

public partial class Animation
{
	public int animationOffset;
	public int animationLength;
}

public partial class ButtonCondition
{
	public enum Condition
	{
		ROLLOVER       = (1 << 0),
		ROLLOUT	       = (1 << 1),
		PRESS          = (1 << 2),
		RELEASE        = (1 << 3),
		DRAGOUT        = (1 << 4),
		DRAGOVER       = (1 << 5),
		RELEASEOUTSIDE = (1 << 6),
		KEYPRESS       = (1 << 7)
	}

	public int condition;
	public int keyCode;
	public int animationId;
}

public partial class Button
{
	public int width;
	public int height;
	public int matrixId;
	public int colorTransformId;
	public int conditionId;
	public int conditions;
}

public partial class Label : StringBase
{
	public int frameNo;
}

public partial class InstanceName : StringBase
{
}

public partial class Event : StringBase
{
}

public partial class String
{
	public int stringOffset;
	public int stringLength;
}

public partial class Place
{
	public int depth;
	public int objectId;
	public int instanceId;
	public int matrixId;
}

public partial class ControlMoveM
{
	public int placeId;
	public int matrixId;
}

public partial class ControlMoveC
{
	public int placeId;
	public int colorTransformId;
}

public partial class ControlMoveMC
{
	public int placeId;
	public int matrixId;
	public int colorTransformId;
}

public partial class Control
{
	public enum Type
	{
		MOVE,
		MOVEM,
		MOVEC,
		MOVEMC,
		ANIMATION,
		CONTROL_MAX
	}

	public int controlType;
	public int controlId;
}

public partial class Frame
{
	public int controlOffset;
	public int controls;
}

public partial class MovieClipEvent
{
	public enum ClipEvent
	{
		LOAD	   = (1 << 0),
		UNLOAD	   = (1 << 1),
		ENTERFRAME = (1 << 2),
	}

	public int clipEvent;
	public int animationId;
}

public partial class Movie
{
	public int depths;
	public int labelOffset;
	public int labels;
	public int frameOffset;
	public int frames;
	public int clipEventId;
	public int clipEvents;
}

public partial class MovieLinkage : StringBase
{
	public int movieId;
}

public partial class ItemArray
{
	public int offset;
	public int length;
}

public partial class Header
{
	public byte id0;
	public byte id1;
	public byte id2;
	public byte id3;
	public byte formatVersion0;
	public byte formatVersion1;
	public byte formatVersion2;
	public byte option;
	public int width;
	public int height;
	public int frameRate;
	public int rootMovieId;
	public int nameStringId;
	public int backgroundColor;
	public ItemArray stringBytes;
	public ItemArray animationBytes;
	public ItemArray translate;
	public ItemArray matrix;
	public ItemArray color;
	public ItemArray alphaTransform;
	public ItemArray colorTransform;
	public ItemArray objectData;
	public ItemArray texture;
	public ItemArray textureFragment;
	public ItemArray bitmap;
	public ItemArray bitmapEx;
	public ItemArray font;
	public ItemArray textProperty;
	public ItemArray text;
	public ItemArray particleData;
	public ItemArray particle;
	public ItemArray programObject;
	public ItemArray graphicObject;
	public ItemArray graphic;
	public ItemArray animation;
	public ItemArray buttonCondition;
	public ItemArray button;
	public ItemArray label;
	public ItemArray instanceName;
	public ItemArray eventData;
	public ItemArray place;
	public ItemArray controlMoveM;
	public ItemArray controlMoveC;
	public ItemArray controlMoveMC;
	public ItemArray control;
	public ItemArray frame;
	public ItemArray movieClipEvent;
	public ItemArray movie;
	public ItemArray movieLinkage;
	public ItemArray stringData;
	public int lwfLength;
}

}	// namespace Format
}	// namespace LWF
