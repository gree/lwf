global.LWF.Script = global.LWF.Script || {};
global.LWF.Script["sample3_max_optimized"] = function() {
	var LWF = global.LWF.LWF;
	var Loader = global.LWF.Loader;
	var Movie = global.LWF.Movie;
	var Property = global.LWF.Property;
	var Point = global.LWF.Point;
	var Matrix = global.LWF.Matrix;
	var Color = global.LWF.Color;
	var ColorTransform = global.LWF.ColorTransform;
	var Tween = global.LWF.Tween;
	var _root;

	var Script = (function() {function Script() {}

	Script.prototype["init"] = function() {
		_root = this;
	};

	Script.prototype["destroy"] = function() {
		_root = null;
	};

	Script.prototype["_root_144_1"] = function() {
		this.stop();
	};

	Script.prototype["_root_144_button_btn_press"] = function() {
		this.gotoAndPlay(1);
	};

	Script.prototype["_root_60_1"] = function() {
		this.stop();
	};

	Script.prototype["_root_60_button_btn_press"] = function() {
		this.play();
	};

	Script.prototype["_root_load"] = function() {
		if (_root.lwf.privateData == undefined){
			_root.ATK="1000";
			_root.DEF="2000";
			_root.player_name="player_hoge";
			_root.enemy_name="enemy_hoge";
		}else{
			_root.ATK=_root.lwf.privateData["ATK"];
			_root.DEF=_root.lwf.privateData["DEF"];
			_root.player_name=_root.lwf.privateData["player_name"];
			_root.enemy_name=_root.lwf.privateData["enemy_name"];
		}
		
	};

	Script.prototype["fire_0_1"] = function() {
		this.gotoAndPlay(Math.floor(Math.random()*50));
	};

	Script.prototype["status_load"] = function() {
		this.ATK=_root.ATK;
		this.DEF=_root.DEF;
		this.player_name=_root.player_name;
		this.enemy_name=_root.enemy_name;
	};

	return Script;

	})();

	return new Script();
};
