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
LWF.prototype.fastForward;
LWF.prototype.fastForwardTimeout;
LWF.prototype.frameRate;
LWF.prototype.frameSkip;
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

function Data() {}
Data.prototype.alphaTransforms;
Data.prototype.animations;
Data.prototype.bitmapExs;
Data.prototype.bitmaps;
Data.prototype.buttonConditions;
Data.prototype.buttons;
Data.prototype.colorTransforms;
Data.prototype.colors;
Data.prototype.controlMoveCs;
Data.prototype.controlMoveMCs;
Data.prototype.controlMoveMs;
Data.prototype.controls;
Data.prototype.events;
Data.prototype.fonts;
Data.prototype.frames;
Data.prototype.graphicObjects;
Data.prototype.graphics;
Data.prototype.header;
Data.prototype.instanceNames;
Data.prototype.labels;
Data.prototype.matrices;
Data.prototype.movieClipEvents;
Data.prototype.movieLinkages;
Data.prototype.movies;
Data.prototype.objects;
Data.prototype.particleDatas;
Data.prototype.particles;
Data.prototype.places;
Data.prototype.programObjects;
Data.prototype.strings;
Data.prototype.textProperties;
Data.prototype.texts;
Data.prototype.textureFragments;
Data.prototype.textures;
Data.prototype.translates;

function Format() {};
Format.ProgramObject = function() {};
Format.ProgramObject.prototype.colorTransformId;
Format.ProgramObject.prototype.height;
Format.ProgramObject.prototype.matrixId;
Format.ProgramObject.prototype.stringId;
Format.ProgramObject.prototype.width;

function Movie() {}
Movie.prototype.active;
Movie.prototype.alpha;
Movie.prototype.attachName;
Movie.prototype.blendMode;
Movie.prototype.currentFrame;
Movie.prototype.data;
Movie.prototype.depth;
Movie.prototype.lwf;
Movie.prototype.playing;
Movie.prototype.postLoaded;
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

function BitmapClip() {}
BitmapClip.prototype.alpha;
BitmapClip.prototype.depth;
BitmapClip.prototype.height;
BitmapClip.prototype.lwf;
BitmapClip.prototype.name;
BitmapClip.prototype.regX;
BitmapClip.prototype.regY;
BitmapClip.prototype.rotation;
BitmapClip.prototype.scaleX;
BitmapClip.prototype.scaleY;
BitmapClip.prototype.visible;
BitmapClip.prototype.width;
BitmapClip.prototype.x;
BitmapClip.prototype.y;

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
