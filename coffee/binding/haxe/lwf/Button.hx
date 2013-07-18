package lwf;

typedef ButtonEventListener = Void->Void;

@:native("LWF.Button")
extern class Button
{
	public var hitX(default,never):Float;
	public var hitY(default,never):Float;
	public var name(default,never):String;

	public function new():Void;

	public function getFullName():String;

	public function addEventListener(event:String, listener:ButtonEventListener):Void;

	public function clearEventListener(?event:String):Void;

	public function removeEventListener(event:String, listener:ButtonEventListener):Void;

	public function setEventListener(event:String, listener:ButtonEventListener):Void;
}
