package lwf;

typedef StageSize = {
	width:Float,
	height:Float,
}

typedef EventListener = Movie->Button->Void;

@:native("LWF.LWF")
extern class LWF
{
	public var data(default,never):Data;
	public var depth(default,never):Int;
	public var fastForward(default,never):Bool;
	public var fastForwardTimeout(default,never):Int;
	public var frameRate(default,never):Int;
	public var frameSkip(default,never):Bool;
	public var height(default,never):Float;
	public var interactive(default,never):Bool;
	public var name(default,never):String;
	public var pointX(default,never):Float;
	public var pointY(default,never):Float;
	public var pressing(default,never):Bool;
	public var privateData:Dynamic;
	public var property(default,never):Property;
	public var rendererFactory(default,never):Dynamic;
	public var resourceCache(default,never):Dynamic;
	public var rootMovie(default,never):Movie;
	public var stage(default,never):Dynamic;
	public var tick(default,never):Float;
	public var time(default,never):Float;
	public var width(default,never):Float;

	static public function useCanvasRenderer():Void;

	static public function useWebGLRenderer():Void;

	static public function useWebkitCSSRenderer():Void;

	public function addAllowButton(buttonName:String):Bool;

	public function addButtonEventListener(instanceName:String, listeners:{}):Void;

	public function addDenyButton(buttonName:String):Bool;

	public function addEventListener(event:String, listener:EventListener):Void;

	public function addMovieEventListener(instanceName:String, listeners:{}):Void;

	public function clearAllowButton():Void;

	public function clearButtonEventListener(instanceName:String, ?type:String):Void;

	public function clearDenyButton():Void;

	public function clearEventListener(event:String):Void;

	public function clearMovieEventListener(instanceName:String, ?type:String):Void;

	public function denyAllButtons():Void;

	public function destroy():Void;

	public function dispatchEvent(event:String, ?movie:Movie, ?button:Button):Void;

	public function exec(?tick:Float, ?matrix:Matrix, ?colorTransform:ColorTransform):Void;

	public function fitForHeight(stageWidth:Float, stageHeight:Float):Void;

	public function fitForWidth(stageWidth:Float, stageHeight:Float):Void;

	public function forceExec(?matrix:Matrix, ?colorTransform:ColorTransform):Void;

	public function forceExecWithoutProgress(?matrix:Matrix, ?colorTransform:ColorTransform):Void;

	public function getStageSize():StageSize;

	public function getStringId(str:String):Int;

	public function init():Void;

	public function inputKeyPress(code:Int):Void;

	public function inputPoint(x:Float, y:Float):Button;

	public function inputPress():Void;

	public function inputRelease():Void;

	public function inspect(inspector:Movie->Int->Int->Int->Void):Void;

	public function removeAllowButton(buttonName:String):Bool;

	public function removeButtonEventListener(instanceName:String, listeners:{}):Void;

	public function removeDenyButton(buttonName:String):Bool;

	public function removeEventListener(event:String, listener:EventListener):Void;

	public function removeMovieEventListener(instanceName:String, listeners:{}):Void;

	public function render():Void;

	public function scaleForHeight(stageWidth:Float, stageHeight:Float):Void;

	public function scaleForWidth(stageWidth:Float, stageHeight:Float):Void;

	public function searchAttachedLWF(attachName:String):LWF;

	public function searchAttachedMovie(attachName:String):Movie;

	public function searchEventId(event:String):Int;

	public function searchFrame(movie:Movie, label:String):Int;

	public function searchProgramObjectId(name:String):Int;

	public function setButtonEventListener(instanceName:String, listeners:{}):Void;

	public function setEventListener(event:String, listener:EventListener):Void;

	public function setFastForward(fastForward:Bool):Void;

	public function setFastForwardTimeout(fastForwardTimeout:Int):Void;

	public function setFrameRate(frameRate:Int):Void;

	public function setFrameSkip(frameSkip:Bool):Void;

	public function setMovieCommand(instanceNames:Array<String>, cmd:Movie->Void):Void;

	public function setMovieEventListener(instanceName:String, listeners:{}):Void;

	public function setPreferredFrameRate(preferredFrameRate:Int, ?execLimit:Int):Void;

	public function setProgramObjectConstructor(name:String, ctor:Dynamic->Int->Int->Int):Renderer;
}
