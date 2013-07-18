package lwf;

@:native("LWF.ColorTransform")
extern class ColorTransform
{
	public var multi:Color;

	@:overload(function(r:Float, g:Float, b:Float, a:Float):Void{})
	public function new():Void;

	public function clear():Void;

	public function set(c:ColorTransform):ColorTransform;
}


