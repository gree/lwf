package lwf;

@:native("LWF.BitmapClip")
extern class BitmapClip
{
	public var alpha:Float;
	public var depth:Int;
	public var height(default,never):Int;
	public var lwf(default,never):LWF;
	public var name(default,never):String;
	public var regX:Float;
	public var regY:Float;
	public var rotation:Float;
	public var scaleX:Float;
	public var scaleY:Float;
	public var visible:Bool;
	public var width(default,never):Int;
	public var x:Float;
	public var y:Float;

	public function setMatrix(matrix:Matrix):Void;
	public function detachFromParent():Void;
}
