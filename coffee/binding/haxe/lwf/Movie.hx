package lwf;

typedef EventParams = {
	type:String,
	param:Dynamic,
}

typedef Bounds = {
	xMin:Float,
	xMax:Float,
	yMin:Float,
	yMax:Float,
}

typedef MovieEventListener = ?EventParams->Void;

@:native("LWF.Movie")
extern class Movie
{
	public var active:Bool;
	public var alpha:Float;
	public var blendMode:String;
	public var currentFrame(default,never):Int;
	public var depth(default,never):Int;
	public var lwf(default,never):LWF;
	public var name(default,never):String;
	public var playing(default,never):Bool;
	public var rotation:Float;
	public var scaleX:Float;
	public var scaleY:Float;
	public var totalFrames(default,never):Int;
	public var visible:Bool;
	public var x:Float;
	public var y:Float;

	@:overload(function(event:String, listener:Void->Void):Movie{})
	public function addEventListener(event:String, listener:MovieEventListener):Void;

	public function attachBitmap(bitmapName:String, depth:Int):BitmapClip;

	public function attachEmptyMovie(attachName:String, ?options:{}):Movie;

	public function attachLWF(attachLWF:LWF, attachName:String, ?options:{}):Void;

	@:overload(function(linkageName:String, attachName:String, ?options:{}):Movie{})
	public function attachMovie(linkageName:LWF, attachName:String, ?options:{}):Movie;

	public function clearEventListener(?event:String):Void;

	public function detachBitmap(depth:Int):Void;

	public function detachFromParent():Void;

	@:overload(function(attachName:String):Void{})
	@:overload(function(depth:Int):Void{})
	public function detachLWF(lwf:LWF):Void;

	@:overload(function(attachName:String):Void{})
	@:overload(function(depth:Int):Void{})
	public function detachMovie(movie:Movie):Void;

	@:overload(function(e:EventParams):Void{})
	public function dispatchEvent(e:String):Void;

	public function getAttachedBitmap(depth:Int):BitmapClip;

	public function getAttachedBitmaps():Array<BitmapClip>;

	@:overload(function(attachName:String):LWF{})
	public function getAttachedLWF(depth:Int):LWF;

	@:overload(function(attachName:String):Movie{})
	public function getAttachedMovie(depth:Int):Movie;

	public function getBounds():Bounds;

	public function globalToLocal(point:Point):Point;

	@:overload(function(label:String):Void{})
	public function gotoAndPlay(frame:Int):Void;

	@:overload(function(label:String):Void{})
	public function gotoAndStop(frame:Int):Void;

	public function gotoFrame(frame:Int):Void;

	public function gotoLabel(label:String):Void;

	public function localToGlobal(point:Point):Point;

	public function move(x:Float, y:Float):Void;

	public function moveTo(x:Float, y:Float):Void;

	public function nextEnterFrame(func:Void->Void):Void;

	public function nextFrame():Void;

	public function play():Void;

	public function prevFrame():Void;

	public function removeEventListener(event:String, listener:MovieEventListener):Void;

	public function removeMovieClip():Void;

	public function requestCalculateBounds(?callback:Void->Void):Void;

	public function rotate(degree:Float):Void;

	public function rotateTo(degree:Float):Void;

	public function scale(x:Float, y:Float):Void;

	public function scaleTo(x:Float, y:Float):Void;

	public function searchAttachedLWF(attachName:String, ?recursive:Bool):LWF;

	public function searchAttachedMovie(attachName:String, ?recursive:Bool):Movie;

	public function searchMovieInstance(name:String, ?recursive:Bool):Movie;

	public function setAlpha(alpha:Float):Void;

	public function setColorTransform(colorTransform:ColorTransform):Void;

	public function setEventListener(event:String, listener:MovieEventListener):Void;

	public function setMatrix(matrix:Matrix):Void;

	public function setRenderingOffset(offset:Int):Void;

	public function setVisible(visible:Bool):Void;

	public function stop():Void;

	public function swapAttachedLWFDepth(depth0:Int, depth1:Int):Void;

	public function swapAttachedMovieDepth(depth0:Int, depth1:Int):Void;

	@:overload(function(movie:Movie):Void{})
	public function swapDepths(depth:Int):Void;
}
