package lwf;

@:native("LWF.Matrix")
extern class Matrix
{
	public var scaleX:Float;
	public var scaleY:Float;
	public var skew0:Float;
	public var skew1:Float;
	public var translateX:Float;
	public var translateY:Float;

	@:overload(function(sx:Float, sy:Float, s0:Float, s1:Float, tx:Float, ty:Float):Void{})
	public function new():Void;

	public function clear():Void;

	public function set(m:Matrix):Matrix;
}
