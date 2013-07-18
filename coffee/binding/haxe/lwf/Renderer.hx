package lwf;

@:native("LWF.Renderer")
extern class Renderer
{
	public var lwf(default,never):LWF;

	public function new(l:LWF):Void;

	public function destruct():Void;

	public function update(matrix:Matrix, colorTransform:ColorTransform):Void;

	public function render(matrix:Matrix, colorTransform:ColorTransform, renderingIndex:Int, renderingCount:Int, visible:Bool):Void;
}
