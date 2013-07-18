#if js
package lwf;

import js.html.Node;

typedef Settings = {
	?active:Bool,
	?contentSize:{},
	?execLimit:Int,
	?fitForHeight:Bool,
	?fitForWidth:Bool,
	?imageMap:Dynamic,
	?imagePrefix:String,
	?imageSuffix:String,
	?js:String,
	lwf:String,
	?needsClear:Bool,
	?onload:LWF->Void,
	?onprogress:Int->Int->Void,
	?parentLWF:LWF,
	?preferredFrameRate:Int,
	?prefix:String,
	?privateData:Dynamic,
	?setBackgroundColor:Dynamic,
	stage:Node,
	?textInSubpixel:Bool,
	?use3D:Bool,
	?useBackgroundColor:Bool,
	?worker:Bool,
}

@:native("LWF.ResourceCache")
extern class ResourceCache
{
	static public function get():ResourceCache;

	public function clear():Void;

	public function loadLWF(settings:Settings):Void;

	public function loadLWFs(settingsArray:Array<Settings>, errors:Array<{}> -> Void):Void;

	public function setParticleConstructor(name:String, ctor:LWF->Int->Dynamic->Renderer):Void;

	public function setDOMElementConstructor(name:String, ctor:LWF->String->Int->Int->Renderer):Void;
}
#end
