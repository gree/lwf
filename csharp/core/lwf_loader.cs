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

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LWF {

public partial class Translate
{
	public Translate(BinaryReader br)
	{
		translateX = br.ReadSingle();
		translateY = br.ReadSingle();
	}
}

public partial class Matrix
{
	public Matrix(BinaryReader br)
	{
		scaleX = br.ReadSingle();
		scaleY = br.ReadSingle();
		skew0 = br.ReadSingle();
		skew1 = br.ReadSingle();
		translateX = br.ReadSingle();
		translateY = br.ReadSingle();
	}
}

public partial class Color
{
	public Color(BinaryReader br)
	{
		red = br.ReadSingle();
		green = br.ReadSingle();
		blue = br.ReadSingle();
		alpha = br.ReadSingle();
	}
}

public partial class AlphaTransform
{
	public AlphaTransform(BinaryReader br)
	{
		alpha = br.ReadSingle();
	}
}

public partial class ColorTransform
{
	public ColorTransform(BinaryReader br)
	{
		multi = new Color(br);
		add = new Color(br);
	}
}

namespace Format {

public partial class Texture
{
	public Texture() {}
	public Texture(BinaryReader br)
	{
		stringId = br.ReadInt32();
		format = br.ReadInt32();
		width = br.ReadInt32();
		height = br.ReadInt32();
		scale = br.ReadSingle();
	}

	public void SetFilename(Data data)
	{
		filename = Regex.Replace(
			data.strings[stringId], "\\.[^\\.]*$", string.Empty);
	}
}

public partial class TextureFragment
{
	public TextureFragment() {}
	public TextureFragment(BinaryReader br, bool withOriginalWH)
	{
		stringId = br.ReadInt32();
		textureId = br.ReadInt32();
		rotated = br.ReadInt32();
		x = br.ReadInt32();
		y = br.ReadInt32();
		u = br.ReadInt32();
		v = br.ReadInt32();
		w = br.ReadInt32();
		h = br.ReadInt32();
		if (withOriginalWH) {
			ow = br.ReadInt32();
			oh = br.ReadInt32();
		} else {
			ow = w;
			oh = h;
		}
	}

	public void SetFilename(Data data)
	{
		filename = Regex.Replace(
			data.strings[stringId], "\\.[^\\.]*$", string.Empty);
	}
}

public partial class Bitmap
{
	public Bitmap() {}
	public Bitmap(BinaryReader br)
	{
		matrixId = br.ReadInt32();
		textureFragmentId = br.ReadInt32();
	}
}

public partial class BitmapEx
{
	public BitmapEx() {}
	public BitmapEx(BinaryReader br)
	{
		matrixId = br.ReadInt32();
		textureFragmentId = br.ReadInt32();
		attribute = br.ReadInt32();
		u = br.ReadSingle();
		v = br.ReadSingle();
		w = br.ReadSingle();
		h = br.ReadSingle();
	}
}

public partial class Font
{
	public Font() {}
	public Font(BinaryReader br)
	{
		stringId = br.ReadInt32();
		letterspacing = br.ReadSingle();
	}
}

public partial class TextProperty
{
	public TextProperty() {}
	public TextProperty(BinaryReader br)
	{
		maxLength = br.ReadInt32();
		fontId = br.ReadInt32();
		fontHeight = br.ReadInt32();
		align = br.ReadInt32();
		leftMargin = br.ReadInt32();
		rightMargin = br.ReadInt32();
		letterSpacing = br.ReadSingle();
		leading = br.ReadInt32();
		strokeColorId = br.ReadInt32();
		strokeWidth = br.ReadInt32();
		shadowColorId = br.ReadInt32();
		shadowOffsetX = br.ReadInt32();
		shadowOffsetY = br.ReadInt32();
		shadowBlur = br.ReadInt32();
	}
}

public partial class Text
{
	public Text() {}
	public Text(BinaryReader br)
	{
		matrixId = br.ReadInt32();
		nameStringId = br.ReadInt32();
		textPropertyId = br.ReadInt32();
		stringId = br.ReadInt32();
		colorId = br.ReadInt32();
		width = br.ReadInt32();
		height = br.ReadInt32();
	}
}

public partial class ParticleData
{
	public ParticleData() {}
	public ParticleData(BinaryReader br)
	{
		stringId = br.ReadInt32();
	}
}

public partial class Particle
{
	public Particle() {}
	public Particle(BinaryReader br)
	{
		matrixId = br.ReadInt32();
		colorTransformId = br.ReadInt32();
		particleDataId = br.ReadInt32();
	}
}

public partial class ProgramObject
{
	public ProgramObject() {}
	public ProgramObject(BinaryReader br)
	{
		stringId = br.ReadInt32();
		width = br.ReadInt32();
		height = br.ReadInt32();
		matrixId = br.ReadInt32();
		colorTransformId = br.ReadInt32();
	}
}

public partial class GraphicObject
{
	public GraphicObject() {}
	public GraphicObject(BinaryReader br)
	{
		graphicObjectType = br.ReadInt32();
		graphicObjectId = br.ReadInt32();
	}
}

public partial class Graphic
{
	public Graphic() {}
	public Graphic(BinaryReader br)
	{
		graphicObjectId = br.ReadInt32();
		graphicObjects = br.ReadInt32();
	}
}

public partial class Object
{
	public Object() {}
	public Object(BinaryReader br)
	{
		objectType = br.ReadInt32();
		objectId = br.ReadInt32();
	}
}

public partial class Animation
{
	public Animation() {}
	public Animation(BinaryReader br)
	{
		animationOffset = br.ReadInt32();
		animationLength = br.ReadInt32();
	}
}

public partial class ButtonCondition
{
	public ButtonCondition() {}
	public ButtonCondition(BinaryReader br)
	{
		condition = br.ReadInt32();
		keyCode = br.ReadInt32();
		animationId = br.ReadInt32();
	}
}

public partial class Button
{
	public Button() {}
	public Button(BinaryReader br)
	{
		width = br.ReadInt32();
		height = br.ReadInt32();
		matrixId = br.ReadInt32();
		colorTransformId = br.ReadInt32();
		conditionId = br.ReadInt32();
		conditions = br.ReadInt32();
	}
}

public partial class Label
{
	public Label() {}
	public Label(BinaryReader br)
	{
		stringId = br.ReadInt32();
		frameNo = br.ReadInt32();
	}
}

public partial class InstanceName
{
	public InstanceName() {}
	public InstanceName(BinaryReader br)
	{
		stringId = br.ReadInt32();
	}
}

public partial class Event
{
	public Event() {}
	public Event(BinaryReader br)
	{
		stringId = br.ReadInt32();
	}
}

public partial class String
{
	public String() {}
	public String(BinaryReader br)
	{
		stringOffset = br.ReadInt32();
		stringLength = br.ReadInt32();
	}
}

public partial class Place
{
	public Place() {}
	public Place(BinaryReader br)
	{
		depth = br.ReadInt32();
		objectId = br.ReadInt32();
		instanceId = br.ReadInt32();
		matrixId = br.ReadInt32();
		blendMode = depth >> 24;
		depth &= 0xffffff;
	}
}

public partial class ControlMoveM
{
	public ControlMoveM() {}
	public ControlMoveM(BinaryReader br)
	{
		placeId = br.ReadInt32();
		matrixId = br.ReadInt32();
	}
}

public partial class ControlMoveC
{
	public ControlMoveC() {}
	public ControlMoveC(BinaryReader br)
	{
		placeId = br.ReadInt32();
		colorTransformId = br.ReadInt32();
	}
}

public partial class ControlMoveMC
{
	public ControlMoveMC() {}
	public ControlMoveMC(BinaryReader br)
	{
		placeId = br.ReadInt32();
		matrixId = br.ReadInt32();
		colorTransformId = br.ReadInt32();
	}
}

public partial class ControlMoveMCB
{
	public ControlMoveMCB() {}
	public ControlMoveMCB(BinaryReader br)
	{
		placeId = br.ReadInt32();
		matrixId = br.ReadInt32();
		colorTransformId = br.ReadInt32();
		blendMode = br.ReadInt32();
	}
}

public partial class Control
{
	public Control() {}
	public Control(BinaryReader br)
	{
		controlType = br.ReadInt32();
		controlId = br.ReadInt32();
	}
}

public partial class Frame
{
	public Frame() {}
	public Frame(BinaryReader br)
	{
		controlOffset = br.ReadInt32();
		controls = br.ReadInt32();
	}
}

public partial class MovieClipEvent
{
	public MovieClipEvent() {}
	public MovieClipEvent(BinaryReader br)
	{
		clipEvent = br.ReadInt32();
		animationId = br.ReadInt32();
	}
}

public partial class Movie
{
	public Movie() {}
	public Movie(BinaryReader br)
	{
		depths = br.ReadInt32();
		labelOffset = br.ReadInt32();
		labels = br.ReadInt32();
		frameOffset = br.ReadInt32();
		frames = br.ReadInt32();
		clipEventId = br.ReadInt32();
		clipEvents = br.ReadInt32();
	}
}

public partial class MovieLinkage
{
	public MovieLinkage() {}
	public MovieLinkage(BinaryReader br)
	{
		stringId = br.ReadInt32();
		movieId = br.ReadInt32();
	}
}

public partial class ItemArray
{
	public ItemArray() {}
	public ItemArray(BinaryReader br)
	{
		offset = br.ReadInt32();
		length = br.ReadInt32();
	}
}

public partial class Header
{
	public Header() {}
	public Header(BinaryReader br)
	{
		id0 = br.ReadByte();
		id1 = br.ReadByte();
		id2 = br.ReadByte();
		id3 = br.ReadByte();
		formatVersion0 = br.ReadByte();
		formatVersion1 = br.ReadByte();
		formatVersion2 = br.ReadByte();
		formatVersion =
			(formatVersion0 << 16) | (formatVersion1 << 8) | formatVersion2;
		option = br.ReadByte();
		width = br.ReadInt32();
		height = br.ReadInt32();
		frameRate = br.ReadInt32();
		rootMovieId = br.ReadInt32();
		nameStringId = br.ReadInt32();
		backgroundColor = br.ReadInt32();
		stringBytes = new ItemArray(br);
		animationBytes = new ItemArray(br);
		translate = new ItemArray(br);
		matrix = new ItemArray(br);
		color = new ItemArray(br);
		alphaTransform = new ItemArray(br);
		colorTransform = new ItemArray(br);
		objectData = new ItemArray(br);
		texture = new ItemArray(br);
		textureFragment = new ItemArray(br);
		bitmap = new ItemArray(br);
		bitmapEx = new ItemArray(br);
		font = new ItemArray(br);
		textProperty = new ItemArray(br);
		text = new ItemArray(br);
		particleData = new ItemArray(br);
		particle = new ItemArray(br);
		programObject = new ItemArray(br);
		graphicObject = new ItemArray(br);
		graphic = new ItemArray(br);
		animation = new ItemArray(br);
		buttonCondition = new ItemArray(br);
		button = new ItemArray(br);
		label = new ItemArray(br);
		instanceName = new ItemArray(br);
		eventData = new ItemArray(br);
		place = new ItemArray(br);
		controlMoveM = new ItemArray(br);
		controlMoveC = new ItemArray(br);
		controlMoveMC = new ItemArray(br);
		if (formatVersion >= (int)Format.Constant.FORMAT_VERSION_141211)
			controlMoveMCB = new ItemArray(br);
		else
			controlMoveMCB = new ItemArray();
		control = new ItemArray(br);
		frame = new ItemArray(br);
		movieClipEvent = new ItemArray(br);
		movie = new ItemArray(br);
		movieLinkage = new ItemArray(br);
		stringData = new ItemArray(br);
		lwfLength = br.ReadInt32();
	}
}

}	// namespace Format

public partial class Data
{
	public Data(byte[] bytes)
	{
		if (bytes.Length < (int)Format.Constant.HEADER_SIZE_COMPAT0)
			return;

		Stream s = new MemoryStream(bytes);
		BinaryReader br = new BinaryReader(s);

		header = new Format.Header(br);
		if (!Check())
			return;

		byte[] stringByteData = br.ReadBytes((int)header.stringBytes.length);
		byte[] animationByteData =
			br.ReadBytes((int)header.animationBytes.length);

		translates = new Translate[header.translate.length];
		for (int i = 0; i < translates.Length; ++i)
			translates[i] = new Translate(br);
		matrices = new Matrix[header.matrix.length];
		for (int i = 0; i < matrices.Length; ++i)
			matrices[i] = new Matrix(br);
		colors = new Color[header.color.length];
		for (int i = 0; i < colors.Length; ++i)
			colors[i] = new Color(br);
		alphaTransforms = new AlphaTransform[header.alphaTransform.length];
		for (int i = 0; i < alphaTransforms.Length; ++i)
			alphaTransforms[i] = new AlphaTransform(br);
		colorTransforms = new ColorTransform[header.colorTransform.length];
		for (int i = 0; i < colorTransforms.Length; ++i)
			colorTransforms[i] = new ColorTransform(br);
		objects = new Format.Object[header.objectData.length];
		for (int i = 0; i < objects.Length; ++i)
			objects[i] = new Format.Object(br);
		textures = new Format.Texture[header.texture.length];
		for (int i = 0; i < textures.Length; ++i)
			textures[i] = new Format.Texture(br);
		textureFragments =
			new Format.TextureFragment[header.textureFragment.length];
		for (int i = 0; i < textureFragments.Length; ++i)
			textureFragments[i] = new Format.TextureFragment(br,
				header.formatVersion >=
					(int)Format.Constant.FORMAT_VERSION_141211);
		bitmaps = new Format.Bitmap[header.bitmap.length];
		for (int i = 0; i < bitmaps.Length; ++i)
			bitmaps[i] = new Format.Bitmap(br);
		bitmapExs = new Format.BitmapEx[header.bitmapEx.length];
		for (int i = 0; i < bitmapExs.Length; ++i)
			bitmapExs[i] = new Format.BitmapEx(br);
		fonts = new Format.Font[header.font.length];
		for (int i = 0; i < fonts.Length; ++i)
			fonts[i] = new Format.Font(br);
		textProperties = new Format.TextProperty[header.textProperty.length];
		for (int i = 0; i < textProperties.Length; ++i)
			textProperties[i] = new Format.TextProperty(br);
		texts = new Format.Text[header.text.length];
		for (int i = 0; i < texts.Length; ++i)
			texts[i] = new Format.Text(br);
		particleDatas = new Format.ParticleData[header.particleData.length];
		for (int i = 0; i < particleDatas.Length; ++i)
			particleDatas[i] = new Format.ParticleData(br);
		particles = new Format.Particle[header.particle.length];
		for (int i = 0; i < particles.Length; ++i)
			particles[i] = new Format.Particle(br);
		programObjects =
			new Format.ProgramObject[header.programObject.length];
		for (int i = 0; i < programObjects.Length; ++i)
			programObjects[i] = new Format.ProgramObject(br);
		graphicObjects =
			new Format.GraphicObject[header.graphicObject.length];
		for (int i = 0; i < graphicObjects.Length; ++i)
			graphicObjects[i] = new Format.GraphicObject(br);
		graphics = new Format.Graphic[header.graphic.length];
		for (int i = 0; i < graphics.Length; ++i)
			graphics[i] = new Format.Graphic(br);
		Format.Animation[] animationData =
			new Format.Animation[header.animation.length];
		for (int i = 0; i < animationData.Length; ++i)
			animationData[i] = new Format.Animation(br);
		buttonConditions =
			new Format.ButtonCondition[header.buttonCondition.length];
		for (int i = 0; i < buttonConditions.Length; ++i)
			buttonConditions[i] = new Format.ButtonCondition(br);
		buttons = new Format.Button[header.button.length];
		for (int i = 0; i < buttons.Length; ++i)
			buttons[i] = new Format.Button(br);
		labels = new Format.Label[header.label.length];
		for (int i = 0; i < labels.Length; ++i)
			labels[i] = new Format.Label(br);
		instanceNames = new Format.InstanceName[header.instanceName.length];
		for (int i = 0; i < instanceNames.Length; ++i)
			instanceNames[i] = new Format.InstanceName(br);
		events = new Format.Event[header.eventData.length];
		for (int i = 0; i < events.Length; ++i)
			events[i] = new Format.Event(br);
		places = new Format.Place[header.place.length];
		for (int i = 0; i < places.Length; ++i)
			places[i] = new Format.Place(br);
		controlMoveMs = new Format.ControlMoveM[header.controlMoveM.length];
		for (int i = 0; i < controlMoveMs.Length; ++i)
			controlMoveMs[i] = new Format.ControlMoveM(br);
		controlMoveCs = new Format.ControlMoveC[header.controlMoveC.length];
		for (int i = 0; i < controlMoveCs.Length; ++i)
			controlMoveCs[i] = new Format.ControlMoveC(br);
		controlMoveMCs =
			new Format.ControlMoveMC[header.controlMoveMC.length];
		for (int i = 0; i < controlMoveMCs.Length; ++i)
			controlMoveMCs[i] = new Format.ControlMoveMC(br);
		controlMoveMCBs =
			new Format.ControlMoveMCB[header.controlMoveMCB.length];
		for (int i = 0; i < controlMoveMCBs.Length; ++i)
			controlMoveMCBs[i] = new Format.ControlMoveMCB(br);
		controls = new Format.Control[header.control.length];
		for (int i = 0; i < controls.Length; ++i)
			controls[i] = new Format.Control(br);
		frames = new Format.Frame[header.frame.length];
		for (int i = 0; i < frames.Length; ++i)
			frames[i] = new Format.Frame(br);
		movieClipEvents =
			new Format.MovieClipEvent[header.movieClipEvent.length];
		for (int i = 0; i < movieClipEvents.Length; ++i)
			movieClipEvents[i] = new Format.MovieClipEvent(br);
		movies = new Format.Movie[header.movie.length];
		for (int i = 0; i < movies.Length; ++i)
			movies[i] = new Format.Movie(br);
		movieLinkages = new Format.MovieLinkage[header.movieLinkage.length];
		for (int i = 0; i < movieLinkages.Length; ++i)
			movieLinkages[i] = new Format.MovieLinkage(br);
		Format.String[] stringData =
			new Format.String[header.stringData.length];
		for (int i = 0; i < stringData.Length; ++i)
			stringData[i] = new Format.String(br);

		animations = new int[animationData.Length][];
		for (int i = 0; i < animationData.Length; ++i) {
			animations[i] = ReadAnimation(animationByteData,
				(int)animationData[i].animationOffset,
				(int)animationData[i].animationLength);
		}

		strings = new string[stringData.Length];
		for (int i = 0; i < stringData.Length; ++i) {
			strings[i] = System.Text.Encoding.UTF8.GetString(stringByteData,
				(int)stringData[i].stringOffset,
				(int)stringData[i].stringLength);
		}

		stringMap = new Dictionary<string, int>();
		for (int i = 0; i < strings.Length; ++i)
			stringMap[strings[i]] = i;

		instanceNameMap = new Dictionary<int, int>();
		for (int i = 0; i < instanceNames.Length; ++i)
			instanceNameMap[instanceNames[i].stringId] = i;

		eventMap = new Dictionary<int, int>();
		for (int i = 0; i < events.Length; ++i)
			eventMap[events[i].stringId] = i;

		movieLinkageMap = new Dictionary<int, int>();
		movieLinkageNameMap = new Dictionary<int, int>();
		for (int i = 0; i < movieLinkages.Length; ++i) {
			movieLinkageMap[movieLinkages[i].stringId] = i;
			movieLinkageNameMap[movieLinkages[i].movieId] =
				movieLinkages[i].stringId;
		}

		programObjectMap = new Dictionary<int, int>();
		for (int i = 0; i < programObjects.Length; ++i)
			programObjectMap[programObjects[i].stringId] = i;

		labelMap = new Dictionary<int, int>[movies.Length];
		for (int i = 0; i < movies.Length; ++i) {
			Format.Movie m = movies[i];
			int o = m.labelOffset;
			Dictionary<int, int> map = new Dictionary<int, int>();
			for (int j = 0; j < m.labels; ++j) {
				Format.Label l = labels[o + j];
				map[l.stringId] = l.frameNo;
			}
			labelMap[i] = map;
		}

		for (int i = 0; i < textures.Length; ++i)
			textures[i].SetFilename(this);

		bitmapMap = new Dictionary<string, int>();
		var bitmapList = new List<Format.Bitmap>(bitmaps);
		for (int i = 0; i < textureFragments.Length; ++i) {
			textureFragments[i].SetFilename(this);
			bitmapMap[textureFragments[i].filename] = bitmapList.Count;
			bitmapList.Add(
				new Format.Bitmap{matrixId = 0, textureFragmentId = i});
		}
		bitmaps = bitmapList.ToArray();
	}

	int[] ReadAnimation(byte[] bytes, int offset, int length)
	{
		Stream s = new MemoryStream(bytes, offset, length);
		BinaryReader br = new BinaryReader(s);
		ArrayList array = new ArrayList();

		for (;;) {
			byte code = br.ReadByte();
			array.Add((int)code);

			switch ((Animation)code) {
			case Animation.PLAY:
			case Animation.STOP:
			case Animation.NEXTFRAME:
			case Animation.PREVFRAME:
				break;

			case Animation.GOTOFRAME:
			case Animation.GOTOLABEL:
			case Animation.EVENT:
			case Animation.CALL:
				array.Add(br.ReadInt32());
				break;

			case Animation.SETTARGET:
				{
					int count = br.ReadInt32();
					array.Add(count);
					for (int i = 0; i < (int)count; ++i) {
						int target = br.ReadInt32();
						array.Add(target);
					}
				}
				break;

			case Animation.END:
				return (int[])array.ToArray(typeof(int));
			}
		}
	}
}

}	// namespace LWF
