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
 *	  claim that you wrote the original software. If you use this software
 *	  in a product, an acknowledgment in the product documentation would be
 *	  appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *	  misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

var doc;
var lib;
var flaDir;
var flaName;
var message;
var buttonColors = {};
var buttonColor = 0;

function error(msg)
{
	message += "ERROR: " + msg + "\n";
}

function warn(msg)
{
	message += "WARNING: " + msg + "\n";
}

function main()
{
	message = "";
	fl.outputPanel.clear();
	fl.showIdleMessage(false);

	doc = fl.getDocumentDOM();
	if (doc.getPlayerVersion().match(/^FlashLite/)) {
		if (! confirm("WARNING: the current player version is "
					  + doc.getPlayerVersion()
					  + ". Can we continue to publish?")) {
			return;
		}
	}

	lib = doc.library;
	var uri = doc.pathURI;
	if (! uri) {
		if (fl.saveDocumentAs(doc)) {
			uri = doc.pathURI;
		} else {
			return;
		}
	}
	FLfile.remove(uri + '~');
	FLfile.copy(uri, uri + '~');
	flaDir = uri.substr(0, uri.lastIndexOf("/") + 1);
	flaName = uri.substr(flaDir.length);
	flaName = flaName.substr(0, flaName.lastIndexOf("."));

	var bitmapDir = flaDir + flaName + ".bitmap/";
	FLfile.remove(bitmapDir);
	FLfile.createFolder(bitmapDir);

	if (doc.setPlayerVersion(20)) {
		// Flash CC
		doc.asVersion = 3;
	} else {
		// Flash CS6
		doc.setPlayerVersion(8);
		doc.asVersion = 1;
	}

	var items = lib.items;
	var movies = 0;
	var buttons = 0;
	var libitems = [];
	for (var i = 0; i < items.length; ++i) {
		var item = items[i];
		if (item.name.match(/__IGNORE__/) !== null)
			continue;
		switch (item.itemType) {
		case "bitmap":
		case "movie clip":
		case "button":
			clearLinkage(item);
			libitems.push(item);
		}
	}
	for (var i = 0; i < libitems.length; ++i) {
		var item = libitems[i];

		switch (item.itemType) {
		case "bitmap":
			item.compressionType = "lossless";
			var name = item.name;
			if (name.match(/\.(gif|jpg|jpeg|png)$/i) === null)
				name += ".png";
			name = name.replace(/(_rgb_[0-9a-f]{6}|_rgb_[0-9]+,[0-9]+,[0-9]+|_rgba_[0-9a-f]{8}|_rgba_[0-9]+,[0-9]+,[0-9]+,[0-9]+|_add_[0-9a-f]{6}|_add_[0-9]+,[0-9]+,[0-9]+)\/([^\/]+)\.(gif|jpg|jpeg|png)$/i, "$2$1.$3").replace(/\//g, "_");
			setLinkage(item, name);

			m = name.match(/(_rgb_[0-9a-f]{6}|_rgb_[0-9]+,[0-9]+,[0-9]+|_rgba_[0-9a-f]{8}|_rgba_[0-9]+,[0-9]+,[0-9]+,[0-9]+|_add_[0-9a-f]{6}|_add_[0-9]+,[0-9]+,[0-9]+)\.(gif|jpg|jpeg|png)$/i);
			if (m === null)
				item.exportToFile(bitmapDir + name);
			break;
	
		case "movie clip":
		case "button":
			var name = item.name.replace(/\//g, "_");
			if (name.match(/[^0-9a-zA-Z_]/) !== null) {
				if (item.itemType === "movie clip") {
					name = "movie" + movies++;
				} else {
					name = "button" + buttons++;
					correctButtonShapeColor(item);
				}
				warn("Corrected LinkageName: " + item.itemType + ": \"" +
					item.name + "\" -> \"" + name + "\"");
			}
			setLinkage(item, name);
			break;
		}
	}
	
	fl.showIdleMessage(true);

	doc.save();
	doc.publish();

	fl.trace(message);
}

function clearLinkage(item)
{
	if (item.linkageImportForRS === true) {
		item.linkageImportForRS = false;
	} else {
		item.linkageExportForAS = false;
		item.linkageExportForRS = false;
	}
}

function setLinkage(item, name)
{
	try {
		item.linkageExportForRS = true;
		item.linkageIdentifier = name;
		item.linkageExportForAS = true;
		item.linkageExportInFirstFrame = true;
		item.linkageURL = doc.name;
		item.linkageClassName = "";
	}
	catch (e)
	{
		error(item.name + ": " + e.toString().replace(/^Error: /, ""));
	}
}

function correctButtonShapeColor(item)
{
	if (item.timeline.frameCount == 4 && item.timeline.layerCount == 1) {
		var frame = item.timeline.layers[0].frames[3];
		if (frame.elements.length == 1 && frame.elements[0].elementType === "shape") {
			var shape = frame.elements[0];
			if (shape.vertices.length == 4 && shape.contours.length == 2) {
				var color = shape.contours[1].fill.color;
warn(color);
				if (color !== undefined) {
					var n = parseInt(color.substring(1), 16);
					if (buttonColors[n] === undefined) {
						buttonColors[n] = true;
					} else {
						while (buttonColors[buttonColor] !== undefined) {
							buttonColor++;
						}
						var n = buttonColor++;
						lib.selectItem(item.name);
						item.timeline.setSelectedLayers(0);
						item.timeline.setSelectedFrames(3, 3);
						var fill = doc.getCustomFill();
						fill.color = n;
						doc.setCustomFill(fill);
						doc.selectNone();
						buttonColors[n] = true;
						warn("Corrected Button Shape Color: " + color + " -> " + shape.contours[1].fill.color);
					}
				}
			}
		}
	}
}

main();
