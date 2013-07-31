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
	lib = doc.library;
	var uri = doc.pathURI;
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
		doc.setPlayerVersion(7);
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
			if (name.match(/\.(jpg|jpeg|png)$/i) === null)
				name += ".png";
			name = name.replace(/(_rgb_[0-9a-f]{6}|_rgb_[0-9]+,[0-9]+,[0-9]+|_rgba_[0-9a-f]{8}|_rgba_[0-9]+,[0-9]+,[0-9]+,[0-9]+|_add_[0-9a-f]{6}|_add_[0-9]+,[0-9]+,[0-9]+)\/([^\/]+)\.(jpg|jpeg|png)$/i, "$2$1.$3").replace(/\//g, "_");
			setLinkage(item, name);

			m = name.match(/(_rgb_[0-9a-f]{6}|_rgb_[0-9]+,[0-9]+,[0-9]+|_rgba_[0-9a-f]{8}|_rgba_[0-9]+,[0-9]+,[0-9]+,[0-9]+|_add_[0-9a-f]{6}|_add_[0-9]+,[0-9]+,[0-9]+)\.(jpg|jpeg|png)$/i);
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

main();
