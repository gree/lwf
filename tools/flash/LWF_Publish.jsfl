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

var ExportBitmaps = true;
var lineBreak = "\n";

var doc = fl.getDocumentDOM();
var lib;
var items;

var BitmapDirName;
var FlaDirName;
var FileID;

var TraceLog = "";
var AlertLog = "";
var ExtentionError_Count = 0;
var SpaceError_Count = 0;
var NotPngError_Count = 0;
var NonTextVarError_Count = 0;
var NonTextVarWarning_Count1 = 0;
var NonTextVarWarning_Count2 = 0;
var ShapeWarning_Count = 0;
var BytesStringError_Count = 0;
var NotSameTextVarError_Count = 0;

Init();

function Init()
{
	if (!doc) {
		alert("[ERROR] Can't open fla file.");
	} else {
		lib = doc.library;
		items = lib.items;
		var startTime = new Date();
		fl.outputPanel.clear();
		fl.trace("Now checking...");
	
		FlaDirName = doc.pathURI.substr(0, doc.pathURI.lastIndexOf("/") +1);
		FileID = doc.pathURI.substr(FlaDirName.length);
		FileID = FileID.substr(0, FileID.lastIndexOf("."));
		if (FileID.indexOf("リカバリ") != -1 || FileID.indexOf("%e3%83%aa%e3%82%ab%e3%83%90%e3%83%aa") != -1) {
			alert("[ERROR] This fla file is recovery file!!");
			throw 0;
		}

		if (FileID.indexOf(" ") != -1) {
			alert("[ERROR] File name contatin space!! FileID = " + FileID);
			throw 0;
		}

		BitmapDirName = FlaDirName + FileID + ".bitmap/";

		fl.showIdleMessage(false);
		CheckVersion();
		CheckLibraryAndElement();
		fl.showIdleMessage(true);

		StartPublish();

		ShowResult();
		fl.trace("Publish SWF completed! " +
			((new Date() - startTime) / 1000) + " sec");
	}
}

function CheckVersion()
{
	var allStr = doc.exportPublishProfileString().split(lineBreak + '    ').join(lineBreak);
	var xmlArray = allStr.split(lineBreak);

	for (var i=0, il = xmlArray.length; i < il; i++) {
		var s = xmlArray[i];
		if (s.substr(0,9) == "<Version>") {
			if (s != "<Version>7</Version>") {
				SetLog("Fixed Flash Player version error." + lineBreak);
			}
			xmlArray[i] = "<Version>7</Version>";
		} else if (s.substr(0,16) == "<ExternalPlayer>") {
			xmlArray[i] = "<ExternalPlayer></ExternalPlayer>";
		} else if (s.substr(0,21) == "<ActionScriptVersion>") {
			if (s != "<ActionScriptVersion>1</ActionScriptVersion>") {
				SetLog("Fixed ActionScript version error." + lineBreak);
			}
			xmlArray[i] = "<ActionScriptVersion>1</ActionScriptVersion>";
		} else if (s == "<html>1</html>") {
			xmlArray[i] = "<html>0</html>";
			SetLog("Disable HTML output." + lineBreak);
		} else if (s == "<CompressMovie>1</CompressMovie>") {
			xmlArray[i] = "<CompressMovie>0</CompressMovie>";
		} else if (s == "<InvisibleLayer>1</InvisibleLayer>") {
			xmlArray[i] = "<InvisibleLayer>0</InvisibleLayer>";
		} else if (s == "<IncludeXMP>1</IncludeXMP>") {
			xmlArray[i] = "<IncludeXMP>0</IncludeXMP>";
			SetLog("Disable including XML." + lineBreak);
		} else if (s == "<LoopCount></LoopCount>") {
			xmlArray[i] = "<LoopCount>0</LoopCount>";
		}
	}

	doc.importPublishProfileString(xmlArray.join(lineBreak))
}

function createBitmapDir()
{
	if (ExportBitmaps) {
		BitmapDirName = FlaDirName + FileID + ".bitmap/";
		FLfile.remove(BitmapDirName);
		FLfile.createFolder(BitmapDirName);
	}
}

