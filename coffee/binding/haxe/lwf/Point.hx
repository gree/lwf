package lwf;

@:native("LWF.Point")
extern class Point
{
	public var x:Float;
	public var y:Float;

	@:overload(function(ax:Float, ay:Float):Void{})
	public function new():Void;
}
