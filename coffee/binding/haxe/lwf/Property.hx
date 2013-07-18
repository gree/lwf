package lwf;

@:native("LWF.Property")
extern class Property
{
	public function clear():Void;

	public function move(x:Float, y:Float):Void;

	public function moveTo(x:Float, y:Float):Void;

	public function rotate(v:Float):Void;

	public function rotateTo(v:Float):Void;

	public function scale(x:Float, y:Float):Void;

	public function scaleTo(x:Float, y:Float):Void;

	public function setAlpha(a:Float):Void;

	public function setColorTransform(c:ColorTransform):Void;

	public function setMatrix(m:Matrix, ?sx:Float, ?sy:Float, ?r:Float):Void;
}
