package lwf;

@:native("LWF.Color")
extern class Color
{
	public var alpha:Float;
	public var blue:Float;
	public var green:Float;
	public var red:Float;

	@:overload(function(r:Float, g:Float, b:Float, a:Float):Void{})
	public function new():Void;

	@:overload(function(r:Float, g:Float, b:Float, a:Float):Void{})
	public function set(c:Color):Void;
}
