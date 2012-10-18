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

function LWF() {}
LWF.prototype.backgroundColor;
LWF.prototype.data;
LWF.prototype.depth;
LWF.prototype.frameRate;
LWF.prototype.height;
LWF.prototype.interactive;
LWF.prototype.name;
LWF.prototype.pointX;
LWF.prototype.pointY;
LWF.prototype.pressing;
LWF.prototype.privateData;
LWF.prototype.property;
LWF.prototype.rendererFactory;
LWF.prototype.resourceCache;
LWF.prototype.rootMovie;
LWF.prototype.stage;
LWF.prototype.tick;
LWF.prototype.time;
LWF.prototype.width;

function Movie() {}
Movie.prototype.alpha;
Movie.prototype.attachName;
Movie.prototype.currentFrame;
Movie.prototype.data;
Movie.prototype.depth;
Movie.prototype.lwf;
Movie.prototype.playing;
Movie.prototype.property;
Movie.prototype.rotation;
Movie.prototype.scaleX;
Movie.prototype.scaleY;
Movie.prototype.totalFrames;
Movie.prototype.visible;
Movie.prototype.x;
Movie.prototype.y;

function Button() {}
Button.prototype.hitX;
Button.prototype.hitY;

function Point() {}
Point.prototype.x;
Point.prototype.y;

function Matrix() {}
Matrix.prototype.scaleX;
Matrix.prototype.scaleY;
Matrix.prototype.skew0;
Matrix.prototype.skew1;
Matrix.prototype.translateX;
Matrix.prototype.translateY;

function Color() {}
Color.prototype.alpha;
Color.prototype.blue;
Color.prototype.green;
Color.prototype.red;

function ColorTransform() {}
ColorTransform.prototype.multi;