function CheckLibraryAndElement()
{
	createBitmapDir();
	
	for (var i0=-1, len=items.length; i0<len; i0++) {
		
		var isRoot = i0==-1;
		
		var item = isRoot ? null : items[i0];
		var isButton = isRoot ? false : item.itemType == "button";
		var name = isRoot ? "root" : item.name;
		lib.editItem(name);
		var tl = doc.getTimeline();
		var layers = tl.layers;

		if (!isRoot) {
			CheckPngLinkage(lib, item, name);
		}
		
		switch (lib.getItemType(name)) {
		case 'bitmap':
			continue;
		case 'movie clip':
		case 'button':
			lib.selectItem(name);
			setLinkageName(lib, name.split("/").join("_"));
			break;
		}

		for (var i = 0, layers = tl.layers, il = layers.length; i < il; i++) {
			var layer = layers[i];
			if (layer.layerType == "guide" || !layer.visible)
				continue;

			for (var i2 = 0, frames = layer.frames, il2 = frames.length; i2 < il2; i2++) {
				var frame = frames[i2];
				for (var i3 = 0, elements = frame.elements,
						il3 = elements.length; i3 < il3; i3++) {
					var element = frame.elements[i3];
					var isInstance = element.elementType == "instance";
					var isElement = element.elementType == "shape";
					var isText = element.elementType == "text";
					
					if (isInstance && element.name.substr(element.name.length - 7) == "_frames") {
						SetLog("Warning name suffix '_frames' is maybe '_frame' ?  instance name = "+element.name+" / symbole name = " + name + lineBreak);
					}

					if (isElement && name.indexOf("ProgramObject") == -1 && !isButton) {
						ShapeWarning_Count++;
						SetLog("Warning shape is contained "+ShapeWarning_Count+
								 ". symbole name = " + name + lineBreak);
					} else if (isText) {// && !element.variableName) {
						if (!element.variableName && element.name) {
							element.variableName = element.name;
							NonTextVarWarning_Count1++;
							SetLog("Fixed error missing text variable "+NonTextVarWarning_Count1+
									 ". text name = " + element.name + 
									 ". symbole name = " + name + lineBreak);
						} else if (element.variableName && !element.name) {
							element.name = element.variableName;
							NonTextVarWarning_Count2++;
							SetLog("Fixed error missing text instance name "+NonTextVarWarning_Count2+
									 ". text name = " + element.name + 
									 ". symbole name = " + name + lineBreak);
						} else if (!element.variableName && !element.name) {
							NonTextVarError_Count++;
							var errorText = "Error! missing text instance name "+NonTextVarError_Count+
									 ". text name = " + element.name + 
									 ". symbole name = " + name + lineBreak;
							SetLog(errorText);
							SetAlert(errorText);
						} else if ((element.variableName && element.name) && (element.variableName != element.name)) {
							
							//if name is conflict, prioritize element.name
							element.variableName = element.name;
							
							NotSameTextVarError_Count++;
							var errorText = "Fixed text name is not same error "+NotSameTextVarError_Count+
									 ". text name = " + element.name + 
									 ". symbole name = " + name + lineBreak;
							SetLog(errorText);
							//SetAlert(errorText);
						}
					}
				}
			}
		}
	}
	lib.editItem("root");
}

function CheckPngLinkage(lib, item, name)
{
	if ('bitmap' == lib.getItemType(name)) {
		if (item.compressionType != "lossless") {
			NotPngError_Count++;
			SetLog("Fixed error img not lossless " + NotPngError_Count +
					 ". symbole name = " + name + lineBreak);
		}
		item.compressionType = "lossless";
		
		lib.selectItem(name);

		var isRename = false;

		if (name.indexOf("ビットマップ") != -1) {
			isRename = true;
			BytesStringError_Count++;
			SetLog("Fixed error img 2byte name "+BytesStringError_Count+
					 ". symbole name = " + name + lineBreak);
			name = name.split("ビットマップ").join("bitmap");
		}
		
		if (name.indexOf("コピー") != -1) {
			isRename = true;
			BytesStringError_Count++;
			SetLog("Fixed error img 2byte name "+BytesStringError_Count+
					 ". symbole name = " + name + lineBreak);
			name = name.split("の").join("_s_");
			name = name.split("コピー").join("copy");
		}
		if (name.indexOf("アセット") != -1) {
			isRename = true;
			BytesStringError_Count++;
			SetLog("Fixed error img 2byte name "+BytesStringError_Count+
					 ". symbole name = " + name + lineBreak);
			name = name.split("アセット").join("_asset_");
		}
		
		name = name.split("#").join("_1_");
		name = name.split("$").join("_2_");
		name = name.split("%").join("_3_");
		name = name.split("&").join("_4_");
		
		if (name.indexOf(" ") != -1) {
			isRename = true;
			SpaceError_Count++;
			SetLog("Fixed warning img name contain space " + SpaceError_Count +
					 ". symbole name = " + name + lineBreak);
			name = name.split(" ").join("");
		}

		var ext = null;
		var m = name.match(/(\.png|\.jpg)$/i);
		if (m != null)
			ext = m[1];

		if (ext == null) {
			ext = ".png";
			isRename = true;
			name = name + ext;
			ExtentionError_Count++;
			SetLog("Fixed warning no extention " +
				ExtentionError_Count + ". symbole name = " + name + lineBreak);
		}

		if (isRename) {
			var count = 0;
			while(count++ < 1024) {
				if (lib.getItemType(name) != "bitmap") {					
					break;
				}
				name = "bitmap_new" + count + ext;
			}
		}

		name = name.split("/").join("_");
		setLinkageName(lib, name);

		if (ExportBitmaps) {
			m = name.match(/_rgb_[0-9a-f]{6}/i);
			if (m === null)
				item.exportToFile(BitmapDirName + name);
		}
	}
	
}

function setLinkageName(lib, name)
{
	if (lib.getItemProperty('linkageImportForRS') == true)
		lib.setItemProperty('linkageImportForRS', false);
	lib.setItemProperty('linkageExportForAS', false);
	lib.setItemProperty('linkageExportForRS', true);
	lib.setItemProperty('linkageExportInFirstFrame', true);
	lib.setItemProperty('linkageClassName', '');
	lib.setItemProperty('linkageIdentifier', name);	
}

function StartPublish()
{
	doc.save();
	doc.publish();
}	

function SetLog(s)
{
	TraceLog = TraceLog + s;
}

function SetAlert(s)
{
	AlertLog = AlertLog + s;
}

function ShowResult()
{
	fl.trace(!TraceLog ? "Congratulations! No error!" : (TraceLog + ""));
	if (AlertLog) {
		alert(AlertLog);
	}
}
